using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                                        Type = "RequestType"
                                    },
                                    Response = new AuditInfo.InnerInfo()
                                    {
                                        CorrelationGuid = Guid.NewGuid(),
                                        Namespace = "ResponseNamespace",
                                        Serialized = "ResponseSerialized",
                                        Type = "ResponseType"
                                    },
                                    Exception = "Exception",
                                    ServerMachineName = "ServerMachineName"
                                };

            var service = new ClerkService();
            EndPointConfig.Kernel = container.Kernel;

            using (var scope = new TransactionScope())
            {
                service.Write(auditInfo);
                scope.Complete();
            }

            using (var tx = Session.BeginTransaction())
            {
                var auditEntryModel = Session.CreateCriteria<AuditEntryModel>().UniqueResult<AuditEntryModel>();

                Assert.That(() => auditEntryModel.RequestCorrelationGuid, Is.EqualTo(auditInfo.Request.CorrelationGuid));
                Assert.That(() => auditEntryModel.RequestNamespace, Is.EqualTo(auditInfo.Request.Namespace));
                Assert.That(() => auditEntryModel.RequestSerialized, Is.EqualTo(auditInfo.Request.Serialized));
                Assert.That(() => auditEntryModel.RequestType, Is.EqualTo(auditInfo.Request.Type));

                Assert.That(() => auditEntryModel.ResponseCorrelationGuid, Is.EqualTo(auditInfo.Response.CorrelationGuid));
                Assert.That(() => auditEntryModel.ResponseNamespace, Is.EqualTo(auditInfo.Response.Namespace));
                Assert.That(() => auditEntryModel.ResponseSerialized, Is.EqualTo(auditInfo.Response.Serialized));
                Assert.That(() => auditEntryModel.ResponseType, Is.EqualTo(auditInfo.Response.Type));

                Assert.That(() => auditEntryModel.Exception, Is.EqualTo(auditInfo.Exception));
                Assert.That(() => auditEntryModel.ServerMachineName, Is.EqualTo(auditInfo.ServerMachineName));
                tx.Commit();
            }
        }

        [Test]
        public void It_should_strip_strings_if_too_large_where_needed()
        {
            var moreThan255Chars = new string('a', 300);
            Assert.That(() => moreThan255Chars.Length, Is.AtLeast(256));
            var strippedMoreThan255Chars = moreThan255Chars.Substring(0, 255);
            Assert.That(() => strippedMoreThan255Chars.Length, Is.AtMost(255));

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
                ServerMachineName = moreThan255Chars
            };

            var service = new ClerkService();
            EndPointConfig.Kernel = container.Kernel;

            using (var scope = new TransactionScope())
            {
                service.Write(auditInfo);
                scope.Complete();
            }

            using (var tx = Session.BeginTransaction())
            {
                var auditEntryModel = Session.CreateCriteria<AuditEntryModel>().UniqueResult<AuditEntryModel>();

                Assert.That(() => auditEntryModel.RequestCorrelationGuid, Is.EqualTo(auditInfo.Request.CorrelationGuid));
                Assert.That(() => auditEntryModel.RequestNamespace, Is.EqualTo(strippedMoreThan255Chars));
                Assert.That(() => auditEntryModel.RequestSerialized, Is.EqualTo(auditInfo.Request.Serialized));
                Assert.That(() => auditEntryModel.RequestType, Is.EqualTo(strippedMoreThan255Chars));

                Assert.That(() => auditEntryModel.ResponseCorrelationGuid, Is.EqualTo(auditInfo.Response.CorrelationGuid));
                Assert.That(() => auditEntryModel.ResponseNamespace, Is.EqualTo(strippedMoreThan255Chars));
                Assert.That(() => auditEntryModel.ResponseSerialized, Is.EqualTo(auditInfo.Response.Serialized));
                Assert.That(() => auditEntryModel.ResponseType, Is.EqualTo(strippedMoreThan255Chars));

                Assert.That(() => auditEntryModel.Exception, Is.EqualTo(auditInfo.Exception));
                Assert.That(() => auditEntryModel.ServerMachineName, Is.EqualTo(strippedMoreThan255Chars));
                tx.Commit();
            }
        }
    }
}
