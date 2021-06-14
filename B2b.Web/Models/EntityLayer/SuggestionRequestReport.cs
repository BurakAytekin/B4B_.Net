using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;


namespace B2b.Web.v4.Models.EntityLayer
{

    public class SuggestionRequestReport : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Header { get; set; }
        public string Message{ get; set; }
        public string Answer { get; set; }
        public string PhoneNumber { get; set; }
        public string ProductCode { get; set; }
        public int SubjectTypeId { get; set; }
        public string SubjectTypeTitle { get; set; }

        #endregion

        #region Methods

        //public static CouponCs GetCouponStatistics(int pCuponId)
        //{
        //    CouponCs list = new CouponCs();
        //    DataTable dt = DAL.GetCouponStatistics(pCuponId);

        //    foreach (DataRow row in dt.Rows)
        //    {
        //        CouponCs obj = new CouponCs()
        //        {
        //            CustomerCount = Convert.ToInt32(row["CustomerCount"]),
        //            ActiveCustomerCount = Convert.ToInt32(row["ActiveCustomerCount"]),
        //            UsedCount = Convert.ToInt32(row["UsedCount"]),
        //            UnUsedCount = Convert.ToInt32(row["UnUsedCount"]),
        //            CouponTotal = Convert.ToDouble(row["CouponTotal"]),
        //        };
        //        list = obj;
        //    }
        //    return list;
        //}

        public static List<SuggestionRequestReport> GetListSuggestionRequestReport()
        {
            List<SuggestionRequestReport> list = new List<SuggestionRequestReport>();
            DataTable dt = DAL.GetListSuggestionRequestReport();

            foreach (DataRow row in dt.Rows)
            {
                SuggestionRequestReport obj = new SuggestionRequestReport()
                {
                    Id = row.Field<int>("Id"),
                    CustomerId = row.Field<int>("CustomerId"),
                    CustomerCode = row.Field<string>("CustomerCode"),
                    CustomerName = row.Field<string>("CustomerName"),
                    Email = row.Field<string>("Email"),
                    Header = row.Field<string>("Header"),
                    Message = row.Field<string>("Message"),

                    Answer = row.Field<string>("Answer"),


                    PhoneNumber = row.Field<string>("PhoneNumber"),
                    ProductCode = row.Field<string>("ProductCode"),

                    SubjectTypeId = row.Field<int>("SubjectTypeId"),
                    SubjectTypeTitle = row.Field<string>("SubjecTypeTitle"),

                    CreateDate = row.Field<DateTime>("CreateDate"),
                    CreateDateStr = row.Field<string>("CreateDateStr"),

                    EditDate = row.Field<DateTime>("CreateDate"),
                    EditDateStr = row.Field<string>("EditDateStr")

                };
                list.Add(obj);
            }
            return list;
        }


        public bool Add()
        {
            return DAL.InsertsuggestionRequestReport(SubjectTypeId, Email, Header, PhoneNumber, ProductCode, Message, CreateId);
        }

        public static bool SaveSuggestionRequestReportAnswer(int pId, int pEditId, string pAnswer)
        {
            return DAL.SaveSuggestionRequestReportAnswer(pId, pEditId, pAnswer);
        }


        #endregion
    }

    public partial class DataAccessLayer
    {
        public DataTable GetListSuggestionRequestReport()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_SuggestionRequestReport", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

        public bool InsertsuggestionRequestReport(int pSubjectTypeId, string pEmail, string pHeader, string pPhoneNumber, string pProductcode, string pMessage, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_RequestSuggestionReport", MethodBase.GetCurrentMethod().GetParameters(), new object[] {pSubjectTypeId, pEmail, pHeader, pPhoneNumber, pProductcode, pMessage, pCreateId });
        }

        public bool SaveSuggestionRequestReportAnswer(int pId, int pEditId, string pAnswer)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_SuggestionRequestAnswer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId, pAnswer });
        }
    }
}