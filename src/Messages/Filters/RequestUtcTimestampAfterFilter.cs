using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages.Filters
{
    public class RequestUtcTimestampAfterFilter : BaseFilter<DateTime>
    {
        public override string Label
        {
            get { return "request UTC timestamp after"; }
        }
    }
}
