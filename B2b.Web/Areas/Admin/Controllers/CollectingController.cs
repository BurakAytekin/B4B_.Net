using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.Log;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class CollectingController : AdminBaseController
    {
        // GET: Admin/Collecting
        public ActionResult Index()
        {
              return View();
        }

        public ActionResult Detail()
        {
             return View();
        }


        #region  HttpPost Methods
        [HttpPost]
        public string GetListCollectHeader(CollectSearchCriteria collectSearchCriteria)
        {
            collectSearchCriteria.EndDate = collectSearchCriteria.EndDate.Date.Add(new TimeSpan(23, 59, 59));
            return JsonConvert.SerializeObject(CollectingHeader.GetCollectingHeaderList(collectSearchCriteria.StartDate, collectSearchCriteria.EndDate, collectSearchCriteria.CollectStatu));
        }


        [HttpPost]
        public JsonResult UpdateCollectStatus(int cId)
        {
            CollectingHeader ch = new CollectingHeader { Id = cId };
            var message = ch.Update() ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        [HttpPost]
        public JsonResult DeleteCollectStatus(int cId)
        {
             CollectingHeader ch = new CollectingHeader { Id = cId };
            var message = ch.Delete() ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        [HttpPost]
        public string GetCollectingDetail(int id)
        {
            List<Collecting> list = Collecting.GetListByHeaderId(id);
            return JsonConvert.SerializeObject(list);
        }
        #endregion

    }
    public class CollectSearchCriteria
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CollectStatu { get; set; }
        public string Text { get; set; }

    }
}