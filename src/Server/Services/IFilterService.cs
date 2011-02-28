using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.Clerk.Messages.Filters;

namespace Colombo.Clerk.Server.Services
{
    public interface IFilterService
    {
        IEnumerable<FilterDescription> GetFiltersDescriptions();
        FilterDescription GetFilterDescription(IFilter filter);
    }

    public class FilterDescription
    {
        public Type FilterType { get; set; }

        public string FilterName { get; set; }
    }
}
