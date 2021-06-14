using B2b.Web.v4.Models.Log.EPayment;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.Helper
{
    public class GlobalSettings
    {
        public static string Ip => ConfigurationManager.AppSettings["ip"];

        public static string UserName => ConfigurationManager.AppSettings["username"];

        public static string Password => ConfigurationManager.AppSettings["password"];

        public static uint Port => Convert.ToUInt32(ConfigurationManager.AppSettings["port"]);

        public static string Database => ConfigurationManager.AppSettings["database"];

        public static string DatabaseEncoding => ConfigurationManager.AppSettings["dbEncoding"];
        
      

        public static string CookieName => ConfigurationManager.AppSettings["cookieName"];

        public static string CookieNameUserName => ConfigurationManager.AppSettings["cookieNameForUserName"];

        public static string B2bAddress => ConfigurationManager.AppSettings["b2bAddress"];
        public static string B2bAddressLocal => ConfigurationManager.AppSettings["b2bAddressLocal"];
        public static string FtpServerAddress => "https://"+ConfigurationManager.AppSettings["ftpServerAddress"];
        public static string FtpServerAddressFull => "https://"+ConfigurationManager.AppSettings["ftpServerAddress"]+ ConfigurationManager.AppSettings["ftpCompanyName"];
        public static string FtpServerUploadAddress => "ftp://" + ConfigurationManager.AppSettings["ftpServerUploadAddress"];
        public static string FtpCompanyName => ConfigurationManager.AppSettings["ftpCompanyName"];
        public static string FtpUserName => ConfigurationManager.AppSettings["ftpUserName"];
        public static string FtpPassword => ConfigurationManager.AppSettings["ftpPassword"];
        public static string CompanyName => ConfigurationManager.AppSettings["companyName"];
        public static string CompanyPath => ConfigurationManager.AppSettings["companyPath"];
        public static string GeneralPath = "General/";
        public static string SalesmanPath = "Salesman/";
        public static string EncryptPassword => ConfigurationManager.AppSettings["encryptPassword"];

        public static string PaymentLogPassword => ConfigurationManager.AppSettings["paymentLogPassword"];

        public static string PaymentLogAdress => ConfigurationManager.AppSettings["paymentLogAdress"];
        public static string EncryptKey { get {return "cNqXTA87wed24nFmRq"; } }

        public static bool MarsEntegration => Convert.ToBoolean(ConfigurationManager.AppSettings["marsEntegration"]);

        public static string ConnectionString
        {
            get
            {
                string cs = string.Empty;
                MySqlConnectionStringBuilder mcsb = new MySqlConnectionStringBuilder();
                mcsb.Database = GlobalSettings.Database;
                mcsb.UserID = GlobalSettings.UserName;
                mcsb.Password = GlobalSettings.Password;
                mcsb.CharacterSet = GlobalSettings.DatabaseEncoding;
                mcsb.Server = GlobalSettings.Ip;
                mcsb.Port = GlobalSettings.Port;
                mcsb.ConnectionTimeout = 120;
                cs = mcsb.ToString();

                return cs;
            }
        }

    }
}