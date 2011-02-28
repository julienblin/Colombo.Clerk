using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Messages.Filters;
using Colombo.Clerk.Server.Models;
using Colombo.Clerk.Server.Services;
using NHibernate;
using NHibernate.Criterion;

namespace Colombo.Clerk.Server.Handlers
{
    public class CreateStreamRequestHandler : RequestHandler<CreateStreamRequest, CreateStreamResponse>
    {
        private readonly ISession session;

        public CreateStreamRequestHandler(ISession session)
        {
            this.session = session;
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
                switch (filterRequest.FilterCategory)
                {
                    case FilterCategory.SingleValue:
                        var filterModel = new FilterModel { FilterName = filterRequest.GetType().Name };
                        filterModel.SetValue(filterRequest.ValueTypes.First(), filterRequest.GetValues().First());
                        stream.Filters.Add(filterModel);
                        break;
                    case FilterCategory.Range:
                        var valueTypes = filterRequest.ValueTypes.ToArray();
                        var values = filterRequest.GetValues().ToArray();

                        if (values[0] != null)
                        {
                            var filterModelAfter = new FilterModel { FilterName = filterRequest.GetType().Name + "After" };
                            filterModelAfter.SetValue(valueTypes[0], values[0]);
                            stream.Filters.Add(filterModelAfter);
                        }

                        if (values[1] != null)
                        {
                            var filterModelBefore = new FilterModel { FilterName = filterRequest.GetType().Name + "Before" };
                            filterModelBefore.SetValue(valueTypes[1], values[1]);
                            stream.Filters.Add(filterModelBefore);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            session.Save(stream);
            Response.Id = stream.Id;
        }
    }
}
