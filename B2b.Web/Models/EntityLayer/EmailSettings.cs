using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class EmailSettings : DataAccess
    {
        public int Id { get; set; }
        public string Host { get; set; }
        public int? HostPort { get; set; }
        public string FromAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool UseSsl { get; set; }
        public int Type { get; set; }
        public bool IsEryaz { get; set; }



        public static List<string> GetMailAddressByType(int type)
        {
            List<string> mailList = new List<string>();
            DataTable dt = DAL.GetMailAddressByType(type);
            foreach (DataRow row in dt.Rows)
            {
                mailList.Add(row.Field<string>("MailAddress"));
            }
            return mailList;
        }


        public static EmailSettings GetByType(int type)
        {
            EmailSettings mailSettings = new EmailSettings();
            DataTable dt = DAL.GetMailSettings(type);
            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];
                mailSettings.Id = row.Field<int>("Id");
                mailSettings.Host = row.Field<string>("Host");
                mailSettings.HostPort = row.Field<int?>("HostPort");
                mailSettings.FromAddress = row.Field<string>("FromAddress");
                mailSettings.UserName = row.Field<string>("UserName");
                mailSettings.Password = row.Field<string>("Password");
                mailSettings.UseSsl = row.Field<bool>("UseSsl");
                mailSettings.Type = row.Field<int>("Type");
                mailSettings.IsEryaz = row.Field<bool>("IsEryaz");
            }
            return mailSettings;
        }



        public bool Update()
        {
            return DAL.UpdateMailSettings(Type, Host, HostPort ?? 0, FromAddress, UserName, Password, Convert.ToInt16(UseSsl), Convert.ToInt16(IsEryaz));
        }
    }
    public partial class DataAccessLayer
    {
        public DataTable GetMailSettings(int pType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_MailSettings", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pType });
        }
        public DataTable GetMailAddressByType(int pType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_MailAddress", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pType });
        }
        public bool UpdateMailSettings(int pType, string pHost, int pHostPort, string pFromAddress, string pUserName, string pPassword, int pUseSsl, int pIsEryaz)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_MailSettings", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pType, pHost, pHostPort, pFromAddress, pUserName, pPassword, pUseSsl, pIsEryaz });
        }
    }
}