using AADTest1.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AADTest1.Infrastructure.Factory
{
    public class DefaultConnectionDbContextFactory : IDefaultDbContextFactory
    {
        public SqlServerContext Create()
        {
            return new SqlServerContext();
        }
    }
}