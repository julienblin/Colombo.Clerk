using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages.Filters
{
    public class RequestCorrelationGuidFilter : BaseFilter<Guid>
    {
        public override string Label
        {
            get { return "request correlation guid"; }
        }
    }
}
