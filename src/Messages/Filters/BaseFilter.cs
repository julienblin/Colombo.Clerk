using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages.Filters
{
    public abstract class BaseFilter<TValue> : IFilter
    {
        public abstract string Label { get; }

        public Type ValueType
        {
            get { return typeof(TValue); }
        }

        public TValue Value { get; set; }

        public object GetValue()
        {
            return Value;
        }
    }
}
