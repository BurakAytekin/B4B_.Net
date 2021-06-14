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
    public class ApproveCommentsController : AdminBaseController
    {
        // GET: Admin/ApproveComments
        public ActionResult Index()
        {
              return View();
        }

        #region HttpPost Methods
        [HttpPost]
        public string GetCommentList()
        {
            List<BlogComment> list = BlogComment.GetBlogCommentListForApprove();
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string GetOemBlackListByType(int type, int oemType)
        {
            List<OemBlackList> list = OemBlackList.GetOemBlackListByType(type, oemType);
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public JsonResult EditOemById(int id, int oemId, string brand, string oemNo)
        {
             bool result = false;

            result = Oem.Update(oemId, brand, oemNo, AdminCurrentSalesman.Id);
            OemBlackList item = new OemBlackList()
            {
                Id = id,
                EditId = AdminCurrentSalesman.Id,
                IsConfirm = true
            };
            result = item.Update();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        [HttpPost]
        public JsonResult InsertOem(int id, int productId, int oemType, string brand, string oemNo)
        {
            bool result = false;
            Product product = Product.GetById(productId);
            Oem oemItem = new Oem()
            {
                Brand = brand,
                OemNo = oemNo
            };

            Oem.Insert(product, oemItem, (OemType)oemType, AdminCurrentSalesman.Id, 1);

            OemBlackList item = new OemBlackList()
            {
                Id = id,
                EditId = AdminCurrentSalesman.Id,
                IsConfirm = true
            };
            result = item.Update();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        [HttpPost]
        public JsonResult CloseOemBlackList(int id)
        {
              bool result = false;

            OemBlackList item = new OemBlackList()
            {
                Id = id,
                EditId = AdminCurrentSalesman.Id,
                IsConfirm = true
            };
            result = item.Update();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        [HttpPost]
        public JsonResult DeleteOemBlackList(int id)
        {
             bool result = false;

            OemBlackList item = new OemBlackList()
            {
                Id = id,
                EditId = AdminCurrentSalesman.Id,
            };
            result = item.Delete();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        [HttpPost]
        public JsonResult DeleteOem(int id, int oemId)
        {
             bool result = false;

            Oem item = new Oem()
            {
                Id = oemId,
                EditId = AdminCurrentSalesman.Id,
            };
            item.Delete();

            OemBlackList bItem = new OemBlackList()
            {
                Id = id,
                EditId = AdminCurrentSalesman.Id,
                IsConfirm = true
            };
            result = bItem.Update();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        #endregion

    }
}