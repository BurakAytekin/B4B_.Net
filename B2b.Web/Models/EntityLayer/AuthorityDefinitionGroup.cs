using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class AuthorityDefinitionGroup : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Name { get; set; }

        #endregion

        #region Methods


        public static List<AuthorityDefinitionGroup> GetList()
        {
            List<AuthorityDefinitionGroup> list = new List<AuthorityDefinitionGroup>();
            DataTable dt = DAL.GetAuthorityDefinitionGroup();

            foreach (DataRow row in dt.Rows)
            {
                AuthorityDefinitionGroup obj = new AuthorityDefinitionGroup()
                {
                    Id = row.Field<int>("Id"),
                    Name = row.Field<string>("Name"),

                };
                list.Add(obj);
            }
            return list;
        }

        public bool Add()
        {
            return DAL.AddAuthorityDefinitionGroup(Name, CreateId);
        }

        public bool Update()
        {
            return DAL.UpdateAuthorityDefinitionGroup(Id, Name, EditId, Deleted);
        }


        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetAuthorityDefinitionGroup()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_AuthorityDefinitionGroup");
        }



        public bool AddAuthorityDefinitionGroup(string pName, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_AuthorityDefinitionGroup", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pName, pCreateId });
        }

        public bool UpdateAuthorityDefinitionGroup(int pId, string pName, int pEditId, bool pDeleted)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_AuthorityDefinitionGroup", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pName, pEditId, pDeleted });
        }

    }
}