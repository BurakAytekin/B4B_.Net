using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class T9Controller : AdminBaseController
    {
        // GET: Admin/T9
        public ActionResult Index()
        {
               return View();
        }

        #region   HttpPost Methods
        [HttpPost]
        public string GetListT9(string searchText)
        {
            var list = T9.GetT9List(searchText ?? "");

            return JsonConvert.SerializeObject(list);
        }
        [HttpPost]
        public string DeleteT9(int id)
        {
               T9 t9 = new T9();
            t9.Id = id;

            MessageBox messageBox;

            bool result = t9.Delete();
            messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);


        }

        [HttpPost]
        public string AddT9Data(string key, string t9Datas, bool type)
        {
              T9 t9 = new T9();
            t9.Key = key;
            t9.Data = t9Datas;
            t9.Type = type;

            MessageBox messageBox;
            t9.CreateId = AdminCurrentSalesman.Id;
            bool result = t9.Insert();
            messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);

        }
        #endregion

    }
}