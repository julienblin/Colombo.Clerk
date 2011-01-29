using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Colombo.Clerk.Service;

namespace Colombo.Clerk.Client
{
    public interface IClerkServiceFactory
    {
        IClerkService CreateClerkService();
    }
}
