using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class AnalysisControl : DataAccess
    {
        #region Properties

        public int Count { get; set; }
        public int CountCustomer { get; set; }
        public int CountProduct { get; set; }

        #endregion

        #region Methods

        public static AnalysisControl CheckRuleDublicate()
        {
            AnalysisControl item = new AnalysisControl();
            DataTable dt = DAL.CheckRuleDublicate();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                item.Count = Convert.ToInt32(row["Count"]);

            }

            return item;
        }

        public static AnalysisControl CheckRuleCount()
        {
            AnalysisControl item = new AnalysisControl();
            DataTable dt = DAL.CheckRuleCount();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                item.Count = Convert.ToInt32(row["Count"]);

            }

            return item;
        }

        public static AnalysisControl CheckPriceControl()
        {
            AnalysisControl item = new AnalysisControl();
            DataTable dt = DAL.CheckPriceControl();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                item.Count = Convert.ToInt32(row["Count"]);

            }

            return item;
        }


        public static AnalysisControl CheckCustomerAndProductRule()
        {
            AnalysisControl item = new AnalysisControl();
            DataTable dt = DAL.CheckCustomerAndProductRule();

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                item.CountProduct = Convert.ToInt32(row["CountProduct"]);
                item.CountCustomer = Convert.ToInt32(row["CountCustomer"]);

            }

            return item;
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable CheckRuleDublicate()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Check_RuleDublicateCount", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

        public DataTable CheckRuleCount()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Check_RuleCount", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }
        public DataTable CheckCustomerAndProductRule()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Check_CustomerAndProductRule", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

        public DataTable CheckPriceControl()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Check_PriceControl", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

    }

}