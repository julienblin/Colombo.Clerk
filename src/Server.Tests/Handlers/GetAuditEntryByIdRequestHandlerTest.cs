#region License
// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
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
                Context = new List<ContextEntryModel>
                {
                    new ContextEntryModel { Key = "key1", Value = "value1"}
                }
            };

            using (var tx = Session.BeginTransaction())
            {
                Session.Save(auditEntryReference);
                tx.Commit();
            }

            StubMessageBus.TestHandler<GetAuditEntryByIdRequestHandler>();
            var request = new GetAuditEntryByIdRequest { Id = auditEntryReference.Id };

            var response = MessageBus.Send(request);

            Assert.That(response.Found, Is.True);

            Assert.That(response.AuditEntry.Request.CorrelationGuid, Is.EqualTo(auditEntryReference.RequestCorrelationGuid));
            Assert.That(response.AuditEntry.Request.Namespace, Is.EqualTo(auditEntryReference.RequestNamespace));
            Assert.That(response.AuditEntry.Request.Serialized, Is.EqualTo(auditEntryReference.RequestSerialized));
            Assert.That(response.AuditEntry.Request.Type, Is.EqualTo(auditEntryReference.RequestType));

            Assert.That(response.AuditEntry.Response.CorrelationGuid, Is.EqualTo(auditEntryReference.ResponseCorrelationGuid));
            Assert.That(response.AuditEntry.Response.Namespace, Is.EqualTo(auditEntryReference.ResponseNamespace));
            Assert.That(response.AuditEntry.Response.Serialized, Is.EqualTo(auditEntryReference.ResponseSerialized));
            Assert.That(response.AuditEntry.Response.Type, Is.EqualTo(auditEntryReference.ResponseType));

            Assert.That(response.AuditEntry.Exception, Is.EqualTo(auditEntryReference.Exception));

            Assert.That(response.AuditEntry.RequestContext["key1"], Is.EqualTo("value1"));
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
