using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class SalesmanNotes : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Notes { get; set; }
        public bool IsActive { get; set; }

        #endregion
        #region Methods

        public static List<SalesmanNotes> GetSalesmanNoteList(int salesmanId)
        {
            List<SalesmanNotes> list = new List<SalesmanNotes>();
            DataTable dt = DAL.GetSalesmanNoteList(salesmanId);

            foreach (DataRow row in dt.Rows)
            {
                SalesmanNotes obj = new SalesmanNotes()
                {
                    Id = row.Field<int>("Id"),
                    Notes = row.Field<string>("Notes"),
                    IsActive = row.Field<bool>("IsActive"),
                    
                };
                list.Add(obj);
            }
            return list;
        }

        public bool Update()
        {
            return DAL.UpdateSalesmanNote(Id, IsActive,Deleted, EditId);
        }

        public bool Add()
        {
            return DAL.InsertSalesmanNote( Notes, CreateId);
        }
        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetSalesmanNoteList(int pSalesmanId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_SalesmanNotes", MethodBase.GetCurrentMethod().GetParameters(), new object[] {  pSalesmanId });
        }

        public bool InsertSalesmanNote(string pNotes,int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_SalesmanNote", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pNotes, pCreateId});
        }
        public bool UpdateSalesmanNote(int pId, bool pIsActive, bool pDeleted, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_SalesmanNote", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pIsActive, pDeleted, pEditId });
        }
    }
}