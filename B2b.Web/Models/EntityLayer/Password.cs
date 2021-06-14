
using B2b.Web.v4.Models.Helper;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Password : DataAccess
    {
        #region Properties
        public LoginType Type { get; set; }
        public int UserId { get; set; }
        public string NewPassword { get; set; }
        #endregion


        #region Methods
        public static bool ChangePasWordWindows(string pNewPassword, int pUserId, int pType)
        {
            if (pType == (int)LoginType.Customer)
                return DAL.UpdatePasswordCustomer(pNewPassword, pUserId);

            if (pType == (int)LoginType.Salesman || pType == (int)LoginType.Admin)
                return DAL.UpdatePasswordSalesman(pNewPassword, pUserId);

            return false;
        }
        #endregion
    }
    public partial class DataAccessLayer
    {
        public bool UpdatePasswordCustomer(string pPassword, int pUserId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_CustomerPassword", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pPassword, pUserId });
        }

        public bool UpdatePasswordSalesman(string pPassword, int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_SalesmanPassword", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pPassword,pId });
        }
    }
}