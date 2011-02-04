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
    public class FullTextSearchAuditEntryRequestHandlerTest : BaseDbTest
    {
        [Test]
        public void It_should_return_zero_when_no_AuditEntry_in_db()
        {
            StubMessageBus.TestHandler<FullTextSearchAuditEntryRequestHandler>();
            var request = new FullTextSearchAuditEntryRequest { SearchQuery = "Foo" };

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
                RequestUtcTimestamp = DateTime.UtcNow,

                ResponseNamespace = "ResponseNamespace",
                ResponseType = "ResponseType",
                ResponseSerialized = "ResponseSerialized",
                ResponseCorrelationGuid = Guid.NewGuid(),
                ResponseUtcTimestamp = DateTime.UtcNow.AddDays(2),

                Exception = "Exception",

                Context = new List<ContextEntryModel>
                                              {
                                                  new ContextEntryModel { ContextKey = "key1", ContextValue = "value1"}
                                              }
            };
            using (var tx = Session.BeginTransaction())
            {
                Session.Save(auditEntryReference);
                tx.Commit();
            }

            StubMessageBus.TestHandler<FullTextSearchAuditEntryRequestHandler>();
            var request = new FullTextSearchAuditEntryRequest { SearchQuery = "RequestNamespace:RequestNamespace" };

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
            Assert.That(firstAuditEntry.Request.UtcTimestamp, Is.EqualTo(auditEntryReference.RequestUtcTimestamp));

            Assert.That(firstAuditEntry.Response.Namespace, Is.EqualTo(auditEntryReference.ResponseNamespace));
            Assert.That(firstAuditEntry.Response.Type, Is.EqualTo(auditEntryReference.ResponseType));
            Assert.That(firstAuditEntry.Response.Serialized, Is.EqualTo(auditEntryReference.ResponseSerialized));
            Assert.That(firstAuditEntry.Response.CorrelationGuid, Is.EqualTo(auditEntryReference.ResponseCorrelationGuid));
            Assert.That(firstAuditEntry.Response.UtcTimestamp, Is.EqualTo(auditEntryReference.ResponseUtcTimestamp));

            Assert.That(firstAuditEntry.Exception, Is.EqualTo(auditEntryReference.Exception));

            Assert.That(firstAuditEntry.RequestContext["key1"], Is.EqualTo("value1"));
        }

        [Test]
        public void It_should_paginate_accordingly()
        {
            using (var tx = Session.BeginTransaction())
            {
                for (var i = 0; i < 50; i++)
                {
                    var auditEntryReference = new AuditEntryModel { RequestNamespace = "RequestNamespace" };
                    Session.Save(auditEntryReference);
                }
                tx.Commit();
            }


            StubMessageBus.TestHandler<FullTextSearchAuditEntryRequestHandler>();
            var request = new FullTextSearchAuditEntryRequest { SearchQuery = "RequestNamespace:RequestNamespace", PerPage = 10 };
            var response = MessageBus.Send(request);
            Assert.That(response.TotalEntries, Is.EqualTo(50));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(10));
            Assert.That(response.Results.Count, Is.EqualTo(10));

            request = new FullTextSearchAuditEntryRequest { SearchQuery = "RequestNamespace:RequestNamespace", CurrentPage = 1, PerPage = 10 };
            response = MessageBus.Send(request);
            Assert.That(response.TotalEntries, Is.EqualTo(50));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(10));
            Assert.That(response.Results.Count, Is.EqualTo(10));

            request = new FullTextSearchAuditEntryRequest { SearchQuery = "RequestNamespace:RequestNamespace", CurrentPage = 5, PerPage = 10 };
            response = MessageBus.Send(request);
            Assert.That(response.TotalEntries, Is.EqualTo(50));
            Assert.That(response.CurrentPage, Is.EqualTo(5));
            Assert.That(response.PerPage, Is.EqualTo(10));
            Assert.That(response.Results.Count, Is.EqualTo(0));
        }

        [Test]
        public void It_should_index_RequestNamespace()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2, auditEntryReference3 = null;

            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { RequestNamespace = "Colombo.Clerk.Foo" };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { RequestNamespace = "Colombo.Clerk" };
                Session.Save(auditEntryReference2);

                auditEntryReference3 = new AuditEntryModel { RequestNamespace = "Bar" };
                Session.Save(auditEntryReference3);

                tx.Commit();
            }

            StubMessageBus.TestHandler<FullTextSearchAuditEntryRequestHandler>();
            var request = new FullTextSearchAuditEntryRequest { SearchQuery = "Colombo" };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(2));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(2));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference1.Id)
                .And.Contains(auditEntryReference2.Id));
        }

        [Test]
        public void It_should_index_RequestType()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2 = null;

            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { RequestType = "Foo" };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { RequestType = "Bar" };
                Session.Save(auditEntryReference2);

                tx.Commit();
            }

            StubMessageBus.TestHandler<FullTextSearchAuditEntryRequestHandler>();
            var request = new FullTextSearchAuditEntryRequest { SearchQuery = "Bar" };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference2.Id));
        }

        [Test]
        public void It_should_index_RequestCorrelationGuid()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2 = null;

            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { RequestCorrelationGuid = Guid.NewGuid() };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { RequestCorrelationGuid = Guid.NewGuid() };
                Session.Save(auditEntryReference2);

                tx.Commit();
            }

            StubMessageBus.TestHandler<FullTextSearchAuditEntryRequestHandler>();
            var request = new FullTextSearchAuditEntryRequest { SearchQuery = auditEntryReference1.RequestCorrelationGuid.ToString() };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference1.Id));
        }

        [Test]
        public void It_should_index_ResponseNamespace()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2, auditEntryReference3 = null;

            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { ResponseNamespace = "Colombo.Clerk.Foo" };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { ResponseNamespace = "Colombo.Clerk" };
                Session.Save(auditEntryReference2);

                auditEntryReference3 = new AuditEntryModel { ResponseNamespace = "Bar" };
                Session.Save(auditEntryReference3);

                tx.Commit();
            }

            StubMessageBus.TestHandler<FullTextSearchAuditEntryRequestHandler>();
            var request = new FullTextSearchAuditEntryRequest { SearchQuery = "Colombo" };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(2));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(2));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference1.Id)
                .And.Contains(auditEntryReference2.Id));
        }

        [Test]
        public void It_should_index_ResponseType()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2 = null;

            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { ResponseType = "Foo" };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { ResponseType = "Bar" };
                Session.Save(auditEntryReference2);

                tx.Commit();
            }

            StubMessageBus.TestHandler<FullTextSearchAuditEntryRequestHandler>();
            var request = new FullTextSearchAuditEntryRequest { SearchQuery = "Bar" };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference2.Id));
        }

        [Test]
        public void It_should_index_ResponseCorrelationGuid()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2 = null;

            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { ResponseCorrelationGuid = Guid.NewGuid() };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { ResponseCorrelationGuid = Guid.NewGuid() };
                Session.Save(auditEntryReference2);

                tx.Commit();
            }

            StubMessageBus.TestHandler<FullTextSearchAuditEntryRequestHandler>();
            var request = new FullTextSearchAuditEntryRequest { SearchQuery = auditEntryReference1.ResponseCorrelationGuid.ToString() };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference1.Id));
        }

        [Test]
        public void It_should_index_Exception()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2, auditEntryReference3 = null;

            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { Exception = "Foo" };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { Exception = "Foo Bar" };
                Session.Save(auditEntryReference2);

                auditEntryReference3 = new AuditEntryModel { Exception = "Bar" };
                Session.Save(auditEntryReference3);

                tx.Commit();
            }

            StubMessageBus.TestHandler<FullTextSearchAuditEntryRequestHandler>();
            var request = new FullTextSearchAuditEntryRequest { SearchQuery = "Foo" };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(2));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(2));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference1.Id)
                .And.Contains(auditEntryReference2.Id));
        }

        [Test]
        [Ignore]
        public void It_should_index_Context()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2, auditEntryReference3 = null;

            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel
                {
                    Context = new List<ContextEntryModel> {
                        new ContextEntryModel { ContextKey = "key1", ContextValue= "value1" },
                        new ContextEntryModel { ContextKey = "key2", ContextValue= "value2" }
                    }
                };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel
                {
                    Context = new List<ContextEntryModel> {
                        new ContextEntryModel { ContextKey = "key1", ContextValue= "value3" }
                    }
                };
                Session.Save(auditEntryReference2);

                auditEntryReference3 = new AuditEntryModel
                {
                    Context = new List<ContextEntryModel> {
                        new ContextEntryModel { ContextKey = "key3", ContextValue= "foo" }
                    }
                };
                Session.Save(auditEntryReference3);

                tx.Commit();
            }

            StubMessageBus.TestHandler<FullTextSearchAuditEntryRequestHandler>();
            var request = new FullTextSearchAuditEntryRequest { SearchQuery = "key1" };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(2));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(2));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference1.Id)
                .And.Contains(auditEntryReference2.Id));

            request = new FullTextSearchAuditEntryRequest { SearchQuery = "foo" };
            response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(0));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference3.Id));
        }
    }
}
