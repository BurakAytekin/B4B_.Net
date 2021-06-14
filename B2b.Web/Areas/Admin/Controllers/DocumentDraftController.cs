using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.Log;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class DocumentDraftController : AdminBaseController
    {
        // GET: Admin/DocumentDraft
        public ActionResult Index()
        {
             return View();
        }

        public ActionResult Collecting()
        {
            return View();
        }
        public ActionResult Order()
        {
            return View();
        }
        public ActionResult Finance()
        {
            return View();
        }

        #region   HttpPost Methods
        [HttpPost]
        public JsonResult GetDocumentDraftItem(int type, bool defaultData)
        {
            DocumentDraftcs item = DocumentDraftcs.GetDocumentDraft(type, defaultData);
            return Json(item);
        }


        [HttpPost]
        public JsonResult SavePdf(DocumentDraftcs documentDraft, bool type)//
        {
            if (type)
            {
                bool result = false;

                if (documentDraft.Id == 0)
                {
                    documentDraft.CreateId = AdminCurrentSalesman.Id;
                    result = documentDraft.Add();
                }
                else
                {
                    documentDraft.EditId = AdminCurrentSalesman.Id;
                    result = documentDraft.Update();
                }

                MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
                return Json(messageBox);
            }
            else
            {
                string contentHtml = "";
                contentHtml = documentDraft.ContentBody;
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



                string path = "Files/" + documentDraft.Header + ".pdf";

                doc.Save(Server.MapPath("~/" + path));
                // save pdf document
                //byte[] pdf = doc.Save();

                // close pdf document
                doc.Close();


                // return resulted pdf document
                //FileResult fileResult = new FileContentResult(pdf, "application/pdf");
                //fileResult.FileDownloadName = "Document.pdf";


                return Json(GlobalSettings.B2bAddress + path);

                //return Json("http://localhost:35066/" + path);
            }
        }


        #endregion
    }
}