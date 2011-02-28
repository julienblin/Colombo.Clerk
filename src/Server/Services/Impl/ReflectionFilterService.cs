using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.Clerk.Messages.Filters;

namespace Colombo.Clerk.Server.Services.Impl
{
    public class ReflectionFilterService : IFilterService
    {
        private IDictionary<Type, FilterDescription> loadedFilters;

        public ReflectionFilterService()
        {
            loadedFilters = LoadFilters();
        }

        private static IDictionary<Type, FilterDescription> LoadFilters()
        {
            var filterTypes = typeof(IFilter).Assembly.GetTypes().Where(
                t => typeof(IFilter).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

            return filterTypes.ToDictionary(
                filterType => filterType,
                filterType => new FilterDescription
                {
                    FilterName = filterType.Name,
                    FilterType = ((IFilter)Activator.CreateInstance(filterType)).ValueType
                }
            );
        }

        public IEnumerable<FilterDescription> GetFiltersDescriptions()
        {
            return loadedFilters.Values;
        }

        public FilterDescription GetFilterDescription(IFilter filter)
        {
            return loadedFilters[filter.GetType()];
        }
    }
}
