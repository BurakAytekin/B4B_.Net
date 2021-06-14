using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class EmailSettingsController : AdminBaseController
    {
        // GET: Admin/EmailSettings
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult GetMailSettings(int type)
        {
            EmailSettings eSettings = EmailSettings.GetByType(type);
            return Json(eSettings);
        }

        [HttpPost]
        public JsonResult UpdateMailSettings(EmailSettings selectedMailSettings)
        {
            bool result = false;

            result = selectedMailSettings.Update();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        [HttpPost]
        public JsonResult SendTestMail(EmailSettings selectedMailSettings, string mailAddress)
        {
            MailMessage mail = new MailMessage();
            mail.Body = "Test Mailidir.";
            mail.To.Add(mailAddress);
            mail.Subject = "Test Maili";
            bool result = false;
            string errorMessage = "";
            //AUTH
            Tuple<bool, string> tupl = EmailHelper.Send(mail, selectedMailSettings.Type);
            result = tupl.Item1;
            errorMessage = tupl.Item2;

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir." + errorMessage);
            return Json(message);
        }


    }
}