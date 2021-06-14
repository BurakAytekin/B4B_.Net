using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class RuleAdditional : DataAccess
    {
        public RuleAdditional()
        {
            PriceNumber = -1;
        }

        #region Properties
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Manufacturer { get; set; }
        public string ProductGroup1 { get; set; }
        public string ProductGroup2 { get; set; }
        public string ProductGroup3 { get; set; }
        public double Disc1 { get; set; }
        public double Disc2 { get; set; }
        public double Disc3 { get; set; }
        public double Disc4 { get; set; }
        public double Rate { get; set; }
        public bool MainDiscountPassive { get; set; }
        public int DueDay { get; set; }
        public string Type { get; set; }
        public double SalesPrice { get; set; }
        public string Currency { get; set; }
        public double CurrenctyRate { get; set; }
        public int PriceNumber { get; set; }

        #endregion

        #region Methods
        public static List<RuleAdditional> GetList(int customerId)
        {
            List<RuleAdditional> list = new List<RuleAdditional>();
            DataTable dt = DAL.GetRuleAdditionalList(customerId);
            foreach (DataRow row in dt.Rows)
            {
                RuleAdditional ruleAdditional = new RuleAdditional()
                {
                    Id = row.Field<int>("Id"),
                    CustomerId = row.Field<int>("CustomerId"),
                    Manufacturer = row.Field<string>("Manufacturer"),
                    ProductId = Convert.ToInt32(row["ProductId"]),
                    ProductCode = row.Field<string>("Code"),
                    ProductName = row.Field<string>("Name"),
                    ProductGroup1 = row.Field<string>("ProductGroup1"),
                    ProductGroup2 = row.Field<string>("ProductGroup2"),
                    ProductGroup3 = row.Field<string>("ProductGroup3"),
                    DueDay = row.Field<int>("DueDay"),
                    Rate = row.Field<double>("Rate"),
                    MainDiscountPassive = row.Field<bool>("MainDiscountPassive"),
                    Disc1 = row.Field<double>("Disc1"),
                    Disc2 = row.Field<double>("Disc2"),
                    Disc3 = row.Field<double>("Disc3"),
                    Disc4 = row.Field<double>("Disc4"),
                    PriceNumber = row.Field<int>("PriceNumber")
                };
                list.Add(ruleAdditional);
            }
            return list;
        }

        public static bool UpdateRuleAdditional(string pName,double pValue,int pId)
        {
            return DAL.UpdateRuleAdditionalQuery(pName, pValue, pId);
        }

        public bool Insert()
        {
            return DAL.InsertRuleAdditional(CustomerId, Manufacturer, ProductId, ProductGroup1, ProductGroup2, ProductGroup3, Disc1, Disc2, Disc3, Disc4, DueDay, Rate, MainDiscountPassive, PriceNumber, CreateId);
        }

        public bool Update()
        {
            return DAL.UpdateRuleAdditional(Id, CustomerId, Manufacturer, Disc1, Disc2, Disc3, Disc4, DueDay, PriceNumber, EditId);
        }

        public bool Delete()
        {
            return DAL.DeleteRuleAdditional(Id, EditId);
        }
        #endregion
    }

    public partial class DataAccessLayer
    {
        public DataTable GetRuleAdditionalList(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_RuleAdditional", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }
        public bool InsertRuleAdditional(int pCustomerId, string pManufacturer, int pProductId, string pProductGroup1, string pProductGroup2, string pProductGroup3, double pDisc1, double pDisc2, double pDisc3, double pDisc4, double pDueDay, double pRate, bool pMainDiscountPassive,int pPriceNumber, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_RuleAdditional", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pManufacturer, pProductId, pProductGroup1, pProductGroup2, pProductGroup3, pDisc1, pDisc2, pDisc3, pDisc4, pDueDay, pRate, pMainDiscountPassive, pPriceNumber, pCreateId });
        }

        public bool UpdateRuleAdditional(int pId, int pCustomerId, string pManufacturer, double pDisc1, double pDisc2, double pDisc3, double pDisc4, double pDueDay,int pPriceNumber, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_RuleAdditional", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pCustomerId, pManufacturer, pDisc1, pDisc2, pDisc3, pDisc4, pDueDay, pPriceNumber, pEditId });
        }
        public bool UpdateRuleAdditionalQuery(string pName, double pValue, int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_RuleAddtional", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pName, pValue, pId });
        }

        public bool DeleteRuleAdditional(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Delete_RuleAdditional", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }
    }
}