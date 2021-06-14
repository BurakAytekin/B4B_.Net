using B2b.Web.v4.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Log;
using B2b.Web.v4.Models.Helper;
using Newtonsoft.Json;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class ErpFunctionsController : AdminBaseController
    {
        // GET: Admin/ErpFunctions
        public ActionResult Index()
        {
             return View();
        }

        #region   HttpPost Methods
        [HttpPost]
        public string GetErpFunctionTypeList()
        {
            List<ErpFunctionType> list = ErpFunctionType.GetList();
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public JsonResult GetErpFunctionDetail(int typeId)
        {
            List<ErpFunctionDetail> list = ErpFunctionDetail.GetDetailList(typeId);
            return Json(list);
        }

        [HttpPost]
        public JsonResult SaveErpFunctionDetail(ErpFunctionDetail item)
        {
            bool result = false;

            if (item.Id == 0)
            {
                item.CreateId = AdminCurrentSalesman.Id;
                item.Id = item.Add();
                result = true;
            }
            else
            {
                item.EditId = AdminCurrentSalesman.Id;
                result = item.Update();
            };

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .", item.Id) : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);

        }

        #endregion
    }
}