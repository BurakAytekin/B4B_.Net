using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class AuthorityDefinition : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public int SalesmanId { get; set; }
        public int AuthorityGroupHeaderId { get; set; }
        public int DefinitionGroupId { get; set; }
        #endregion

        #region Methods

        public static List<AuthorityDefinition> GetList(int definitionGroupId)
        {
            List<AuthorityDefinition> list = new List<AuthorityDefinition>();
            DataTable dt = DAL.GetDefinitionList(definitionGroupId);

            foreach (DataRow row in dt.Rows)
            {
                AuthorityDefinition obj = new AuthorityDefinition()
                {
                    Id = row.Field<int>("Id"),
                    SalesmanId = row.Field<int>("SalesmanId"),
                    AuthorityGroupHeaderId = row.Field<int>("AuthorityGroupHeaderId"),
                    DefinitionGroupId = definitionGroupId,

                };
                list.Add(obj);
            }
            return list;
        }

        public bool Delete()
        {
            return DAL.DeleteAuthorityDefinition(DefinitionGroupId, EditId);
        }

        public bool Add()
        {
            return DAL.AddAuthorityDefinition(SalesmanId, AuthorityGroupHeaderId, DefinitionGroupId, CreateId);
        }


        #endregion
    }

    public partial class DataAccessLayer
    {
        public DataTable GetDefinitionList(int pDefinitionGroupId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_AuthorityDefinitionList", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pDefinitionGroupId });
        }

        public bool DeleteAuthorityDefinition(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_AuthorityDefinition", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }

        public bool AddAuthorityDefinition(int pSalesmanId, int pAuthorityGroupHeaderId, int pDefinitionGroupId, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_AuthorityDefinition", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSalesmanId, pAuthorityGroupHeaderId, pDefinitionGroupId, pCreateId });
        }
    }
}