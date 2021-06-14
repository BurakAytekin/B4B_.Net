using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class PartialController : Controller
    {
        // GET: Admin/Partial
        public PartialViewResult DeleteModal()
        {
            return new PartialViewResult();
        }

        public PartialViewResult ConfirmationModal()
        {
            return new PartialViewResult();
        }
        public PartialViewResult DeleteAndReasonModal()
        {
            return new PartialViewResult();
        }
    }
}