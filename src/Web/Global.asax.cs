#region License
// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Web.Environments;
using Colombo.Wcf;
using Combres;

namespace Colombo.Clerk.Web
{
    public class MvcApplication : HttpApplication
    {
        private static IWindsorContainer container;

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.AddCombresRoute("Combres");
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "Entries",
                "Entries/{serverName}",
                new { controller = "Entries", action = "Index", serverName = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        private static void ConfigureClientRestService()
        {
            ClientRestService.RegisterRequest<GetStatsByServerRequest>();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
            ConfigureClientRestService();

            container = new WindsorContainer().Install(new EnvironmentsInstaller());
            var env = container.Resolve<IEnvironment>();
            env.BootstrapContainer(container);
            env.RegisterGlobalFilters(GlobalFilters.Filters);
            env.OnApplicationStart();
        }

        protected void Application_End()
        {
            container.Dispose();
        }
    }
}