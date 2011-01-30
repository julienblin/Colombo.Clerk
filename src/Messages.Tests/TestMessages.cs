using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.TestSupport;
using NUnit.Framework;

namespace Colombo.Clerk.Messages.Tests
{
    [TestFixture]
    public class TestMessages
    {
        [Test]
        public void Messages_should_conforms_to_Colombo()
        {
            ColomboTest.AssertThat.AllMessagesAreConformInAssemblyThatContains<AuditEntry>();
        }
    }
}
