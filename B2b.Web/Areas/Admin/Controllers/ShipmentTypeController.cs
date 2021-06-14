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
    public class ShipmentTypeController : AdminBaseController
    {
        // GET: Admin/ShipmentType
        public ActionResult Index()
        {
             return View();
        }

        #region   HttpPost Methods

        [HttpPost]
        public string GetShipmentTypeList()
        {
            List<ShipmentType> list = ShipmentType.GetShipmentTypeListForAdmin();
            return JsonConvert.SerializeObject(list);
        }


        [HttpPost]
        public JsonResult UpdateShipmentType(int id, string name, int priority)
        {
              bool result = false;
            if (id == 0)
            {
                ShipmentType item = new ShipmentType()
                {
                    Name = name,
                    CreateId = AdminCurrentSalesman.Id,
                    Priority = priority
                };
                result = item.Add();
            }
            else
            {
                ShipmentType item = new ShipmentType()
                {
                    Id = id,
                    Name = name,
                    EditId = AdminCurrentSalesman.Id,
                    Priority = priority
                };
                result = item.Update();
            }
            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        [HttpPost]
        public JsonResult DeleteShipmentType(int id)
        {
            bool result = false;
            ShipmentType item = new ShipmentType()
            {
                Id = id,
                EditId = AdminCurrentSalesman.Id
            };
            result = item.Delete();


            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        #endregion

    }
}