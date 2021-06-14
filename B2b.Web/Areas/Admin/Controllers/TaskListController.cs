using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.Log;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class TaskListController : AdminBaseController
    {
        // GET: Admin/TaskList
        public ActionResult Index()
        {
                ViewBag.AdminCurrentSalesman = AdminCurrentSalesman;
            return View();
        }

        #region   HttpPost Methods
        [HttpPost]
        public JsonResult ChangeTaskStatus(int id, int status)
        {
            TaskList item = new TaskList()
            {
                Id = id,
                Status = (TaskListSatus)status,
                EditDate = DateTime.Now,
                EditId = AdminCurrentSalesman.Id
            };
            bool result = item.ChangeStatus();


            string message = "";
            message = result ? "{\"statu\":\"success\",\"message\":\"İşleminiz gerçekleştirilmiştir.\"}" : "{\"statu\":\"error\",\"message\":\"İşleminizde bir hata gerçekleşmiştir\"}";

            return Json(message);
        }

        [HttpPost]
        public JsonResult DeleteTask(int id)
        {
              TaskList item = new TaskList()
            {
                Id = id,
            };
            bool result = item.Delete();


            string message = "";
            message = result ? "{\"statu\":\"success\",\"message\":\"İşleminiz gerçekleştirilmiştir.\"}" : "{\"statu\":\"error\",\"message\":\"İşleminizde bir hata gerçekleşmiştir\"}";

            return Json(message);
        }


        [HttpPost]
        public JsonResult DeleteTaskComment(int id)
        {
              TaskListComment item = new TaskListComment()
            {
                Id = id,
            };
            bool result = item.Delete();


            string message = "";
            message = result ? "{\"statu\":\"success\",\"message\":\"İşleminiz gerçekleştirilmiştir.\"}" : "{\"statu\":\"error\",\"message\":\"İşleminizde bir hata gerçekleşmiştir\"}";

            return Json(message);
        }

        [HttpPost]
        public JsonResult GetTaskList(string startDate, string endDate, string generalSearchText, int statu)
        {

            List<TaskList> list = TaskList.GetTaskList(DateTime.Parse(startDate),DateTime.Parse(endDate), generalSearchText, statu);
            return Json(list);
        }


        [HttpPost]
        public JsonResult GetTaskListComment(int taskListId)
        {
            List<TaskListComment> list = TaskListComment.GetTaskListComment(taskListId);
            return Json(list);
        }
        [HttpPost]
        public JsonResult AddTask(string header, string content)
        {
             TaskList item = new TaskList()
            {
                AddId = AdminCurrentSalesman.Id,
                Header = header,
                Content = content
            };
            bool result = item.Add();


            string message = "";
            message = result ? "{\"statu\":\"success\",\"message\":\"Görev eklenmiştir\"}" : "{\"statu\":\"error\",\"message\":\"Görev eklemenizde bir hata gerçekleşmiştir\"}";

            return Json(message);
        }

        [HttpPost]
        public JsonResult AddTaskComment(int taskListId, string content)
        {
             TaskListComment item = new TaskListComment()
            {
                TaskListId = taskListId,
                SalesmanId = AdminCurrentSalesman.Id,
                Content = content,

            };
            bool result = item.Add();


            string message = "";
            message = result ? "{\"statu\":\"success\",\"message\":\"Yorum eklenmiştir\"}" : "{\"statu\":\"error\",\"message\":\"Yorum eklemenizde bir hata gerçekleşmiştir\"}";

            return Json(message);
        }
        #endregion

    }
}