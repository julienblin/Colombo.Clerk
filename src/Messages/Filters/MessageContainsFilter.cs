using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages.Filters
{
    public class MessageContainsFilter : BaseFilter<string>
    {
        public override string Label
        {
            get { return "message contains"; }
        }
    }
}
