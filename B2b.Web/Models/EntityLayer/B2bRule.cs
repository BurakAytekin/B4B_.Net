using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using B2b.Web.v4.Models.Helper;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class B2bRule : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public double MinOrderTotal { get; set; }
        public bool SalesmanWebLogin { get; set; }
        public bool CustomerWebLogin { get; set; }
        public bool OrderAutomaticTransfer { get; set; }
        public string StartScreen { get; set; }
        public bool Maintenance { get; set; }

        #endregion

        #region Methods

        public static B2bRule GetGeneralRuleList()
        {
            B2bRule b2BRule = new B2bRule();
            DataTable dt = DAL.GetGeneralRuleList();
            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];
                b2BRule.MinOrderTotal = row.Field<double>("MinOrderTotal");
                b2BRule.CustomerWebLogin = row.Field<bool>("CustomerWebLogin");
                b2BRule.SalesmanWebLogin = row.Field<bool>("SalesmanWebLogin");
                b2BRule.OrderAutomaticTransfer = row.Field<bool>("OrderAutomaticTransfer");
                b2BRule.StartScreen = row.Field<string>("StartScreen");
                b2BRule.Maintenance = row.Field<bool>("Maintenance");
            }
            return b2BRule;
        }

        public static bool UpdateBoolen(string Field,bool UpdateValue)
        {
            return DAL.UpdateB2bRule( Field, UpdateValue);
        }
        public static bool UpdateMinOrderTotal(double total)
        {
            return DAL.UpdateMinOrderTotal(total);
        }

        public static bool ChangeStartScreen(string startScreen)
        {
            return DAL.ChangeStartScreen(startScreen);
        }

        public static bool CheckMaintenance()
        {
            return DAL.CheckMaintenance().Rows[0].Field<bool>("Maintenance");
        }


        #endregion
    }

    public partial class DataAccessLayer
    {
        public bool ChangeStartScreen(string pStartScreen)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_B2BRuleStartScreen", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pStartScreen });
        }

        public DataTable GetGeneralRuleList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_B2BRule", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }
        public bool UpdateMinOrderTotal(double pTotal)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_B2BRuleMinOrderTotal", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pTotal });
        }
      
        public bool UpdateB2bRule(string pField, bool pUpdateValue)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_B2BRule", MethodBase.GetCurrentMethod().GetParameters(), new object[] {  pField, pUpdateValue });
        }
        public DataTable CheckMaintenance()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_Maintenance");
        }


    }



}