using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log.EPayment;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class SystemAnalysisController : AdminBaseController
    {
        // GET: Admin/SystemAnalysis
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string CheckWebServiceConnection(Settings settings)
        {
            AnalysisResult resultItem = new AnalysisResult();

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

                GeneralParameters parametres = new GeneralParameters();
                parametres.CommandType = CommandType.StoredProcedure;
                parametres.CommandText = "";

                parametres.Authenticate = authenticate;
                parametres.WsConnection = conn;


                string url = settings.ServiceAddress + "/api/checkRunning";
                var json = JsonConvert.SerializeObject(parametres);
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

                resultItem.Message = responseStr.ToString();
                if (responseStr == "true")
                    resultItem.Result = true;
                else
                    resultItem.Result = false;
            }
            catch (Exception ex)
            {
                resultItem.Result = false;
                resultItem.Message = ex.Message;
            }


            return JsonConvert.SerializeObject(resultItem);
        }

        [HttpPost]
        public string CheckWindowsServiceConnection(Settings settings)
        {
            AnalysisResult resultItem = new AnalysisResult();

            try
            {
                string param = Token.Encrypt(("CheckService") + GlobalSettings.B2bAddress, GlobalSettings.EncryptKey);
                Socket soket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                soket.Connect(new IPEndPoint(IPAddress.Parse(settings.ServerIp), 8984));
                byte[] sendData = Encoding.UTF8.GetBytes(param);
                soket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback(AsyncCallBack), null);

                resultItem.Result = true;
            }
            catch (Exception ex)
            {
                resultItem.Result = false;
                resultItem.Message = ex.Message;

            }

            return JsonConvert.SerializeObject(resultItem);
        }


        [HttpPost]
        public string UploadFileControl()
        {
            AnalysisResult resultItem = new AnalysisResult();

            byte[] fileData = null;
            string fullFtpFilePath = GlobalSettings.FtpServerUploadAddress ;

            string[] items = { "General/", "Pictures/", "Salesman/" };
            FileStream fs = new FileStream(Server.MapPath("~/Content/images/404.jpg"), FileMode.Open, FileAccess.Read);
            fileData = new byte[fs.Length];

            try
            {
                foreach (var item in items)
                {
                    string path = fullFtpFilePath + item + "404.jpg";
                    bool result = FtpHelper.UploadRemoteServer(fileData, path);
                    if (result == false)
                    {
                        resultItem.Result = false;
                        resultItem.Message = "İşlem Başarısız";
                    }
                }
                resultItem.Result = true;

            }
            catch (Exception ex)
            {
                resultItem.Result = false;
                resultItem.Message = ex.Message;
            }



            return JsonConvert.SerializeObject(resultItem);
        }


        [HttpPost]
        public string CheckRuleCount()
        {
            AnalysisResult resultItem = new AnalysisResult();

            AnalysisControl item = AnalysisControl.CheckRuleCount();

            resultItem.Result = item.Count > 0 ? true : false;
            return JsonConvert.SerializeObject(resultItem);
        }

        [HttpPost]
        public string CheckRuleDublicateCount()
        {
            AnalysisResult resultItem = new AnalysisResult();

            AnalysisControl item = AnalysisControl.CheckRuleDublicate();

            if (item.Count > 0)
            {
                resultItem.Result = false;
                resultItem.Message = item.Count.ToString();
            }
            else
                resultItem.Result = true;

            return JsonConvert.SerializeObject(resultItem);
        }

        [HttpPost]
        public string CheckCustomerAndProductRule()
        {
            AnalysisResult resultItem = new AnalysisResult();

            AnalysisControl item = AnalysisControl.CheckCustomerAndProductRule();

            if (item.CountCustomer > 0 || item.CountProduct > 0)
            {
                resultItem.Result = false;
                resultItem.Message = (item.CountCustomer > 0 ?  "Koşulu olmayan müşteri sayısı : " +item.CountCustomer.ToString() : "")+ (item.CountProduct > 0 ? " Koşulu olmayan ürün sayısı : " + item.CountProduct.ToString() : "");
            }
            else
                resultItem.Result = true;

            return JsonConvert.SerializeObject(resultItem);
        }


 [HttpPost]
        public string CheckPriceControl()
        {
            AnalysisResult resultItem = new AnalysisResult();

            AnalysisControl item = AnalysisControl.CheckPriceControl();

            if (item.Count > 0)
            {
                resultItem.Result = false;
                resultItem.Message = item.Count.ToString();
            }
            else
                resultItem.Result = true;

            return JsonConvert.SerializeObject(resultItem);
        }

        private void AsyncCallBack(IAsyncResult ar)
        {

        }
    }
    public class AnalysisResult
    {
        public AnalysisResult()
        {
            Message = string.Empty;
        }

        public string Message { get; set; }
        public bool Result { get; set; }
    }
}