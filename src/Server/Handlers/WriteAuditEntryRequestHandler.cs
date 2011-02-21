#region License
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

using System.Collections.Generic;
using System.Linq;
using Colombo.Clerk.Messages;
using Colombo.Clerk.Server.Models;
using NHibernate;
using Omu.ValueInjecter;

namespace Colombo.Clerk.Server.Handlers
{
    public class WriteAuditEntryRequestHandler : RequestHandler<WriteAuditEntryRequest, WriteAuditEntryResponse>
    {
        private readonly ISession session;

        public WriteAuditEntryRequestHandler(ISession session)
        {
            this.session = session;
        }

        protected override void Handle()
        {
            var auditEntryModel = new AuditEntryModel();
            auditEntryModel.InjectFrom(Request);
            auditEntryModel.Context = new List<ContextEntryModel>(
                Request.RequestContext.Select(kv => new ContextEntryModel { ContextKey = kv.Key, ContextValue = kv.Value })
            );

            session.Save(auditEntryModel);

            Response.Id = auditEntryModel.Id;
        }
    }
}
