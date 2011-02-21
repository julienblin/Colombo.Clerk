using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace Colombo.Clerk.Web.Services.Impl
{
    public class DefaultIdentityService : IIdentityService
    {
        public IPrincipal GetCurrentIdentity()
        {
            return HttpContext.Current.User;
        }
    }
}
