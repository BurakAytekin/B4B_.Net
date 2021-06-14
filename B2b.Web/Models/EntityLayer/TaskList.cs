using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class TaskList : DataAccess
    {
        #region properties

        public int Id { get; set; }
        public int AddId { get; set; }
        public Salesman Salesman { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public DateTime? FinishDate { get; set; }
        public TaskListSatus Status { get; set; }
        public string StatusStr { get { return (Status.ToString()); } }
        public string TaskClass
        {
            get
            {
                switch (Status)
                {
                    case TaskListSatus.Beklemede:
                        return "priority-normal";
                    case TaskListSatus.Yapılıyor:
                        return "priority-medium";
                    case TaskListSatus.Yapıldı:
                        return "priority-low";
                    case TaskListSatus.İptal:
                        return "priority-high";
                    default:
                        return "priority-normal";
                }


            }
        }
        public int FinishedId { get; set; }

        #endregion

        #region Methods

        public bool Add()
        {
            return DAL.InsertTaskList(AddId, Header, Content);
        }

        public bool ChangeStatus()
        {
            return DAL.ChangeStatus(Id, (int)Status,Convert.ToDateTime(EditDate),EditId);
        }
        public bool Delete()
        {
            return DAL.DeleteTask(Id);
        }


        public static List<TaskList> GetTaskList(DateTime startDate, DateTime endDate, string generalSearchText, int statu)
        {
            List<TaskList> list = new List<TaskList>();
            DataTable dt = DAL.GetTaskList(startDate, endDate, generalSearchText, statu);

            foreach (DataRow row in dt.Rows)
            {
                TaskList item = new TaskList()
                {
                    Id = row.Field<int>("Id"),
                    Header = row.Field<string>("Header"),
                    Content = row.Field<string>("Content"),
                    AddId = row.Field<int>("AddId"),
                    Salesman = new Salesman()
                    {
                        Id = row.Field<int>("AddId"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name")
                    },
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    FinishDate = row["FinishDate"] as DateTime?,
                    Status = (TaskListSatus)row.Field<int>("Status"),


                };
                list.Add(item);
            }

            return list;
        }

        #endregion

    }
    public enum TaskListSatus
    {
        Beklemede = 0,
        Yapılıyor = 1,
        Yapıldı = 2,
        İptal = 3
    }

    public partial class DataAccessLayer
    {
        public DataTable GetTaskList(DateTime pStartDate, DateTime pEndDate, string pGeneralSearchText, int pStatu)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_TaskList", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pStartDate, pEndDate, pGeneralSearchText, pStatu });
        }

        public bool InsertTaskList(int pAddId, string pHeader, string pContent)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_TaskList", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pAddId, pHeader, pContent });
        }

        public bool ChangeStatus(int pId, int pStatus,DateTime pEditDate, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_TaskListStatus", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pStatus, pEditDate,pEditId });
        }

        public bool DeleteTask(int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Delete_TaskListItem", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }
    }
}