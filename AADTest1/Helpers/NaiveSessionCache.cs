using System.Threading;
using System.Web;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace AADTest1.Helpers
{
    public class NaiveSessionCache : TokenCache
    {
        private static readonly ReaderWriterLockSlim SessionLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private string UserObjectId { get; }
        private string CacheId { get; }
        private HttpContextBase HttpContext { get; }
        public NaiveSessionCache(string userId, HttpContextBase httpContext)
        {
            UserObjectId = userId;
            CacheId = UserObjectId + "_TokenCache";
            HttpContext = httpContext;
            AfterAccess = AfterAccessNotification;
            BeforeAccess = BeforeAccessNotification;
            Load();
        }

        public void Load()
        {
            SessionLock.EnterReadLock();
            Deserialize((byte[])HttpContext.Session[CacheId]);
            SessionLock.ExitReadLock();
        }

        public void Persist()
        {
            SessionLock.EnterWriteLock();

            // Optimistically set HasStateChanged to false. We need to do it early to avoid losing changes made by a concurrent thread.
            HasStateChanged = false;

            // Reflect changes in the persistent store
            HttpContext.Session[CacheId] = this.Serialize();
            SessionLock.ExitWriteLock();
        }

        // Empties the persistent store.
        public override void Clear()
        {
            base.Clear();
            HttpContext.Session.Remove(CacheId);
        }

        // Triggered right before ADAL needs to access the cache.
        // Reload the cache from the persistent store in case it changed since the last access.
        private void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            Load();
        }

        // Triggered right after ADAL accessed the cache.
        private void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if the access operation resulted in a cache update
            if (HasStateChanged)
            {
                Persist();
            }
        }
    }
}