using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Colombo.Clerk.Server.Mappings;
using NUnit.Framework;

namespace Colombo.Clerk.Server.Tests.Mappings
{
    [TestFixture]
    public class AutoMapperTest
    {
        [Test]
        public void Test_AutoMapper_mappings()
        {
            AutoMapperConfiguration.Configure();
            Mapper.AssertConfigurationIsValid();
        }
    }
}
