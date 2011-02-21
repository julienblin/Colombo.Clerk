using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Colombo.Clerk.Web.Helpers
{
    public static class MainMenuHelper
    {
        public static HtmlString MainMenuLink(this HtmlHelper helper, string controllerName, string actionName = "Index", string label = null)
        {
            label = label ?? controllerName;
            var active = helper.ViewContext.RouteData.Values["controller"].ToString()
                            .Equals(controllerName, StringComparison.InvariantCultureIgnoreCase);
            return new HtmlString(string.Format("<li {0}>{1}</li>", active ? "class=\"current\"" : "", helper.ActionLink(label, actionName, controllerName)));
        }
    }
}