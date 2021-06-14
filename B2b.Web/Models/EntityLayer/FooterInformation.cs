using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class FooterInformation : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Facebook { get; set; }
        public string Twitter { get; set; }
        public string Google { get; set; }
        public string Skype { get; set; }
        public string Email { get; set; }
        public string ConditionsForReturn { get; set; }
        public string PrivacyPolicy { get; set; }
        public string TermOfUse { get; set; }
        public string DistanceSalesContract { get; set; }
        public string Linkedin { get; set; }
        public string Instagram { get; set; }
        public string KvkkContract { get; set; }
        public string PaymentContract { get; set; }
        #endregion

        #region Methods

        public static FooterInformation GetFooterItem()
        {
            DataTable dt = DAL.GetFooterItem();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                FooterInformation item = new FooterInformation()
                {
                    Id = row.Field<int>("Id"),
                    ConditionsForReturn = row.Field<string>("ConditionsForReturn"),
                    DistanceSalesContract = row.Field<string>("DistanceSalesContract"),
                    Email = row.Field<string>("Email"),
                    Facebook = row.Field<string>("Facebook"),
                    Google = row.Field<string>("Google"),
                    PrivacyPolicy = row.Field<string>("PrivacyPolicy"),
                    Skype = row.Field<string>("Skype"),
                    TermOfUse = row.Field<string>("TermOfUse"),
                    Twitter = row.Field<string>("Twitter"),
                    Instagram = row.Field<string>("Instagram"),
                    Linkedin = row.Field<string>("Linkedin"),
                    KvkkContract = row.Field<string>("KvkkContract"),
                    PaymentContract = row.Field<string>("PaymentContract")
                };
                return item;
            }

            return null;
        }

        public bool Add()
        {
            return DAL.InsertFooter(Facebook, Twitter, Google, Skype, Email, ConditionsForReturn, PrivacyPolicy, TermOfUse, DistanceSalesContract, KvkkContract, PaymentContract, Instagram,Linkedin,CreateId);
        }

        public bool Update()
        {
            return DAL.UpdateFooter(Id, Facebook, Twitter, Google, Skype, Email, ConditionsForReturn, PrivacyPolicy, TermOfUse, DistanceSalesContract, KvkkContract, PaymentContract, Instagram, Linkedin);
        }
        #endregion

    }
    public partial class DataAccessLayer
    {
        public DataTable GetFooterItem()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_FooterInformation");
        }

        public bool UpdateFooter(int pId, string pFacebook, string pTwitter, string pGoogle,string pSkype, string pEmail, string pConditionsForReturn, string pPrivacyPolicy, string pTermOfUse, string pDistanceSalesContract,string pKvkkContract, string pPaymentContract, string pInstagram,string pLinkedin)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_FooterInformation", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pFacebook, pTwitter, pGoogle, pSkype,pEmail, pConditionsForReturn, pPrivacyPolicy, pTermOfUse, pDistanceSalesContract, pKvkkContract, pPaymentContract, pInstagram , pLinkedin });
        }

        public bool InsertFooter( string pFacebook, string pTwitter, string pGoogle, string pSkype, string pEmail, string pConditionsForReturn, string pPrivacyPolicy, string pTermOfUse, string pDistanceSalesContract,string pKvkkContract, string pPaymentContract, string pInstagram, string pLinkedin, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_FooterInformation", MethodBase.GetCurrentMethod().GetParameters(), new object[] {  pFacebook, pTwitter, pGoogle, pSkype, pEmail, pConditionsForReturn, pPrivacyPolicy, pTermOfUse, pDistanceSalesContract, pKvkkContract, pPaymentContract, pInstagram, pLinkedin, pCreateId });
        }
    }

}