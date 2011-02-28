using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages.Filters
{
    public interface IFilter
    {
        string Label { get; }
        Type ValueType { get; }

        object GetValue();
    }
}
