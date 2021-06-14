using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Log;

namespace B2b.Web.v4.Controllers
{
    public class FooterController : BaseController
    {
        // GET: Footer
        public ActionResult Index()
        {
              return View();
        }

        public ActionResult ConditionsForReturn()
        {
              return View();
        }
        public ActionResult PrivacyPolicy()
        {
              return View();
        }
        public ActionResult TermOfUse()
        {
           return View();
        }
        public ActionResult DistanceSalesContract()
        {
              return View();
        }
    }
}