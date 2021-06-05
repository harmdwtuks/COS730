using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using UserAuthenticationMS.Models;
using WebMatrix.WebData;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace UserAuthenticationMS
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private static SimpleMembershipInitializer _initializer;
        private static object _initializerLock = new object();
        private static bool _isInitialized;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        }

        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    // Ensure ASP.NET Simple Membership is initialized only once per app start
        //    LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
        //}

        //public class SimpleMembershipInitializer
        //{
        //    public SimpleMembershipInitializer()
        //    {
        //        using (var context = new UsersContext())
        //            context.UserProfiles.Find(1);

        //        if (!WebSecurity.Initialized)
        //            WebSecurity.InitializeDatabaseConnection("CoachItEntities", "webpages_Users", "UserId", "UserName", autoCreateTables: false);
        //    }
        //}

        private class SimpleMembershipInitializer
        {
            public SimpleMembershipInitializer()
            {
                Database.SetInitializer<UsersContext>(null);
                try
                {
                    using (var context = new UsersContext())
                    {
                        if (!context.Database.Exists())
                        {
                            // Create the SimpleMembership database without Entity Framework migration schema
                            ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                        }
                    }

                    WebSecurity.InitializeDatabaseConnection("DefaultConnection", "webpages_Users", "UserId", "UserName", autoCreateTables: false);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                }
            }
        }

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
        public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
        {
            private static SimpleMembershipInitializer _initializer;
            private static object _initializerLock = new object();
            private static bool _isInitialized;

            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                // Ensure ASP.NET Simple Membership is initialized only once per app start
                LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
            }

            private class SimpleMembershipInitializer
            {
                public SimpleMembershipInitializer()
                {
                    Database.SetInitializer<UsersContext>(null);

                    try
                    {
                        using (var context = new UsersContext())
                        {
                            if (!context.Database.Exists())
                            {
                                // Create the SimpleMembership database without Entity Framework migration schema
                                ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
                            }
                        }

                        //WebSecurity.InitializeDatabaseConnection("DefaultConnection", "webpages_Users", "UserId", "UserName", autoCreateTables: false);
                        WebSecurity.InitializeDatabaseConnection("CoachItEntities", "webpages_Users", "UserId", "UserName", autoCreateTables: false);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
                    }
                }
            }
        }
    }
}
