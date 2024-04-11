using MRSTWEb.BuisnessLogic.Infrastructure;
using Ninject;
using Ninject.Modules;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Ninject.Web.Mvc;
using MRSTWEb.Util;

namespace MRSTWEb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            NinjectModule orderModule = new OrderModule();
            NinjectModule serviceModule = new ServiceModule("Connection");
            NinjectModule cartModule = new CartModule();


            var kernel = new StandardKernel(serviceModule,cartModule,orderModule);
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
        }
    }
}
