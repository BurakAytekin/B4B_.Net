using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class AuthorityUser : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int UserId { get; set; }
        public bool _B2b { get; set; }
        public bool _Android { get; set; }
        public bool _Ios { get; set; }
        public bool _CampaignStatu { get; set; }
        public bool _LockUser { get; set; }
        public bool _ProductRestoration { get; set; }
        public bool _RuleActive { get; set; }
        public bool _ShowQuantity { get; set; }

        public string Field { get; set; }
        public string Comment { get; set; }
        public bool UpdateValue { get; set; }

        #endregion

        #region Methods

        public static Aut_UserTuple GetAuthorityUser(int userId)
        {
            List<AuthorityUser> listField = new List<AuthorityUser>();
            DataTable dtList = new DataTable();
            DataSet dt = DAL.GetAuthorityUser(userId);

            foreach (DataRow row in dt.Tables[0].Rows)
            {
                AuthorityUser obj = new AuthorityUser()
                {
                    Field = row.Field<string>("Field"),
                    Comment = row.Field<string>("Comment")
                };
                listField.Add(obj);
            }

            dtList = dt.Tables[1];
            Aut_UserTuple aut_CustomerTuple = new Aut_UserTuple(listField, dtList);

            return aut_CustomerTuple;
        }

        public bool Update()
        {
            return DAL.UpdateAuthorityUser(Id, Field, UpdateValue);
        }


        #endregion


    }

    public partial class DataAccessLayer
    {
        public bool UpdateAuthorityUser(int pId, string pField, bool pUpdateValue)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_AuthorityUser", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pField, pUpdateValue });
        }

        public DataSet GetAuthorityUser(int pUserId)
        {
            return DatabaseContext.ExecuteReaderDs(CommandType.StoredProcedure, "_Admin_GetItem_AuthorityUserById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pUserId });
        }
    }

    public class Aut_UserTuple : Tuple<List<AuthorityUser>, DataTable>
    {
        public Aut_UserTuple(List<AuthorityUser> mList1, DataTable mList2) : base(mList1, mList2) { }
        public List<AuthorityUser> FieldList => Item1;
        public DataTable AuthorityItem => Item2;
    }
}