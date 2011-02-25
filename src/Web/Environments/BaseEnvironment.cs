using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Windsor;

namespace Colombo.Clerk.Web.Environments
{
    public abstract class BaseEnvironment : IEnvironment
    {
        public const string DefaultEnvironmnent = @"Production";

        public virtual string Name
        {
            get { return GetType().Name; }
        }

        public abstract void BootstrapContainer(IWindsorContainer container);
    }
}