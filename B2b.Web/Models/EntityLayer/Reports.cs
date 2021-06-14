using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log.EPayment;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class AdminReports : DataAccess
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public string Name { get; set; }
        public string Function { get; set; }
        public int FunctionType { get; set; }
        public List<procedureParameters> Parameters { get; set; }
        public string ParametersStr { get; set; }
        public bool Type { get; set; }
        public bool IsActive { get; set; }
        public string ReportCreateName { get; set; }
        public string ReportEditName { get; set; }
        public DateTime? ReportCreateDate { get; set; }
        public DateTime? ReportEditDate { get; set; }
        public bool IsChart { get; set; }
        public Settings CompanySettings { get; set; }
        public List<Custo> Customers { get; set; }
        public List<Sales> Salesmans { get; set; }
        public int IsB2bShow { get; set; }

        #region Methods

        // tamam sorun yok
        public static List<AdminReports> GetReportMenuList(int reportIsActive, bool salesmanType)
        {
            List<AdminReports> priceList = new List<AdminReports>();
            DataTable dt = DAL.GetReportMenuList(1, reportIsActive, salesmanType);
            foreach (DataRow row in dt.Rows)
            {
                List<procedureParameters> procParamsList = new List<procedureParameters>();
                procedureParameters proc;
                var data = (!string.IsNullOrEmpty(row.Field<string>("Parameters"))) ? row.Field<string>("Parameters").Split(';') : null;
                if (data != null)
                    foreach (var item2 in data)
                    {
                        var par = item2.Split('=');
                        proc = new procedureParameters
                        {
                            paramName = par[0].ToString(),
                            Header = par[1].ToString(),
                            Type = par[2].ToString()
                        };
                        procParamsList.Add(proc);
                    }

                AdminReports report = new AdminReports
                {
                    Id = row.Field<int>("Id"),
                    Header = row.Field<string>("Header"),
                    Name = row.Field<string>("Name"),
                    Type = row.Field<bool>("Type"),
                    ReportCreateName = row.Field<string>("CreateName"),
                    ReportEditName = row.Field<string>("EditName"),
                    ReportEditDate = row["EditDate"] as DateTime?,
                    ReportCreateDate = row["CreateDate"] as DateTime?,
                    Function = row.Field<string>("Function"),
                    FunctionType = row.Field<int>("FunctionType"),
                    Parameters = procParamsList,
                    IsActive = row.Field<bool>("IsActive"),
                    IsChart = row.Field<bool>("IsChart"),
                    CompanySettings = new Settings
                    {
                        Id = row.Field<int>("SettingsId")
                    },
                    IsB2bShow = row.Field<int>("IsB2bShow"),
                };
                priceList.Add(report);
            }


            return priceList;
        }
        // tamam sorun yok
        public int SaveAndUpdate()
        {
            return DAL.SaveAndUpdateReport(Id, Name, Header, CompanySettings.Id, Type, Function, FunctionType, ParametersStr, IsActive, int.Parse(ReportCreateName), ReportCreateDate, int.Parse(ReportEditName), ReportEditDate, IsB2bShow);
        }
        // tamam sorun yok
        public bool Delete()
        {
            return DAL.DeleteReport(Id);
        }
        // tamam sorun yok
        public static DataTable GetReport(string procName, string[] paramsa, string[] types, string[] values, int exportViewType, Settings set)
        {
            DataTable dt = new DataTable();
            ReportType reportType = set == null ? ReportType.WebService : ReportType.Local ;
            switch (reportType)
            {
                case ReportType.Local:
                    {
                        MySqlParameter[] par = DatabaseContext.GenerateMysqlParamByArray(paramsa, types, values);
                        dt = DAL.GetReport(procName, par, exportViewType);
                    }; break;
                case ReportType.WebService:
                    {
                        Authenticate authenticate = new Authenticate();
                        authenticate.Username = set.ServiceUserName;
                        authenticate.Password = set.ServicePassword;

                        WsConnection conn = new WsConnection();
                        conn.Password = set.DbPassword;
                        conn.DataBaseTypes = (DataBaseType)set.DatabaseType;
                        conn.Database = set.Database;
                        conn.Ip = set.ServerName;
                        conn.DatabaseEncoding = "utf8";
                        conn.Port = set.Port;
                        conn.UserName = set.DbUser;

                        GeneralParameters parameters = new GeneralParameters();
                        parameters.ParameterNames = paramsa;
                        parameters.ParameterValues = values;
                        parameters.Authenticate = authenticate;
                        parameters.WsConnection = conn;
                        parameters.CommandType = exportViewType == 0 ? CommandType.StoredProcedure : CommandType.Text;
                        parameters.CommandText = exportViewType == 1 ? "select * from " + procName : procName;


                        string dataTable = FireServiceMethod(parameters, set.ServiceAddress);
                        dt = JsonConvert.DeserializeObject<Tuple<bool, DataTable, string>>(dataTable).Item2;
                    }
                    break;
                default: break;
            }
            return dt;
        }
        // tamam sorun yok
        public static AdminReports GetReport_ByReportId(int id, int SalesmanId)
        {
            AdminReports report = new AdminReports();
            DataTable dt = DAL.GetReportMenuList(id, 1, true);
            List<procedureParameters> procList = new List<procedureParameters>();

            foreach (DataRow row in dt.Rows)
            {
                procedureParameters proc;
                var data = (!string.IsNullOrEmpty(row.Field<string>("Parameters"))) ? row.Field<string>("Parameters").Split(';') : null;
                if (data != null)
                    foreach (var item2 in data)
                    {
                        var par = item2.Split('=');
                        if (par.Length == 3 && par[0].ToString() == "CariKod")
                            report.Customers = Customer.GetListCustomersBySalesmanId(SalesmanId).Select(x => new Custo { Code = x.Code, Name = x.Name }).ToList();
                        else if (par.Length == 3 && par[0].ToString() == "PlasiyerKod")
                            report.Salesmans = Salesman.GetList(SalesmanId).Select(x => new Sales { Code = x.Code, Name = x.Name }).ToList();

                        proc = new procedureParameters
                        {
                            paramName = par[0].ToString(),
                            Header = par[1].ToString(),
                            Type = par[2].ToString()
                        };
                        procList.Add(proc);
                    }
                report.Id = row.Field<int>("Id");
                report.Header = row.Field<string>("Header");
                report.Name = row.Field<string>("Name");
                report.Function = row.Field<string>("Function");
                report.FunctionType = row.Field<int>("FunctionType");
                report.Parameters = procList;
                report.Type = row.Field<bool>("Type");
                report.IsChart = row.Field<bool>("IsChart");
                if (row["SettingsId"].ToString() != "-1")
                    report.CompanySettings = new Settings()
                    {
                        Id = row.Field<int>("SettingsId"),
                        CompanyName = row.Field<string>("CompanyName"),
                        Company = row.Field<int>("Company"),
                        ServerName = row.Field<string>("ServerName"),
                        Database = row.Field<string>("Database"),
                        DbPassword = row.Field<string>("DbPassword"),
                        DbUser = row.Field<string>("DbUser"),
                        Donem = row.Field<int>("Donem"),
                        ErpName = row.Field<string>("ErpName"),
                        ErpPassword = Token.Decrypt(row.Field<string>("ErpPassword"), GlobalSettings.EncryptKey),
                        ErpUserName = Token.Decrypt(row.Field<string>("ErpUserName"), GlobalSettings.EncryptKey),
                        DatabaseType = row.Field<int>("DatabaseType"),
                        ServiceUserName = Token.Decrypt(row.Field<string>("ServiceUserName"), GlobalSettings.EncryptKey),
                        ServicePassword = Token.Decrypt(row.Field<string>("ServicePassword"), GlobalSettings.EncryptKey),
                        ServiceAddress = Token.Decrypt(row.Field<string>("ServiceAddress"), GlobalSettings.EncryptKey),
                        ServiceAddressLocal = Token.Decrypt(row.Field<string>("ServiceAddressLocal"), GlobalSettings.EncryptKey),
                        Port = Convert.ToUInt32(row["Port"]),
                        ServerIp = row.Field<string>("ServerIp")
                    };
            }
            return report;
        }
        // tamam sorun yok
        private static string FireServiceMethod(GeneralParameters parameters, string serverIp)
        {
            try
            {
                string url = parameters.ParameterNames == null ? serverIp + "/api/executereader/query" : serverIp + "/api/executereader/queryparams";
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
                //responseValue.SettingsId = Convert.ToInt32(context.JobDetail.Key.Name);

                //AddLogMesssage("Erp Sisteminden Veriler Alındı", LogMessageType.Success, item);

                return responseStr;
            }
            catch (Exception ex)
            {
                //AddLogMesssage("Hata Oluştu" + ex.Message, LogMessageType.Error, item);
                return string.Empty;
            }

        }
        #endregion
    }

    public class Custo
    {
        public string Code { get; set; }
        public string Name { get; set; }

    }

    public class Sales
    {
        public string Code { get; set; }
        public string Name { get; set; }

    }


    public class procedureParameters
    {
        public string paramName { get; set; }
        public string Header { get; set; }
        public string Type { get; set; }
    }
    public partial class DataAccessLayer
    {
        public DataTable GetReportMenuList(int pId, int pIsActive, bool pSalesmanType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Report_GetList_Reports", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pIsActive, pSalesmanType });
        }


        public DataTable GetReportMenuList(int pId, int pIsActive)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Report_GetList_Reports", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pIsActive });
        }
        public DataTable GetReport_ById(int pId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Report_GetReport_ById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }


        public DataTable GetReport(string procName, MySqlParameter[] mysqlParams, int exportViewType /*ParameterInfo[] @params, string[] values*/)
        {
            procName = exportViewType == 1 ? "select * from " + procName : procName;
            return DatabaseContext.ExecuteReaderReport(exportViewType == 0 ? CommandType.StoredProcedure : CommandType.Text, procName, mysqlParams /*@params,*//*MethodBase.GetCurrentMethod().GetParameters(), values*/);
        }

        public int SaveAndUpdateReport(int pId, string pName, string pHeader, int pSettingsId, bool pType, string pFunctions, int pFunctionType,
            string pParameters, bool pIsActive, int pCreateName, DateTime? pCreateDate, int pEditName, DateTime? pEditDate, int pIsB2bShow)
        {
            return DatabaseContext.ExecuteScalar(CommandType.StoredProcedure, "_Report_InsertAndUpdate_Report", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pName, pHeader, pSettingsId, pType, pFunctions,
                                                                                                         pFunctionType,pParameters,pIsActive,pCreateName,pCreateDate,pEditName,pEditDate,pIsB2bShow});
        }

        public bool DeleteReport(int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Report_Delete_Report", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }


    }
}
public enum ReportType
{
    Local = 0,
    WebService = 1
}