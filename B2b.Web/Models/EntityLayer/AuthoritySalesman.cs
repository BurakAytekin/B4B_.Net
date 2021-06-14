using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class AuthoritySalesman : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int SalesmanId { get; set; }
        public bool IsSpecDiscount { get; set; }
        public bool B2b { get; set; }
        public bool Moderator { get; set; }
        public bool Android { get; set; }
        public bool Ios { get; set; }
        public bool CampaignStatu { get; set; }
        public bool EnteringInformation { get; set; }
        public bool LockSalesman { get; set; }
        public bool HidePassword { get; set; }
        public bool CustomerType { get; set; }
        public bool Collecting { get; set; }
        public bool CheckBasket { get; set; }
        public bool ProductRestoration { get; set; }
        public bool UpdateValue { get; set; }
        public string Field { get; set; }
        public string Comment { get; set; }
        public bool WebLogin { get; set; }
        public bool SendPool { get; set; }

        #endregion

        #region Methods
        public static Aut_SalesmanTuple GetAuthoritySalesman(int salesmanId)
        {
            List<AuthoritySalesman> listField = new List<AuthoritySalesman>();
            DataTable dtList = new DataTable();
            DataSet dt = DAL.GetAuthoritySalesman(salesmanId);

            foreach (DataRow row in dt.Tables[0].Rows)
            {
                AuthoritySalesman obj = new AuthoritySalesman()
                {
                    Field = row.Field<string>("Field"),
                    Comment = row.Field<string>("Comment")
                };
                listField.Add(obj);
            }

            dtList = dt.Tables[1];
            Aut_SalesmanTuple aut_SalesmanTuple = new Aut_SalesmanTuple(listField, dtList);

            return aut_SalesmanTuple;
        }
        public bool Update()
        {
            return DAL.UpdateAuthoritySalesman(Id, Field, UpdateValue);
        }
        #endregion
    }
    public partial class DataAccessLayer
    {
        public DataSet GetAuthoritySalesman(int pSalesmanId)
        {
            return DatabaseContext.ExecuteReaderDs(CommandType.StoredProcedure, "_Admin_GetItem_AuthoritySalesmanById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSalesmanId });
        }
        public bool UpdateAuthoritySalesman(int pId, string pField, bool pUpdateValue)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_AuthoritySalesman", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pField, pUpdateValue });
        }
    }
    public class Aut_SalesmanTuple : Tuple<List<AuthoritySalesman>, DataTable>
    {
        public Aut_SalesmanTuple(List<AuthoritySalesman> mList1, DataTable mList2) : base(mList1, mList2) { }
        public List<AuthoritySalesman> FieldList => Item1;
        public DataTable AuthorityItem => Item2;
    }
}