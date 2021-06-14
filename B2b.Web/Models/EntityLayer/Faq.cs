using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Faq : DataAccess
    {

        #region Properties

        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        #endregion

        #region Methods
        public static List<Faq> GetFaqList()
        {
            List<Faq> list = new List<Faq>();
            DataTable dt = DAL.GetFaqList();

            if (dt.Rows.Count > 0)
            {

                foreach (DataRow row in dt.Rows)
                {
                    Faq item = new Faq()
                    {
                        Id = row.Field<int>("Id"),
                        Question = row["Question"].ToString(),
                        Answer = row["Answer"].ToString(),
                        CreateDate = row.Field<DateTime>("CreateDate"),
                        EditDate = row.Field<DateTime>("EditDate"),
                        CreateId = Convert.ToInt32(row["CreateId"]),
                        EditId = Convert.ToInt32(row["EditId"])
                    };
                    list.Add(item);
                }
            }

            return list;
        }

        public bool AddOrEdit()
        {
            return DAL.InsertOrUpdateFAQ(Id,Question, Answer,CreateId,EditId);
        }

        public bool Delete()
        {
            return DAL.DeleteFAQ(Id, EditId);
        }


        #endregion
    }

    public partial class DataAccessLayer
    {
        public DataTable GetFaqList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Faq", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

        public bool InsertOrUpdateFAQ(int pId,string pQuestion, string pAnswer, int? pCreateId, int? pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Or_Update_FAQ", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pQuestion, pAnswer , pCreateId, pEditId });
        }

        public bool DeleteFAQ(int? pId, int? pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_FAQ", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }
      
    }
}