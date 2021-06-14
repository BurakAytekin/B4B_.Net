using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using System.Net;
using B2b.Web.v4.Models.Helper;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;
using RestSharp;

namespace B2b.Web.v4.Areas.Admin.Models
{
    public class AdminBaseController : Controller
    {
        #region Properties
        protected Salesman AdminCurrentSalesman
        {
            get { return (Salesman)Session["AdminSalesman"]; }
            set { Session["AdminSalesman"] = value; }
        }
        protected List<Currency> CurrencyList
        {
            get { return (List<Currency>)Session["CurrencyListAdmin"]; }
            set { Session["CurrencyListAdmin"] = value; }
        }

        #endregion

        #region Methods
        public string GetUserIpAddress()
        {
            string ip = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
            return ip;
        }
        public string GetControllerName()
        {
            return this.ControllerContext.RouteData.Values["controller"] + "=>" + this.ControllerContext.RouteData.Values["action"];
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!filterContext.Controller.ControllerContext.HttpContext.Request.CurrentExecutionFilePath.Contains("FireSyncDesign"))
            {
                if (AdminCurrentSalesman == null || AdminCurrentSalesman.Id == 0)
                {
                    filterContext.Result = new RedirectResult("~/Admin/Login/Logout");
                }
                else if (AdminCurrentSalesman.Locked)
                {
                    filterContext.Result = new RedirectResult("~/Admin/Login/Locked");
                }
                else
                {
                    Logger.LogNavigation(-1, -1, AdminCurrentSalesman.Id,
                GetControllerName(), ClientType.Admin, GetUserIpAddress());
                    ViewBag.AdminCurrentSalesman = AdminCurrentSalesman;
                    List<AuthorityGroup> AuthoritySalesman = Session["AuthoritySalesman"] as List<AuthorityGroup>;
                    string page = this.ControllerContext.RouteData.Values["controller"].ToString() + "/" + this.ControllerContext.RouteData.Values["action"];
                    if (AuthoritySalesman != null && AuthoritySalesman.Where(x => x.PageNameRef.Contains(page)).Count() > 0)
                        filterContext.Result = new RedirectResult("~/Admin/Login/UnAuthorized");
                }

            }


        }
        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Session["AdminSalesman"] == null)
            {
                filterContext.Result = new RedirectResult("~/Admin/Login/Logout");
            }

        }
        protected void GetActualCurrencies()
        {
            CurrencyList = Currency.GetList();
        }
       

        protected byte[] Parse(string base64Content)
        {
            if (string.IsNullOrEmpty(base64Content))
            {
                throw new ArgumentNullException(nameof(base64Content));
            }

            int indexOfSemiColon = base64Content.IndexOf(";", StringComparison.OrdinalIgnoreCase);

            string dataLabel = base64Content.Substring(0, indexOfSemiColon);

            string contentType = dataLabel.Split(':').Last();

            var startIndex = base64Content.IndexOf("base64,", StringComparison.OrdinalIgnoreCase) + 7;

            var fileContents = base64Content.Substring(startIndex);

            byte[] bytes = Convert.FromBase64String(fileContents);

            return bytes;
        }

        protected string GetFileType(string imageBase)
        {
            int indexOfSemiColon = imageBase.IndexOf(";", StringComparison.OrdinalIgnoreCase);
            string dataLabel = imageBase.Substring(0, indexOfSemiColon);
            string imgType = dataLabel.Split(':').Last().Split('/').Last();

            #region allType
            switch (imgType)
            {
                case "msword":
                    imgType = "doc";
                    break;
                case "vnd.openxmlformats-officedocument.wordprocessingml.document":
                    imgType = "docx";
                    break;
                case "vnd.openxmlformats-officedocument.wordprocessingml.template":
                    imgType = "dotx";
                    break;
                case "vnd.ms-word.document.macroEnabled.12":
                    imgType = "docm";
                    break;
                case "vnd.ms-word.template.macroEnabled.12":
                    imgType = "dotm";
                    break;
                case "vnd.ms-excel":
                    imgType = "xls";
                    break;
                case "vnd.openxmlformats-officedocument.spreadsheetml.sheet":
                    imgType = "xlsx";
                    break;
                case "vnd.openxmlformats-officedocument.spreadsheetml.template":
                    imgType = "xltx";
                    break;
                case "vnd.ms-excel.sheet.macroEnabled.12":
                    imgType = "xlsm";
                    break;
                case "vnd.ms-excel.template.macroEnabled.12":
                    imgType = "xltm";
                    break;
                case "vnd.ms-excel.addin.macroEnabled.12":
                    imgType = "xlam";
                    break;
                case "vnd.ms-excel.sheet.binary.macroEnabled.12":
                    imgType = "xlsb";
                    break;
                case "vnd.ms-powerpoint":
                    imgType = "ppt";
                    break;
                case "vnd.openxmlformats-officedocument.presentationml.presentation":
                    imgType = "pptx";
                    break;
                case "vnd.openxmlformats-officedocument.presentationml.template":
                    imgType = "potx";
                    break;
                case "vnd.openxmlformats-officedocument.presentationml.slideshow":
                    imgType = "ppsx";
                    break;
                case "vnd.ms-powerpoint.addin.macroEnabled.12":
                    imgType = "ppam";
                    break;
                case "vnd.ms-powerpoint.presentation.macroEnabled.12":
                    imgType = "pptm";
                    break;
                case "vnd.ms-powerpoint.template.macroEnabled.12":
                    imgType = "potm";
                    break;
                case "vnd.ms-powerpoint.slideshow.macroEnabled.12":
                    imgType = "ppsm";
                    break;
                case "vnd.ms-access":
                    imgType = "mdb";
                    break;

                case "video/3gpp":
                    imgType = "3gp";
                    break;
                case "":
                    imgType = "";
                    break;
                case "video/x-ms-asf":
                    imgType = "asf";
                    break;
                case "video/x-msvideo":
                    imgType = "avi";
                    break;
                case "application/octet-stream":
                    imgType = "bin";
                    break;
                case "image/bmp":
                    imgType = "bmp";
                    break;
                case "text/css":
                    imgType = "css";
                    break;
                case "text/csv":
                    imgType = "csv";
                    break;
                case "image/vnd.dwg":
                    imgType = "dwg";
                    break;
                case "image/vnd.dxf":
                    imgType = "dxf";
                    break;
                case "x-msdownload":
                    imgType = "exe";
                    break;
                case "video/x-flv":
                    imgType = "flv";
                    break;
                case "image/gif":
                    imgType = "gif";
                    break;
                case "text/html":
                    imgType = "html";
                    break;
                case "	image/x-icon":
                    imgType = "ico";
                    break;
                case "java-archive":
                    imgType = "";
                    break;
                case "javascript":
                    imgType = "js";
                    break;
                case "video/mp4":
                    imgType = "mp4";
                    break;
                case "video/mpeg":
                    imgType = "mpeg";
                    break;
                case "vnd.oasis.opendocument.database":
                    imgType = "odb";
                    break;
                case "vnd.oasis.opendocument.chart":
                    imgType = "odb";
                    break;
                case "vnd.oasis.opendocument.formula":
                    imgType = "odf";
                    break;

                case "vnd.oasis.opendocument.spreadsheet":
                    imgType = "ods";
                    break;

                case "vnd.oasis.opendocument.text":
                    imgType = "odt";
                    break;

                case "x-font-otf":
                    imgType = "otf";
                    break;

                case "vnd.oasis.opendocument.presentation-template":
                    imgType = "otp";
                    break;

                case "vnd.oasis.opendocument.text-template":
                    imgType = "ott";
                    break;

                case "vnd.palm":
                    imgType = "pdb";
                    break;
                case "pdf":
                    imgType = "pdf";
                    break;
                case "vnd.adobe.photoshop":
                    imgType = "psd";
                    break;
                case "x-mspublisher":
                    imgType = "pub";
                    break;
                case "x-rar-compressed":
                    imgType = "rar";
                    break;
                case "svg+xml":
                    imgType = "svg";
                    break;
                case "x-shockwave-flash":
                    imgType = "swf";
                    break;
                case "tiff":
                    imgType = "tiff";
                    break;
                case "plain":
                    imgType = "txt";
                    break;
                case "x-ms-wma":
                    imgType = "wma";
                    break;
                case "vnd.ms-works":
                    imgType = "wps";
                    break;
                case "vnd.xara":
                    imgType = "xar";
                    break;
                case "xml":
                    imgType = "xml";
                    break;
                case "zip":
                    imgType = "zip";
                    break;
                default:
                    break;
            }
            #endregion

            return imgType;
        }

        public string FireServiceMethod(GeneralParameters parameters, Settings settings)
        {
            try
            {
                Authenticate authenticate = new Authenticate();
                authenticate.Username = settings.ServiceUserName;
                authenticate.Password = settings.ServicePassword;

                WsConnection conn = new WsConnection();
                conn.Password = settings.DbPassword;
                conn.DataBaseTypes = (DataBaseType)settings.DatabaseType;
                conn.Database = settings.Database;
                conn.Ip = settings.ServerName;
                conn.DatabaseEncoding = "utf8";
                conn.Port = settings.Port;
                conn.UserName = settings.DbUser;


                parameters.Authenticate = authenticate;
                parameters.WsConnection = conn;


                string url = parameters.ParameterNames == null ? settings.ServiceAddress + "/api/executereader/query" : settings.ServiceAddress + "/api/executereader/queryparams";
                var json = JsonConvert.SerializeObject(parameters);
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", json, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                string responseStr = response.Content;
                responseStr = responseStr.TrimStart('\"');
                responseStr = responseStr.TrimEnd('\"');
                responseStr = responseStr.Replace("\\", "");
                responseStr = responseStr.Replace("\b", "");
                responseStr = responseStr.Replace("\f", "");
                responseStr = responseStr.Replace("\n", "");
                responseStr = responseStr.Replace("\r", "");
                responseStr = responseStr.Replace("\t", "");

                return responseStr.ToString();
            }
            catch (Exception ex)
            {
                //AddLogMesssage("Hata Oluştu" + ex.Message, LogMessageType.Error, item);
                return string.Empty;
            }

        }


        #endregion


    }
}