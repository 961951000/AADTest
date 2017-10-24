using AADTest1.DatabaseContext;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace AADTest1.Infrastructure.Factory
{
    public interface IDefaultDbContextFactory : IDbContextFactory<SqlServerContext>
    {
    }
}