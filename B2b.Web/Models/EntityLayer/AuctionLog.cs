using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class AuctionLog : DataAccess
    {

        #region Properties
        public int Id { get; set; }
        public int AuctionId { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int SalesmanId { get; set; }
        public double Price { get; set; }

        #endregion

        #region Methods

        public void Add()
        {
            DAL.InsertAuctionLog(AuctionId,CustomerId,UserId,SalesmanId,Price);
        }

        #endregion
    }

    public partial class DataAccessLayer
    {
        public void InsertAuctionLog(int pAuctionId, int pCustomerId, int pUserId, int pSalesmanId, double pPrice)
        {
            DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_AuctionLog", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pAuctionId, pCustomerId, pUserId, pSalesmanId, pPrice});

        }
    }
}