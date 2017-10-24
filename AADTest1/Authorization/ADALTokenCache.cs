// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdalTokenCache.cs" company="Microsoft">
//   Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   Generated ADAL token cache by VS template.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Security;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using AADTest1.DatabaseContext;
using System.Security.Cryptography;
using System.Diagnostics;
using AADTest1.Models;

namespace AADTest1.Authorization
{


    /// <summary>
    /// Generated ADAL token cache by VS template.
    /// </summary>
    public class ADALTokenCache : TokenCache, IDisposable
    {
        private const string AdalProtectCacheKey = "ADALCache";

        private readonly SqlServerContext _db;
        private readonly bool _disposeDbContext;
        private readonly string _userId;
        private UserTokenCache _cache;

        public ADALTokenCache(string signedInUserId, SqlServerContext dbContext, bool disposeDbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            this._db = dbContext;
            this._disposeDbContext = disposeDbContext;
            // associate the cache to the current user of the web app
            this._userId = signedInUserId;
            this.AfterAccess = AfterAccessNotification;
            this.BeforeAccess = BeforeAccessNotification;
            this.BeforeWrite = BeforeWriteNotification;
            // look up the entry in the database
            this.LoadToken();
        }

        private void LoadToken()
        {
            this._cache = _db.UserTokenCacheList.OrderByDescending(c => c.UserTokenCacheId).FirstOrDefault(c => c.WebUserUniqueId == this._userId);
            // load last token for the user
            // place the entry in memory
            try
            {
                this.Deserialize((this._cache == null)
                    ? null
                    : MachineKey.Unprotect(this._cache.CacheBits, AdalProtectCacheKey));
            }
            catch (CryptographicException ex)
            {
                Trace.TraceWarning($"Can't decode cached token MachineKey.Unprotect failed. Algorithm changed or host changed. Renewing token by discarding cached token.{Environment.NewLine}{ex}");
                this.Deserialize(null);
            }
        }

        private void PersistToken()
        {
            this._cache = new UserTokenCache
            {
                WebUserUniqueId = this._userId,
                CacheBits = MachineKey.Protect(this.Serialize(), AdalProtectCacheKey),
                LastWrite = DateTime.UtcNow
            };
            // update the DB and the lastwrite 
            this._db.Entry(this._cache).State = this._cache.UserTokenCacheId == 0 ? EntityState.Added : EntityState.Modified;
            this._db.SaveChanges();
            this.HasStateChanged = false;
        }

        // clean up the database
        public override void Clear()
        {
            base.Clear();
            foreach (var cacheEntry in this._db.UserTokenCacheList.Where(c => c.WebUserUniqueId == this._userId))
            {
                this._db.UserTokenCacheList.Remove(cacheEntry);
            }
            this._db.SaveChanges();
        }

        // Notification raised before ADAL accesses the cache.
        // This is your chance to update the in-memory copy from the DB, if the in-memory version is stale
        void BeforeAccessNotification(TokenCacheNotificationArgs args)
        {
            this.LoadToken();
        }

        // Notification raised after ADAL accessed the cache.
        // If the HasStateChanged flag is set, ADAL changed the content of the cache
        void AfterAccessNotification(TokenCacheNotificationArgs args)
        {
            // if state changed
            if (this.HasStateChanged)
            {
                this.PersistToken();
            }
        }

        void BeforeWriteNotification(TokenCacheNotificationArgs args)
        {
            // if you want to ensure that no concurrent write take place, use this notification to place a lock on the entry
        }

        public override void DeleteItem(TokenCacheItem item)
        {
            base.DeleteItem(item);
        }

        public void Dispose()
        {
            if (this._disposeDbContext)
            {
                this._db.Dispose();
            }
        }
    }
}