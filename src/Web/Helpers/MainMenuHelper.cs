using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Colombo.Clerk.Messages;

namespace Colombo.Clerk.Web.Helpers
{
    public static class MainMenuHelper
    {
        public static HtmlString MainMenuLink(this HtmlHelper helper, string controllerName, string actionName = "Index", string label = null, bool includeServerNames = false)
        {
            label = label ?? controllerName;
            var active = helper.ViewContext.RouteData.Values["controller"].ToString()
                            .Equals(controllerName, StringComparison.InvariantCultureIgnoreCase);

            var innerHtml = new HtmlString("");

            if(includeServerNames)
            {
                var distinctValuesResponse = ((GetDistinctValuesResponse)helper.ViewData["MachineNames"]).Values;
                innerHtml = new HtmlString(string.Format("<ul>{0}</ul>", string.Join("", distinctValuesResponse.Select(x => string.Format("<li>{0}</li>", helper.RouteLink(x, controllerName, new { serverName = x }))))));
            }

            return new HtmlString(string.Format("<li {0}>{1}{2}</li>", active ? "class=\"current\"" : "", helper.ActionLink(label, actionName, controllerName), innerHtml));
        }
    }
}