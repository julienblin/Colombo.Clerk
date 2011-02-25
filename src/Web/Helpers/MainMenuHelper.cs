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

using System;
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