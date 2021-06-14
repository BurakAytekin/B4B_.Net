using System.Data;
using System.Reflection;
using B2b.Web.v4.Models.Helper;
using System.ComponentModel.DataAnnotations;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Logon : DataAccess
    {
        #region Properties
        [Display(Name = "Müşteri kodu")]
        public string CustomerCode { get; set; }
        [Required(ErrorMessage = "Lütfen Parolanızı giriniz.")]
        [Display(Name = "Parola")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Code { get; set; }
        [Required(ErrorMessage = "Lütfen Kullanıcı Kodunuzu giriniz.")]
        [Display(Name = "Kullanıcı Kodu")]
        public string UserCode { get; set; }

        [Display(Name = "Doğrulama Kodu")]
        //[Required(ErrorMessage = "Lütfen doğrulama kodunu boş bırakmayınız.")]
        public string Captcha { get; set; }
        #endregion

        #region Methods
        public Customer CustomerLogin()
        {
            DataTable dt = DAL.LoginCustomer(CustomerCode, UserCode, Password, (int)SystemType.Web);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return Customer.GetById(row.Field<int>("Id"), row.Field<int>("UserId"));
            }
            return null;
        }

        public Salesman SalesmanLogin()
        {
            DataTable dt = DAL.LoginSalesman(UserCode, Password, (int)SystemType.Web);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return Salesman.GetById(row.Field<int>("Id"));
            }
            return null;
        }

        public Salesman AdminSalesmanLogin()
        {
            DataTable dt;

            dt = DAL.AdminSalesmanLogin(UserCode, Password);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                return Salesman.GetById(row.Field<int>("Id"));
            }
            return null;
        }
        public Salesman AdminSalesmanAuthenticationCheck()
        {
            DataTable dt;

            dt = DAL.AdminSalesmanAuthenticationCheck(UserCode);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                Salesman item = new Salesman()
                {
                    Id = row.Field<int>("Id"),
                    IsAuthenticator = row.Field<bool>("IsAuthenticator"),
                    AuthenticatorGuid = row.Field<string>("AuthenticatorGuid"),
                    IsB2bAuthenticator = row.Field<bool>("IsB2bAuthenticator"),
                };
                return item;
            }
            return null;
        }


        public Users UserAuthenticationCheck()
        {
            DataTable dt;

            dt = DAL.UserAuthenticationCheck(CustomerCode, UserCode);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                Users item = new Users()
                {
                    Id = row.Field<int>("Id"),
                    IsAuthenticator = row.Field<bool>("IsAuthenticator"),
                    AuthenticatorGuid = row.Field<string>("AuthenticatorGuid"),
                };
                return item;
            }
            return null;
        }


        #endregion
    }

    public partial class DataAccessLayer
    {
        public DataTable LoginCustomer(string pCustomerCode, string pUserCode, string pPassword, int pSystemType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_Customer_Login", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerCode, pUserCode, pPassword, pSystemType });
        }

        public DataTable LoginSalesman(string pUserCode, string pPassword, int pSystemType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_Salesman_Login", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pUserCode, pPassword, pSystemType });
        }

        public DataTable AdminSalesmanAuthenticationCheck(string pUserCode)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_Salesman_Authentication", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pUserCode });
        }

        public DataTable UserAuthenticationCheck(string pCustomerCode, string pUserCode)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_User_Authentication", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerCode, pUserCode });
        }

        public DataTable AdminSalesmanLogin(string pCode, string pPass)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetItem_Salesman_Login", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCode, pPass });
        }
    }
}