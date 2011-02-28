using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages.Filters
{
    public class ResponseNamespaceFilter : BaseFilter<string>
    {
        public override string Label
        {
            get { return "response namespace"; }
        }
    }
}
