using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;


namespace B2b.Web.v4.Models.EntityLayer
{
    public class Notifications : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Header { get; set; }
        public string Message { get; set; }
        public int CustomerId { get; set; }
        public int SalesmanId { get; set; }
        public bool IsOnlySalesman { get; set; }
        public bool IsDaily { get; set; }
        public DateTime SelectTime { get; set; }
        #endregion
        #region Methods

        public static List<Notifications> GetNotifications(Customer customer, Salesman salesman, LoginType loginType)
        {
            List<Notifications> list = new List<Notifications>();
            DataTable dt = DAL.GetNotifications(customer.Id, salesman.Id, (int)loginType);

            foreach (DataRow row in dt.Rows)
            {
                Notifications item = new Notifications()
                {

                    Id = row.Field<int>("Id"),
                    Header = row.Field<string>("Header"),
                    Message = row.Field<string>("Message"),
                    IsOnlySalesman = row.Field<bool>("IsOnlySalesman"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    SelectTime = DateTime.Now
                };
                list.Add(item);
            }

            return list;
        }


        public static void AddNotification(string pHeader, string pMessage, int pCustomerId, int pSalesmanId, bool pIsOnlySalesman, bool pIsDaily, int pCreateId)
        {
            DAL.InsertNotification(pHeader, pMessage, pCustomerId, pSalesmanId, pIsOnlySalesman, pIsDaily, pCreateId);
        }

        public static void DeleteDailiyNotification(int pType)
        {
            DAL.DeleteDailiyNotification(pType);
        }

        public static void GenerateAutoNotificationsBasket(string pMessage)
        {
            DAL.GenerateAutoNotificationsBasket(pMessage);
        }

        public static void GenerateAutoNotificationsCampaign(string pMessage)
        {
            DAL.GenerateAutoNotificationsCampaign(pMessage);
        }

        public static void GenerateAutoNotificationsCustomers(string pIds, string pHeader, string pMessage)
        {
            DAL.GenerateAutoNotificationsCustomers(pIds, pHeader, pMessage);
        }

        public bool Delete()
        {
            return DAL.DeleteNotification(Id, EditId);
        }

        #endregion
    }

    public partial class DataAccessLayer
    {
        public DataTable GetNotifications(int pCustomerId, int pSalesmanId, int pLoginType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Notifications", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pSalesmanId, pLoginType });
        }

        public bool InsertNotification(string pHeader, string pMessage, int pCustomerId, int pSalesmanId, bool pIsOnlySalesman, bool pIsDaily, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Notification", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pHeader, pMessage, pCustomerId, pSalesmanId, pIsOnlySalesman, pIsDaily, pCreateId });
        }

        public bool DeleteDailiyNotification(int pType)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_DailyNotification", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pType });
        }

        public bool GenerateAutoNotificationsBasket(string pMessage)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Generate_AutoNotificationBasket", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pMessage });
        }

        public bool GenerateAutoNotificationsCampaign(string pMessage)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Generate_AutoNotificationCampaign", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pMessage });
        }


        public bool GenerateAutoNotificationsCustomers(string pIds, string pHeader, string pMessage)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Generate_AutoNotificationCustomers", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pIds, pHeader, pMessage });
        }

        public bool DeleteNotification(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Delete_Notification", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }

    }
}