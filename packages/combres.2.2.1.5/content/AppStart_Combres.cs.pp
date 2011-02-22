[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.AppStart_Combres), "Start")]
namespace $rootnamespace$ {
	using System.Web.Routing;
	using Combres;
	
    public static class AppStart_Combres {
        public static void Start() {
            RouteTable.Routes.AddCombresRoute("Combres");
        }
    }
}