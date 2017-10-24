using AADTest1.Models;
using System.Data.Entity;

namespace AADTest1.DatabaseContext
{
    public class SqlServerContext : DbContext
    {
        public SqlServerContext() : base("name=DefaultConnection") { }

        public static SqlServerContext Create()
        {
            return new SqlServerContext();
        }

        public DbSet<User> User { get; set; }

        public DbSet<UserTokenCache> UserTokenCacheList { get; set; }
    }
}
