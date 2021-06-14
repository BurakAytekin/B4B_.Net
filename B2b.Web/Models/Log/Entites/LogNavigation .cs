
using B2b.Web.v4.Models.EntityLayer;
using System;
using System.Data;
using System.Reflection;
using B2b.Web.v4.Models.Log;


namespace B2b.Web.v4.Models.EntityLayer
{
    class LogNavigation : DataAccess
    {
        #region Constructors
        public LogNavigation()
        {
            ClientType = ClientType.B2BWeb;
            CustomerId = -1;
            SalesmanId = -1;
            UserId = -1; ;

        }
        #endregion

        #region Parametres
        public int Id { get; set; }
        public ClientType ClientType { get; set; }
        public int CustomerId { get; set; }
        public int SalesmanId { get; set; }
        public int UserId { get; set; }
        public string Navigation { get; set; }
        public string IpAddress { get; set; }
        #endregion

        #region Methods

        public bool Save()
        {
            return DAL.InsertLogNavigation(CustomerId,UserId,SalesmanId,Navigation,ClientType.ToString(),IpAddress);
        }

        #endregion

    }
    public partial class DataAccessLayer
    {
        public bool InsertLogNavigation(int pCustomerId, int pUserId, int pSalesmanId, string pNavigation, string pClientType, string pIpAddress)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_Log_Navigation", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId, pSalesmanId, pNavigation, pClientType, pIpAddress });
        }

    }
}