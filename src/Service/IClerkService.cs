using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.ServiceModel;
using System.Text;

namespace Colombo.Clerk.Service
{
    [ServiceContract(Namespace = @"http://Colombo.Clerk")]
    public interface IClerkService
    {
        [OperationContract(IsOneWay = true)]
        void Write(AuditInfo auditInfo);
    }
}
