
using Quartz;
using Quartz.Impl;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using B2b.Web.v4.Models.EntityLayer;
using System.Data;
using RestSharp;
using Newtonsoft.Json;
using B2b.Web.v4.Models.SyncLayer;
using B2b.Web.v4.Models.Helper;
using MySql.Data.MySqlClient;
using B2b.Web.v4.Models.ErpLayer;

namespace B2b.Web.v4.Areas.Admin.Models
{
    public class JobScheduler
    {
        static IScheduler scheduler;
        public static void Start()
        {
            var schedulerFactory = new StdSchedulerFactory();
            scheduler = schedulerFactory.GetScheduler().Result;

            scheduler.DeleteJob(new JobKey("daily"));

            if (!scheduler.IsStarted)
                scheduler.Start().Wait();

            // daily
            {
                IJobDetail process = JobBuilder.Create<JobNotification>().WithIdentity("daily").Build();
                ITrigger tetikle = TriggerBuilder.Create()
                    .UsingJobData("daily", "daily")
                    .WithIdentity("daily", "daily")
                    .WithDailyTimeIntervalSchedule(
                        s => s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(23, 50)) //hergün çalışacağı bilgisi
                        .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(23, 50)) //hergün çalışacağı bilgisi
                    ).Build();
                scheduler.ScheduleJob(process, tetikle);
            }

            // hourly
            {
                IJobDetail jobDetail = JobBuilder.Create<JobSystemResponssClear>().WithIdentity("hourly").Build();
                ITrigger trigger = TriggerBuilder.Create()
                    .WithDailyTimeIntervalSchedule(
                        s => s.WithIntervalInHours(1)
                    ).StartNow()
                    .Build();
                scheduler.ScheduleJob(jobDetail, trigger);
            }

            //Mars Entegrasyon Mars'a gönderme
            {
                IJobDetail job = JobBuilder.Create<JobMarsEntegrationTo>().WithIdentity("mars_to").Build();
                ITrigger trigger = TriggerBuilder.Create()
                    .WithSimpleSchedule(x => x.WithIntervalInMinutes(5).RepeatForever())
                    .StartNow()
                    .Build();
                scheduler.ScheduleJob(job, trigger);
            }


            //Mars Entegrasyon Mars'tan alma
            {
                IJobDetail job = JobBuilder.Create<JobMarsEntegrationFrom>().WithIdentity("mars_from").Build();
                ITrigger trigger = TriggerBuilder.Create()
                    .WithSimpleSchedule(x => x.WithIntervalInMinutes(5).RepeatForever())
                    .StartNow()
                    .Build();
                scheduler.ScheduleJob(job, trigger);
            }

