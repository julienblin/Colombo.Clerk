using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;

namespace Colombo.Clerk.Web.Environments
{
    public interface IEnvironment
    {
        string Name { get; }

        void BootstrapContainer(IWindsorContainer container);
    }
}
