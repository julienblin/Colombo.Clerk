using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages.Filters
{
    public abstract class BaseRangeFilter<TValue> : IFilter
    {
        public FilterCategory FilterCategory { get { return FilterCategory.Range; } }

        public IEnumerable<Type> ValueTypes
        {
            get
            {
                yield return typeof(TValue);
                yield return typeof(TValue);
            }
        }

        public TValue ValueAfter { get; set; }

        public TValue ValueBefore { get; set; }

        public abstract IEnumerable<object> GetValues();
    }
}
