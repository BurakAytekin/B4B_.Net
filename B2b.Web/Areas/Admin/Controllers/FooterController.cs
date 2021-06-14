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

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class FooterController : AdminBaseController
    {
        // GET: Admin/Footer
        public ActionResult Index()
        {
             return View();
        }

        #region   HttpPost Methods
        
        [HttpPost]
        public JsonResult GetFooterItem()
        {
            FooterInformation item = FooterInformation.GetFooterItem();
            if (item == null)
                item = new FooterInformation();
            return Json(item);
        }

        [HttpPost]
        public JsonResult SaveFooter(FooterInformation footerItem)
        {
              bool result = false;
            footerItem.CreateId = AdminCurrentSalesman.Id;
            footerItem.EditId = AdminCurrentSalesman.Id;
            if (footerItem != null) result = footerItem.Id == 0 ? footerItem.Add() : footerItem.Update();


            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }
        #endregion

    }
}