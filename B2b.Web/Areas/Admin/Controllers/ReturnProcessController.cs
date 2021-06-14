using B2b.Web.v4.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Log;
using B2b.Web.v4.Models.Helper;
using Newtonsoft.Json;
using SelectPdf;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class ReturnProcessController : AdminBaseController
    {
        // GET: Admin/ReturnProcess
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string GetListReturnForm(CollectSearchCriteria collectSearchCriteria)
        {
            collectSearchCriteria.EndDate = collectSearchCriteria.EndDate.Date.Add(new TimeSpan(23, 59, 59));
            return JsonConvert.SerializeObject(ReturnForm.GetListForAdmin(collectSearchCriteria.StartDate, collectSearchCriteria.EndDate, collectSearchCriteria.Text, collectSearchCriteria.CollectStatu));
        }


        [HttpPost]
        public JsonResult UpdateReturnProcessStatus(ReturnForm item)
        {
            bool result = false;
            item.EditId = AdminCurrentSalesman.Id;
            result = item.Update();


            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        [HttpPost]
        public JsonResult SavePdf(ReturnForm eItem)//
        {
            string contentHtml = "";
            contentHtml = System.IO.File.ReadAllText(Server.MapPath("/files/mailtemplate/returnProduct.html"));
            bool headerFirstPage = true;
            bool footerFirstPage = true;
            contentHtml = contentHtml.Replace("{TypeStr}", eItem.TypeStr).Replace("{ProductName}", eItem.ProductName).Replace
            ("{ProductManuCode}", eItem.ProductManuCode).Replace
            ("{Quantity}", eItem.Quantity.ToString()).Replace
            ("{ReturnReason}", eItem.ReturnReason).Replace
            ("{ProductCode}", eItem.ProductCode).Replace
            ("{ProductManu}", eItem.Manufacturer).Replace
            ("{InvoiceDate}", eItem.InvoiceDate.ToShortDateString()).Replace
            ("{Price}", eItem.Price.ToString("N2")).Replace
            ("{Explanation}", eItem.Explanation).Replace
            ("{PartsInstallationKM}", eItem.PartsInstallationKM);

            //contentHtml = contentHtml.Replace("{ReturnProducType}", eItem.Type == 1 ? "style='display:none;'" : "");


            //List<string> imageList = RegisterCustomer.GetRegisterCustomersImageList(eItem.Id);

            //for (int i = 0; i < imageList.Count; i++)
            //{
            //    contentHtml = contentHtml.Replace("{Picture" + i + "}", "<img width='1000' height='1400' src='http://fileserver.donmez-tr.com/Donmez/RegisterCustomer/" + imageList[i] + "' />");
            //}

            //contentHtml = contentHtml.Replace("{Picture0}", "");
            //contentHtml = contentHtml.Replace("{Picture1}", "");
            //contentHtml = contentHtml.Replace("{Picture2}", "");
            //contentHtml = contentHtml.Replace("{Picture3}", "");
            //contentHtml = contentHtml.Replace("{Picture4}", "");
            //contentHtml = contentHtml.Replace("{Picture5}", "");
            //contentHtml = contentHtml.Replace("{Picture6}", "");
            //contentHtml = contentHtml.Replace("{Picture7}", "");
            //contentHtml = contentHtml.Replace("{Picture8}", "");
            //contentHtml = contentHtml.Replace("{Picture9}", "");


            HtmlToPdf converter = new HtmlToPdf();
            // header settings
            converter.Options.DisplayHeader = true;
            converter.Header.DisplayOnFirstPage = headerFirstPage;
            converter.Header.DisplayOnOddPages = converter.Options.DisplayHeader;
            converter.Header.DisplayOnEvenPages = converter.Options.DisplayHeader;
            converter.Header.Height = 50;
            //PdfHtmlSection headerHtmlSection = new PdfHtmlSection(headerHtml, string.Empty);
            //headerHtmlSection.AutoFitHeight = HtmlToPdfPageFitMode.AutoFit;
            //converter.Header.Add(headerHtmlSection);
            // footer settings
            converter.Options.DisplayFooter = true;
            converter.Footer.DisplayOnFirstPage = footerFirstPage;
            converter.Footer.DisplayOnOddPages = converter.Options.DisplayFooter;
            converter.Footer.DisplayOnEvenPages = converter.Options.DisplayFooter;
            converter.Footer.Height = 50;

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
                    page.PageSize.Width, 50);
                page.CustomHeader = customHeader;
            }

            string path = "Files/ReturnProduct/" + Guid.NewGuid() + ".pdf";

            if (System.IO.File.Exists(Server.MapPath(path)))
            {
                System.IO.File.Delete(Server.MapPath(path));
            }

            doc.Save(Server.MapPath("~/" + path));
            // save pdf document
            //byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();


            return Json(GlobalSettings.B2bAddress + path);
            // return Json("http://localhost:35001/" + path);

        }



    }
}