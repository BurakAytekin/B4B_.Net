using System.Web.Mvc;

namespace B2b.Web.v4.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
               // return View(model: "Admin area home controller");
                 return "Admin";
            }
        }
       
        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                   defaults: new
                   {
                       controller = "Home",
                       action = "Index",
                       id = UrlParameter.Optional
                   }
            //  new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}