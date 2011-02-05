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
using System.Linq;
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
                                  RequestUtcTimestamp = DateTime.UtcNow,
                                  ResponseCorrelationGuid = Guid.NewGuid(),
                                  ResponseNamespace = "Responsenamespace",
                                  ResponseSerialized = "ResponseSerialized",
                                  ResponseType = "ResponseType",
                                  ResponseUtcTimestamp = DateTime.UtcNow.AddDays(2),
                                  Exception = "Exception",
                                  Message = "Message",
                                  ServerMachineName = "ServerMachineName",
                                  RequestContext =
                                      {
                                          { "key1", "value1" }
                                      }
                              };

            var response = MessageBus.Send(request);

            Assert.That(response.IsValid());

            using (var tx = Session.BeginTransaction())
            {
                var auditEntryModel = Session.Get<AuditEntryModel>(response.Id);
                Assert.That(auditEntryModel.RequestCorrelationGuid, Is.EqualTo(request.RequestCorrelationGuid));
                Assert.That(auditEntryModel.RequestNamespace, Is.EqualTo(request.RequestNamespace));
                Assert.That(auditEntryModel.RequestSerialized, Is.EqualTo(request.RequestSerialized));
                Assert.That(auditEntryModel.RequestType, Is.EqualTo(request.RequestType));
                Assert.That(auditEntryModel.RequestUtcTimestamp, Is.EqualTo(request.RequestUtcTimestamp));

                Assert.That(auditEntryModel.ResponseCorrelationGuid, Is.EqualTo(request.ResponseCorrelationGuid));
                Assert.That(auditEntryModel.ResponseNamespace, Is.EqualTo(request.ResponseNamespace));
                Assert.That(auditEntryModel.ResponseSerialized, Is.EqualTo(request.ResponseSerialized));
                Assert.That(auditEntryModel.ResponseType, Is.EqualTo(request.ResponseType));
                Assert.That(auditEntryModel.ResponseUtcTimestamp, Is.EqualTo(request.ResponseUtcTimestamp));

                Assert.That(auditEntryModel.Exception, Is.EqualTo(request.Exception));
                Assert.That(auditEntryModel.Message, Is.EqualTo(request.Message));

                Assert.That(auditEntryModel.Context[0].ContextKey, Is.EqualTo("key1"));
                Assert.That(auditEntryModel.Context[0].ContextValue, Is.EqualTo("value1"));

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
