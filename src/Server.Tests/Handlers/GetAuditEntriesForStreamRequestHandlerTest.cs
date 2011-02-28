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
    public class GetAuditEntriesForStreamRequestHandlerTest : BaseDbTest
    {
        [Test]
        public void It_should_return_zero_when_no_AuditEntry_in_db()
        {
            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();

            var streamModel = new StreamModel();
            using (var tx = Session.BeginTransaction())
            {
                Session.Save(streamModel);
                tx.Commit();
            }

            var request = new GetAuditEntriesForStreamRequest { Id = streamModel.Id };

            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(0));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.TotalPages, Is.EqualTo(0));
            Assert.That(response.Results.Count, Is.EqualTo(0));
        }

        [Test]
        public void It_should_throw_an_exception_if_stream_not_found()
        {
            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();

            var request = new GetAuditEntriesForStreamRequest { Id = Guid.NewGuid() };

            Assert.That(() => MessageBus.Send(request), Throws.Exception);
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
                Message = "Message",

                Context = new List<ContextEntryModel>
                                              {
                                                  new ContextEntryModel { ContextKey = "key1", ContextValue = "value1"}
                                              }
            };
            var streamModel = new StreamModel();
            using (var tx = Session.BeginTransaction())
            {
                Session.Save(auditEntryReference);
                Session.Save(streamModel);
                tx.Commit();
            }

            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();
            var request = new GetAuditEntriesForStreamRequest { Id = streamModel.Id };

            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
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
            Assert.That(firstAuditEntry.Message, Is.EqualTo(auditEntryReference.Message));

            Assert.That(firstAuditEntry.RequestContext["key1"], Is.EqualTo("value1"));
        }

        [Test]
        public void It_should_paginate_accordingly()
        {
            var streamModel = new StreamModel();
            using (var tx = Session.BeginTransaction())
            {
                for (var i = 0; i < 50; i++)
                {
                    var auditEntryReference = new AuditEntryModel
                    {
                        Context = new List<ContextEntryModel>
                                    {
                                        new ContextEntryModel(),
                                        new ContextEntryModel()
                                    }
                    };
                    Session.Save(auditEntryReference);
                }
                Session.Save(streamModel);
                tx.Commit();
            }


            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();

            var request = new GetAuditEntriesForStreamRequest { Id = streamModel.Id, PerPage = 10 };
            var response = MessageBus.Send(request);
            Assert.That(response.TotalEntries, Is.EqualTo(50));
            Assert.That(response.TotalPages, Is.EqualTo(5));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(10));
            Assert.That(response.Results.Count, Is.EqualTo(10));

            request = new GetAuditEntriesForStreamRequest { Id = streamModel.Id, CurrentPage = 2, PerPage = 10 };
            response = MessageBus.Send(request);
            Assert.That(response.TotalEntries, Is.EqualTo(50));
            Assert.That(response.TotalPages, Is.EqualTo(5));
            Assert.That(response.CurrentPage, Is.EqualTo(2));
            Assert.That(response.PerPage, Is.EqualTo(10));
            Assert.That(response.Results.Count, Is.EqualTo(10));

            request = new GetAuditEntriesForStreamRequest { Id = streamModel.Id, CurrentPage = 6, PerPage = 10 };
            response = MessageBus.Send(request);
            Assert.That(response.TotalEntries, Is.EqualTo(50));
            Assert.That(response.TotalPages, Is.EqualTo(5));
            Assert.That(response.CurrentPage, Is.EqualTo(6));
            Assert.That(response.PerPage, Is.EqualTo(10));
            Assert.That(response.Results.Count, Is.EqualTo(0));
        }

        [Test]
        public void It_should_filter_by_RequestNamespace()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2, auditEntryReference3 = null;
            var streamModel = new StreamModel
            {
                Filters = new List<FilterModel>
                {
                    new FilterModel { FilterName = "RequestNamespaceFilter", StringValue = "Colombo.Clerk" }
                }
            };

            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { RequestNamespace = "Colombo.Clerk.Foo" };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { RequestNamespace = "Colombo.Clerk" };
                Session.Save(auditEntryReference2);

                auditEntryReference3 = new AuditEntryModel { RequestNamespace = "Bar" };
                Session.Save(auditEntryReference3);

                Session.Save(streamModel);
                tx.Commit();
            }

            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();
            var request = new GetAuditEntriesForStreamRequest { Id = streamModel.Id };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference2.Id));
        }

        [Test]
        public void It_should_filter_by_RequestType()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2 = null;
            var streamModel = new StreamModel
            {
                Filters = new List<FilterModel>
                {
                    new FilterModel { FilterName = "RequestTypeFilter", StringValue = "Bar" }
                }
            };

            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { RequestType = "Foo" };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { RequestType = "Bar" };
                Session.Save(auditEntryReference2);

                Session.Save(streamModel);
                tx.Commit();
            }

            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();
            var request = new GetAuditEntriesForStreamRequest { Id = streamModel.Id };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference2.Id));
        }

        [Test]
        public void It_should_filter_by_RequestCorrelationGuid()
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

            var streamModel = new StreamModel
            {
                Filters = new List<FilterModel>
                {
                    new FilterModel { FilterName = "RequestCorrelationGuidFilter", GuidValue = auditEntryReference1.RequestCorrelationGuid }
                }
            };
            using (var tx = Session.BeginTransaction())
            {
                Session.Save(streamModel);
                tx.Commit();
            }

            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();
            var request = new GetAuditEntriesForStreamRequest { Id = streamModel.Id };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference1.Id));
        }

        [Test]
        public void It_should_filter_by_ResponseNamespace()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2, auditEntryReference3 = null;
            var streamModel = new StreamModel
            {
                Filters = new List<FilterModel>
                {
                    new FilterModel { FilterName = "ResponseNamespaceFilter", StringValue = "Colombo.Clerk" }
                }
            };
            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { ResponseNamespace = "Colombo.Clerk.Foo" };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { ResponseNamespace = "Colombo.Clerk" };
                Session.Save(auditEntryReference2);

                auditEntryReference3 = new AuditEntryModel { ResponseNamespace = "Bar" };
                Session.Save(auditEntryReference3);

                Session.Save(streamModel);

                tx.Commit();
            }

            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();
            var request = new GetAuditEntriesForStreamRequest { Id = streamModel.Id };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference2.Id));
        }

        [Test]
        public void It_should_filter_by_ResponseType()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2 = null;
            var streamModel = new StreamModel
            {
                Filters = new List<FilterModel>
                {
                    new FilterModel { FilterName = "ResponseTypeFilter", StringValue = "Bar" }
                }
            };
            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { ResponseType = "Foo" };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { ResponseType = "Bar" };
                Session.Save(auditEntryReference2);

                Session.Save(streamModel);
                tx.Commit();
            }

            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();
            var request = new GetAuditEntriesForStreamRequest { Id = streamModel.Id };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference2.Id));
        }

        [Test]
        public void It_should_filter_by_ResponseCorrelationGuid()
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

            var streamModel = new StreamModel
            {
                Filters = new List<FilterModel>
                {
                    new FilterModel { FilterName = "ResponseCorrelationGuidFilter", GuidValue = auditEntryReference1.ResponseCorrelationGuid }
                }
            };
            using (var tx = Session.BeginTransaction())
            {
                Session.Save(streamModel);
                tx.Commit();
            }

            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();
            var request = new GetAuditEntriesForStreamRequest { Id = streamModel.Id };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference1.Id));
        }

        [Test]
        public void It_should_filter_by_ExceptionContains()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2, auditEntryReference3 = null;
            var streamModel = new StreamModel
            {
                Filters = new List<FilterModel>
                {
                    new FilterModel { FilterName = "ExceptionContainsFilter", StringValue = "Foo" }
                }
            };
            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { Exception = "Foo" };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { Exception = "FooBar" };
                Session.Save(auditEntryReference2);

                auditEntryReference3 = new AuditEntryModel { Exception = "Bar" };
                Session.Save(auditEntryReference3);

                Session.Save(streamModel);
                tx.Commit();
            }

            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();
            var request = new GetAuditEntriesForStreamRequest { Id = streamModel.Id };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(2));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(2));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference1.Id)
                .And.Contains(auditEntryReference2.Id));
        }

        [Test]
        public void It_should_filter_by_HasException()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2, auditEntryReference3 = null;
            var streamModel1 = new StreamModel
            {
                Filters = new List<FilterModel>
                {
                    new FilterModel { FilterName = "HasExceptionFilter", BoolValue = true },
                }
            };
            var streamModel2 = new StreamModel
            {
                Filters = new List<FilterModel>
                {
                    new FilterModel { FilterName = "HasExceptionFilter", BoolValue = false },
                }
            };

            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { Exception = "Foo" };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { Exception = "" };
                Session.Save(auditEntryReference2);

                auditEntryReference3 = new AuditEntryModel();
                Session.Save(auditEntryReference3);

                Session.Save(streamModel1);
                Session.Save(streamModel2);
                tx.Commit();
            }

            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();
            var request = new GetAuditEntriesForStreamRequest { Id = streamModel1.Id };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference1.Id));

            request = new GetAuditEntriesForStreamRequest { Id = streamModel2.Id };
            response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(2));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(2));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference2.Id)
                .And.Contains(auditEntryReference3.Id));
        }

        [Test]
        public void It_should_filter_by_MessageContains()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2, auditEntryReference3 = null;
            var streamModel = new StreamModel
            {
                Filters = new List<FilterModel>
                {
                    new FilterModel { FilterName = "MessageContainsFilter", StringValue = "Foo" },
                }
            };
            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { Message = "Foo" };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { Message = "FooBar" };
                Session.Save(auditEntryReference2);

                auditEntryReference3 = new AuditEntryModel { Message = "Bar" };
                Session.Save(auditEntryReference3);

                Session.Save(streamModel);
                tx.Commit();
            }

            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();
            var request = new GetAuditEntriesForStreamRequest { Id = streamModel.Id };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(2));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(2));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference1.Id)
                .And.Contains(auditEntryReference2.Id));
        }

        [Test]
        public void It_should_filter_by_HasMessage()
        {
            AuditEntryModel auditEntryReference1, auditEntryReference2, auditEntryReference3 = null;
            var streamModel1 = new StreamModel
            {
                Filters = new List<FilterModel>
                {
                    new FilterModel { FilterName = "HasMessageFilter", BoolValue = true },
                }
            };
            var streamModel2 = new StreamModel
            {
                Filters = new List<FilterModel>
                {
                    new FilterModel { FilterName = "HasMessageFilter", BoolValue = false },
                }
            };

            using (var tx = Session.BeginTransaction())
            {

                auditEntryReference1 = new AuditEntryModel { Message = "Foo" };
                Session.Save(auditEntryReference1);

                auditEntryReference2 = new AuditEntryModel { Message = "" };
                Session.Save(auditEntryReference2);

                auditEntryReference3 = new AuditEntryModel();
                Session.Save(auditEntryReference3);

                Session.Save(streamModel1);
                Session.Save(streamModel2);
                tx.Commit();
            }

            StubMessageBus.TestHandler<GetAuditEntriesForStreamRequestHandler>();
            var request = new GetAuditEntriesForStreamRequest { Id = streamModel1.Id };
            var response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(1));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(1));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference1.Id));

            request = new GetAuditEntriesForStreamRequest { Id = streamModel2.Id };
            response = MessageBus.Send(request);

            Assert.That(response.TotalEntries, Is.EqualTo(2));
            Assert.That(response.CurrentPage, Is.EqualTo(1));
            Assert.That(response.PerPage, Is.EqualTo(request.PerPage));
            Assert.That(response.Results.Count, Is.EqualTo(2));
            Assert.That(response.Results.Select(x => x.Id),
                Contains.Item(auditEntryReference2.Id)
                .And.Contains(auditEntryReference3.Id));
        }
    }
}
