using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Dashboard : DataAccess
    {
        #region Properties

        public Customer Customer { get; set; }

        public int BasketCount { get; set; }
        public double BasketTotal { get; set; }
        public DateTime? EndOrderDate { get; set; }
        public string EndOrderDateStr { get { return EndOrderDate == null ? "-" : Convert.ToDateTime(EndOrderDate).ToString("dd.MM.yyyy hh:ss:mm"); } }
        public DateTime? EndLoginDate { get; set; }
        public string EndLoginDateStr { get { return EndLoginDate == null ? "-" : Convert.ToDateTime(EndLoginDate).ToString("dd.MM.yyyy hh:ss:mm"); } }
        public double OrderTotal { get; set; }
        public string SalesmanCode { get; set; }
        public double RiskLimit { get; set; }
        public double Balance { get; set; }
        public double AvailableBalance { get; set; }
        public int UserCount { get; set; }

        #region İskonto Çekme Kodları

        public List<RuleAdditional> Rule { get; set; }

        #endregion



        #endregion

        #region Methods


        public static Dashboard GetCustomerDetailCalculate(int customerId)
        {
            Dashboard item = new Dashboard();
            DataTable dt = DAL.GetCustomerDetailCalculate(customerId);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                item.BasketCount = row.Field<int>("BasketCount");
                item.EndOrderDate = row["EndOrderDate"] as DateTime?;
                item.EndLoginDate = row["EndLoginDate"] as DateTime?;
                item.SalesmanCode = row.Field<string>("SalesmanCode");
                item.OrderTotal = Convert.ToDouble(row["OrderTotal"]);
                item.UserCount = Convert.ToInt32(row["UserCount"]);
                item.Balance = Convert.ToDouble(row["Balance"]);
                item.RiskLimit = Convert.ToDouble(row["RiskLimit"]);
                item.AvailableBalance = Convert.ToDouble(row["AvailableBalance"]);
                item.Customer = new Customer()
                {
                    Id = customerId,
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    RuleCode = row.Field<string>("RuleCode"),
                    Password = row.Field<string>("Password")
                };

            }

            return item;
        }

        public static List<RuleAdditional> GetCustomerDetailDiscoun(int customerId, string customerRuleCode)
        {
            List<RuleAdditional> list = new List<RuleAdditional>();
            DataTable dt = DAL.GetCustomerDetailDiscountList(customerRuleCode, customerId);

            foreach (DataRow row in dt.Rows)
            {
                RuleAdditional ruleItem = new RuleAdditional();
                ruleItem.Type = row.Field<string>("Type");
                ruleItem.ProductCode = row.Field<string>("Product");
                ruleItem.ProductGroup1 = row.Field<string>("ProductGroup1");
                ruleItem.ProductGroup2 = row.Field<string>("ProductGroup2");
                ruleItem.ProductGroup3 = row.Field<string>("ProductGroup3");
                ruleItem.Manufacturer = row.Field<string>("Manufacturer");
                ruleItem.Disc1 = row.Field<double>("Disc1");
                ruleItem.Disc2 = row.Field<double>("Disc2");
                ruleItem.Disc3 = row.Field<double>("Disc3");
                ruleItem.Disc4 = row.Field<double>("Disc4");
                ruleItem.DueDay = row.Field<int>("DueDay");
                ruleItem.Rate = row.Field<double>("Rate");
                list.Add(ruleItem);
            }
            return list;
        }

        #endregion

    }


    public partial class DataAccessLayer
    {
        public DataTable GetCustomerDetailCalculate(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_CustomerDetailCalculate", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }
        public DataTable GetCustomerDetailDiscountList(string pCustomerRule, int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_CustomerDiscountByCustomerSelect", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerRule, pCustomerId });
        }

    }
}