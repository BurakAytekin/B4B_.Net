using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class CatalogLink : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Header { get; set; }
        public string Link { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region Methods

        public static List<CatalogLink> GetList()
        {
            List<CatalogLink> list = new List<CatalogLink>();
            DataTable dt = DAL.GetCatalogLinkList();

            foreach (DataRow row in dt.Rows)
            {
                CatalogLink obj = new CatalogLink()
                {
                    Id = row.Field<int>("Id"),
                    Header = row.Field<string>("Header"),
                    Link = row.Field<string>("Link"),
                    IsActive = row.Field<bool>("IsActive")
                };
                list.Add(obj);
            }
            return list;
        }

        public bool Delete()
        {
            return DAL.DeleteCatalogLink(Id,EditId);
        }

        public bool Update()
        {
            return DAL.UpdateCatalogLink(Id, Header, Link,IsActive, EditId);
        }

        public bool Add()
        {
            return DAL.InsertCatalogLink(Header, Link, CreateId);
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetCatalogLinkList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_CatalogLink");
        }

        public bool DeleteCatalogLink(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_CatalogLink", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }

        public bool InsertCatalogLink(string pHeader, string pLink, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_CatalogLink", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pHeader, pLink, pCreateId });
        }

        public bool UpdateCatalogLink(int pId, string pHeader, string pLink,bool pIsActive, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_CatalogLink", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pHeader, pLink,pIsActive, pEditId });
        }
    }

}