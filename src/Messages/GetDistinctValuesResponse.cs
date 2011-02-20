using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class GetDistinctValuesResponse : Response
    {
        public GetDistinctValuesResponse()
        {
            Values = new List<string>();
        }

        public virtual IEnumerable<string> Values { get; set; }
    }
}
