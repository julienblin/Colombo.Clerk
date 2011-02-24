using System.Web.Mvc;
using Castle.Facilities.Logging;
using Castle.MicroKernel.Lifestyle;
using Castle.Windsor;
using Colombo.Clerk.Web.Infra;
using Colombo.Clerk.Web.Tests;
using Colombo.Facilities;
using Colombo.TestSupport;

namespace Colombo.Clerk.Web.Environments.Impl
{
    public class DevStation : BaseEnvironment
    {
        public override void BootstrapContainer(IWindsorContainer container)
        {
            container.AddFacility<LoggingFacility>(f =>
            {
                f.LogUsing(LoggerImplementation.Log4net).WithConfig("log4net.config");
            });

            container.AddFacility<ColomboFacility>(f =>
            {
                f.ClientOnly();
                f.StatefulMessageBusLifestyle(typeof(PerWebRequestLifestyleManager));
                f.TestSupportMode();
            });

            container.Resolve<IStubMessageBus>().ConfigureFakeSends();

            var controllerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }
    }
}