            // Marstan gelen irsaliye numaralarının içeriye işlenmesi 
            //{
            //    IJobDetail job = JobBuilder.Create<JobProcessSerialNumber>().WithIdentity("process_serial").Build();
            //    ITrigger trigger = TriggerBuilder.Create()
            //        .WithSimpleSchedule(x => x.WithIntervalInMinutes(5).WithRepeatCount(0))
            //        .StartNow()
            //        .Build();
            //    scheduler.ScheduleJob(job, trigger);
            //}
        }

        public static void Fire(SyncSettings item = null)
        {
            scheduler.TriggerJob(new JobKey(item.TransferTypeId.ToString(), "DEFAULT"));
        }

        public static void Stop()
        {
            if (scheduler.IsStarted)
                scheduler.Shutdown(false);
        }
    }
    public class JobNotification : IJob
    {
        //  public static SyncResponseValues responseValue = new SyncResponseValues();
        public Task Execute(IJobExecutionContext context)
        {
            Notifications.DeleteDailiyNotification(0);// Günlük Bilgilendirmeleri siler
            Notifications.DeleteDailiyNotification(1);// Gecikmeli bilgilendirmeleri siler
            Notifications.GenerateAutoNotificationsBasket("Sepetinizde bekleyen ürünleriniz vardır");
            Notifications.GenerateAutoNotificationsCampaign("Bugün kampanyaya giren ürünler vardır");
            List<CustomerSmall> customerList = customerList = Customer.GetListLimited(string.Empty, 0, "*", "*", true, -1, 0, 5000);

            List<ErpFunctionDetail> BackOrderYearList = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.BackOrder);
            string customerIds = string.Empty;
            List<ErpFunctionDetail> customerBalance = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.CustomerDashboard);

            foreach (CustomerSmall item in customerList)
            {

                try
                {
                    ErpFunctionDetail yearItem = BackOrderYearList.Where(x => x.Settings.IsActiveCompany).First();

                    GeneralParameters parametres = new GeneralParameters();
                    parametres.CommandType = yearItem.FunctionType == 0 ? CommandType.StoredProcedure : CommandType.Text;
                    parametres.ParameterNames = (new string[1] { "CustomerCode" });
                    parametres.ParameterValues = (new string[1] { item.Code });
                    parametres.CommandText = yearItem.FunctionName;

                    DataTable dt = FireServiceMethod(parametres, yearItem.Settings).ConvertResponseDataTable();
                    List<BackOrder> list = new List<BackOrder>();
                    list = dt.DataTableToList<BackOrder>();
                    if (list.Where(x => x.Quantity > 0).Count() > 0)
                    {
                        customerIds += item.Id + ",";
                    }
                }
                catch (Exception)
                {
                }


                CustomerBalanceInformation balanceItem = new CustomerBalanceInformation();

                try
                {
                    ErpFunctionDetail yearItem2 = customerBalance.Where(x => x.Settings.IsActiveCompany).First();

                    List<CustomerInvoice> financeListTmp = new List<CustomerInvoice>();


                    GeneralParameters parametres = new GeneralParameters();
                    parametres.CommandType = yearItem2.FunctionType == 0 ? CommandType.StoredProcedure : CommandType.Text;
                    parametres.ParameterNames = (new string[1] { "CustomerCode" });
                    parametres.ParameterValues = (new string[1] { item.Code });
                    parametres.CommandText = yearItem2.FunctionName;

                    DataTable dt = FireServiceMethod(parametres, yearItem2.Settings).ConvertResponseDataTable();
                    // List<CustomerInvoice> list = new List<CustomerInvoice>();
                    balanceItem = dt.DataTableToItem<CustomerBalanceInformation>();
                }
                catch (Exception)
                {

                }
                if (balanceItem.UnClosedBalance > 0)
                {
                    balanceItem.UnClosedBalanceStr = balanceItem.UnClosedBalance.ToString("N2") + " " + item.CurrencyType;
                    Notifications.GenerateAutoNotificationsCustomers(item.Id.ToString(), "Geciken Ödeme", "Vadesi geçen " + balanceItem.UnClosedBalanceStr + " değerinde borcunuz bulunmaktadır");
                }




            }
            if (customerIds.Length > 1)
            {
                customerIds = customerIds.Substring(0, customerIds.Length - 1);
                Notifications.GenerateAutoNotificationsCustomers(customerIds, "Bakiye Siparişler", "Bakiye ürünlerinizde stoğa giren ürünler vardır");
            }



            HttpRuntime.UnloadAppDomain();
            return null;
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


        public JobNotification()
        {
            // responseValue = new SyncResponseValues();
            //responseValue = new SyncResponseValues();
        }
    }


    public class JobSystemResponssClear : IJob
    {
        //  public static SyncResponseValues responseValue = new SyncResponseValues();
        public Task Execute(IJobExecutionContext context)
        {
            RuningControl.SyncResponseValues.Log = new List<LogMessage>();

            return null;
        }
    }



    #region Api Request Class
    public class GeneralParameters
    {
        public WsConnection WsConnection { get; set; }
        public Authenticate Authenticate { get; set; }
        public CommandType CommandType { get; set; }
        public string CommandText { get; set; }
        public object[] ParameterNames { get; set; }
        public object[] ParameterValues { get; set; }

    }
    public class Authenticate
    {
        public string Username;
        public string Password;
    }
    public class WsConnection
    {
        #region Properties

        public string Ip { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public uint Port { get; set; }
        public string Database { get; set; }
        public string DatabaseEncoding { get; set; }
        public DataBaseType DataBaseTypes { get; set; }
        #endregion

    }

    public enum DataBaseType
    {
        MsSql = 1,
        Mysql = 2,
        Oracle = 3,
        FireBird = 4
    }

    #endregion



    public class SyncResponseValues
    {
        public int SettingsId { get; set; }
        public int ProgressValue { get; set; }
        public int Status { get; set; }
        public List<LogMessage> Log { get; set; }
        public SyncResponseValues()
        {
            Log = new List<LogMessage>();
        }


    }
    public class LogMessage
    {
        public string Message { get; set; }
        public LogMessageType Type { get; set; }

    }
    public enum LogMessageType
    {
        Success = 0,
        Error = 1
    }
}