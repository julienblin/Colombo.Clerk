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
using System.Transactions;
using Colombo.Clerk.Server.Models;
using Colombo.Clerk.Service;
using NUnit.Framework;

namespace Colombo.Clerk.Server.Tests
{
    [TestFixture]
    public class ClerkServiceTest : BaseDbTest
    {
        [Test]
        public void It_should_write_AuditInfo()
        {
            var auditInfo = new AuditInfo
                                {
                                    Request = new AuditInfo.InnerInfo()
                                    {
                                        CorrelationGuid = Guid.NewGuid(),
                                        Namespace = "RequestNamespace",
                                        Serialized = "RequestSerialized",
                                        Type = "RequestType",
                                        UtcTimestamp = DateTime.UtcNow
                                    },
                                    Response = new AuditInfo.InnerInfo()
                                    {
                                        CorrelationGuid = Guid.NewGuid(),
                                        Namespace = "ResponseNamespace",
                                        Serialized = "ResponseSerialized",
                                        Type = "ResponseType",
                                        UtcTimestamp = DateTime.UtcNow.AddDays(2)
                                    },
                                    Exception = "Exception",
                                    Message = "Message",
                                    Context =
                                        {
                                            { "key1", "value1" },
                                            { "key2", "value2" }
                                        }
                                };

            var service = new ClerkService();
            EndPointConfig.Kernel = Container.Kernel;

            using (var scope = new TransactionScope())
            {
                service.Write(auditInfo);
                scope.Complete();
            }

            using (var tx = Session.BeginTransaction())
            {
                var auditEntryModel = Session.QueryOver<AuditEntryModel>().Fetch(x => x.Context).Eager.List().First();

                Assert.That(auditEntryModel.RequestCorrelationGuid, Is.EqualTo(auditInfo.Request.CorrelationGuid));
                Assert.That(auditEntryModel.RequestNamespace, Is.EqualTo(auditInfo.Request.Namespace));
                Assert.That(auditEntryModel.RequestSerialized, Is.EqualTo(auditInfo.Request.Serialized));
                Assert.That(auditEntryModel.RequestType, Is.EqualTo(auditInfo.Request.Type));
                Assert.That(auditEntryModel.RequestUtcTimestamp, Is.EqualTo(auditInfo.Request.UtcTimestamp));

                Assert.That(auditEntryModel.ResponseCorrelationGuid, Is.EqualTo(auditInfo.Response.CorrelationGuid));
                Assert.That(auditEntryModel.ResponseNamespace, Is.EqualTo(auditInfo.Response.Namespace));
                Assert.That(auditEntryModel.ResponseSerialized, Is.EqualTo(auditInfo.Response.Serialized));
                Assert.That(auditEntryModel.ResponseType, Is.EqualTo(auditInfo.Response.Type));
                Assert.That(auditEntryModel.ResponseUtcTimestamp, Is.EqualTo(auditInfo.Response.UtcTimestamp));

                Assert.That(auditEntryModel.Exception, Is.EqualTo(auditInfo.Exception));
                Assert.That(auditEntryModel.Message, Is.EqualTo(auditInfo.Message));

                Assert.That(auditEntryModel.Context[0].ContextKey, Is.EqualTo("key1"));
                Assert.That(auditEntryModel.Context[0].ContextValue, Is.EqualTo("value1"));

                Assert.That(auditEntryModel.Context[1].ContextKey, Is.EqualTo("key2"));
                Assert.That(auditEntryModel.Context[1].ContextValue, Is.EqualTo("value2"));

                tx.Commit();
            }
        }

        [Test]
        public void It_should_strip_strings_if_too_large_where_needed()
        {
            var moreThan255Chars = new string('a', 300);
            var moreThan2000Chars = new string('a', 3000);
            Assert.That(() => moreThan255Chars.Length, Is.AtLeast(256));
            Assert.That(() => moreThan2000Chars.Length, Is.AtLeast(2001));
            var strippedMoreThan255Chars = moreThan255Chars.Substring(0, 255);
            var strippedMoreThan2000Chars = moreThan2000Chars.Substring(0, 2000);
            Assert.That(() => strippedMoreThan2000Chars.Length, Is.AtMost(2000));

            var auditInfo = new AuditInfo
            {
                Request = new AuditInfo.InnerInfo()
                {
                    CorrelationGuid = Guid.NewGuid(),
                    Namespace = moreThan255Chars,
                    Serialized = moreThan255Chars,
                    Type = moreThan255Chars
                },
                Response = new AuditInfo.InnerInfo()
                {
                    CorrelationGuid = Guid.NewGuid(),
                    Namespace = moreThan255Chars,
                    Serialized = moreThan255Chars,
                    Type = moreThan255Chars
                },
                Exception = moreThan255Chars,
                Message = moreThan2000Chars,
                Context = {
                    { moreThan255Chars, moreThan255Chars }
                }
            };

            var service = new ClerkService();
            EndPointConfig.Kernel = Container.Kernel;

            using (var scope = new TransactionScope())
            {
                service.Write(auditInfo);
                scope.Complete();
            }

            using (var tx = Session.BeginTransaction())
            {
                var auditEntryModel = Session.QueryOver<AuditEntryModel>().Fetch(x => x.Context).Eager.List().First();

                Assert.That(auditEntryModel.RequestCorrelationGuid, Is.EqualTo(auditInfo.Request.CorrelationGuid));
                Assert.That(auditEntryModel.RequestNamespace, Is.EqualTo(strippedMoreThan255Chars));
                Assert.That(auditEntryModel.RequestSerialized, Is.EqualTo(auditInfo.Request.Serialized));
                Assert.That(auditEntryModel.RequestType, Is.EqualTo(strippedMoreThan255Chars));

                Assert.That(auditEntryModel.ResponseCorrelationGuid, Is.EqualTo(auditInfo.Response.CorrelationGuid));
                Assert.That(auditEntryModel.ResponseNamespace, Is.EqualTo(strippedMoreThan255Chars));
                Assert.That(auditEntryModel.ResponseSerialized, Is.EqualTo(auditInfo.Response.Serialized));
                Assert.That(auditEntryModel.ResponseType, Is.EqualTo(strippedMoreThan255Chars));

                Assert.That(auditEntryModel.Exception, Is.EqualTo(auditInfo.Exception));
                Assert.That(auditEntryModel.Message, Is.EqualTo(strippedMoreThan2000Chars));

                Assert.That(auditEntryModel.Context[0].ContextKey, Is.EqualTo(strippedMoreThan255Chars));
                Assert.That(auditEntryModel.Context[0].ContextValue, Is.EqualTo(moreThan255Chars));
                tx.Commit();
            }
        }
    }
}
