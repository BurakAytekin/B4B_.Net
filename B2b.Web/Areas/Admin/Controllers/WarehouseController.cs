using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class WarehouseController : AdminBaseController
    {
        // GET: Admin/Warehouse
        public ActionResult Index()
        {
               return View();
        }
        #region   HttpPost Methods

        [HttpPost]
        public string GetWarehouseList()
        {
            List<Warehouse> list = Warehouse.GetList();
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public JsonResult UpdateWarehouse(int id, string code, string name, int priority)
        {
             bool result = false;
            if (id == 0)
            {
                Warehouse item = new Warehouse()
                {
                    Code = code,
                    Name = name,
                    CreateId = AdminCurrentSalesman.Id,
                    Priority = priority
                };
                result = item.Add();
            }
            else
            {
                Warehouse item = new Warehouse()
                {
                    Id = id,
                    Code = code,
                    Name = name,
                    Priority = priority
                };
                result = item.Update();
            }
            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        [HttpPost]
        public JsonResult DeleteWarehouse(int id)
        {
              bool result = false;
            Warehouse item = new Warehouse()
            {
                Id = id
            };
            result = item.Delete();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        #endregion

    }
}