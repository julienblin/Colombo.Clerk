﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colombo.Clerk.Messages
{
    public class DeleteStreamRequest : Request<DeleteStreamResponse>
    {
        public Guid Id { get; set; }
    }
}