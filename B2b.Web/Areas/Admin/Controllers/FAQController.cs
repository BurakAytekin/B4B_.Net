using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class FAQController : AdminBaseController
    {
        // GET: Admin/FAQ
        // GET: Admin/FAQ
        public ActionResult List()
        {
            return View();
        }

        public JsonResult GetFAQList()
        {
            List<Faq> FaqList = Faq.GetFaqList();

            return Json(FaqList);
        }

        public string Save(Faq faq)
        {

            faq.CreateId = AdminCurrentSalesman.Id;
            faq.EditId = AdminCurrentSalesman.Id;

            bool result = false;

            try
            {
                result = faq.AddOrEdit();
                result = true;
            }
            catch (Exception)
            {
                throw;
            }


            MessageBox messageBox = result
                   ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                   : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");

            return JsonConvert.SerializeObject(messageBox);

        }
        public string Delete(Faq faq)
        {
            faq.EditId = AdminCurrentSalesman.Id;
            
            bool result = false;

            try
            {
                result = faq.Delete();
                result = true;
            }
            catch (Exception)
            {
                throw;
            }


            MessageBox messageBox = result
                   ? new MessageBox(MessageBoxType.Success, "Silme İşleminiz Tamamlandı")
                   : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");

            return JsonConvert.SerializeObject(messageBox);

        }
    }
}