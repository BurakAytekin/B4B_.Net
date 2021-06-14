using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class AuthoritySteps : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int HeaderId { get; set; }
        public string PageNameRef { get; set; }
        public string Content { get; set; }
        public string Explanation { get; set; }
        public int Type { get; set; }
        public bool IsChecked { get; set; }
        public int AuthorityGroupId { get; set; }
        public int AgHeaderId { get; set; }
        public bool Disabled { get; set; }
        #endregion

        #region Methods

        public static List<AuthoritySteps> GetStepGroupList()
        {
            List<AuthoritySteps> list = new List<AuthoritySteps>();
            DataTable dt = DAL.GetStepGroupList();

            foreach (DataRow row in dt.Rows)
            {
                AuthoritySteps obj = new AuthoritySteps()
                {
                    Id = row.Field<int>("Id"),
                    GroupId = row.Field<int>("GroupId"),
                    GroupName = row.Field<string>("GroupName"),

                };
                list.Add(obj);
            }
            return list;
        }

        public static List<AuthoritySteps> GetStepList(int pGroupId, int pHeaderId)
        {
            List<AuthoritySteps> list = new List<AuthoritySteps>();
            DataTable dt = DAL.GetStepList(pGroupId, pHeaderId);

            foreach (DataRow row in dt.Rows)
            {
                AuthoritySteps obj = new AuthoritySteps()
                {
                    Id = row.Field<int>("Id"),
                    AgHeaderId = pHeaderId,
                    GroupId = row.Field<int>("GroupId"),
                    GroupName = row.Field<string>("GroupName"),
                    HeaderId = row.Field<int>("HeaderId"),
                    PageNameRef = row.Field<string>("PageNameRef"),
                    Content = row.Field<string>("Content"),
                    Explanation = row.Field<string>("Explanation"),
                    Type = row.Field<int>("Type"),
                    IsChecked = Convert.ToBoolean(row["IsChecked"]),
                    AuthorityGroupId = Convert.ToInt32(row["AuthorityGroupId"])

                };
                list.Add(obj);
            }
            return list;
        }

        #endregion
    }

    public partial class DataAccessLayer
    {
        public DataTable GetStepList(int pGroupId, int pHeaderId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_AuthorityStepListByHeaderId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pGroupId, pHeaderId });
        }



        public DataTable GetStepGroupList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_AuthorityStepGroup");
        }
    }
}