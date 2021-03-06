﻿#region License
// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Colombo.Clerk.Messages
{
    public class WriteAuditEntryRequest : Request<WriteAuditEntryResponse>
    {
        [Required(AllowEmptyStrings = false)]
        [StringLength(255)]
        public string RequestNamespace { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(255)]
        public string RequestType { get; set; }

        public string RequestSerialized { get; set; }

        public Guid RequestCorrelationGuid { get; set; }

        public DateTime RequestUtcTimestamp { get; set; }

        [StringLength(255)]
        public string ResponseNamespace { get; set; }

        [StringLength(255)]
        public string ResponseType { get; set; }

        public string ResponseSerialized { get; set; }

        public Guid ResponseCorrelationGuid { get; set; }

        public DateTime ResponseUtcTimestamp { get; set; }

        public string Exception { get; set; }

        [StringLength(2000)]
        public string Message { get; set; }

        private IDictionary<string, string> requestContext;
        /// <summary>
        /// Context of the request. Garanteed to be non-null.
        /// </summary>
        public IDictionary<string, string> RequestContext
        {
            get { return requestContext ?? (requestContext = new Dictionary<string, string>()); }
            set { requestContext = value; }
        }

        [Required(AllowEmptyStrings = false)]
        [StringLength(255)]
        public string ServerMachineName { get; set; }
    }
}
