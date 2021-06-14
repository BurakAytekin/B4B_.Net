using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Controllers
{
    public class CatalogController : BaseController
    {
        // GET: Catalog
        public ActionResult Index()
        {
            return View(CatalogLink.GetList().Where(x => x.IsActive).ToList());
        }
    }
}