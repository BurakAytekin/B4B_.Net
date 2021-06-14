using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class AuthorityGroupHeader : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Name { get; set; }

        #endregion

        #region Methods


        public static List<AuthorityGroupHeader> GetList()
        {
            List<AuthorityGroupHeader> list = new List<AuthorityGroupHeader>();
            DataTable dt = DAL.GetAuthorityGroup();

            foreach (DataRow row in dt.Rows)
            {
                AuthorityGroupHeader obj = new AuthorityGroupHeader()
                {
                    Id = row.Field<int>("Id"),
                    Name = row.Field<string>("Name"),

                };
                list.Add(obj);
            }
            return list;
        }

        public bool Delete()
        {
            return DAL.DeleteAuthorityGroupHeader(Id);
        }

        public bool Add()
        {
            return DAL.AddAuthorityGroupHeader(Id, Name, CreateId);
        }


        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetAuthorityGroup()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_AuthorityGroup");
        }

        public bool DeleteAuthorityGroupHeader(int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_AuthorityGroupHeader", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }

        public bool AddAuthorityGroupHeader(int pId, string pName, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_AuthorityGroupHeader", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pName, pCreateId });
        }
    }
}