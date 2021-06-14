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
using SelectPdf;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class PaymentController : AdminBaseController
    {
        // GET: Admin/Payment
        public ActionResult Index()
        {
             return View();
        }

        public ActionResult PaymentList()
        {
            return View();
        }

        public ActionResult Detail(int id = -1)
        {
            if (id < 1)
                return View();
            else
                return View();
        }

        #region   HttpPost Methods

        [HttpPost]
        public JsonResult SavePdf(EPayment eItem)//
        {
            DocumentDraftcs documentDraft = DocumentDraftcs.GetDocumentDraft(0, false);

            EPayment item = EPayment.GetListByEpaymentById(eItem.Id);

            string contentHtml = "";
            contentHtml = documentDraft.ContentBody;
            contentHtml = contentHtml.Replace("{|PaymentId|}", item.PaymentId).Replace("{|PaymentNumber|}", item.Id.ToString()).Replace("{|CustomerCode|}", item.Customer.Code).Replace("{|CustomerName|}", item.Customer.Name).Replace("{|3DSecure|}", item._3DSecure ? "3D Secure" : "Normal").Replace("{|BankName|}", item.BankName).Replace("{|Amount|}", item.Amount).Replace("{|Installment|}", item.Installment).Replace("{|InstallmentAmount|}", (item.Total / Convert.ToInt32(item.Installment)).ToString("N2")).Replace("{|CreateDate|}", item.RecordDate.ToString()).Replace("{|ExpendableBonus|}", item.ExpendableBonus).Replace("{|AuthCode|}", item.AuthCode).Replace("{|CardNumber|}", item.CardNumber).Replace("{|NameSurname|}", item.NameSurname).Replace("{|PhoneNumber|}", item.PhoneNumber).Replace("{|Email|}", item.Customer.Mail);
            bool headerFirstPage = true;
            float headerHeight = documentDraft.HeaderHeight;
            bool footerFirstPage = true;
            float footerHeight = documentDraft.FooterHeight;

            string headerHtml = !documentDraft.IsDisplayHeader ? "" : documentDraft.HeaderBody;// headerHtml.Replace("{|pagebreak|}", string.Empty);
                                                                                               //contentHtml = contentHtml.Replace("{|pagebreak|}", "<div style='page-break-after: always'></div>");
            string footerHtml = !documentDraft.IsDisplayFooter ? "" : documentDraft.FooterBody;// footerHtml.Replace("{|pagebreak|}", string.Empty);
                                                                                               // instantiate a html to pdf converter object
            HtmlToPdf converter = new HtmlToPdf();

            // header settings
            converter.Options.DisplayHeader = documentDraft.IsDisplayHeader;
            converter.Header.DisplayOnFirstPage = headerFirstPage;
            converter.Header.DisplayOnOddPages = converter.Options.DisplayHeader;
            converter.Header.DisplayOnEvenPages = converter.Options.DisplayHeader;
            converter.Header.Height = headerHeight;

            PdfHtmlSection headerHtmlSection = new PdfHtmlSection(headerHtml, string.Empty);
            headerHtmlSection.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
            converter.Header.Add(headerHtmlSection);

            // footer settings
            converter.Options.DisplayFooter = documentDraft.IsDisplayFooter;
            converter.Footer.DisplayOnFirstPage = footerFirstPage;
            converter.Footer.DisplayOnOddPages = converter.Options.DisplayFooter;
            converter.Footer.DisplayOnEvenPages = converter.Options.DisplayFooter;
            converter.Footer.Height = footerHeight;

            PdfHtmlSection footerHtmlSection = new PdfHtmlSection(footerHtml, string.Empty);
            footerHtmlSection.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
            converter.Footer.Add(footerHtmlSection);

            // add page numbering element to the footer
            if (documentDraft.IsPageNumber)
            {
                // page numbers can be added using a PdfTextSection object
                PdfTextSection text = new PdfTextSection(0, 10,
                    "Page: {page_number} of {total_pages}  ",
                    new System.Drawing.Font("Arial", 8));
                text.HorizontalAlign = PdfTextHorizontalAlign.Right;
                converter.Footer.Add(text);
            }

            converter.Options.MarginLeft = 40;
            converter.Options.MarginRight = 40;
            converter.Options.MarginTop = 10;
            converter.Options.MarginBottom = 10;
            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertHtmlString(contentHtml, string.Empty);



            // custom header on page 3
            if (doc.Pages.Count >= 3)
            {
                PdfPage page = doc.Pages[2];

                PdfTemplate customHeader = doc.AddTemplate(
                    page.PageSize.Width, headerHeight);
                page.CustomHeader = customHeader;
            }



            string path = "Files/Payment/" + Guid.NewGuid() + ".pdf";

            if (System.IO.File.Exists(Server.MapPath(path)))
            {
                System.IO.File.Delete(Server.MapPath(path));
            }

            doc.Save(Server.MapPath("~/" + path));
            // save pdf document
            //byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();


            // return resulted pdf document
            //FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            //fileResult.FileDownloadName = "Document.pdf";


            return Json(GlobalSettings.B2bAddress + path);
            //return Json("http://localhost:35002/" + path);

        }


        [HttpPost]
        public string GetListPayment(PaymentSearchCriteria paymentSearchCriteria)
        {
            paymentSearchCriteria.EndDate = paymentSearchCriteria.EndDate.Date.Add(new TimeSpan(23, 59, 59));
            return JsonConvert.SerializeObject(EPayment.GetListEpayment(paymentSearchCriteria.StartDate, paymentSearchCriteria.EndDate, paymentSearchCriteria.T9Text, paymentSearchCriteria.PaymentStatu));
        }

        [HttpPost]
        public JsonResult GetBankList()
        {
            List<PosBank> list = PosBank.GetList();
            return Json(list);
        }


        [HttpPost]
        public JsonResult GetPosOfBankList(int posBankId)
        {
            List<PosOfBank> list = PosOfBank.GetPosOfBankList(posBankId);
            return Json(list);
        }

        [HttpPost]
        public JsonResult UpdatePosOfBank(PosOfBank posOfBankItem)
        {
              bool result = false;

            posOfBankItem.EditId = AdminCurrentSalesman.Id;
            result = posOfBankItem.Update();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        [HttpPost]
        public JsonResult UpdatePaymentStatus(int id, int status)
        {
           bool result = false;

            result = EPayment.UpdatePaymentStatus(id, status);
            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }
        [HttpPost]
        public JsonResult GetPosBankDetail(int posBankId)
        {
            PosBankDetail item = PosBankDetail.GetPosBankdEtailByPosBankId(posBankId);
            return Json(item);
        }


        [HttpPost]
        public JsonResult SaveDetailItem(PosBankDetail selectedItem, List<PosInstallment> installmentList)
        {
               bool result = false;
            int id = 0;

            selectedItem.CreateId = AdminCurrentSalesman.Id;
            selectedItem.EditId = AdminCurrentSalesman.Id;


            if (selectedItem.Id != 0)
                result = selectedItem.Update();
            else
                id = selectedItem.Add();

            selectedItem.Id = id;

            if (id > 0)
                result = true;


            foreach (PosInstallment item in installmentList.Where(x => x.Checked).ToList())
            {
                item.PosBankId = selectedItem.PosBankId;
                item.CreateId = AdminCurrentSalesman.Id;
                item.Add();
            }

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .", selectedItem.Id) : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");

            return Json(message);
        }

        [HttpPost]
        public JsonResult GetPosInstallment(int posBankId)
        {
            List<PosInstallment> list = PosInstallment.GetInstallmentByPosBankId(posBankId);
            return Json(list);
        }

        [HttpPost]
        public JsonResult UpdatePosBank(PosBank selectedItem)
        {
              bool result = false;

            selectedItem.EditId = AdminCurrentSalesman.Id;
            result = selectedItem.Update();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        #endregion
        public class PaymentSearchCriteria
        {
            public string T9Text { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int PaymentStatu { get; set; }
        }

    }
}