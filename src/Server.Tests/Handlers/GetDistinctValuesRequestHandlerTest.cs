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
    public class GetDistinctValuesRequestHandlerTest : BaseDbTest
    {
        [Test]
        public void It_should_return_RequestNamespace()
        {
            StubMessageBus.TestHandler<GetDistinctValuesRequestHandler>();

            var request = new GetDistinctValuesRequest { ValueType = GetDistinctValueType.RequestNamespace };
            var response = MessageBus.Send(request);

            Assert.That(response.Values, Is.Empty);

            using (var tx = Session.BeginTransaction())
            {
                Session.Save(new AuditEntryModel { RequestNamespace = "Colombo.Clerk.Foo" });
                Session.Save(new AuditEntryModel { RequestNamespace = "Colombo.Clerk" });
                Session.Save(new AuditEntryModel { RequestNamespace = "Bar" });

                tx.Commit();
            }

            request = new GetDistinctValuesRequest { ValueType = GetDistinctValueType.RequestNamespace };
            response = MessageBus.Send(request);
            Assert.That(response.Values.Count(), Is.EqualTo(3));
            Assert.That(response.Values, Contains.Item("Colombo.Clerk.Foo"));
            Assert.That(response.Values, Contains.Item("Colombo.Clerk"));
            Assert.That(response.Values, Contains.Item("Bar"));
        }

        [Test]
        public void It_should_return_RequestType()
        {
            StubMessageBus.TestHandler<GetDistinctValuesRequestHandler>();

            var request = new GetDistinctValuesRequest { ValueType = GetDistinctValueType.RequestType };
            var response = MessageBus.Send(request);

            Assert.That(response.Values, Is.Empty);

            using (var tx = Session.BeginTransaction())
            {
                Session.Save(new AuditEntryModel { RequestType = "Foo" });
                Session.Save(new AuditEntryModel { RequestType = "Bar" });

                tx.Commit();
            }

            request = new GetDistinctValuesRequest { ValueType = GetDistinctValueType.RequestType };
            response = MessageBus.Send(request);
            Assert.That(response.Values.Count(), Is.EqualTo(2));
            Assert.That(response.Values, Contains.Item("Foo"));
            Assert.That(response.Values, Contains.Item("Bar"));
        }

        [Test]
        public void It_should_return_ResponseNamespace()
        {
            StubMessageBus.TestHandler<GetDistinctValuesRequestHandler>();

            var request = new GetDistinctValuesRequest { ValueType = GetDistinctValueType.ResponseNamespace };
            var response = MessageBus.Send(request);

            Assert.That(response.Values, Is.Empty);

            using (var tx = Session.BeginTransaction())
            {
                Session.Save(new AuditEntryModel { ResponseNamespace = "Colombo.Clerk.Foo" });
                Session.Save(new AuditEntryModel { ResponseNamespace = "Colombo.Clerk" });
                Session.Save(new AuditEntryModel { ResponseNamespace = "Bar" });

                tx.Commit();
            }

            request = new GetDistinctValuesRequest { ValueType = GetDistinctValueType.ResponseNamespace };
            response = MessageBus.Send(request);
            Assert.That(response.Values.Count(), Is.EqualTo(3));
            Assert.That(response.Values, Contains.Item("Colombo.Clerk.Foo"));
            Assert.That(response.Values, Contains.Item("Colombo.Clerk"));
            Assert.That(response.Values, Contains.Item("Bar"));
        }

        [Test]
        public void It_should_return_ResponseType()
        {
            StubMessageBus.TestHandler<GetDistinctValuesRequestHandler>();

            var request = new GetDistinctValuesRequest { ValueType = GetDistinctValueType.ResponseType };
            var response = MessageBus.Send(request);

            Assert.That(response.Values, Is.Empty);

            using (var tx = Session.BeginTransaction())
            {
                Session.Save(new AuditEntryModel { ResponseType = "Foo" });
                Session.Save(new AuditEntryModel { ResponseType = "Bar" });

                tx.Commit();
            }

            request = new GetDistinctValuesRequest { ValueType = GetDistinctValueType.ResponseType };
            response = MessageBus.Send(request);
            Assert.That(response.Values.Count(), Is.EqualTo(2));
            Assert.That(response.Values, Contains.Item("Foo"));
            Assert.That(response.Values, Contains.Item("Bar"));
        }

        [Test]
        public void It_should_return_ContextKey()
        {
            StubMessageBus.TestHandler<GetDistinctValuesRequestHandler>();

            var request = new GetDistinctValuesRequest { ValueType = GetDistinctValueType.ContextKey };
            var response = MessageBus.Send(request);

            Assert.That(response.Values, Is.Empty);

            using (var tx = Session.BeginTransaction())
            {
                Session.Save(new AuditEntryModel
                {
                    Context = new List<ContextEntryModel> {
                        new ContextEntryModel { ContextKey = "key1", ContextValue= "value1" },
                        new ContextEntryModel { ContextKey = "key2", ContextValue= "value2" }
                    }
                });
                Session.Save(new AuditEntryModel
                {
                    Context = new List<ContextEntryModel> {
                        new ContextEntryModel { ContextKey = "key3", ContextValue= "foo" }
                    }
                });

                tx.Commit();
            }

            request = new GetDistinctValuesRequest { ValueType = GetDistinctValueType.ContextKey };
            response = MessageBus.Send(request);
            Assert.That(response.Values.Count(), Is.EqualTo(3));
            Assert.That(response.Values, Contains.Item("key1"));
            Assert.That(response.Values, Contains.Item("key2"));
            Assert.That(response.Values, Contains.Item("key3"));
        }
    }
}
