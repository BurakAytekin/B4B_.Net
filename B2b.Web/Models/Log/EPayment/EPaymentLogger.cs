using System;
using B2b.Web.v4.Models.Helper;
using Newtonsoft.Json;
using RestSharp;
using B2b.Web.v4.Models.EntityLayer;

namespace B2b.Web.v4.Models.Log.EPayment
{
    public class EPaymentLogger
    {
        public static void LogDB(EPaymentLog log, string apiAdress)
        {
            try
            {
                if (apiAdress == "/api/payment/Add")
                {
                    if (Token.GetTableControl())
                    {
                        string token = Token.GetUserToken();
                        if (token.Contains("ERROR"))
                        {
                            Logger.LogGeneral(LogGeneralErrorType.Error,ClientType.B2BWeb, "Api Error", new Exception(token),log.IpAddress, log.CustomerId);
                        }
                        else
                        {

                            string url = GlobalSettings.PaymentLogAdress + apiAdress;
                            var json = JsonConvert.SerializeObject(log);
                            var client = new RestClient(url);
                            var request = new RestRequest(Method.POST);
                            request.AddHeader("cache-control", "no-cache");
                            request.AddHeader("authorization", "Bearer " + token);
                            request.AddHeader("content-type", "application/json");
                            request.AddParameter("application/json", json, ParameterType.RequestBody);
                            IRestResponse response = client.Execute(request);

                        }

                    }
                }
                else
                {
                    string token = Token.GetUserToken();
                    if (token.Contains("ERROR"))
                    {
                        Logger.LogGeneral(LogGeneralErrorType.Error,ClientType.B2BWeb, "Api Error", new Exception(token),log.IpAddress, log.CustomerId);
                    }
                    else
                    {

                        string url = GlobalSettings.PaymentLogAdress + apiAdress;
                        var json = JsonConvert.SerializeObject(log);
                        var client = new RestClient(url);
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("cache-control", "no-cache");
                        request.AddHeader("authorization", "Bearer " + token);
                        request.AddHeader("content-type", "application/json");
                        request.AddParameter("application/json", json, ParameterType.RequestBody);
                        IRestResponse response = client.Execute(request);

                    }

                }





            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error,ClientType.B2BWeb, "Api Error Catch", ex,log.IpAddress, log.CustomerId);
            }
        }
    }
}