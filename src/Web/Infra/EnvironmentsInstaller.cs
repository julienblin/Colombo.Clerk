using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Colombo.Clerk.Web.Environments;

namespace Colombo.Clerk.Web.Infra
{
    public class EnvironmentsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var currentEnvironment = ConfigurationManager.AppSettings["Environment"] ??
                                     BaseEnvironment.DefaultEnvironmnent;

            var envType = GetType().Assembly.GetTypes()
                .Where(
                    t =>
                    (t.GetInterface(typeof(IEnvironment).Name) != null) &&
                    (t.Name.Equals(currentEnvironment, StringComparison.InvariantCultureIgnoreCase)) &&
                    !t.IsAbstract &&
                    !t.IsInterface
                 )
                .FirstOrDefault();

            if(envType == null)
                throw new Exception(string.Format("Unknown environment {0}.", currentEnvironment));

            container.Register(Component.For<IEnvironment>().ImplementedBy(envType));
        }
    }
}