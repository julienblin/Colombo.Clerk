using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Handlers;
using Colombo.Clerk.Server.Models;
using NUnit.Framework;

namespace Colombo.Clerk.Server.Tests.Handlers
{
    [TestFixture]
    public class GetAuditEntryByIdRequestHandlerTest : BaseHandlerTest
    {
        [Test]
        public void It_should_return_AuditEntry_when_found()
        {
            var auditEntryReference = new AuditEntry
            {
                RequestCorrelationGuid = Guid.NewGuid(),
                RequestNamespace = "RequestNamespace",
                RequestSerialized = "RequestSerialized",
                RequestType = "RequestType"
            };

            using (var tx = Session.BeginTransaction())
            {
                Session.Save(auditEntryReference);
                tx.Commit();
            }

            StubMessageBus.TestHandler<GetAuditEntryByIdRequestHandler>();
            var request = new GetAuditEntryByIdRequest { Id = auditEntryReference.Id };

            var response = MessageBus.Send(request);

            Assert.That(() => response.Found, Is.True);
            Assert.That(() => response.Request.Namespace, Is.EqualTo(auditEntryReference.RequestNamespace));
        }
    }
}
