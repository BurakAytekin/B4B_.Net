

using System;
using System.Data;
using System.Reflection;
using B2b.Web.v4.Models.Helper;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class BasketCount : DataAccess
    {
        #region Properties
        public int CustomerCart { get; set; }
        public int SalesmanCart { get; set; }
        public LoginType LoginType { get; set; }
        #endregion

        #region Constructor
        public BasketCount()
        {
            CustomerCart = 0;
            SalesmanCart = 0;
            LoginType = LoginType.Customer;
        }
        #endregion

        public static BasketCount GetCount(int customerId, int usersId, LoginType loginType)
        {
            BasketCount basketItem = new BasketCount();
            DataTable dt = DAL.GetBasketCount(customerId, usersId);

            foreach (DataRow row in dt.Rows)
            {
                basketItem = new BasketCount()
                {
                    CustomerCart = Convert.ToInt32(row["CustomerCart"]),
                    SalesmanCart = loginType == LoginType.Salesman ? Convert.ToInt32(row["SalesmanCart"]) : 0,
                    LoginType = loginType


                };

            }
            return basketItem;
        }
    }
    public partial class DataAccessLayer
    {
        public DataTable GetBasketCount(int pCustomerId, int pUserId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_BasketCount", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId });
        }

    }
}