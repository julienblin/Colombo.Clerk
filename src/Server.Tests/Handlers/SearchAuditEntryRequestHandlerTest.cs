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
    public class SearchAuditEntryRequestHandlerTest : BaseDbTest
    {
        [Test]
        public void It_should_return_zero_when_no_AuditEntry_in_db()
        {
            StubMessageBus.TestHandler<SearchAuditEntryRequestHandler>();
            var request = new SearchAuditEntryRequest();

            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(0));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(0));
        }

        [Test]
        public void It_should_map_response_correctly()
        {
            var auditEntryReference = new AuditEntryModel
                                          {
                                              RequestNamespace = "RequestNamespace",
                                              RequestType = "RequestType",
                                              RequestSerialized = "RequestSerialized",
                                              RequestCorrelationGuid = Guid.NewGuid(),

                                              ResponseNamespace = "ResponseNamespace",
                                              ResponseType = "ResponseType",
                                              ResponseSerialized = "ResponseSerialized",
                                              ResponseCorrelationGuid = Guid.NewGuid(),

                                              Exception = "Exception",
                                              ServerMachineName = "ServerMachineName"
                                          };
            using (var tx = Session.BeginTransaction())
            {
                Session.Save(auditEntryReference);
                tx.Commit();
            }

            StubMessageBus.TestHandler<SearchAuditEntryRequestHandler>();
            var request = new SearchAuditEntryRequest();

            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));

            var firstAuditEntry = response.Results[0];
            Assert.That(firstAuditEntry.Id, Is.EqualTo(auditEntryReference.Id));

            Assert.That(firstAuditEntry.Request.Namespace, Is.EqualTo(auditEntryReference.RequestNamespace));
            Assert.That(firstAuditEntry.Request.Type, Is.EqualTo(auditEntryReference.RequestType));
            Assert.That(firstAuditEntry.Request.Serialized, Is.EqualTo(auditEntryReference.RequestSerialized));
            Assert.That(firstAuditEntry.Request.CorrelationGuid, Is.EqualTo(auditEntryReference.RequestCorrelationGuid));

            Assert.That(firstAuditEntry.Response.Namespace, Is.EqualTo(auditEntryReference.ResponseNamespace));
            Assert.That(firstAuditEntry.Response.Type, Is.EqualTo(auditEntryReference.ResponseType));
            Assert.That(firstAuditEntry.Response.Serialized, Is.EqualTo(auditEntryReference.ResponseSerialized));
            Assert.That(firstAuditEntry.Response.CorrelationGuid, Is.EqualTo(auditEntryReference.ResponseCorrelationGuid));

            Assert.That(firstAuditEntry.Exception, Is.EqualTo(auditEntryReference.Exception));
            Assert.That(firstAuditEntry.ServerMachineName, Is.EqualTo(auditEntryReference.ServerMachineName));
        }

        [Test]
        public void It_should_paginate_accordingly()
        {
            using (var tx = Session.BeginTransaction())
            {
                for (int i = 0; i < 50; i++)
                {
                    var auditEntryReference = new AuditEntryModel();
                    Session.Save(auditEntryReference);
                }
                tx.Commit();
            }


            StubMessageBus.TestHandler<SearchAuditEntryRequestHandler>();

            var request = new SearchAuditEntryRequest { PerPage = 10 };
            var response = MessageBus.Send(request);
            Assert.That(response.TotalEntries, Is.EqualTo(50));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(10));
            Assert.That(response.Results.Count, Is.EqualTo(10));

            request = new SearchAuditEntryRequest { CurrentPage = 1, PerPage = 10 };
            response = MessageBus.Send(request);
            Assert.That(response.TotalEntries, Is.EqualTo(50));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(10));
            Assert.That(response.Results.Count, Is.EqualTo(10));

            request = new SearchAuditEntryRequest { CurrentPage = 5, PerPage = 10 };
            response = MessageBus.Send(request);
            Assert.That(response.TotalEntries, Is.EqualTo(50));
            Assert.That(response.CurrentPage, Is.EqualTo(5));
            Assert.That(response.PerPage, Is.EqualTo(10));
            Assert.That(response.Results.Count, Is.EqualTo(0));
        }

        [Test]
        public void It_should_filter_by_RequestNamespaceLike()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2, auditEntryReference3 = null;

            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel {RequestNamespace = "Colombo.Clerk.Foo"};
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { RequestNamespace = "Colombo.Clerk" };
                Session.Save(auditEntryReference2);

                auditEntryReference3 = new AuditEntryModel { RequestNamespace = "Bar" };
                Session.Save(auditEntryReference3);

                tx.Commit();
            }

            StubMessageBus.TestHandler<SearchAuditEntryRequestHandler>();
            var request = new SearchAuditEntryRequest { RequestNamespaceLike = "Colombo.Clerk" };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(2));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(2));
            Assert.That(response.Results.Select(x => x.Id), 
                Contains.Item(auditEntryReference1.Id)
                .And.Contains(auditEntryReference2.Id));
        }
    }
}
