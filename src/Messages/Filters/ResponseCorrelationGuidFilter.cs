using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages.Filters
{
    public class ResponseCorrelationGuidFilter : BaseFilter<Guid>
    {
        public override string Label
        {
            get { return "response correlation guid"; }
        }
    }
}
