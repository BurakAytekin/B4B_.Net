using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Xml.Serialization;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class LogSearchOpenDetail : DataAccess
    {

        #region Parametres
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int SalesmanId { get; set; }
        public string CustomerCode { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public DateTime RecordDate { get; set; }
        public int LogHeaderId { get; set; }

        #endregion

        public bool Insert()
        {

            return DAL.InserLogSearchOpenDetail(CustomerId, CustomerCode, UserId, SalesmanId, ProductId, ProductCode, LogHeaderId);

        }


    }



    public partial class DataAccessLayer
    {


        public bool InserLogSearchOpenDetail(int pCustomerId, string pCustomerCode, int pUserId, int pSalesmanId, int pProductId, string pProductCode,int pLogHeaderId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_Log_Open_Detail", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pCustomerCode, pUserId, pSalesmanId, pProductId, pProductCode , pLogHeaderId });
        }



    }
}