using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages.Filters
{
    public abstract class BaseSimpleFilter<TValue> : IFilter
    {
        public FilterCategory FilterCategory { get { return FilterCategory.SingleValue; } }

        public IEnumerable<Type> ValueTypes
        {
            get { yield return typeof(TValue); }
        }

        public TValue Value { get; set; }

        public IEnumerable<object> GetValues()
        {
            yield return Value;
        }
    }
}
