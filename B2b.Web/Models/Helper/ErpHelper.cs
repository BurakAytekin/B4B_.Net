using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.Helper
{
    public static class ErpHelper
    {
        public static string FireServiceMethod(GeneralParameters parameters, Settings settings)
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


        public static DataTable FireServiceMethodForMarsIntegrations(string query)
        {
            try
            {
                ErpFunctionDetail yearItem = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.MarsSevkEmri).Where(x => x.Settings.IsActiveCompany).First();

                GeneralParameters parametres = new GeneralParameters();
                parametres.CommandType = CommandType.Text;
                parametres.CommandText = query;
                parametres.ParameterNames = new object[] { };
                parametres.ParameterValues =  new object[] { };

                DataTable dt = ErpHelper.FireServiceMethod(parametres, yearItem.Settings).ConvertResponseDataTable();
                return dt;

            }
            catch (Exception ex)
            {
                return new DataTable();
            }

        }
    }
}