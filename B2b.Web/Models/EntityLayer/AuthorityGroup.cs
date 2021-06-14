using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class AuthorityGroup: DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int AgHeaderId { get; set; }
        public int HeaderId { get; set; }
        public int Type { get; set; }
        public string GroupName { get; set; }
        public int AsGroupId { get; set; }
        public int AsId { get; set; }
        public string PageNameRef { get; set; }
        #endregion

        #region Methods

        public static List<AuthorityGroup> GetSalesmanAuthority(int salesmanId)
        {
            List<AuthorityGroup> list = new List<AuthorityGroup>();
            DataTable dt = DAL.GetSalesmanAuthority(salesmanId);

            foreach (DataRow row in dt.Rows)
            {
                AuthorityGroup obj = new AuthorityGroup()
                {
                    Id = row.Field<int>("Id"),
                    AsGroupId = row.Field<int>("AsGroupId"),
                    PageNameRef = row.Field<string>("PageNameRef")
                };
                list.Add(obj);
            }
            return list;
        }

        public int Add()
        {
            DataTable dt = DAL.InsertAuthorityGroup(AgHeaderId, HeaderId, Type, GroupName, AsGroupId, AsId, CreateId);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0][0]) : -10;
        }

        public bool Delete()
        {
            return DAL.DeleteAuthorityGroup(Id, EditId);
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetSalesmanAuthority(int pSalesmanId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_SalesmanAuthority", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSalesmanId });
        }

        public bool DeleteAuthorityGroup(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_AuthorityGroup", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }

        public DataTable InsertAuthorityGroup(int pAgHeaderId, int pHeaderId, int pType, string pGroupName, int pAsGroupId, int pAsId, int pCreateId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_Insert_AuthorityGroup", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pAgHeaderId, pHeaderId, pType, pGroupName, pAsGroupId, pAsId, pCreateId });
        }
    }
}