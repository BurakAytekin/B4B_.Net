using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class LogProductOfDay : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int ProductOfDayId { get; set; }
        public int CustomerId { get; set; }
        public int SalesmanId { get; set; }

        #endregion

        #region Methods

        public bool Add()
        {
            return DAL.InsertProductOfDayLog(ProductOfDayId, CustomerId, SalesmanId);
        }


        #endregion

    }

    public partial class DataAccessLayer
    {
        public bool InsertProductOfDayLog(int pProductOfDayId, int pCustomerId, int pSalesmanId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_ProductOfDayLog", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductOfDayId, pCustomerId, pSalesmanId });
        }
    }
}