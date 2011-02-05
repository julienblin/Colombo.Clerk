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
using System.ServiceModel;
using System.Linq;
using Colombo.Clerk.Server.Models;
using Colombo.Clerk.Service;
using NHibernate;
using Omu.ValueInjecter;

namespace Colombo.Clerk.Server
{
    public class ClerkService : IClerkService
    {
        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public void Write(AuditInfo auditInfo)
        {
            var auditEntry = new AuditEntryModel();
            auditEntry.InjectFrom<FlatLoopValueInjection>(auditInfo);
            auditEntry.Context = new List<ContextEntryModel>(
                auditInfo.Context.Select(kv => new ContextEntryModel { ContextKey = kv.Key, ContextValue = kv.Value })
            );

            StripLongStrings(auditEntry);

            ISession session = null;
            try
            {
                session = EndPointConfig.Kernel.Resolve<ISession>();
                session.Save(auditEntry);
            }
            finally
            {
                if(session != null)
                    EndPointConfig.Kernel.ReleaseComponent(session);
            }
        }

        private static void StripLongStrings(AuditEntryModel auditEntry)
        {
            if ((auditEntry.RequestNamespace != null) && (auditEntry.RequestNamespace.Length > 255))
                auditEntry.RequestNamespace = auditEntry.RequestNamespace.Substring(0, 255);

            if ((auditEntry.RequestType != null) && (auditEntry.RequestType.Length > 255))
                auditEntry.RequestType = auditEntry.RequestType.Substring(0, 255);

            if ((auditEntry.ResponseNamespace != null) && (auditEntry.ResponseNamespace.Length > 255))
                auditEntry.ResponseNamespace = auditEntry.ResponseNamespace.Substring(0, 255);

            if ((auditEntry.ResponseType != null) && (auditEntry.ResponseType.Length > 255))
                auditEntry.ResponseType = auditEntry.ResponseType.Substring(0, 255);

            if ((auditEntry.Message != null) && (auditEntry.Message.Length > 2000))
                auditEntry.Message = auditEntry.Message.Substring(0, 2000);

            foreach (var kv in auditEntry.Context)
            {
                if ((kv.ContextKey != null) && (kv.ContextKey.Length > 255))
                    kv.ContextKey = kv.ContextKey.Substring(0, 255);
            }
        }
    }
}
