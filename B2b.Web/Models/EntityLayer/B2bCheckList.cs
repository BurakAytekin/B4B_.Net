using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class B2bCheckList : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public string Notes { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public string StatusStr
        {
            get {
                switch (Status)
                {
                    case 0:
                        return "Beklemede";
                    case 1:
                        return "Yapıldı";
                    case 2:
                        return "İhtiyaç Değil";

                    default:
                        return "İhtiyaç Değil";

                }
                
            }
        }


        #endregion

        #region Methods

        public static List<B2bCheckList> GetList()
        {
            List<B2bCheckList> list = new List<B2bCheckList>();
            DataTable dt = DAL.GetB2bCheckList();

            foreach (DataRow row in dt.Rows)
            {
                B2bCheckList obj = new B2bCheckList()
                {
                    Id = row.Field<int>("Id"),
                    Header = row.Field<string>("Header"),
                    Content = row.Field<string>("Content"),
                    Notes = row.Field<string>("Notes"),
                    Status = row.Field<int>("Status"),
                    Description = row.Field<string>("Description"),
                    EditDateStr = String.IsNullOrEmpty(row["EditDate"].ToString()) ? string.Empty : Convert.ToDateTime(row["EditDate"]).ToString(),
                    LastUpdateName = row.Field<string>("LastUpdateName")

                };
                list.Add(obj);
            }
            return list;
        }

        public bool Update()
        {
            return DAL.UpdateB2bCheckList(Id, Status, Notes, EditId);
        }

        #endregion

    }
    public partial class DataAccessLayer
    {
        public bool UpdateB2bCheckList(int pId, int pStatus, string pNotes, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_B2bCheckList", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pStatus, pNotes, pEditId });
        }


        public DataTable GetB2bCheckList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_B2bCheckList");
        }
    }
}