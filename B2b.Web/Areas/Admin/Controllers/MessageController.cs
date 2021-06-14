using B2b.Web.v4.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Log;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class MessageController : AdminBaseController
    {
        // GET: Admin/Message
        public ActionResult Index()
        {
             return View();
        }
    }
}