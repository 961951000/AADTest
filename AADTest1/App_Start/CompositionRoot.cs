using AADTest1.DatabaseContext;
using AADTest1.Infrastructure.Factory;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace AADTest1.App_Start
{
    public class CompositionRoot
    {
        public static void Initialize()
        {
            var container = new Container();
            container.Options.DefaultLifestyle = new WebRequestLifestyle();
            RegisterServices(container);
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterMvcIntegratedFilterProvider();

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        private static void RegisterServices(Container container)
        {

            container.Register<IDefaultDbContextFactory, DefaultConnectionDbContextFactory>();

            container.Register<SqlServerContext>(() => container.GetInstance<DefaultConnectionDbContextFactory>().Create());
        }
    }
}