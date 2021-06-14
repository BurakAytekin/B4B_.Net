using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class TaskListComment : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int TaskListId { get; set; }
        public int SalesmanId { get; set; }
        public Salesman Salesman { get; set; }
        public string Content { get; set; }
        #endregion

        #region Methods

        public bool Add()
        {
            return DAL.InsertTaskListComment(TaskListId, SalesmanId, Content);
        }

        public bool Delete()
        {
            return DAL.DeleteTaskListComment(Id);
        }
        public static List<TaskListComment> GetTaskListComment(int taskListId)
        {
            List<TaskListComment> list = new List<TaskListComment>();
            DataTable dt = DAL.GetTaskListComment(taskListId);

            foreach (DataRow row in dt.Rows)
            {
                TaskListComment item = new TaskListComment()
                {
                    Id = row.Field<int>("Id"),
                    Content = row.Field<string>("Content"),
                    SalesmanId = row.Field<int>("SalesmanId"),
                    Salesman = new Salesman()
                    {
                        Id = row.Field<int>("SalesmanId"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        PicturePath = String.IsNullOrEmpty(row.Field<string>("PicturePath")) ? "/Content/images/avatar/noavatar.png" : row.Field<string>("PicturePath"),
                    },
                    CreateDate = row.Field<DateTime>("CreateDate"),


                };
                list.Add(item);
            }

            return list;
        }



        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetTaskListComment(int pTaskListId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_TaskListCommentByTaskId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pTaskListId });
        }

        public bool InsertTaskListComment(int pTaskListId, int pSalesmanId, string pContent)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_TaskListComment", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pTaskListId, pSalesmanId, pContent });
        }

        public bool DeleteTaskListComment(int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Delete_TaskListComment", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }

    }

}