using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class AuthorityCustomer : DataAccess
    {

        #region Properties

        public int Id { get; set; }
        public int CustomerId{ get; set; }
        public bool _B2b { get; set; }
        public bool _Android { get; set; }
        public bool _Ios { get; set; }
        public bool _EnteringInformation { get; set; }
        public bool _LockCustomer { get; set; }
        public bool _CheckBasket { get; set; }
        public bool _ProductRestoration { get; set; }
        public bool _ShowQuantity { get; set; }
        public bool _WebLogin { get; set; }
        public string Field { get; set; }
        public string Comment { get; set; }
        public bool UpdateValue { get; set; }
        #endregion

        #region Methods

        public static Aut_CustomerTuple GetAuthorityCustomer(int customerId)
        {
            List<AuthorityCustomer> listField = new List<AuthorityCustomer>();
            DataTable dtList = new DataTable();
            DataSet dt = DAL.GetAuthorityCustomer(customerId);
           
            foreach (DataRow row in dt.Tables[0].Rows)
            {
                AuthorityCustomer obj = new AuthorityCustomer()
                {
                    Field = row.Field<string>("Field"),
                    Comment = row.Field<string>("Comment")
                };
                listField.Add(obj);
            }

            dtList = dt.Tables[1];
            Aut_CustomerTuple aut_CustomerTuple = new Aut_CustomerTuple(listField, dtList);

            return aut_CustomerTuple;
        }

        public bool Update()
        {
            return DAL.UpdateAuthorityCustomer(Id, Field, UpdateValue);
        }

        #endregion

    }
    public partial class DataAccessLayer
    {
        public bool UpdateAuthorityCustomer(int pId, string pField, bool pUpdateValue)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_AuthorityCustomer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pField, pUpdateValue });
        }

        public DataSet GetAuthorityCustomer(int pCustomerId)
        {
            return DatabaseContext.ExecuteReaderDs(CommandType.StoredProcedure, "_Admin_GetItem_AuthoritycustomerById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }
    }

    public class Aut_CustomerTuple : Tuple<List<AuthorityCustomer>, DataTable >
    {
        public Aut_CustomerTuple(List<AuthorityCustomer> mList1, DataTable mList2) : base(mList1, mList2) { }
        public List<AuthorityCustomer> FieldList => Item1;
        public DataTable AuthorityItem => Item2;
    }
}