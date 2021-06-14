using B2b.Web.v4.Models.EntityLayer;
using System;
using System.Data;
using System.Reflection;
using System.Web.Helpers;
using B2b.Web.v4.Models.Log;


namespace B2b.Web.v4.Models.EntityLayer
{
    public class LogPayment : DataAccess
    {
        #region Constructors
        public LogPayment()
        {
            Client = ClientType.B2BWeb;
            CustomerId = -1;
            SalesmanId = -1;
        }
        #endregion

        #region Properties
        public int Id { get; set; }
        public ClientType Client { get; set; }
        public int CustomerId { get; set; }
        public int SalesmanId { get; set; }
        public DateTime ServerDate { get; set; }
        public LogPaymentType LogType { get; set; }
        public string Source { get; set; }
        public string Explanation { get; set; }
        public string CurrentPaymentId { get; set; }
        public string BankName { get; set; }
        #endregion

        #region Methods
        public bool Save()
        {
            return DAL.InsertLogPayment(Client.ToString(), CustomerId, SalesmanId, LogType.ToString(), Source, Explanation, CurrentPaymentId, BankName);
        }
        #endregion
    }
    public partial class DataAccessLayer
    {
        public bool InsertLogPayment(string pClient, int pCustomerId, int pSalesmanId, string pLogType, string pSource, string pExplanation, string pCurrentPaymentId, string pBankName)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "Insert_LogPayment", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pClient, pCustomerId, pSalesmanId, pLogType, pSource, pExplanation, pCurrentPaymentId, pBankName });
        }

    }
    public enum LogPaymentType
    {
        Error,
        Information,
        Request,
        Response,
        PointQuery

    }
}