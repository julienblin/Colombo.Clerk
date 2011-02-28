using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages.Filters
{
    public class RequestUtcTimestampFilter : BaseRangeFilter<DateTime?>
    {
        public override IEnumerable<object> GetValues()
        {
            if (ValueAfter.HasValue)
                yield return ValueAfter.Value;
            else
                yield return null;

            if (ValueBefore.HasValue)
                yield return ValueBefore.Value;
            else
                yield return null;
        }
    }
}
