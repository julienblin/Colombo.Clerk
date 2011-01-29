using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Colombo.Clerk.Client.Impl;
using Colombo.Clerk.Service;
using NUnit.Framework;

namespace Colombo.Clerk.Client.Tests.Impl
{
    [TestFixture]
    public class ClerkServiceFactoryTest
    {
        [Test]
        public void It_should_create_IClerkService()
        {
            var factory = new ClerkServiceFactory();

            var clerkService = factory.CreateClerkService();

            Assert.That(() => clerkService,
                Is.Not.Null
                .And.AssignableTo<IClerkService>());
        }
    }
}
