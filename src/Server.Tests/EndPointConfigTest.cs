using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using NHibernate;
using NUnit.Framework;

namespace Colombo.Clerk.Server.Tests
{
    [TestFixture]
    public class EndPointConfigTest
    {
        [Test]
        public void It_should_register_other_components()
        {
            var container = new WindsorContainer();

            var endPointConfig = new EndPointConfig();
            endPointConfig.RegisterOtherComponents(container);

            Assert.That(container.Kernel.HasComponent(typeof(ISessionFactory)));
            Assert.That(container.Kernel.HasComponent(typeof(ISession)));
        }

        [Test]
        public void It_should_position_static_kernel_variable_when_start()
        {
            var container = new WindsorContainer();

            var endPointConfig = new EndPointConfig();
            endPointConfig.Start(container);

            Assert.That(EndPointConfig.Kernel, Is.EqualTo(container.Kernel));
        }
    }
}
