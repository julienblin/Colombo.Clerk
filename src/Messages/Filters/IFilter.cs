using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages.Filters
{
    public interface IFilter
    {
        FilterCategory FilterCategory { get; }

        IEnumerable<Type> ValueTypes { get; }

        IEnumerable<object> GetValues();
    }

    public enum FilterCategory
    {
        SingleValue,
        Range
    }
}
