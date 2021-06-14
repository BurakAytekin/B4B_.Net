using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class B2bColor : DataAccess
    {
        #region Properites

        public int Id { get; set; }
        public string Header { get; set; }
        public string Color1 { get; set; }
        public string Color2 { get; set; }
        public string Color3 { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region Methods

        public static List<B2bColor> GetList()
        {
            List<B2bColor> list = new List<B2bColor>();
            DataTable dt = DAL.GetB2bColorList();

            foreach (DataRow row in dt.Rows)
            {
                B2bColor obj = new B2bColor()
                {
                    Id = row.Field<int>("Id"),
                    Header = row.Field<string>("Header"),
                    Color1 = row.Field<string>("Color1"),
                    Color2 = row.Field<string>("Color2"),
                    Color3 = row.Field<string>("Color3"),
                    IsActive = row.Field<bool>("IsActive")
                };
                list.Add(obj);
            }
            return list;
        }

        public bool Update()
        {
            return DAL.UpdateB2bColor(Id, Header, Color1, Color2, Color3, Deleted, IsActive, EditId);
        }

        public bool Add()
        {
            return DAL.InsertB2bColor(Header, Color1, Color2, Color3, IsActive, CreateId);
        }

        #endregion

    }


    public partial class DataAccessLayer
    {
        public DataTable GetB2bColorList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_B2bColor");
        }

        public bool UpdateB2bColor(int pId, string pHeader, string pColor1, string pColor2, string pColor3, bool pDeleted, bool pIsActive, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_B2bColor", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pHeader, pColor1, pColor2, pColor3, pDeleted, pIsActive, pEditId });
        }

        public bool InsertB2bColor(string pHeader, string pColor1, string pColor2, string pColor3, bool pIsActive, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_B2bColor", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pHeader, pColor1, pColor2, pColor3, pIsActive, pCreateId });
        }
    }
}