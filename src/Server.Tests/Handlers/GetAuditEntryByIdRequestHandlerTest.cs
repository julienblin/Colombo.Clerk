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
    public class GetAuditEntryByIdRequestHandlerTest : BaseDbTest
    {
        [Test]
        public void It_should_return_AuditEntry_when_found()
        {
            var auditEntryReference = new AuditEntryModel
            {
                RequestCorrelationGuid = Guid.NewGuid(),
                RequestNamespace = "RequestNamespace",
                RequestSerialized = "RequestSerialized",
                RequestType = "RequestType",
                ResponseCorrelationGuid = Guid.NewGuid(),
                ResponseNamespace = "ResponseNamespace",
                ResponseSerialized = "ResponseSerialized",
                ResponseType = "ResponseType",
                Exception = "Exception",
                ServerMachineName = "ServerMachineName"
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

            Assert.That(() => response.AuditEntry.Request.CorrelationGuid, Is.EqualTo(auditEntryReference.RequestCorrelationGuid));
            Assert.That(() => response.AuditEntry.Request.Namespace, Is.EqualTo(auditEntryReference.RequestNamespace));
            Assert.That(() => response.AuditEntry.Request.Serialized, Is.EqualTo(auditEntryReference.RequestSerialized));
            Assert.That(() => response.AuditEntry.Request.Type, Is.EqualTo(auditEntryReference.RequestType));

            Assert.That(() => response.AuditEntry.Response.CorrelationGuid, Is.EqualTo(auditEntryReference.ResponseCorrelationGuid));
            Assert.That(() => response.AuditEntry.Response.Namespace, Is.EqualTo(auditEntryReference.ResponseNamespace));
            Assert.That(() => response.AuditEntry.Response.Serialized, Is.EqualTo(auditEntryReference.ResponseSerialized));
            Assert.That(() => response.AuditEntry.Response.Type, Is.EqualTo(auditEntryReference.ResponseType));

            Assert.That(() => response.AuditEntry.Exception, Is.EqualTo(auditEntryReference.Exception));
            Assert.That(() => response.AuditEntry.ServerMachineName, Is.EqualTo(auditEntryReference.ServerMachineName));
        }

        [Test]
        public void It_should_return_a_response_when_not_found()
        {
            StubMessageBus.TestHandler<GetAuditEntryByIdRequestHandler>();
            var request = new GetAuditEntryByIdRequest { Id = Guid.NewGuid() };

            var response = MessageBus.Send(request);

            Assert.That(() => response.Found, Is.False);
            Assert.That(() => response.AuditEntry, Is.Null);
        }
    }
}
