using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Colombo.Clerk.Web.Services
{
    public interface IIdentityService
    {
        IPrincipal GetCurrentIdentity();
    }
}
