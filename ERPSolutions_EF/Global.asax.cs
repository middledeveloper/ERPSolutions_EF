using System.Web.Routing;

namespace ERPSolutions_EF
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //AreaRegistration.RegisterAllAreas(); // low performance
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
