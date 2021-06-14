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
using System.Net.Mail;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class ContactController : AdminBaseController
    {
        // GET: Admin/Contact
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SuggestionRequest()
        {
            return View();
        }
        #region   HttpPost Methods
        [HttpPost]
        public string GetContactItem()
        {
            List<CompanyInformation> list = CompanyInformation.GetAll();
            return JsonConvert.SerializeObject(list);
        }
        [HttpPost]
        public JsonResult DeleteContact(int id)
        {
            bool result = false;
            result = CompanyInformation.Delete(id);
            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        [HttpPost]
        public JsonResult SaveContact(CompanyInformation contact, string imageBase)
        {
            bool result = false;
            string path = string.Empty;
            byte[] fileData = null;

            string filename = Guid.NewGuid().ToString();
            string imgType = string.Empty;

            try
            {
                int indexOfSemiColon = imageBase.IndexOf(";", StringComparison.OrdinalIgnoreCase);

                string dataLabel = imageBase.Substring(0, indexOfSemiColon);

                imgType = dataLabel.Split(':').Last().Split('/').Last();

                string fullFtpFilePath = GlobalSettings.FtpServerUploadAddress + GlobalSettings.FtpCompanyName + GlobalSettings.GeneralPath + filename + "." + imgType;
                fileData = Parse(imageBase);


            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.Admin, "SaveContact", ex, GetUserIpAddress(), -1, -1, AdminCurrentSalesman.Id);

            }
            CompanyInformation item = contact;
            item.Id = contact == null ? 0 : contact.Id;
            item.Picture = fileData;
            item.CreateId = AdminCurrentSalesman.Id;
            item.EditId = AdminCurrentSalesman.Id;
            if (contact != null) result = contact.Id == 0 ? item.Add() : item.Update();
            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }



        [HttpPost]
        public string SaveSuggestionRequestReportAnswer(int Id, string Answer)
        {
            bool result = false;
            MessageBox messageBox;

            result = SuggestionRequestReport.SaveSuggestionRequestReportAnswer(Id, AdminCurrentSalesman.Id, Answer);

            //Mail Gönder
            #region Mail Gönder
            try
            {
                //MailMessage mail = new MailMessage();

                //mail.Subject = string.Format("Talebiniz Cevaplandı");
                //string body = System.IO.File.ReadAllText(Server.MapPath("/files/mailtemplate/customer_suggestion_request_answer.html"));
                //body = body.Replace("#CustomerName#", suggestion.Customer.Name);
                //body = body.Replace("#SuggestionCreateDate#", suggestion.CreateDate.ToString());
                //body = body.Replace("#CustomerSuggestionMessage#", suggestion.Message);
                //body = body.Replace("#SuggestionAnswer#", suggestion.Answer);
                //body = body.Replace("#TodayDate#", DateTime.Now.ToString());
                //mail.Body = body;
                //mail.IsBodyHtml = true;


                //mail.To.Add(suggestion.Customer.Mail);

                //Tuple<bool, string> SendMailReturns = EmailHelper.Send(mail, null, AdminCurrentSalesman);
                //Eğer mail gönderme sonucu false ise
                //if (!SendMailReturns.Item1)
                //{
                //    throw new Exception();//exception a düş...
                //};

                //messageBox = new MessageBox(MessageBoxType.Success, "Cevap Gönderildi");

            }
            catch (Exception)
            {
                messageBox = new MessageBox(MessageBoxType.Error, "Cevap perakendeciye iletilemedi hata oluştu");

            }
            #endregion


            messageBox = result ? new MessageBox(MessageBoxType.Success, "Cevap Gönderildi") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public JsonResult GetListSuggestionRequestReport()
        {
            bool result = true;
            List<SuggestionRequestReport> list = new List<SuggestionRequestReport>();
            try
            {
                list = SuggestionRequestReport.GetListSuggestionRequestReport();

            }
            catch (Exception)
            {
                result = false;
                throw;
            }
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "Bilgiler Başarıyla Gönderildi") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return Json(new { messageBox = messageBox, list = list });
        }


        #endregion


        public new byte[] Parse(string base64Content)
        {
            if (string.IsNullOrEmpty(base64Content))
            {
                throw new ArgumentNullException(nameof(base64Content));
            }

            int indexOfSemiColon = base64Content.IndexOf(";", StringComparison.OrdinalIgnoreCase);

            string dataLabel = base64Content.Substring(0, indexOfSemiColon);

            string contentType = dataLabel.Split(':').Last();

            var startIndex = base64Content.IndexOf("base64,", StringComparison.OrdinalIgnoreCase) + 7;

            var fileContents = base64Content.Substring(startIndex);

            byte[] bytes = Convert.FromBase64String(fileContents);

            return bytes;
        }


    }


}