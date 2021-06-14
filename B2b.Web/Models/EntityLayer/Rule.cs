using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;


namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class Rule : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public string Product { get; set; }
        public string Customer { get; set; }
        public int PaymentType { get; set; }
        public double Disc1 { get; set; }
        public double Disc2 { get; set; }
        public double Disc3 { get; set; }
        public double Disc4 { get; set; }
        public int DueDay { get; set; }
        public int PriceNumber { get; set; }
        public double Rate { get; set; }
        public bool IsRuleAdditional { get; set; }
        #endregion

        #region Methods
        public static List<Rule> GetRuleProductList()
        {
            DataTable dt = new DataTable();
            List<Rule> list = new List<Rule>();

            dt = DAL.GetRuleProductList();

            foreach (DataRow row in dt.Rows)
            {
                Rule item = new Rule()
                {
                    Product = row.Field<string>("Product")
                };
                list.Add(item);
            }

            return list;
        }
        public static List<Rule> GetRuleCustomerList()
        {
            DataTable dt = new DataTable();
            List<Rule> list = new List<Rule>();

            dt = DAL.GetRuleCustomerList();

            foreach (DataRow row in dt.Rows)
            {
                Rule item = new Rule()
                {
                    Customer = row.Field<string>("Customer")
                };
                list.Add(item);
            }

            return list;
        }


        public static List<Rule> GetRuleList()
        {
            DataTable dt = new DataTable();
            List<Rule> list = new List<Rule>();

            dt = DAL.GetRuleList();

            foreach (DataRow row in dt.Rows)
            {
                Rule item = new Rule()
                {
                    Id = row.Field<int>("Id"),
                    Product = row.Field<string>("Product"),
                    Customer = row.Field<string>("Customer"),
                    PaymentType = row.Field<int>("PaymentType"),
                    Disc1 = row.Field<double>("Disc1"),
                    Disc2 = row.Field<double>("Disc2"),
                    Disc3 = row.Field<double>("Disc3"),
                    Disc4 = row.Field<double>("Disc4"),
                    PriceNumber = row.Field<int>("PriceNumber"),
                    Rate = row.Field<double>("Rate")
                };
                list.Add(item);
            }

            return list;
        }

        public bool Add()
        {
            return DAL.InsertRule(Product, Customer, PaymentType, Disc1, Disc2, Disc3, Disc4, DueDay, PriceNumber, Rate, CreateId);
        }
        public bool Update()
        {
            return DAL.UpdateRule(Id, Product, Customer, PaymentType, Disc1, Disc2, Disc3, Disc4, DueDay, PriceNumber, Rate, EditId, Deleted);
        }
        public bool Delete()
        {
            return DAL.DeleteRule(Id,EditId);
        }


        #endregion
    }
    public partial class DataAccessLayer
    {
        public DataTable GetRuleProductList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_Rule_Product", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }
        public DataTable GetRuleCustomerList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_Rule_Customer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

        public DataTable GetRuleList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_Rule", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

        public bool InsertRule(string pProduct, string pCustomer, int pPaymentType, double pDisc1, double pDisc2, double pDisc3, double pDisc4, int pDueDay, int pPriceNumber, double pRate, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Rule", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProduct, pCustomer, pPaymentType, pDisc1, pDisc2, pDisc3, pDisc4, pDueDay, pPriceNumber, pRate, pCreateId });
        }
        public bool UpdateRule(int pId, string pProduct, string pCustomer, int pPaymentType, double pDisc1, double pDisc2, double pDisc3, double pDisc4, int pDueDay, int pPriceNumber, double pRate, int pEditId, bool pDeleted)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Rule", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pProduct, pCustomer, pPaymentType, pDisc1, pDisc2, pDisc3, pDisc4, pDueDay, pPriceNumber, pRate, pEditId, pDeleted });
        }

        public bool DeleteRule(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_Rule", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }

    }
}