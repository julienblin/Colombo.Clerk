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
    public class WriteAuditEntryRequestHandlerTest : BaseDbTest
    {
        [Test]
        public void It_should_write_a_correct_request()
        {
            StubMessageBus.TestHandler<WriteAuditEntryRequestHandler>();
            var request = new WriteAuditEntryRequest
                              {
                                  CorrelationGuid = Guid.NewGuid(),
                                  RequestCorrelationGuid = Guid.NewGuid(),
                                  RequestNamespace = "Requestnamespace",
                                  RequestSerialized = "RequestSerialized",
                                  RequestType = "RequestType",
                                  ResponseCorrelationGuid = Guid.NewGuid(),
                                  ResponseNamespace = "Responsenamespace",
                                  ResponseSerialized = "ResponseSerialized",
                                  ResponseType = "ResponseType",
                                  Exception = "Exception",
                                  ServerMachineName = "ServerMachineName"
                              };

            var response = MessageBus.Send(request);

            Assert.That(response.IsValid());

            using (var tx = Session.BeginTransaction())
            {
                var auditEntryModel = Session.Get<AuditEntryModel>(response.Id);
                Assert.That(() => auditEntryModel.RequestCorrelationGuid, Is.EqualTo(request.RequestCorrelationGuid));
                Assert.That(() => auditEntryModel.RequestNamespace, Is.EqualTo(request.RequestNamespace));
                Assert.That(() => auditEntryModel.RequestSerialized, Is.EqualTo(request.RequestSerialized));
                Assert.That(() => auditEntryModel.RequestType, Is.EqualTo(request.RequestType));

                Assert.That(() => auditEntryModel.ResponseCorrelationGuid, Is.EqualTo(request.ResponseCorrelationGuid));
                Assert.That(() => auditEntryModel.ResponseNamespace, Is.EqualTo(request.ResponseNamespace));
                Assert.That(() => auditEntryModel.ResponseSerialized, Is.EqualTo(request.ResponseSerialized));
                Assert.That(() => auditEntryModel.ResponseType, Is.EqualTo(request.ResponseType));

                Assert.That(() => auditEntryModel.Exception, Is.EqualTo(request.Exception));
                Assert.That(() => auditEntryModel.ServerMachineName, Is.EqualTo(request.ServerMachineName));

                tx.Commit();
            }
        }

        [Test]
        public void It_should_validate_requests()
        {
            StubMessageBus.TestHandler<WriteAuditEntryRequestHandler>().ShouldBeInterceptedBeforeHandling();
            var request = new WriteAuditEntryRequest
                              {
                                  RequestCorrelationGuid = Guid.NewGuid(),
                                  RequestNamespace = "",
                                  RequestSerialized = "",
                                  RequestType = "",
                                  ServerMachineName = ""
                              };

            var response = MessageBus.Send(request);

            Assert.That(!response.IsValid());

            Assert.That(response.ValidationResults.Count(), Is.EqualTo(3));
        }
    }
}
