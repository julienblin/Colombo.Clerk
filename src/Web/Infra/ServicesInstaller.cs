using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Colombo.Clerk.Web.Infra
{
    public class ServicesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                AllTypes.FromThisAssembly()
                    .Where(x => x.Namespace.StartsWith("Colombo.Clerk.Web.Services.Impl"))
                    .WithService.AllInterfaces()
            );
        }
    }
}