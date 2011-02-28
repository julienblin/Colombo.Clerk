using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Messages.Filters;
using Colombo.Clerk.Server.Handlers;
using Colombo.Clerk.Server.Models;
using Colombo.Clerk.Server.Services;
using Colombo.Clerk.Server.Services.Impl;
using NUnit.Framework;

namespace Colombo.Clerk.Server.Tests.Handlers
{
    [TestFixture]
    public class CreateStreamRequestHandlerTest : BaseDbTest
    {
        [Test]
        public void It_should_be_able_to_save_a_Stream()
        {
            Container.Register(Component.For<IFilterService>().ImplementedBy<ReflectionFilterService>());
            StubMessageBus.TestHandler<CreateStreamRequestHandler>();

            var filterService = Container.Resolve<IFilterService>();

            var request = new CreateStreamRequest { Name = "StreamName" };
            var requestNamespaceFilter = new RequestNamespaceFilter { Value = "RequestNamespaceValue" };
            request.Filters.Add(requestNamespaceFilter);
            var requestTypeFilter = new RequestTypeFilter {Value = "RequestTypeValue"};
            request.Filters.Add(requestTypeFilter);
            var requestCorrelationGuidFilter = new RequestCorrelationGuidFilter() { Value = Guid.NewGuid() };
            request.Filters.Add(requestCorrelationGuidFilter);

            var responseNamespaceFilter = new ResponseNamespaceFilter { Value = "ResponseNamespaceValue" };
            request.Filters.Add(responseNamespaceFilter);
            var responseTypeFilter = new ResponseTypeFilter { Value = "ResponseTypeFilter" };
            request.Filters.Add(responseTypeFilter);
            var responseCorrelationGuidFilter = new ResponseCorrelationGuidFilter() { Value = Guid.NewGuid() };
            request.Filters.Add(responseCorrelationGuidFilter);

            var hasExceptionFilter = new HasExceptionFilter() { Value = true };
            request.Filters.Add(hasExceptionFilter);

            var requestUtcTimestampAfterFilter = new RequestUtcTimestampAfterFilter() { Value = new DateTime(2011, 02, 01) };
            request.Filters.Add(requestUtcTimestampAfterFilter);

            var response = MessageBus.Send(request);

            Assert.That(response.IsValid());

            using (var tx = Session.BeginTransaction())
            {
                var streamModel = Session.Get<StreamModel>(response.Id);

                Assert.That(streamModel.Name, Is.EqualTo(request.Name));

                var requestNamespaceFilterModel =
                    streamModel.Filters.Where(
                        f => f.FilterName == filterService.GetFilterDescription(requestNamespaceFilter).FilterName)
                        .FirstOrDefault();

                Assert.That(requestNamespaceFilterModel, Is.Not.Null);
                Assert.That(requestNamespaceFilterModel.StringValue, Is.EqualTo(requestNamespaceFilter.Value));

                var requestTypeFilterModel =
                    streamModel.Filters.Where(
                        f => f.FilterName == filterService.GetFilterDescription(requestTypeFilter).FilterName)
                        .FirstOrDefault();

                Assert.That(requestTypeFilterModel, Is.Not.Null);
                Assert.That(requestTypeFilterModel.StringValue, Is.EqualTo(requestTypeFilter.Value));

                var requestCorrelationGuidFilterModel =
                    streamModel.Filters.Where(
                        f => f.FilterName == filterService.GetFilterDescription(requestCorrelationGuidFilter).FilterName)
                        .FirstOrDefault();

                Assert.That(requestCorrelationGuidFilterModel, Is.Not.Null);
                Assert.That(requestCorrelationGuidFilterModel.GuidValue, Is.EqualTo(requestCorrelationGuidFilter.Value));

                var responseNamespaceFilterModel =
                    streamModel.Filters.Where(
                        f => f.FilterName == filterService.GetFilterDescription(responseNamespaceFilter).FilterName)
                        .FirstOrDefault();

                Assert.That(responseNamespaceFilterModel, Is.Not.Null);
                Assert.That(responseNamespaceFilterModel.StringValue, Is.EqualTo(responseNamespaceFilter.Value));

                var responseTypeFilterModel =
                    streamModel.Filters.Where(
                        f => f.FilterName == filterService.GetFilterDescription(responseTypeFilter).FilterName)
                        .FirstOrDefault();

                Assert.That(responseTypeFilterModel, Is.Not.Null);
                Assert.That(responseTypeFilterModel.StringValue, Is.EqualTo(responseTypeFilter.Value));

                var responseCorrelationGuidFilterModel =
                    streamModel.Filters.Where(
                        f => f.FilterName == filterService.GetFilterDescription(responseCorrelationGuidFilter).FilterName)
                        .FirstOrDefault();

                Assert.That(responseCorrelationGuidFilterModel, Is.Not.Null);
                Assert.That(responseCorrelationGuidFilterModel.GuidValue, Is.EqualTo(responseCorrelationGuidFilter.Value));

                var hasExceptionFilterModel =
                    streamModel.Filters.Where(
                        f => f.FilterName == filterService.GetFilterDescription(hasExceptionFilter).FilterName)
                        .FirstOrDefault();

                Assert.That(hasExceptionFilterModel, Is.Not.Null);
                Assert.That(hasExceptionFilterModel.BoolValue, Is.EqualTo(hasExceptionFilter.Value));

                var requestUtcTimestampAfterFilterModel =
                    streamModel.Filters.Where(
                        f => f.FilterName == filterService.GetFilterDescription(requestUtcTimestampAfterFilter).FilterName)
                        .FirstOrDefault();

                Assert.That(requestUtcTimestampAfterFilterModel, Is.Not.Null);
                Assert.That(requestUtcTimestampAfterFilterModel.DateTimeValue, Is.EqualTo(requestUtcTimestampAfterFilter.Value));

                tx.Commit();
            }
        }

        [Test]
        public void It_should_reject_Streams_with_no_names_or_duplicate()
        {
            Container.Register(Component.For<IFilterService>().ImplementedBy<ReflectionFilterService>());
            StubMessageBus.TestHandler<CreateStreamRequestHandler>();

            var request = new CreateStreamRequest();
            var response = MessageBus.Send(request);

            Assert.That(response.IsValid(), Is.False);
            Assert.That(response.ValidationResults.Count, Is.EqualTo(1));
            Assert.That(response.ValidationResults[0].MemberNames.First(), Contains.Substring("Name"));

            var streamModel = new StreamModel { Name = "StreamName" };
            using (var tx = Session.BeginTransaction())
            {
                Session.Save(streamModel);
                tx.Commit();
            }

            request = new CreateStreamRequest { Name = streamModel.Name };
            response = MessageBus.Send(request);

            Assert.That(response.IsValid(), Is.False);
            Assert.That(response.ValidationResults.Count, Is.EqualTo(1));
            Assert.That(response.ValidationResults[0].MemberNames.First(), Contains.Substring("Name"));
        }
    }
}
