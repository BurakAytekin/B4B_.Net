using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Controllers
{
    public class ContactController : BaseController
    {

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult FAQ()
        {

            return View();
        }

        public JsonResult GetListFAQ()
        {
            List<Faq> list = Faq.GetFaqList().ToList();

            return Json(list);
        }

        public JsonResult GetListSuggestionType()
        {
            List<SysType> list = SysType.GetListType(1).ToList();
            list.Insert(0, new SysType() { Title = "Seçiniz",KeyId = 0});

            return Json(list);
        }

        #region HttpPost Methods
        [HttpPost]
        public string SaveSuggestionRequestReport(SuggestionRequestReport suggestionRequestReport)
        {
            bool result = false;

            suggestionRequestReport.CreateId = CurrentCustomer.Id;
            result = suggestionRequestReport.Add();


            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "Bilgiler Başarıyla Gönderildi") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string GetContactList()
        {
            return JsonConvert.SerializeObject(CompanyInformation.GetAll());
        } 
        #endregion
    }
}