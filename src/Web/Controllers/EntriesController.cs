using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Colombo.Clerk.Web.Controllers
{
    public class EntriesController : ApplicationController
    {
        public ActionResult Index(string serverName = null)
        {
            ViewData["serverName"] = serverName;
            return View();
        }
    }
}
