using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Colombo.Clerk.Messages.Filters;

namespace Colombo.Clerk.Messages
{
    public class CreateStreamRequest : Request<CreateStreamResponse>
    {
        public CreateStreamRequest()
        {
            Filters = new List<IFilter>();
        }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }

        public IList<IFilter> Filters { get; set; }
    }
}
