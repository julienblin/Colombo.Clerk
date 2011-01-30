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
using System.Threading;
using Colombo.Clerk.Service;
using NUnit.Framework;
using Rhino.Mocks;

namespace Colombo.Clerk.Client.Tests
{
    [TestFixture]
    public class ClerkHandleInterceptorTest
    {
        [Test]
        public void It_should_throw_an_exception_if_no_IClerkServiceFactory_is_provided()
        {
            Assert.That(() => new ClerkHandleInterceptor(null),
                Throws.Exception.TypeOf<ArgumentNullException>());

            var mocks = new MockRepository();
            var factory = mocks.Stub<IClerkServiceFactory>();

            Assert.DoesNotThrow(() => new ClerkHandleInterceptor(factory));
        }

        [Test]
        public void It_should_write_to_service_requests_and_response()
        {
            var mocks = new MockRepository();
            var factory = mocks.StrictMock<IClerkServiceFactory>();
            var service = mocks.StrictMock<IClerkService>();
            var invocation = mocks.StrictMock<IColomboRequestHandleInvocation>();
            var request = new TestRequest { Name = "Foo", Context = { { "TenantId", "Something" } } };
            var response = new TestResponse { Message = "Bar" };

            AuditInfo verifyInfo = null;

            With.Mocks(mocks).Expecting(() =>
            {
                Expect.Call(factory.CreateClerkService()).Return(service);

                SetupResult.For(invocation.Request).Return(request).Repeat.AtLeastOnce();
                SetupResult.For(invocation.Response).Return(response).Repeat.AtLeastOnce();
                invocation.Proceed();

                service.Write(null);
                LastCall.IgnoreArguments().Do(new WriteDelegate(ai => verifyInfo = ai));
            }).Verify(() =>
            {
                var interceptor = new ClerkHandleInterceptor(factory);
                interceptor.Intercept(invocation);
                Thread.Sleep(200);

                Assert.That(() => verifyInfo, Is.Not.Null);

                Assert.That(() => verifyInfo.Request.Namespace, Is.EqualTo("Colombo.Clerk.Client.Tests"));
                Assert.That(() => verifyInfo.Request.Type, Is.EqualTo("TestRequest"));
                Assert.That(() => verifyInfo.Request.CorrelationGuid, Is.EqualTo(request.CorrelationGuid));
                Assert.That(() => verifyInfo.Context, Is.EqualTo(request.Context));
                Assert.That(() => verifyInfo.Request.Serialized, Contains.Substring("Foo"));

                Assert.That(() => verifyInfo.Response.Namespace, Is.EqualTo("Colombo.Clerk.Client.Tests"));
                Assert.That(() => verifyInfo.Response.Type, Is.EqualTo("TestResponse"));
                Assert.That(() => verifyInfo.Response.CorrelationGuid, Is.EqualTo(response.CorrelationGuid));
                Assert.That(() => verifyInfo.Response.Serialized, Contains.Substring("Bar"));

                Assert.That(() => verifyInfo.ServerMachineName, Is.EqualTo(Environment.MachineName));
            });
        }

        [Test]
        public void It_should_write_request_and_exception_in_case_of_error()
        {
            var mocks = new MockRepository();
            var factory = mocks.StrictMock<IClerkServiceFactory>();
            var service = mocks.StrictMock<IClerkService>();
            var invocation = mocks.StrictMock<IColomboRequestHandleInvocation>();
            var request = new TestRequest { Name = "Foo", Context = { { "TenantId", "Something" } } };
            var response = new TestResponse { Message = "Bar" };

            AuditInfo verifyInfo = null;

            With.Mocks(mocks).Expecting(() =>
            {
                Expect.Call(factory.CreateClerkService()).Return(service);

                SetupResult.For(invocation.Request).Return(request).Repeat.AtLeastOnce();
                SetupResult.For(invocation.Response).Return(response).Repeat.AtLeastOnce();
                invocation.Proceed();
                LastCall.Throw(new NotImplementedException());

                service.Write(null);
                LastCall.IgnoreArguments().Do(new WriteDelegate(ai => verifyInfo = ai));
            }).Verify(() =>
            {
                var interceptor = new ClerkHandleInterceptor(factory);
                Assert.That(() => interceptor.Intercept(invocation), Throws.Exception.TypeOf<NotImplementedException>());
                Thread.Sleep(200);

                Assert.That(() => verifyInfo, Is.Not.Null);

                Assert.That(() => verifyInfo.Request.Namespace, Is.EqualTo("Colombo.Clerk.Client.Tests"));
                Assert.That(() => verifyInfo.Request.Type, Is.EqualTo("TestRequest"));
                Assert.That(() => verifyInfo.Request.CorrelationGuid, Is.EqualTo(request.CorrelationGuid));
                Assert.That(() => verifyInfo.Context, Is.EqualTo(request.Context));
                Assert.That(() => verifyInfo.Request.Serialized, Contains.Substring("Foo"));

                Assert.That(() => verifyInfo.Exception, Contains.Substring("NotImplementedException"));

                Assert.That(() => verifyInfo.ServerMachineName, Is.EqualTo(Environment.MachineName));
            });
        }

        [Test]
        public void It_should_alert_when_no_clerk_service_is_returned_from_factory()
        {
            var mocks = new MockRepository();
            var factory = mocks.StrictMock<IClerkServiceFactory>();
            var invocation = mocks.StrictMock<IColomboRequestHandleInvocation>();
            var alerter1 = mocks.StrictMock<IColomboAlerter>();
            var alerter2 = mocks.StrictMock<IColomboAlerter>();
            var request = new TestRequest();

            AuditInfo verifyInfo = null;

            With.Mocks(mocks).Expecting(() =>
            {
                Expect.Call(factory.CreateClerkService()).Return(null);

                SetupResult.For(invocation.Request).Return(request).Repeat.AtLeastOnce();
                invocation.Proceed();
                LastCall.Throw(new NotImplementedException());

                alerter1.Alert(null);
                LastCall.IgnoreArguments();

                alerter2.Alert(null);
                LastCall.IgnoreArguments();
            }).Verify(() =>
            {
                var interceptor = new ClerkHandleInterceptor(factory) { Alerters = new[] { alerter1, alerter2 } };
                Assert.That(() => interceptor.Intercept(invocation), Throws.Exception.TypeOf<NotImplementedException>());
                Thread.Sleep(200);
            });
        }

        public delegate void WriteDelegate(AuditInfo auditInfo);

        public class TestResponse : Response
        {
            public virtual string Message { get; set; }
        }

        public class TestRequest : Request<TestResponse>
        {
            public string Name { get; set; }
        }
    }
}
