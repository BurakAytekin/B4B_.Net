using B2b.Web.v4.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.UI;
using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Log;
using B2b.Web.v4.Models.Helper;

namespace B2b.Web.v4
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            //RouteTable.Routes.MapHubs();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            JobScheduler.Start();
            BistCurrecyUpdate.Start();
        }
        protected void Application_Error(Object sender, EventArgs e)
        {
            try
            {
                Exception ex = Server.GetLastError();
                if (ex is HttpException && ex.InnerException is ViewStateException)
                {
                    Response.Redirect(Request.Url.AbsoluteUri);
                    return;
                }

                if (ex.Message == "File does not exist." || Request.Url.ToString().Contains("/images/"))
                    return;

                StringBuilder theBody = new StringBuilder();
                theBody.Append("URL: " + Request.Url + "\n");
                theBody.Append("Referer: " + Request.ServerVariables["HTTP_REFERER"] + "\n");
                theBody.Append("IP: " + Request.ServerVariables["REMOTE_ADDR"] + "\n");
                theBody.Append("Error Message: " + ex.ToString() + "\n");
                theBody.Append("Form Values: " + "\n");
                foreach (string s in Request.Form.AllKeys)
                {
                    if (s != "__VIEWSTATE")
                        theBody.Append(s + ":" + Request.Form[s] + "\n");
                }
                theBody.Append("Session Values: " + "\n");
                try
                {
                    foreach (string s in Session.Keys)
                        theBody.Append(s + ":" + Session[s] + "\n");
                }
                catch { }
                var clientType = ClientType.B2BWeb;
                if (Request.Url.ToString().ToLower().Contains("admin"))
                    clientType = ClientType.Admin;
                Logger.LogGeneral(LogGeneralErrorType.Error, clientType, "UnhandledException_CustomHandleError", theBody.ToString(), string.Empty);



            }
            catch (Exception ext)
            {

            }
        }

        protected void Application_BeginRequest()
        {
            if (!Context.Request.IsSecureConnection)
            {
                string url = Context.Request.Url.ToString().ToLower();
                if (url.Contains("localhost") || url.Contains("eryazsoftware.com.tr"))
                {
                    JobScheduler.Stop();
                }
                else
                {
                    Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));
                }
            }

        }

    }
}
