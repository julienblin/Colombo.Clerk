using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages.Filters
{
    public class HasExceptionFilter : BaseFilter<bool>
    {
        public override string Label
        {
            get { return "has exception"; }
        }
    }
}
