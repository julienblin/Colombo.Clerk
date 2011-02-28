using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class GetAuditEntriesForStreamRequest : PaginatedRequest<GetAuditEntriesForStreamResponse>
    {
        public Guid Id { get; set; }
    }
}
