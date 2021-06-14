using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class CheckListController : AdminBaseController
    {
        // GET: Admin/CheckList
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult GetCheckList()
        {
            List<B2bCheckList> list = B2bCheckList.GetList();
            return Json(list);
        }

        [HttpPost]
        public JsonResult UpdateCheckList(B2bCheckList selectedCheckList)
        {
            bool result = false;

            selectedCheckList.EditId = AdminCurrentSalesman.Id;
            result = selectedCheckList.Update();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }



    }
}