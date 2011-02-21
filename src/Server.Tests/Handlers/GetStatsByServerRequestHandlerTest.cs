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
using System.ComponentModel.DataAnnotations;
using Castle.MicroKernel.Registration;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Handlers;
using Colombo.Clerk.Server.Models;
using Colombo.Clerk.Server.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace Colombo.Clerk.Server.Tests.Handlers
{
    [TestFixture]
    public class GetStatsByServerRequestHandlerTest : BaseDbTest
    {
        [Test]
        public void It_should_return_zero_when_no_AuditEntry_in_db()
        {
            var mocks = new MockRepository();
            var clock = mocks.StrictMock<IClock>();

            With.Mocks(mocks).Expecting(() =>
            {
                SetupResult.For(clock.UtcNow).Return(new DateTime(2000, 1, 3));
            }).Verify(() =>
            {
                Container.Register(Component.For<IClock>().Instance(clock));
                StubMessageBus.TestHandler<GetStatsByServerRequestHandler>();
                var response = MessageBus.Send(new GetStatsByServerRequest());

                Assert.That(response.ServerStats.Count, Is.EqualTo(0));
            });
        }

        [Test]
        public void It_should_return_correct_stats()
        {
            const string machineName1 = @"MachineName1";
            const string machineName2 = @"MachineName2";
            const string machineName3 = @"MachineName3";

            using (var tx = Session.BeginTransaction())
            {
                Session.Save(
                    new AuditEntryModel
                    {
                        Context = new List<ContextEntryModel>
                        {
                            new ContextEntryModel { ContextKey = MetaContextKeys.SenderMachineName, ContextValue = machineName1 },
                            new ContextEntryModel { ContextKey = MetaContextKeys.HandlerMachineName, ContextValue = machineName2 },
                        },
                        RequestUtcTimestamp = new DateTime(2000, 1, 1, 1, 0, 0)
                    }
                );
                Session.Save(
                    new AuditEntryModel
                    {
                        Context = new List<ContextEntryModel>
                        {
                            new ContextEntryModel { ContextKey = MetaContextKeys.SenderMachineName, ContextValue = machineName1 },
                            new ContextEntryModel { ContextKey = MetaContextKeys.HandlerMachineName, ContextValue = machineName3 },
                        },
                        RequestUtcTimestamp = new DateTime(2000, 1, 1, 1, 0, 0)
                    }
                );
                Session.Save(
                    new AuditEntryModel
                    {
                        Context = new List<ContextEntryModel>
                        {
                            new ContextEntryModel { ContextKey = MetaContextKeys.SenderMachineName, ContextValue = machineName2 },
                            new ContextEntryModel { ContextKey = MetaContextKeys.HandlerMachineName, ContextValue = machineName3 },
                        },
                        RequestUtcTimestamp = new DateTime(2000, 1, 2, 1, 0, 0)
                    }
                );
                tx.Commit();
            }

            var mocks = new MockRepository();
            var clock = mocks.StrictMock<IClock>();

            With.Mocks(mocks).Expecting(() =>
            {
                SetupResult.For(clock.UtcNow).Return(new DateTime(2000, 1, 3));
            }).Verify(() =>
            {
                Container.Register(Component.For<IClock>().Instance(clock));
                StubMessageBus.TestHandler<GetStatsByServerRequestHandler>();

                var response = MessageBus.Send(new GetStatsByServerRequest {Since = TimeSpan.FromDays(3) });

                Assert.That(response.ServerStats.Count, Is.EqualTo(3));

                Assert.That(response.ServerStats.ContainsKey(machineName1));
                Assert.That(response.ServerStats[machineName1].NumRequestsSent, Is.EqualTo(2));
                Assert.That(response.ServerStats[machineName1].NumRequestsHandled, Is.EqualTo(0));

                Assert.That(response.ServerStats.ContainsKey(machineName2));
                Assert.That(response.ServerStats[machineName2].NumRequestsSent, Is.EqualTo(1));
                Assert.That(response.ServerStats[machineName2].NumRequestsHandled, Is.EqualTo(1));

                Assert.That(response.ServerStats.ContainsKey(machineName3));
                Assert.That(response.ServerStats[machineName3].NumRequestsSent, Is.EqualTo(0));
                Assert.That(response.ServerStats[machineName3].NumRequestsHandled, Is.EqualTo(2));

                response = MessageBus.Send(new GetStatsByServerRequest { Since = TimeSpan.FromDays(1.5) });

                Assert.That(response.ServerStats.Count, Is.EqualTo(3));

                Assert.That(response.ServerStats.ContainsKey(machineName1));
                Assert.That(response.ServerStats[machineName1].NumRequestsSent, Is.EqualTo(0));
                Assert.That(response.ServerStats[machineName1].NumRequestsHandled, Is.EqualTo(0));

                Assert.That(response.ServerStats.ContainsKey(machineName2));
                Assert.That(response.ServerStats[machineName2].NumRequestsSent, Is.EqualTo(1));
                Assert.That(response.ServerStats[machineName2].NumRequestsHandled, Is.EqualTo(0));

                Assert.That(response.ServerStats.ContainsKey(machineName3));
                Assert.That(response.ServerStats[machineName3].NumRequestsSent, Is.EqualTo(0));
                Assert.That(response.ServerStats[machineName3].NumRequestsHandled, Is.EqualTo(1));

                response = MessageBus.Send(new GetStatsByServerRequest { Since = TimeSpan.FromMilliseconds(1) });

                Assert.That(response.ServerStats.Count, Is.EqualTo(3));

                Assert.That(response.ServerStats.ContainsKey(machineName1));
                Assert.That(response.ServerStats[machineName1].NumRequestsSent, Is.EqualTo(0));
                Assert.That(response.ServerStats[machineName1].NumRequestsHandled, Is.EqualTo(0));

                Assert.That(response.ServerStats.ContainsKey(machineName2));
                Assert.That(response.ServerStats[machineName2].NumRequestsSent, Is.EqualTo(0));
                Assert.That(response.ServerStats[machineName2].NumRequestsHandled, Is.EqualTo(0));

                Assert.That(response.ServerStats.ContainsKey(machineName3));
                Assert.That(response.ServerStats[machineName3].NumRequestsSent, Is.EqualTo(0));
                Assert.That(response.ServerStats[machineName3].NumRequestsHandled, Is.EqualTo(0));
            });
        }

        [Test]
        public void It_should_not_allow_TimeSpan_larger_that_31_days()
        {
            var mocks = new MockRepository();
            var clock = mocks.StrictMock<IClock>();

            With.Mocks(mocks).Expecting(() =>
            {
                SetupResult.For(clock.UtcNow).Return(new DateTime(2000, 1, 3));
            }).Verify(() =>
            {
                Container.Register(Component.For<IClock>().Instance(clock));
                StubMessageBus.TestHandler<GetStatsByServerRequestHandler>();
                var response = MessageBus.Send(new GetStatsByServerRequest { Since = TimeSpan.FromDays(31) });

                Assert.That(response.ServerStats.Count, Is.EqualTo(0));

                Assert.That(() => MessageBus.Send(new GetStatsByServerRequest { Since = TimeSpan.FromDays(32) }),
                    Throws.Exception.TypeOf<ValidationException>()
                    .With.Message.ContainsSubstring("Since"));
            });
        }
    }
}
