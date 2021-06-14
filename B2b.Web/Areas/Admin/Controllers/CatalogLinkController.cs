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
    public class CatalogLinkController : AdminBaseController
    {
        // GET: Admin/CatalogLink
        public ActionResult Index()
        {
             return View();
        }


        #region  HttpPost Methods
        [HttpPost]
        public string GetCatalogLinkList()
        {
            List<CatalogLink> list = CatalogLink.GetList();
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public JsonResult UpdateCatalogLink(int id, string header, string link, bool isActive)
        {
            bool result = false;
            if (id == 0)
            {
                CatalogLink item = new CatalogLink()
                {
                    Header = header,
                    CreateId = AdminCurrentSalesman.Id,
                    Link = link,
                };
                result = item.Add();
            }
            else
            {
                CatalogLink item = new CatalogLink()
                {
                    Id = id,
                    Header = header,
                    EditId = AdminCurrentSalesman.Id,
                    Link = link,
                    IsActive = isActive
                };
                result = item.Update();
            }
            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        [HttpPost]
        public JsonResult DeleteCatalogLink(int id)
        {
             bool result = false;
            CatalogLink item = new CatalogLink()
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