using B2b.Web.v4.Models.EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Models.Helper
{
    public class MaintenanceFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (B2bRule.CheckMaintenance())
            {
                filterContext.Result = new RedirectResult("/Login/Maintenance");
            }
        }
    }
}