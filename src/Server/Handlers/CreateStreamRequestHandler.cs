using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Models;
using Colombo.Clerk.Server.Services;
using NHibernate;
using NHibernate.Criterion;

namespace Colombo.Clerk.Server.Handlers
{
    public class CreateStreamRequestHandler : RequestHandler<CreateStreamRequest, CreateStreamResponse>
    {
        private readonly ISession session;

        private readonly IFilterService filterService;

        public CreateStreamRequestHandler(ISession session, IFilterService filterService)
        {
            this.session = session;
            this.filterService = filterService;
        }

        protected override void Handle()
        {
            var nameTaken = session.QueryOver<StreamModel>()
                .Where(x => x.Name == Request.Name)
                .SingleOrDefault();

            if (nameTaken != null)
            {
                Response.ValidationResults.Add(new ValidationResult("{0} must be unique.", new[] { "Name" }));
                return;
            }

            var stream = new StreamModel { Name = Request.Name, Filters = new List<FilterModel>() };

            foreach (var filterRequest in Request.Filters)
            {
                var filterDescription = filterService.GetFilterDescription(filterRequest);
                var filterModel = new FilterModel { FilterName = filterDescription.FilterName };
                filterModel.SetValue(filterDescription.FilterType, filterRequest.GetValue());
                stream.Filters.Add(filterModel);
            }

            session.Save(stream);
            Response.Id = stream.Id;
        }
    }
}
