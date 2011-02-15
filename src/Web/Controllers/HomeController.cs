using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Colombo.Clerk.Web.Controllers
{
    public class HomeController : ApplicationController
    {
        public IMessageBus MessageBus { get; set; }

        public ActionResult Index()
        {
            return View();
        }
    }
}