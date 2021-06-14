using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using AKKNet.NetProv;
using B2b.Web.v4.EpaymentBinService;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using B2b.Web.v4.Models.Log.Entites;
using B2b.Web.v4.Models.Log.EPayment;
using ePayment;
using HtmlAgilityPack;
using _PosnetDotNetModule;
using _PosnetDotNetTDSOOSModule;
using SelectPdf;
using System.Net.Mail;
using System.Collections;

namespace B2b.Web.v4.Controllers
{
    public class PaymentController : BaseController
    {
        #region Fields
        List<EPayment> EPaymentList
        {
            get { return (List<EPayment>)Session["EPaymentList"]; }
            set { Session["EPaymentList"] = value; }
        }
        #endregion

        List<PosInstallment> PosInstallmentList
        {
            get { return (List<PosInstallment>)Session["PosInstallmentList"]; }
            set { Session["PosInstallmentList"] = value; }
        }
        PosPaymentValue PosPaymentValue
        {
            get { return (PosPaymentValue)Session["PosPaymentValue"]; }
            set { Session["PosPaymentValue"] = value; }
        }
        PosBankDetail PosBankDetails
        {
            get { return (PosBankDetail)Session["PosBankDetails"]; }
            set { Session["PosBankDetails"] = value; }
        }
        // GET: Payment
        public ActionResult Index()
        {
            if (Session["PaymentTotal"] != null)
            {
                double total = Convert.ToDouble(Session["PaymentTotal"]);
                ViewBag.totalTl = Math.Floor(total).ToString();
                ViewBag.totalKurus = ((total * 100) % 100).ToString("N0");
                Session["PaymentFromCart"] = true;
                Session["PaymentTotal"] = null;
            }
            //else if (CurrentCustomer.PaymentOnOrder && Session["PaymentTotal"] == null)
            //    throw new Exception(string.Format("{0} {1} Ödeme Tutarı gelmedi.", CurrentCustomer.Code, CurrentCustomer.Name));
            else
                Session["PaymentFromCart"] = null;

            return View();
        }
        #region HttpPostMethods
        [HttpPost]
        public JsonResult PaymenProcesstList()
        {
            EPaymentList = EPayment.GetListByCustomerId(CurrentCustomer.Id);

            return Json(EPaymentList);
        }
        [HttpPost]
        public JsonResult SetInstallmentValue(int id)
        {
            PosPaymentValue posPaymentValue = PosPaymentValue;
            List<PosInstallment> posInstallmentList = PosInstallmentList;

            if (posInstallmentList != null && posInstallmentList.Any(x => x.Id == id))
            {
                PosInstallment installment = posInstallmentList.First(x => x.Id == id);
                posPaymentValue.Installment = installment.Installment;
                posPaymentValue.ExtraInstallment = installment.ExtraInstallment;
                posPaymentValue.DeferalInstallment = installment.DeferalInstallment;
                posPaymentValue.InstallmentNote = installment.Note;
                posPaymentValue.InstallmentAutoNote = installment.AutoNote;
                posPaymentValue.TotalPrice = installment.TotalPrice;
                posPaymentValue.MonthPrice = installment.MonthPrice;
                posPaymentValue.CampaignCode = installment.CampaignCode;
                PosPaymentValue = posPaymentValue;
            }
            return Json(posPaymentValue);
        }

        [HttpPost]
        public JsonResult SetFinalValue(string pointPirce, string pointDecimal, bool usebonus, bool use3D)
        {
            string selectedTotal = pointPirce.Replace(" ", "").Trim().Replace(".", ",") + "." + pointDecimal.Replace(" ", "").Trim().Replace(".", ",");
            PosPaymentValue.ExpendableBonus = selectedTotal;
            PosPaymentValue.UseBonus = usebonus;
            PosPaymentValue._3DSecure = use3D;
            return Json(PosPaymentValue);
        }
        [HttpPost]
        public JsonResult SetPaymentBankSelect()
        {
            try
            {
                PosBankDetails = PosBankDetail.GetPosPaymentChoicePayBank(PosPaymentValue.Installment,
                          PosPaymentValue.BankId);
                if (PosBankDetails.UseBonus)
                {
                    switch (PosBankDetails.PosBanks)
                    {
                        case PosBanks.Akbank:
                            EstAkbankPointQuery(EstSetDefaultValue());
                            break;

                        case PosBanks.Ziraatbank:
                            EstZiraatPointQuery(EstSetDefaultValue());
                            break;

                        case PosBanks.Finansbank:
                            EstFinansPointQuery(EstSetDefaultValue());
                            break;
                        case PosBanks.Ingbank:
                            EstIngPointQuery(EstSetDefaultValue());
                            break;

                        case PosBanks.Halkbank:
                            EstHalbankPointQuery(EstSetDefaultValue());
                            break;
                        case PosBanks.Teb:
                            EstTebPointQuery(EstSetDefaultValue());
                            break;
                        case PosBanks.Hsbc:
                            EstHsbcPointQuery(EstSetDefaultValue());
                            break;
                        case PosBanks.IsBank:
                            EstIsbankPointQuery(EstSetDefaultValue());
                            break;
                        case PosBanks.Sekerbank:
                            EstSekerbankPointQuery(EstSetDefaultValue());
                            break;
                        case PosBanks.Anadolubank:
                            break;

                        case PosBanks.Turkiyefinans:
                            break;
                        case PosBanks.Yapikredi:
                            YkbPointQuery();
                            break;
                        case PosBanks.Kuveytturk:

                            break;

                        case PosBanks.Vakifbank:
                            VakifbankPointSearch();
                            break;

                        case PosBanks.Denizbank:

                            break;
                        case PosBanks.Garanti:
                            GarantiBankBonusQuery();
                            break;
                        default:
                            break;
                    }
                }

                return Json("{\"TotalBonus\":\"" + PosPaymentValue.TotalBonus + "\",\"TotalBonusPrice\":\"" + PosPaymentValue.TotalBonusPrice.ToString() + "\",\"TotalBonusDecimal\":\"" + PosPaymentValue.TotalBonusDecimal.ToString() + "\"}");

            }
            catch (Exception)
            {

                return Json("{\"TotalBonus\":\"" + 0 + "\",\"TotalBonusPrice\":\"" + 0 + "\",\"TotalBonusDecimal\":\"" + 0 + "\"}");
            }
        }



        [HttpPost]
        public JsonResult LoadIntallmentData(string cardNumber, string cardType, string namesurName, bool threeDSecure, string price, string decimalPrice, string cvc, string expDate, string phone, string note)
        {
            double priceVal = Convert.ToDouble(price.Replace(" ", "").Trim().Replace(".", ",") + "," + decimalPrice.Replace(" ", "").Trim().Replace(".", ","));
            string bin = cardNumber.Replace(" ", "").Substring(0, 6);
            string bank, cardCode;
            int CardCode = 0, bankId;
            string explanation;
            EpaymentBinSoapClient BinKontrol = new EpaymentBinSoapClient();
            BinKontrol.BinControl(bin, out explanation, out CardCode, out bank, out bankId, out cardCode);
            BankInformation information = new BankInformation()
            {
                BankId = bankId,
                Explanation = CartTypeCommercialControl(explanation),
                BankName = bank,
                CardCode = cardCode
            };
            string tempExpYear = expDate.Replace(" ", "").Split('/')[1];
            PosPaymentValue posPaymentValue = PosPaymentValue;
            posPaymentValue = posPaymentValue ?? new PosPaymentValue();
            posPaymentValue.CardNumber = cardNumber.Replace(" ", "");
            posPaymentValue.NameSurname = namesurName;
            posPaymentValue._3DSecure = threeDSecure;
            posPaymentValue.Cvc = cvc.Replace(" ", "");
            posPaymentValue.ExpMounth = expDate.Replace(" ", "").Split('/')[0];
            posPaymentValue.ExpYear = tempExpYear.Substring(tempExpYear.Length - 2);
            posPaymentValue.PhoneNumber = phone;
            posPaymentValue.Explanation = note;
            posPaymentValue.Price = Convert.ToInt32(price.Replace(" ", ""));
            posPaymentValue.DecimalPrice = Convert.ToInt32(decimalPrice.Replace(" ", ""));
            posPaymentValue.BankId = information.BankId;
            posPaymentValue.Bank = information.BankName;
            posPaymentValue.CardType = CartTypeControl(cardType);

            PosPaymentValue = posPaymentValue;
            List<PosInstallment> posInstallmentList = PosInstallmentList;
            posInstallmentList = PosInstallment.GetInstallmentList(information.BankId, priceVal, CurrentCustomer.Id, Convert.ToInt32(CurrentCustomer.SpecialInstallment), information.BankId);
            if (posInstallmentList.Count == 0)
            {
                posInstallmentList.Add(PosInstallment.CalculateTotal(new PosInstallment() { Installment = 1, InstallmentText = "Tek Çekim", Id = 1, ExtraInstallment = 0, DeferalInstallment = 0, CommissionRate = 0, BankId = information.BankId, AutoNote = "Bu banka kartına Tek Çekim seçeneği mevcuttur", Note = "" }, priceVal));
            }

            PosInstallmentList = posInstallmentList;
            return Json(posInstallmentList);
        }
        private int CartTypeCommercialControl(string explanation)
        {
            switch (explanation)
            {
                case "EVET":
                    return 1;//Kredi Kartı Ticari Özellikli
                default:
                    return 0;//Kredi Kartı Ticari Özellikli Değil
            }

        }
        #endregion
        public ActionResult PaymentList()
        {
            return View();
        }
        public ActionResult Result()
        {

            EPayment payment = EPayment.GetItemPayment(PosPaymentValue.InsertId);


            PosPaymentValue = new PosPaymentValue();
            PosBankDetails = new PosBankDetail();

            if (payment.ProcReturnCode == "00")
                SendPaymentMail(payment);

            if (Session["PaymentFromCart"] != null && payment.ProcReturnCode == "00")
            {
                Session["PosInsertId"] = payment.PaymentId;
                return RedirectToAction("Index", "Cart");
            }

            return View(payment);
        }
        private void SendPaymentMail(EPayment eItem)
        {

            string contentHtml = "";

            string companyName = CompanyInformation.GetAll()[0].Title.ToString();


            DocumentDraftcs documentDraft = DocumentDraftcs.GetDocumentDraft(0, false);

            EPayment item = EPayment.GetListByEpaymentById(eItem.Id);

            contentHtml = documentDraft.ContentBody;
            contentHtml = contentHtml.Replace("{|PaymentId|}", item.PaymentId).Replace("{|PaymentNumber|}", item.Id.ToString()).Replace("{|CustomerCode|}", item.Customer.Code).Replace("{|CustomerName|}", item.Customer.Name).Replace("{|3DSecure|}", item._3DSecure ? "3D Secure" : "Normal").Replace("{|BankName|}", item.BankName).Replace("{|Amount|}", item.Amount).Replace("{|Installment|}", item.Installment).Replace("{|InstallmentAmount|}", (item.Total / Convert.ToInt32(item.Installment)).ToString("N2")).Replace("{|CreateDate|}", item.RecordDate.ToString()).Replace("{|ExpendableBonus|}", item.ExpendableBonus).Replace("{|AuthCode|}", item.AuthCode).Replace("{|CardNumber|}", item.CardNumber).Replace("{|NameSurname|}", item.NameSurname).Replace("{|PhoneNumber|}", item.PhoneNumber).Replace("{|Email|}", item.Customer.Mail);
            contentHtml = contentHtml.Replace("|", "");
            MailMessage mail = new MailMessage();

            if (CurrentCustomer.Salesman != null && !string.IsNullOrEmpty(CurrentCustomer.Salesman.Email))
                mail.To.Add(CurrentCustomer.Salesman.Email);

            //if (!string.IsNullOrEmpty(CurrentCustomer.Mail))
            //    mail.To.Add(CurrentCustomer.Mail);

            mail.To.Add("r.yalcin@hantech.com.tr");
            mail.To.Add("n.simsek@hantech.com.tr");
            //mail.To.Add("hacer.dogan@eryaz.net");
            mail.Subject = "Yeni Ödemeniz Var";
            mail.Body = contentHtml;
            mail.IsBodyHtml = true;
            EmailHelper.Send(mail);


        }
        [HttpPost]
        public JsonResult GetPayment(int itemId)
        {

            EPayment item = EPayment.GetListByEpaymentById(itemId);
            return Json(item);
        }

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



            string path = "Files/Payment/" + documentDraft.Header + CurrentCustomer.Code + ".pdf";

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
            //return Json("http://localhost:35066/" + path);

        }


        public ActionResult Error(string paymentId)
        {
            //PosPaymentValue = new PosPaymentValue();
            //PosBankDetails = new PosBankDetail();
            ////burda gelen formu hata aldığı için log atabilriz çok işimze yarar

            string errMsg = Request.Form["mdErrorMsg"];

            if (string.IsNullOrEmpty(errMsg))
                errMsg = Request.Form["ErrMsg"];
            if (string.IsNullOrEmpty(errMsg))
                errMsg = Request.Form["mderrormessage"];
            if (string.IsNullOrEmpty(errMsg))
                errMsg = Request.Form.ToString();

            if (Session["VakifBankErrorCode"] != null)
                errMsg = Session["VakifBankErrorCode"].ToString();

            if (Session["VakifBankErrorMessage"] != null)
                errMsg += Session["VakifBankErrorMessage"].ToString();

            Session["VakifBankErrorCode"] = null;
            Session["VakifBankErrorMessage"] = null;


            //EPayment payment = EPayment.GetItemPayment(paymentId);
            //if (payment != null && payment.ProcReturnCode == "00")
            //{
            //    return RedirectToAction("result", "payment", new { paymentId = paymentId });
            //}

            EPayment.Update(PosPaymentValue.InsertId, "", "98", "İşlem Sırasında Hata Oluştu: " + errMsg, PosPaymentValue.IpAddress, PosPaymentValue.PaymentId, PosPaymentValue.PaymentId, "", "", errMsg, PosBankDetails.PosBankName, DateTime.Now, -1);
            ViewBag.PaymentErrMsg = errMsg;
            return CloseFancybox(paymentId);
            // return View();
        }
        public ActionResult PaymentControl()
        {
            string paymentId = Guid.NewGuid().ToString();
            string pMaskedCardNumber = PosPaymentValue.CardNumber.Substring(0, 6) + "******" + PosPaymentValue.CardNumber.Substring(PosPaymentValue.CardNumber.Length - 4, 4);


            DataTable dtInsert = EPayment.Insert(pMaskedCardNumber, PosPaymentValue.NameSurname, PosPaymentValue.ExpMounth, PosPaymentValue.ExpYear, PosPaymentValue.Cvc, PosPaymentValue.TotalPrice + " TL", PosPaymentValue.Installment.ToString(), CurrentCustomer.Id, CurrentCustomer.Code, CurrentCustomer.Name, PosPaymentValue.Bank, PosPaymentValue.TotalPrice, PosPaymentValue.CommissionRate.ToString(), PosPaymentValue.BankId, paymentId, (int)SystemType.Web, PosPaymentValue.PhoneNumber, PosPaymentValue.Explanation, PosPaymentValue.CardType.ToString(), CurrentSalesmanId, PosPaymentValue.ExtraInstallment.ToString(), PosPaymentValue.TotalBonus, PosPaymentValue.ExpendableBonus, PosPaymentValue.UseBonus, PosPaymentValue._3DSecure);
            if (dtInsert.Rows.Count > 0)
            {
                PosPaymentValue.PaymentId = paymentId;
                PosPaymentValue.InsertId = Convert.ToInt32(dtInsert.Rows[0][0].ToString());
                PosPaymentValue.IpAddress = Request.UserHostAddress;


                EPaymentLog log = new EPaymentLog(PosPaymentValue, "", "", "", "-1", "", "");
                EPaymentLogger.LogDB(log, "/api/payment/Add");

                switch (PosBankDetails.PosBanks)
                {
                    case PosBanks.Akbank:
                    case PosBanks.Finansbank:
                    case PosBanks.Halkbank:
                    case PosBanks.Anadolubank:
                    case PosBanks.Teb:
                    case PosBanks.Ziraatbank:
                    case PosBanks.Ingbank:
                    case PosBanks.Sekerbank:
                    case PosBanks.IsBank:
                        if (PosBankDetails._3dSecureSelection == _3DSecureType.Only3D)
                        {
                            return PartialView("Est3D");
                        }
                        else if (PosBankDetails._3dSecureSelection == _3DSecureType.OnlyNonSecure)
                        {
                            return EstPayment();
                        }

                        else
                        {
                            if (PosPaymentValue._3DSecure)
                            {
                                return PartialView("Est3D");
                            }
                            else
                            {
                                return EstPayment();
                            }

                        }
                    case PosBanks.QNBFinansbank:
                        if (PosBankDetails._3dSecureSelection == _3DSecureType.Only3D)
                        {
                            return PartialView("QNBFinans3D");
                        }
                        else if (PosBankDetails._3dSecureSelection == _3DSecureType.OnlyNonSecure)
                        {
                            return QNBFinansbankPayment();
                        }
                        else
                        {
                            if (PosPaymentValue._3DSecure)
                            {
                                return PartialView("QNBFinans3D");
                            }
                            else
                            {
                                return QNBFinansbankPayment();
                            }

                        }
                    case PosBanks.Hsbc://Hsbc sadece 3d İşlem Kabul ediyor
                        return PartialView("Est3D");
                    case PosBanks.Turkiyefinans:
                        if (PosBankDetails._3dSecureSelection == _3DSecureType.Only3D)
                        {
                            return PartialView("TurkiyeFinans3D");
                        }
                        else if (PosBankDetails._3dSecureSelection == _3DSecureType.OnlyNonSecure)
                        {
                            return TurkiyeFinans();
                        }
                        else
                        {
                            if (PosPaymentValue._3DSecure)
                            {
                                return PartialView("TurkiyeFinans3D");
                            }
                            else
                            {
                                return TurkiyeFinans();
                            }

                        }
                    case PosBanks.Yapikredi:
                        if (PosBankDetails._3dSecureSelection == _3DSecureType.Only3D)
                        {
                            return PartialView("Ykb3D");
                        }
                        else if (PosBankDetails._3dSecureSelection == _3DSecureType.OnlyNonSecure)
                        {
                            return YkbPayment();
                        }
                        else
                        {
                            if (PosPaymentValue._3DSecure)
                            {
                                return PartialView("Ykb3D");
                            }
                            else
                            {
                                return YkbPayment();
                            }

                        }
                    case PosBanks.Kuveytturk:
                        if (PosBankDetails._3dSecureSelection == _3DSecureType.Only3D)
                        {
                            return null;
                        }
                        else if (PosBankDetails._3dSecureSelection == _3DSecureType.OnlyNonSecure)
                        {
                            return KuveytTurk();
                        }
                        else
                        {
                            if (PosPaymentValue._3DSecure)
                            {
                                return null;
                            }
                            else
                            {
                                return KuveytTurk();
                            }

                        }
                    case PosBanks.IsBankInnova:

                        if (PosBankDetails._3dSecureSelection == _3DSecureType.Only3D)
                        {
                            return PartialView("Innova3D");

                        }
                        else if (PosBankDetails._3dSecureSelection == _3DSecureType.OnlyNonSecure)
                        {
                            return IsBank();
                        }
                        else
                        {
                            if (PosPaymentValue._3DSecure)
                            {
                                return PartialView("Innova3D");
                            }
                            else
                            {
                                return IsBank();
                            }

                        }
                    case PosBanks.Vakifbank:
                        if (PosBankDetails._3dSecureSelection == _3DSecureType.Only3D)
                        {
                            return PartialView("VakifBank3D");
                        }
                        else if (PosBankDetails._3dSecureSelection == _3DSecureType.NonSecureOr3D)
                        {
                            return Vakifbank();
                        }
                        else
                        {
                            if (PosPaymentValue._3DSecure)
                            {
                                return PartialView("VakifBank3D");
                            }
                            else
                            {
                                return Vakifbank();
                            }

                        }
                    case PosBanks.Denizbank:
                        if (PosBankDetails._3dSecureSelection == _3DSecureType.Only3D)
                        {
                            return PartialView("DenizBank3D");
                        }
                        else if (PosBankDetails._3dSecureSelection == _3DSecureType.OnlyNonSecure)
                        {
                            return DenizBank();
                        }
                        else
                        {
                            if (PosPaymentValue._3DSecure)
                            {
                                return PartialView("DenizBank3D");
                            }
                            else
                            {
                                return DenizBank();
                            }

                        }
                    case PosBanks.Garanti:
                        if (PosBankDetails._3dSecureSelection == _3DSecureType.Only3D)
                        {
                            return PartialView("Garanti3D");
                        }
                        else if (PosBankDetails._3dSecureSelection == _3DSecureType.OnlyNonSecure)
                        {
                            return GarantiBank();
                        }
                        else
                        {
                            if (PosPaymentValue._3DSecure)
                            {
                                return PartialView("Garanti3D");
                            }
                            else
                            {
                                return GarantiBank();
                            }

                        }
                }
            }
            return CloseFancybox(PosPaymentValue.PaymentId);

        }

        public ActionResult KuvetyturkPos()
        {
            return new PartialViewResult();
        }

        public ActionResult QNBFinans3D()
        {
            return View();
        }

        public ActionResult QNBFinans3DP()
        {
            String userCode = PosBankDetails.ApiUser;
            String userPass = PosBankDetails.Password;
            String mdstatus = Request.Form.Get("3DStatus");
            String orderId = Request.Form.Get("OrderId");
            string ProcReturnCode = "99";
            string AuthCode = "";
            string ErrMsg = "İşlem Sırasında Bir Hata Oluştu.";
            string OrderId = "";
            string Extra = "";

            if (mdstatus == "1") // 3D Kullanıcı Dogrulama Basarili
            {
                Hashtable list = new Hashtable();
                HttpWebResponse resp = null;

                String payersecuritylevelval = Request.Form.Get("Eci");
                String payertxnidval = Request.Form.Get("PayerTxnId");
                String payerauthenticationcodeval = Request.Form.Get("PayerAuthenticationCode");
                String merchantId = Request.Form.Get("MerchantID");
                String requestGuid = Request.Form.Get("RequestGuid");

                String format = "{0}={1}&";
                StringBuilder str = new StringBuilder();

                str.AppendFormat(format, "UserCode", userCode);
                str.AppendFormat(format, "UserPass", userPass);
                str.AppendFormat(format, "OrderId", orderId);
                str.AppendFormat(format, "SecureType", "3DModelPayment");
                str.AppendFormat(format, "RequestGuid", requestGuid);

                try
                {
                    #region 3DModel Modeli için 2.POST işlemleri
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://vpos.qnbfinansbank.com/Gateway/Default.aspx");

                    byte[] parameters = Encoding.UTF8.GetBytes(str.ToString());
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = parameters.Length;
                    System.IO.Stream requeststream = request.GetRequestStream();
                    requeststream.Write(parameters, 0, parameters.Length);
                    requeststream.Close();

                    resp = (HttpWebResponse)request.GetResponse();
                    System.IO.StreamReader responsereader = new System.IO.StreamReader(resp.GetResponseStream(), Encoding.UTF8);

                    String responseStr = responsereader.ReadToEnd();
                    string[] paramArr = responseStr.Split(new char[] { ';', ';' }, StringSplitOptions.RemoveEmptyEntries);


                    if (responseStr != null)
                    {
                        foreach (var item in paramArr)
                        {
                            String xkey = item.Split('=')[0];
                            String xval = item.Split('=')[1];
                            list.Add(xkey, xval);
                        }
                    }
                    #endregion

                    //#region 3DPay Modeli ile entegrasyon yapılmak istenirse yukarıdaki region alanı tamamen kaldırılıp bu alan kullanılacak.
                    //IEnumerator enumerator = Request.Form.GetEnumerator();

                    //while (enumerator.MoveNext())
                    //{
                    //    String xkey = (String)enumerator.Current;
                    //    String xval = Request.Form.Get(xkey);

                    //    list.Add(xkey, xval);
                    //}
                    //#endregion

                    ProcReturnCode = list.ContainsKey("ProcReturnCode") ? list["ProcReturnCode"].ToString() : "";
                    AuthCode = list.ContainsKey("AuthCode") ? list["AuthCode"].ToString() : "";
                    ErrMsg = list.ContainsKey("ErrMsg") ? list["ErrMsg"].ToString() : "";
                    OrderId = list.ContainsKey("OrderId") ? list["OrderId"].ToString() : "";
                    Extra = list.ContainsKey("BankInternalResponseMessage") ? list["BankInternalResponseMessage"].ToString() : "";
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
                finally
                {
                    if (resp != null)
                        resp.Close();
                }

                EPayment.Update(PosPaymentValue.InsertId, AuthCode, ProcReturnCode, ErrMsg, PosPaymentValue.IpAddress, PosPaymentValue.PaymentId, PosPaymentValue.PaymentId, "", OrderId, Extra, PosBankDetails.PosBankName, DateTime.Now, 1);
            }
            else
            {
                EPayment.Update(PosPaymentValue.InsertId, AuthCode, "99", "3D Kullanıcı Dogrulama Hatali.", PosPaymentValue.IpAddress, PosPaymentValue.PaymentId, PosPaymentValue.PaymentId, "", OrderId, Extra, PosBankDetails.PosBankName, DateTime.Now, 1);
            }

            return CloseFancybox(PosPaymentValue.PaymentId);
        }

        public ActionResult Est3D()
        {
            return View();
        }
        public ActionResult Est3DP()
        {
            #region Banka işlemleri

            string ErrMsg = "";
            String hashparams = Request.Form.Get("HASHPARAMS");
            String hashparamsval = Request.Form.Get("HASHPARAMSVAL");
            string storekey = string.Empty;
            string nameval = string.Empty;
            string url = string.Empty;
            string passwordval = string.Empty;
            string clientidval = string.Empty;
            string useEpayment = string.Empty;
            int pUserPaymentBankId = 0;

            if (PosBankDetails != null)
            {
                useEpayment = PosBankDetails.PosBankName.Trim().ToUpper();
                pUserPaymentBankId = PosPaymentValue.BankId;
            }

            storekey = PosBankDetails._3dSecureKey;
            nameval = PosBankDetails.ApiUser;
            passwordval = PosBankDetails.Password;
            url = PosBankDetails.PosBank.PostUrl;
            clientidval = PosBankDetails.StoreNumber;
            String paramsval = "";
            int index1 = 0, index2 = 0;
            do
            {
                index2 = hashparams.IndexOf(":", index1);
                String val = Request.Form.Get(hashparams.Substring(index1, index2 - index1)) ?? "";
                paramsval += val;
                index1 = index2 + 1;
            }
            while (index1 < hashparams.Length);

            String hashval = paramsval + storekey;
            String hashparam = Request.Form.Get("HASH");

            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] hashbytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(hashval);
            byte[] inputbytes = sha.ComputeHash(hashbytes);
            String hash = Convert.ToBase64String(inputbytes);
            if (!paramsval.Equals(hashparamsval) || !hash.Equals(hashparam))
            {
                Response.Write("<h4>Güvenlik Uyarısı. Sayısal İmza Geçerli Değil</h4>");
            }
            String modeval = "P";
            String typeval = "Auth";
            String expiresval = Request.Form.Get("Ecom_Payment_Card_ExpDate_Month") + "/" + Request.Form.Get("Ecom_Payment_Card_ExpDate_Year");
            String cv2val = Request.Form.Get("cv2");
            String totalval = Request.Form.Get("amount");
            String numberval = Request.Form.Get("md");
            String strTaksit = PosPaymentValue.Installment.ToString();
            String taksitval = "";
            if (strTaksit == "1")
                taksitval = "";
            else
                taksitval = strTaksit;
            String currencyval = "949";
            String orderidval = PosPaymentValue.PaymentId;
            String mdstatus = Request.Form.Get("mdStatus"); // mdStatus 3d işlemin sonucu ile ilgili bilgi verir. 1,2,3,4 başarılı, 5,6,7,8,9,0 başarısızdır.
            String gelenXml = "";
            String xmlResponse = "";
            String xmlAuthCode = "";
            String xmlHostRefNum = "";
            String xmlProcReturnCode = "";
            String xmlTransId = "";
            String xmlErrMsg = "";
            // 3D parametreler
            if (mdstatus.Equals("1") || mdstatus.Equals("2") || mdstatus.Equals("3") || mdstatus.Equals("4")) //3D Onayı alınmıştır.
            {
                //Response.Write("<h5>3D İşlemi Başarılı</h5><br/>");
                String cardholderpresentcodeval = "13";
                String payersecuritylevelval = Request.Form.Get("eci");
                String payertxnidval = Request.Form.Get("xid");
                String payerauthenticationcodeval = Request.Form.Get("cavv");

                #region
                String ipaddressval = "";
                String emailval = "";
                String groupidval = "";
                String transidval = "";
                String useridval = "";
                //Fatura Bilgileri
                String billnameval = PosPaymentValue.CustomerCode;      //Fatur İsmi
                String billstreet1val = "";   //Fatura adres 1
                String billstreet2val = "";   //Fatura adres 2
                String billstreet3val = "";   //Fatura adres 3
                String billcityval = "";      //Fatura şehir
                String billstateprovval = ""; //Fatura eyalet
                String billpostalcodeval = ""; //Fatura posta kodu

                //Teslimat Bilgileri
                String shipnameval = "";      //isim
                String shipstreet1val = "";   //adres 1
                String shipstreet2val = "";   //adres 2
                String shipstreet3val = "";   //adres 3
                String shipcityval = "";      //şehir
                String shipstateprovval = ""; //eyalet
                String shippostalcodeval = "";//posta kodu
                String extraval = "";

                if (PosPaymentValue.UseBonus)
                {
                    string ExpendableBonus = PosPaymentValue.ExpendableBonus.ToString().Trim();

                    switch (PosBankDetails.PosBanks)
                    {
                        case PosBanks.Akbank:
                            extraval += "<CARDHOLDERNAME>" + PosPaymentValue.NameSurname + "</CARDHOLDERNAME>";
                            extraval += "<CCBCHIPPARA>" + ExpendableBonus + "</CCBCHIPPARA>";

                            break;
                        case PosBanks.Finansbank:
                        case PosBanks.Ziraatbank:
                        case PosBanks.Ingbank:
                        case PosBanks.Teb:
                            extraval += "<KULLANPUAN>" + ExpendableBonus + "</KULLANPUAN>";
                            break;
                        case PosBanks.Halkbank:
                            extraval += "<ODUL>" + ExpendableBonus + "</ODUL>";
                            break;
                        case PosBanks.Hsbc:
                            extraval += "<ODUL>" + ExpendableBonus + "</ODUL>";
                            break;

                    }
                }

                //Ödeme için gerekli xml yapısı oluşturuluyor
                #region Xml Create
                XmlDocument doc = new XmlDocument();

                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "ISO-8859-9", "yes");

                doc.AppendChild(dec);
                XmlElement cc5Request = doc.CreateElement("CC5Request");
                doc.AppendChild(cc5Request);

                XmlElement name = doc.CreateElement("Name");
                name.AppendChild(doc.CreateTextNode(nameval));
                cc5Request.AppendChild(name);

                XmlElement password = doc.CreateElement("Password");
                password.AppendChild(doc.CreateTextNode(passwordval));
                cc5Request.AppendChild(password);

                XmlElement clientid = doc.CreateElement("ClientId");
                clientid.AppendChild(doc.CreateTextNode(clientidval));
                cc5Request.AppendChild(clientid);

                XmlElement ipaddress = doc.CreateElement("IPAddress");
                ipaddress.AppendChild(doc.CreateTextNode(ipaddressval));
                cc5Request.AppendChild(ipaddress);

                XmlElement email = doc.CreateElement("Email");
                email.AppendChild(doc.CreateTextNode(emailval));
                cc5Request.AppendChild(email);

                XmlElement mode = doc.CreateElement("Mode");
                mode.AppendChild(doc.CreateTextNode(modeval));
                cc5Request.AppendChild(mode);

                XmlElement orderid = doc.CreateElement("OrderId");
                orderid.AppendChild(doc.CreateTextNode(orderidval));
                cc5Request.AppendChild(orderid);

                XmlElement groupid = doc.CreateElement("GroupId");
                groupid.AppendChild(doc.CreateTextNode(groupidval));
                cc5Request.AppendChild(groupid);

                XmlElement transid = doc.CreateElement("TransId");
                transid.AppendChild(doc.CreateTextNode(transidval));
                cc5Request.AppendChild(transid);

                XmlElement userid = doc.CreateElement("UserId");
                userid.AppendChild(doc.CreateTextNode(useridval));
                cc5Request.AppendChild(userid);

                XmlElement type = doc.CreateElement("Type");
                type.AppendChild(doc.CreateTextNode(typeval));
                cc5Request.AppendChild(type);

                XmlElement number = doc.CreateElement("Number");
                number.AppendChild(doc.CreateTextNode(numberval));
                cc5Request.AppendChild(number);

                XmlElement expires = doc.CreateElement("Expires");
                expires.AppendChild(doc.CreateTextNode(expiresval));
                cc5Request.AppendChild(expires);

                XmlElement cvv2val = doc.CreateElement("Cvv2Val");
                cvv2val.AppendChild(doc.CreateTextNode(cv2val));
                cc5Request.AppendChild(cvv2val);

                XmlElement total = doc.CreateElement("Total");
                total.AppendChild(doc.CreateTextNode(totalval));
                cc5Request.AppendChild(total);

                XmlElement currency = doc.CreateElement("Currency");
                currency.AppendChild(doc.CreateTextNode(currencyval));
                cc5Request.AppendChild(currency);

                XmlElement taksit = doc.CreateElement("Taksit");
                taksit.AppendChild(doc.CreateTextNode(taksitval));
                cc5Request.AppendChild(taksit);

                XmlElement payertxnid = doc.CreateElement("PayerTxnId");
                payertxnid.AppendChild(doc.CreateTextNode(payertxnidval));
                cc5Request.AppendChild(payertxnid);

                XmlElement payersecuritylevel = doc.CreateElement("PayerSecurityLevel");
                payersecuritylevel.AppendChild(doc.CreateTextNode(payersecuritylevelval));
                cc5Request.AppendChild(payersecuritylevel);

                XmlElement payerauthenticationcode = doc.CreateElement("PayerAuthenticationCode");
                payerauthenticationcode.AppendChild(doc.CreateTextNode(payerauthenticationcodeval));
                cc5Request.AppendChild(payerauthenticationcode);

                XmlElement cardholderpresentcode = doc.CreateElement("CardholderPresentCode");
                cardholderpresentcode.AppendChild(doc.CreateTextNode(cardholderpresentcodeval));
                cc5Request.AppendChild(cardholderpresentcode);

                XmlElement billto = doc.CreateElement("BillTo");
                cc5Request.AppendChild(billto);

                XmlElement billname = doc.CreateElement("Name");
                billname.AppendChild(doc.CreateTextNode(billnameval));
                billto.AppendChild(billname);

                XmlElement billstreet1 = doc.CreateElement("Street1");
                billstreet1.AppendChild(doc.CreateTextNode(billstreet1val));
                billto.AppendChild(billstreet1);

                XmlElement billstreet2 = doc.CreateElement("Street2");
                billstreet2.AppendChild(doc.CreateTextNode(billstreet2val));
                billto.AppendChild(billstreet2);

                XmlElement billstreet3 = doc.CreateElement("Street3");
                billstreet3.AppendChild(doc.CreateTextNode(billstreet3val));
                billto.AppendChild(billstreet3);

                XmlElement billcity = doc.CreateElement("City");
                billcity.AppendChild(doc.CreateTextNode(billcityval));
                billto.AppendChild(billcity);

                XmlElement billstateprov = doc.CreateElement("StateProv");
                billstateprov.AppendChild(doc.CreateTextNode(billstateprovval));
                billto.AppendChild(billstateprov);

                XmlElement billpostalcode = doc.CreateElement("PostalCode");
                billpostalcode.AppendChild(doc.CreateTextNode(billpostalcodeval));
                billto.AppendChild(billpostalcode);

                XmlElement shipto = doc.CreateElement("ShipTo");
                cc5Request.AppendChild(shipto);

                XmlElement shipname = doc.CreateElement("Name");
                shipname.AppendChild(doc.CreateTextNode(shipnameval));
                shipto.AppendChild(shipname);

                XmlElement shipstreet1 = doc.CreateElement("Street1");
                shipstreet1.AppendChild(doc.CreateTextNode(shipstreet1val));
                shipto.AppendChild(shipstreet1);

                XmlElement shipstreet2 = doc.CreateElement("Street2");
                shipstreet2.AppendChild(doc.CreateTextNode(shipstreet2val));
                shipto.AppendChild(shipstreet2);

                XmlElement shipstreet3 = doc.CreateElement("Street3");
                shipstreet3.AppendChild(doc.CreateTextNode(shipstreet3val));
                shipto.AppendChild(shipstreet3);

                XmlElement shipcity = doc.CreateElement("City");
                shipcity.AppendChild(doc.CreateTextNode(shipcityval));
                shipto.AppendChild(shipcity);

                XmlElement shipstateprov = doc.CreateElement("StateProv");
                shipstateprov.AppendChild(doc.CreateTextNode(shipstateprovval));
                shipto.AppendChild(shipstateprov);

                XmlElement shippostalcode = doc.CreateElement("PostalCode");
                shippostalcode.AppendChild(doc.CreateTextNode(shippostalcodeval));
                shipto.AppendChild(shippostalcode);

                XmlElement extra = doc.CreateElement("Extra");
                extra.AppendChild(doc.CreateTextNode(extraval));


                cc5Request.AppendChild(extra);
                #endregion
                #endregion

                String xmlval = doc.OuterXml.Replace("&lt;", "<").Replace("&gt;", ">");     //Oluşturulan xml string olarak alınıyor.



                HttpWebResponse resp = null;
                try
                {
                    System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                    string postdata = "DATA=" + xmlval.ToString();
                    byte[] postdatabytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(postdata);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = postdatabytes.Length;
                    Stream requeststream = request.GetRequestStream();
                    requeststream.Write(postdatabytes, 0, postdatabytes.Length);
                    requeststream.Close();

                    resp = (HttpWebResponse)request.GetResponse();
                    StreamReader responsereader = new StreamReader(resp.GetResponseStream(),
                        Encoding.GetEncoding("ISO-8859-9"));

                    gelenXml = responsereader.ReadToEnd(); //Gelen xml string olarak alındı.

                    XmlDocument gelen = new XmlDocument();
                    gelen.LoadXml(gelenXml);    //string xml dökumanına çevrildi.

                    XmlNodeList list = gelen.GetElementsByTagName("Response");
                    xmlResponse = list[0].InnerText;

                    list = gelen.GetElementsByTagName("AuthCode");
                    xmlAuthCode = list[0].InnerText;
                    list = gelen.GetElementsByTagName("HostRefNum");
                    xmlHostRefNum = list[0].InnerText;
                    list = gelen.GetElementsByTagName("ProcReturnCode");
                    xmlProcReturnCode = list[0].InnerText;
                    list = gelen.GetElementsByTagName("TransId");
                    xmlTransId = list[0].InnerText;
                    list = gelen.GetElementsByTagName("ErrMsg");
                    xmlErrMsg = list[0].InnerText;
                    Logger.LogPayment(LogPaymentType.Information, "EST Islem Sonucu",
                        " XML Response : " + gelen.InnerXml, PosPaymentValue.PaymentId,
                      useEpayment.ToString());

                    #endregion

                    ErrMsg = "Approved".Equals(xmlResponse) ? "Ödeme başarıyla gerçekleştirildi" : "Ödemede hata oluştu !";
                    resp.Close();
                }
                catch (Exception ex)
                {
                    Console.Write(ex.ToString());
                }
                finally
                {
                    if (resp != null)
                        resp.Close();
                }
            }
            else
            {
                ErrMsg = "3D Onayı alınamadı! Güvenlik bilgilerinizde sorun var !";
            }

            EPayment.Update(PosPaymentValue.InsertId, xmlAuthCode, xmlProcReturnCode, xmlErrMsg, PosPaymentValue.IpAddress, PosPaymentValue.PaymentId, PosPaymentValue.PaymentId, "", xmlTransId, gelenXml.ToString(), PosBankDetails.PosBankName, DateTime.Now, pUserPaymentBankId);



            return CloseFancybox(PosPaymentValue.PaymentId);
        }

        public ActionResult TurkiyeFinans3D()
        {
            return View();
        }

        public ActionResult TurkiyeFinans3DP()
        {
            try
            {
                NetProvRemote myProv = new NetProvRemote();
                myProv.OrgNo = Convert.ToInt32(PosBankDetails.ApiUser);//006;
                myProv.FirmNo = Convert.ToInt32(PosBankDetails.StoreNumber);//9626604;
                myProv.TermNo = Convert.ToInt32(PosBankDetails.TerminalNo);

                myProv.CardNo = long.Parse(PosPaymentValue.CardNumber);
                string expriy = "20" + PosPaymentValue.ExpYear.Trim() + PosPaymentValue.ExpMounth.Trim();
                myProv.Expiry = int.Parse(expriy);
                myProv.Cvv2No = int.Parse(PosPaymentValue.Cvc);
                myProv.Amount = Convert.ToDecimal(PosPaymentValue.TotalPrice);
                if (PosPaymentValue.Installment == 1)
                    myProv.Taksit = 0;
                else
                    myProv.Taksit = PosPaymentValue.Installment;
                // myProv.BonusAmount = decimal.Parse(0);
                string OrderID = PosPaymentValue.PaymentId.Substring(0, 15);
                myProv.SipNo = OrderID;
                myProv.CurrencyCode = 949;
                myProv.WaitForSaleCompleted = true;

                myProv.MerchantKey = PosBankDetails._3dSecureKey; //"awNv56fY";

                // Burada ayrıca MPI değişkenlerine ilgili dönüş değerleri atanacak
                // myProv.MPI* = Form[“MPI*”];



                //NetProvRemote_SaleIsCompleted cmp = new NetProvRemote_SaleIsCompleted();//bu kod ön otorizayonu onaylamak için örnek olarak bırakıldı.
                //cmp.AuthCode="418755";
                //cmp.FirmNo = myProv.FirmNo = 9626604;
                //cmp.MerchantKey = myProv.MerchantKey;
                //cmp.OrgNo = myProv.OrgNo;
                //cmp.SaleId = 4580469;
                //cmp.TermNo = myProv.TermNo;
                //var sonuc =cmp.MessageSend();

                myProv.MPIcavv = Request.Form["cavv"];
                myProv.MPIxid = Request.Form["xid"];
                myProv.MPIeci = Request.Form["eci"];
                myProv.MPIversion = Request.Form["version"];
                myProv.MPImerchantID = Request.Form["merchantID"];
                myProv.MPImdStatus = Request.Form["mdStatus"];
                myProv.MPImdErrorMsg = Request.Form["mdErrorMsg"];
                myProv.MPItxstatus = Request.Form["txstatus"];
                myProv.MPIiReqCode = Request.Form["iReqCode"];
                myProv.MPIiReqDetail = Request.Form["iReqDetail"];
                myProv.MPIvendorCode = Request.Form["vendorCode"];
                myProv.MPIcavvAlgorithm = Request.Form["cavvAlgorithm"];
                myProv.MPIPAResVerified = Request.Form["PAResVerified"];
                myProv.MPIPAResSyntaxOK = "0";
                myProv.WaitForSaleCompleted = false;

                string result = "İşlem Başarısız";
                string Code = "007";
                string responseCode;
                string responseDesc;
                string xml;
                string useEpayment = Bank.TURKIYEFINANS.ToString();
                var Sonuc = myProv.MessageSend();
                string autCode = "";
                string trasnId = "";
                responseCode = myProv.ResponseCode;
                if (Sonuc && responseCode == "00")
                {
                    Code = "00";
                    result = "İşlem Başarılı";
                    responseCode = myProv.ResponseCode;
                    responseDesc = myProv.ResponseDescription;
                    string errmsag = myProv.MPImdErrorMsg;
                    var splitCode = responseDesc.Split((char)58);
                    xml = Serializer.Serialize(myProv).InnerXml;
                    if (splitCode.Any())
                        autCode = splitCode[0];

                    if (splitCode.Count() >= 3)
                    {
                        trasnId = splitCode[1];
                        responseDesc = splitCode[2];

                    }

                }
                else
                {
                    xml = Serializer.Serialize(myProv).InnerXml;
                    responseCode = myProv.ResponseCode;
                    responseDesc = myProv.ResponseDescription;
                }


                Logger.LogPayment(LogPaymentType.Information, "TURKIYEFINANS 3D Islem Sonucu",
                  " XML Request : " + xml, PosPaymentValue.PaymentId, PosPaymentValue.Bank
               );
                EPaymentLog log_return = new EPaymentLog(PosPaymentValue, string.Empty, xml, responseCode, Code, responseDesc, OrderID, 97);//97 işlemin bankadan geldiğini gösterir
                EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");
                EPayment.Update(PosPaymentValue.InsertId, autCode, Code, result + " " + responseDesc, PosPaymentValue.IpAddress, OrderID, OrderID, "", trasnId, xml, PosBankDetails.PosBankName, DateTime.Now, PosPaymentValue.BankId);
                return CloseFancybox(PosPaymentValue.PaymentId);
            }
            catch (Exception exception)
            {

                Logger.LogPayment(LogPaymentType.Information, "TURKIYEFINANS 3D Islem Sonucu",
               " XML Request : " + exception, PosPaymentValue.PaymentId, PosPaymentValue.Bank
            );

                EPayment.Update(PosPaymentValue.InsertId, String.Empty, "07", "İşlem Başarısız. Provizyon Alınırken Hata Oluştu", PosPaymentValue.IpAddress, PosPaymentValue.PaymentId, PosPaymentValue.PaymentId, "", PosPaymentValue.PaymentId, string.Empty, PosBankDetails.PosBankName, DateTime.Now, PosPaymentValue.BankId);
                return CloseFancybox(PosPaymentValue.PaymentId);

            }

        }
        public ActionResult DenizBank3D()
        {
            return View();
        }
        public ActionResult DenizBank3DP()
        {

            String hashparams = Request.Form.Get("HASHPARAMS");
            String hashparamsval = Request.Form.Get("HASHPARAMSVAL");
            String storekey = PosBankDetails._3dSecureKey;
            String paramsval = "";
            int index1 = 0, index2 = 0;
            // hash hesaplamada kullanılacak değerler ayrıştırılıp değerleri birleştiriliyor.
            do
            {
                index2 = hashparams.IndexOf(":", index1);
                String val = Request.Form.Get(hashparams.Substring(index1, index2 - index1)) ?? "";
                paramsval += val;
                index1 = index2 + 1;
            }
            while (index1 < hashparams.Length);
            String hashval = paramsval + storekey;         //elde edilecek hash değeri için paramsval e store key ekleniyor. (işyeri anahtarı)
            String hashparam = Request.Form.Get("HASH");
            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] hashbytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(hashval);
            byte[] inputbytes = sha.ComputeHash(hashbytes);
            String hash = Convert.ToBase64String(inputbytes); //Güvenlik ve kontrol amaçlı oluşturulan hash
            if (!paramsval.Equals(hashparamsval) || !hash.Equals(hashparam)) //oluşturulan hash ile gelen hash ve hash parametreleri değerleri ile ayrıştırılıp edilen edilen aynı olmalı.
            {
                Response.Write("<h4>Güvenlik Uyarısı. Sayısal İmza Geçerli Değil</h4>");
            }

            // Ödeme için gerekli parametreler

            String usercode = PosBankDetails.ApiUser; //İşyeri kullanıcı adı
            String userpass = PosBankDetails.Password; //İşyeri şifresi
            String shopcode = Request.Form.Get("ShopCode"); //İşyeri numarası
            String TxnType = "Auth";                        //Auth PreAuth PostAuth Credit Void olabilir.
            String purchamount = Request.Form.Get("PurchAmount"); //Tutar
            String MD = Request.Form.Get("MD");             //Kart numarası olarak 3d sonucu dönem md parametresi kullanılır.
            String installmentcount = PosPaymentValue.Installment.ToString();                   //Taksit sayısı peşin satışlar da boş olarak gönderilmelidir.
            String currencyval = Request.Form.Get("Currency");           //ytl için
            String orderidval = Request.Form.Get("OrderId");                         //Sipariş numarası
            String PayerTxnId = Request.Form.Get("PayerTxnId");
            String PayerAuthenticationCode = Request.Form.Get("PayerAuthenticationCode");
            String Eci = Request.Form.Get("Eci");
            String mdstatus = Request.Form.Get("mdStatus"); // mdStatus 3d işlemin sonucu ile ilgili bilgi verir. 1,2,3,4 başarılı, 5,6,7,8,9,0 başarısızdır.
            // 3D parametreler
            if (mdstatus.Equals("1")) //3D Onayı alınmıştır.
            {
                /*Moto-ecommerce işlem için gerekli alanlar gönderim için listeye ekleniyor.*/
                String format = "{0}={1}&";
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat(format, "ShopCode", shopcode);
                sb.AppendFormat(format, "UserCode", usercode);
                sb.AppendFormat(format, "UserPass", userpass);
                sb.AppendFormat(format, "PurchAmount", purchamount);
                sb.AppendFormat(format, "Currency", currencyval);
                sb.AppendFormat(format, "OrderId", orderidval);
                sb.AppendFormat(format, "InstallmentCount", installmentcount);
                sb.AppendFormat(format, "TxnType", TxnType);
                sb.AppendFormat(format, "MD", MD);
                sb.AppendFormat(format, "SecureType", "NonSecure");
                sb.AppendFormat(format, "Lang", "TR");
                sb.AppendFormat(format, "MOTO", "0");
                sb.AppendFormat(format, "CustomerName", "nonsecurename");
                //Eğer 3D doğrulaması yapılmış ise aşağıdaki alanlar da gönderilmelidir*/							/*
                sb.AppendFormat(format, "PayerAuthenticationCode", PayerAuthenticationCode);
                sb.AppendFormat(format, "Eci", Eci);//Visa - 05,06 MasterCard 01,02 olabilir	
                sb.AppendFormat(format, "PayerTxnId", PayerTxnId);
                // Ödeme için bağlantı kuruluyor. ve post ediliyor
                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PosBankDetails.PosBank.PostUrl);
                    byte[] parameters = Encoding.GetEncoding("ISO-8859-9").GetBytes(sb.ToString());
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = parameters.Length;
                    Stream requeststream = request.GetRequestStream();
                    requeststream.Write(parameters, 0, parameters.Length);
                    requeststream.Close();
                    Response.Write(sb.ToString());
                    HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
                    StreamReader responsereader = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("ISO-8859-9"));
                    String responseStr = responsereader.ReadToEnd();
                    if (responseStr != null)
                    {
                        string procReturnCode = "";
                        string errMsg = "";
                        string oid1 = "";
                        string groupId = "";
                        string transId = "";

                        string authCode = "";
                        var vDenizBank = new PaymentControlDenizBankTemp();
                        string[] paramArr = responseStr.Split(';', ';');
                        vDenizBank = paramArr.Select(p => p.Split('=')).Where(nameValue => nameValue.Length > 1).Aggregate(vDenizBank, (current, nameValue) => current.SetValue(current, nameValue[0], nameValue[1]));
                        procReturnCode = vDenizBank.ProcReturnCode;
                        errMsg = vDenizBank.ErrorMessage;
                        oid1 = vDenizBank.OrderId;
                        groupId = vDenizBank.OrderId;
                        transId = vDenizBank.TransId;
                        authCode = vDenizBank.AuthCode;
                        Logger.LogPayment(LogPaymentType.Information, "Denizbank 3D Islem Başlangıcı",
             " XML Response : " + responseStr, PosPaymentValue.PaymentId, PosPaymentValue.Bank
          );
                        EPayment.Update(PosPaymentValue.InsertId, authCode, procReturnCode, errMsg, PosPaymentValue.IpAddress, oid1, oid1, transId, "", "", "DENIZBANK", DateTime.Now, 34);

                    }
                }
                catch (Exception ex)
                {
                    //Log Atılması gerekli burada
                    Logger.LogPayment(LogPaymentType.Error, "Denizbank 3D Islem Başlangıcı",
" Error : " + ex, PosPaymentValue.PaymentId, PosPaymentValue.Bank
);
                    Response.Write("Hata " + ex.Message + " " + ex.StackTrace + (ex.InnerException != null ? ex.InnerException.Message : ""));
                }

                return CloseFancybox(PosPaymentValue.PaymentId);
            }

            else
            {
                return CloseFancybox(PosPaymentValue.PaymentId);
            }
        }
        public ActionResult Garanti3D()
        {
            return View();
        }

        public ActionResult Garanti3DP()
        {
            string strMode = Request.Form.Get("mode");
            string strApiVersion = Request.Form.Get("apiversion");
            string strTerminalProvUserID = Request.Form.Get("terminalprovuserid");
            string strType = Request.Form.Get("txntype");
            string strAmount = Request.Form.Get("txnamount");

            string strCurrencyCode = Request.Form.Get("txncurrencycode");
            string strInstallmentCount = Request.Form.Get("txninstallmentcount");
            string strTerminalUserID = Request.Form.Get("terminaluserid");
            string strOrderID = Request.Form.Get("oid");
            string strCustomeripaddress = Request.Form.Get("customeripaddress");
            string strcustomeremailaddress = Request.Form.Get("customeremailaddress");
            string strTerminalID = Request.Form.Get("clientid");
            string _strTerminalID = "0" + strTerminalID;
            string strTerminalMerchantID = Request.Form.Get("terminalmerchantid");
            string strStoreKey = PosBankDetails._3dSecureKey;
            //HASH doğrulaması için 3D Secure şifreniz
            string strProvisionPassword = PosBankDetails.Password;


            string strSuccessURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/Payment/Garanti3DP";
            string strErrorURL = Request.Url.Scheme + "://" + Request.Url.Authority + "/Payment/Error?paymentId=" + PosPaymentValue.PaymentId;



            string strCardholderPresentCode = "13";
            //3D Model işlemde bu değer 13 olmalı
            string strMotoInd = "N";

            string strAuthenticationCode = Server.UrlEncode(Request.Form.Get("cavv"));
            string strSecurityLevel = Server.UrlEncode(Request.Form.Get("eci"));
            string strTxnID = Server.UrlEncode(Request.Form.Get("xid"));
            string strMD = Server.UrlEncode(Request.Form.Get("md"));
            string strMDStatus = Request.Form.Get("mdstatus");
            string strMDStatusText = Request.Form.Get("mderrormessage");
            string strHostAddress = PosBankDetails.PosBank.XmlUrl;
            //Provizyon için xml'in post edileceği adres
            string SecurityData = GetSHA1(strProvisionPassword + _strTerminalID).ToUpper();
            string HashData = GetSHA1(strOrderID + strTerminalID + strAmount + SecurityData).ToUpper();
            //Daha kısıtlı bilgileri HASH ediyoruz.

            string totalPonus = PosPaymentValue.ExpendableBonus;
            if (totalPonus.Contains(",") || totalPonus.Contains("."))
            {
                totalPonus = totalPonus.Replace(",", "");
                totalPonus = totalPonus.Replace(".", "");
                totalPonus = totalPonus.Trim();
            }

            //Hashdata kontrolü için bankadan dönen secure3dhash değeri alınıyor.
            string strHashData = Request.Form.Get("secure3dhash");
            string ValidateHashData = GetSHA1(strTerminalID + strOrderID + strAmount + strSuccessURL + strErrorURL + strType + strInstallmentCount + strStoreKey + SecurityData).ToUpper();

            //İlk gönderilen ve bankadan dönen HASH değeri yeni üretilenle eşleşiyorsa;

            string responseFromServer = null;
            string GVPSResponse = "";
            string Message = "";
            string strCardnumber = "";
            string UserID = "";
            string ErrorMsg = "";
            string SysErrMsg = "";
            string RetrefNum = "";
            string AuthCode = "";
            string ProvDate = "";
            string OrderID = "";
            string ProcReturnCode = "";
            string ReasonCode = "";
            string campaingUrl = "";

            if (strHashData == ValidateHashData)
            {
                //lblResult1.Text = "Sayısal Imza Geçerli";

                Response.Write("Sayısal Imza Geçerli");
                //Tam Doğrulama, Kart Sahibi veya bankası sisteme kayıtlı değil, Kartın bankası sisteme kayıtlı değil
                //Doğrulama denemesi, kart sahibi sisteme daha sonra kayıt olmayı seçmiş responselarını alan
                //işlemler için Provizyon almaya çalışıyoruz

                if (strMDStatus == "1" | strMDStatus == "2" | strMDStatus == "3" | strMDStatus == "4")
                {
                    //Provizyona Post edilecek XML Şablonu
                    string strXML = null;
                    strXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                             + "<GVPSRequest>"
                             + "<Mode>" + strMode + "</Mode>" +
                             "<Version>" + strApiVersion + "</Version>" +
                             "<ChannelCode></ChannelCode>" +
                             "<Terminal><ProvUserID>" + strTerminalProvUserID +
                             "</ProvUserID><HashData>" + HashData + "</HashData>" +
                             "<UserID>" + strTerminalUserID + "</UserID>" +
                             "<ID>" + strTerminalID + "</ID>" +
                             "<MerchantID>" + strTerminalMerchantID + "</MerchantID>" +
                             "</Terminal>" + "<Customer>" +
                             "<IPAddress>" + strCustomeripaddress + "</IPAddress>" +
                             "<EmailAddress>" + strcustomeremailaddress + "</EmailAddress>" +
                             "</Customer>" + "<Card><Number>" +
                             "</Number><ExpireDate></ExpireDate><CVV2></CVV2>" +
                             "</Card>" +
                             "<Order><OrderID>" + strOrderID + "</OrderID>" +
                             "<GroupID></GroupID>" +
                             "<AddressList><Address>" +
                             "<Type>B</Type>" +
                             "<Name></Name><LastName>" +
                             "</LastName><Company>" +
                             "</Company><Text>" +
                             "</Text><District>" +
                             "</District><City></City>" +
                             "<PostalCode></PostalCode>" +
                             "<Country></Country>" +
                             "<PhoneNumber>" +
                             "</PhoneNumber>" +
                             "</Address>" +
                             "</AddressList>" +
                             "</Order>" + "" +
                             "<Transaction>" +
                             "<Type>" + strType + "</Type>" +
                             "<InstallmentCnt>" + strInstallmentCount + "</InstallmentCnt>" +
                             "<Amount>" + strAmount + "</Amount>" +
                             "<CurrencyCode>" + strCurrencyCode + "</CurrencyCode>" +
                             "<CardholderPresentCode>" + strCardholderPresentCode + "</CardholderPresentCode><" +
                             "MotoInd>" + strMotoInd + "</MotoInd>";
                    if (PosPaymentValue.UseBonus)
                    {
                        strXML += "<RewardList>" +
                                      "<Reward>" +
                                          "<Type>BNS</Type>" +
                                          "<UsedAmount>" + totalPonus + "</UsedAmount>" +
                                      "</Reward>" +
                                  "</RewardList>";


                    }
                    strXML += "<Secure3D>" +
                            "<AuthenticationCode>" + strAuthenticationCode + "</AuthenticationCode>" +
                            "<SecurityLevel>" + strSecurityLevel + "</SecurityLevel>" +
                            "<TxnID>" + strTxnID + "</TxnID>" +
                            "<Md>" + strMD + "</Md>" +
                            "</Secure3D>" +
                        "</Transaction>" +
                        "</GVPSRequest>";

                    try
                    {
                        string data = "data=" + strXML;

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        WebRequest _WebRequest = WebRequest.Create(strHostAddress);
                        _WebRequest.Method = "POST";

                        byte[] byteArray = Encoding.UTF8.GetBytes(data);
                        _WebRequest.ContentType = "application/x-www-form-urlencoded";
                        _WebRequest.ContentLength = byteArray.Length;

                        Stream dataStream = _WebRequest.GetRequestStream();
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();

                        WebResponse _WebResponse = _WebRequest.GetResponse();
                        Console.WriteLine(((HttpWebResponse)_WebResponse).StatusDescription);
                        dataStream = _WebResponse.GetResponseStream();

                        StreamReader reader = new StreamReader(dataStream);
                        responseFromServer = reader.ReadToEnd();

                        Console.WriteLine(responseFromServer);

                        XmlDocument gelen = new XmlDocument();
                        gelen.LoadXml(responseFromServer);
                        XmlNodeList list = gelen.GetElementsByTagName("GVPSResponse");
                        GVPSResponse = list[0].InnerText;

                        list = gelen.GetElementsByTagName("Message");
                        Message = list[0].InnerText;

                        list = gelen.GetElementsByTagName("CardNumberMasked");
                        strCardnumber = list[0].InnerText;

                        list = gelen.GetElementsByTagName("UserID");
                        UserID = list[0].InnerText;

                        list = gelen.GetElementsByTagName("ErrorMsg");
                        ErrorMsg = list[0].InnerText;
                        Session["ErrMsg"] = ErrorMsg.ToString();

                        list = gelen.GetElementsByTagName("SysErrMsg");
                        SysErrMsg = list[0].InnerText;

                        list = gelen.GetElementsByTagName("RetrefNum");
                        RetrefNum = list[0].InnerText;

                        list = gelen.GetElementsByTagName("AuthCode");
                        AuthCode = list[0].InnerText;
                        list = gelen.GetElementsByTagName("ProvDate");
                        ProvDate = list[0].InnerText;

                        list = gelen.GetElementsByTagName("OrderID");
                        OrderID = list[0].InnerText;

                        list = gelen.GetElementsByTagName("Code");
                        ProcReturnCode = list[0].InnerText;

                        list = gelen.GetElementsByTagName("ReasonCode");
                        ReasonCode = list[0].InnerText;
                        EPaymentLog log_return = new EPaymentLog(PosPaymentValue, strXML, strXML, AuthCode, ProcReturnCode, ErrorMsg, OrderID, 97);//97 işlemin bankadan geldiğini gösterir
                        EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");
                        Logger.LogPayment(LogPaymentType.Information, "Garnti 3D Islem Sonucu",
                 " XML Response : " + strXML, PosPaymentValue.PaymentId, PosPaymentValue.Bank
              );
                        //00 ReasonCode döndüğünde işlem başarılıdır. Müşteriye başarılı veya başarısız şeklinde göstermeniz tavsiye edilir. (Fraud riski)
                        if (responseFromServer.Contains("<ReasonCode>00</ReasonCode>"))
                        {

                            list = gelen.GetElementsByTagName("CampaignChooseLink");
                            campaingUrl = list[0].InnerText;

                            campaingUrl = campaingUrl.Replace(" ", "");
                            if (!string.IsNullOrEmpty(campaingUrl))
                            {
                                PosPaymentValue.CampaingUrl = campaingUrl;
                            }

                            EPayment.Update(PosPaymentValue.InsertId, AuthCode, ProcReturnCode, (SysErrMsg + ErrorMsg), PosPaymentValue.IpAddress, PosPaymentValue.PaymentId, PosPaymentValue.PaymentId, "", OrderID, Message.ToString(), Bank.GARANTIBANK.ToString(), DateTime.Now, 33, campaingUrl);

                            return CloseFancybox(PosPaymentValue.PaymentId);
                        }
                        else
                        {
                            EPayment.Update(PosPaymentValue.InsertId, AuthCode, ProcReturnCode, (SysErrMsg + ErrorMsg), PosPaymentValue.IpAddress, PosPaymentValue.PaymentId, PosPaymentValue.PaymentId, "", OrderID, Message.ToString(), Bank.GARANTIBANK.ToString(), DateTime.Now, 33, campaingUrl);

                            return CloseFancybox(PosPaymentValue.PaymentId);
                        }
                    }
                    catch (Exception)
                    {
                        EPayment.Update(PosPaymentValue.InsertId, AuthCode, ProcReturnCode, (SysErrMsg + ErrorMsg), PosPaymentValue.IpAddress, PosPaymentValue.PaymentId, PosPaymentValue.PaymentId, "", OrderID, Message.ToString(), Bank.GARANTIBANK.ToString(), DateTime.Now, 33, campaingUrl);

                        return CloseFancybox(PosPaymentValue.PaymentId);
                    }
                }
                else
                {
                    EPayment.Update(PosPaymentValue.InsertId, AuthCode, ProcReturnCode, (SysErrMsg + ErrorMsg), PosPaymentValue.IpAddress, PosPaymentValue.PaymentId, PosPaymentValue.PaymentId, "", OrderID, Message.ToString(), Bank.GARANTIBANK.ToString(), DateTime.Now, 33, campaingUrl);

                    return CloseFancybox(PosPaymentValue.PaymentId);
                }
            }
            else
            {
                EPayment.Update(PosPaymentValue.InsertId, AuthCode, ProcReturnCode, (SysErrMsg + ErrorMsg), PosPaymentValue.IpAddress, PosPaymentValue.PaymentId, PosPaymentValue.PaymentId, "", OrderID, Message.ToString(), Bank.GARANTIBANK.ToString(), DateTime.Now, 33, campaingUrl);
                return CloseFancybox(PosPaymentValue.PaymentId);
            }



        }

        public ActionResult Innova3D()
        {



            return PartialView();
        }
        protected Dictionary<String, String> innovaXmlParser(string xmlString)
        {

            //Response.Write(xmlString);
            XmlDocument doc = new XmlDocument();
            doc.Load(new StringReader(xmlString));


            //Status Bilgisi okunuyor
            XmlNode StatusNode = doc.GetElementsByTagName("Status").Item(0);
            String Status = "";
            if (StatusNode != null)
                Status = StatusNode.InnerText;

            //PAReq Bilgisi okunuyor
            XmlNode PAReqNode = doc.GetElementsByTagName("PAReq").Item(0);
            String PaReq = "";
            if (PAReqNode != null)
                PaReq = PAReqNode.InnerText;

            //ACSUrl Bilgisi okunuyor
            XmlNode ACSUrlNode = doc.GetElementsByTagName("ACSUrl").Item(0);
            String ACSUrl = "";
            if (ACSUrlNode != null)
                ACSUrl = ACSUrlNode.InnerText;

            //Term Url Bilgisi okunuyor
            XmlNode TermUrlNode = doc.GetElementsByTagName("TermUrl").Item(0);
            String TermUrl = "";
            if (TermUrlNode != null)
                TermUrl = TermUrlNode.InnerText;

            //MD Bilgisi okunuyor
            XmlNode MDNode = doc.GetElementsByTagName("MD").Item(0);
            String MD = "";
            if (MDNode != null)
                MD = MDNode.InnerText;

            // Sonuç dizisi olusturuluyor
            Dictionary<String, String> dic = new Dictionary<string, string>();
            dic.Add("Status", Status);
            dic.Add("PaReq", PaReq);
            dic.Add("ACSUrl", ACSUrl);
            dic.Add("TermUrl", TermUrl);
            dic.Add("MerchantData", MD);
            Logger.LogPayment(LogPaymentType.Information, "İşbank Islem Giden",
                          " XML REquest : " + doc.InnerXml.ToString(), PosPaymentValue.PaymentId,
                        "ISBANK");
            return dic;
        }

        public ActionResult Innova3DP()
        {
            String postURL = PosBankDetails.PosBank.XmlUrl;
            String MerchantID = Request.Form["MerchantID"];
            String XID = Request.Form["XID"];
            String PAN = PosPaymentValue.CardNumber;//Request.Form["Pan"];
            String Expiry = "20" + Request.Form["Expiry"];
            String brand_name = Request.Form["brand_name"];
            String PurchAmount = Request.Form["PurchAmount"];
            double tutar = Double.Parse(PurchAmount == null ? "0" : PurchAmount);
            tutar = tutar / 100.0;
            PurchAmount = tutar.ToString("0.00").Replace(",", ".");
            String PurchCurrency = Request.Form["PurchCurrency"];
            String CVV2 = PosPaymentValue.Cvc;//Request.Form["CVV2"];
            String numberOfInstallment = PosPaymentValue.Installment.ToString();// Request.Form["NumberOfInstallment"];
            String CAVV = Request.Form["CAVV"];
            String ECI = Request.Form["ECI"];
            string MerchantId = PosBankDetails.StoreNumber;
            string Password = PosBankDetails.Password;
            String PosXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                      "<VposRequest>" +
                      "<MerchantId>" + MerchantId + "</MerchantId>" +
                      "<Password>" + Password + "</Password>" +
                      "<BankId>1</BankId>" +
                      "<TransactionType>Sale</TransactionType>" +
                      "<TransactionId>" + XID + "</TransactionId>" +
                      "<CurrencyAmount>" + PurchAmount + "</CurrencyAmount>" +
                      "<CurrencyCode>" + PurchCurrency + "</CurrencyCode>" +
                      "<Pan>" + PAN + "</Pan>" +
                      "<Cvv>" + CVV2 + "</Cvv>" +
                      "<Expiry>" + Expiry + "</Expiry>" +
                      "<Eci>" + ECI + "</Eci>" +
                      "<Cavv>" + CAVV + "</Cavv>" +
                      "<InstallmentCount>" + numberOfInstallment + "</InstallmentCount>" +
                      "</VposRequest>";
            String response = GetResponseText(postURL, "prmstr=" + PosXML);

            XmlDocument xmlDoc;
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            string ResultCode = "";
            string TransactionId = "";
            string ResultDetail = "";
            string AuthCode = "";
            string HostDate = "";
            string Rrn = "";

            Bank pUserPayment;
            pUserPayment = Bank.ISBANK;

            XmlNodeList nodeResultCode = xmlDoc.GetElementsByTagName("ResultCode");
            XmlNodeList nodeTransactionId = xmlDoc.GetElementsByTagName("TransactionId");
            XmlNodeList nodeResultDetail = xmlDoc.GetElementsByTagName("ResultDetail");
            XmlNodeList nodeAuthCode = xmlDoc.GetElementsByTagName("AuthCode");
            XmlNodeList nodeHostDate = xmlDoc.GetElementsByTagName("HostDate");
            XmlNodeList nodeRrn = xmlDoc.GetElementsByTagName("Rrn");


            if (nodeResultCode.Count > 0)
                ResultCode = nodeResultCode[0].InnerText;

            if (nodeResultDetail.Count > 0)
                ResultDetail = nodeResultDetail[0].InnerText;
            if (nodeAuthCode.Count > 0)
                AuthCode = nodeAuthCode[0].InnerText;
            if (nodeHostDate.Count > 0)
                HostDate = nodeHostDate[0].InnerText;
            if (nodeRrn.Count > 0)
                Rrn = nodeRrn[0].InnerText;
            if (nodeTransactionId.Count > 0)
                TransactionId = nodeTransactionId[0].InnerText;
            Logger.LogPayment(LogPaymentType.Information, "Isbank Islem Sonucu",
                          " XML Response : " + xmlDoc.InnerXml.ToString(), PosPaymentValue.PaymentId,
                        "ISBANK");
            EPaymentLog log_return = new EPaymentLog(PosPaymentValue, "", xmlDoc.InnerXml, xmlDoc.InnerXml, AuthCode, ResultCode + ResultDetail, TransactionId, 97);//97 işlemin bankadan geldiğini gösterir
            EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");
            if (ResultCode == "0000")
            {
                EPayment.Update(PosPaymentValue.InsertId, AuthCode, "00", String.Empty, PosPaymentValue.IpAddress, String.Empty, String.Empty, String.Empty, TransactionId, "Giden XML : " + PosXML.ToString() + "Gelen Sonuc=" + response.ToString(), pUserPayment.ToString(), DateTime.Now, 21);
                return CloseFancybox(PosPaymentValue.PaymentId);
            }
            else
            {
                EPayment.Update(PosPaymentValue.InsertId, AuthCode, ResultCode, ResultDetail, PosPaymentValue.IpAddress, String.Empty, String.Empty, String.Empty, TransactionId, "Giden XML : " + PosXML.ToString() + "Gelen Sonuc=" + response.ToString(), pUserPayment.ToString(), DateTime.Now, 21);
                return CloseFancybox(PosPaymentValue.PaymentId);
            }
            return CloseFancybox(PosPaymentValue.PaymentId);
        }

        public ActionResult VakifBank3D()
        {

            return PartialView();
        }

        public ActionResult VakifBank3DP()
        {
            string pan = PosPaymentValue.CardNumber.Replace(" ", "");
            string expiryDate = "20" + PosPaymentValue.ExpYear.Replace(" ", "") + PosPaymentValue.ExpMounth.Replace(" ", "");
            string amount = "";

            if (PosPaymentValue.UseBonus)//Bonus Kullalanıyor ise normal işlemden bonus tuturını çıkarak gönderiyorum
                amount = (PosPaymentValue.TotalPrice - Convert.ToDouble(PosPaymentValue.ExpendableBonus.Replace(".", ","))).ToString().Replace(",", ".");
            else
                amount = PosPaymentValue.TotalPrice.ToString().Replace(",", ".");




            string TransactionId = Request.Form["TransactionId"];//CurrentPaymentValue.PaymentId;
            string merchantId = PosBankDetails.StoreNumber;
            string password = PosBankDetails.Password;
            string eci = Request.Form["Eci"];
            string cavv = Request.Form["Cavv"];
            string mpiTransactionId = Request.Form["VerifyEnrollmentRequestId"];
            string resultCode = "";
            string resultDescription = "";
            string authCode = "";
            string terminal = PosBankDetails.TerminalNo;
            string cvv = PosPaymentValue.Cvc;
            string installment = PosPaymentValue.Installment.ToString();
            string pointTransactionId = Guid.NewGuid().ToString("N");

            if (PosPaymentValue.UseBonus)
            {
                string amountPoint = PosPaymentValue.ExpendableBonus.ToString().Replace(",", ".");

                string xmlMessagePoint = GenerateVakifbankPaymentXmlDocument(amountPoint, TransactionType.PointSale, pointTransactionId);
                string responseFromServerPoint = VakıfbankNonSecureWebRequest(xmlMessagePoint);

                if (string.IsNullOrEmpty(responseFromServerPoint))
                {
                    return CloseFancybox(PosPaymentValue.PaymentId);
                }
                var xmlResponsePoint = new XmlDocument();
                xmlResponsePoint.LoadXml(responseFromServerPoint);
                var resultCodeNodePoint = xmlResponsePoint.SelectSingleNode("VposResponse/ResultCode");
                var resultDescriptionNodePoint = xmlResponsePoint.SelectSingleNode("VposResponse/ResultDetail");
                var authCodeNodePoint = xmlResponsePoint.SelectSingleNode("VposResponse/AuthCode");
                var pointAmountNodePoint = xmlResponsePoint.SelectSingleNode("VposResponse/PointAmount");
                var totalPointNodePoint = xmlResponsePoint.SelectSingleNode("VposResponse/TotalPoint");
                string resultCodePoint = "";
                string resultDescriptionPoint = "";
                string authCodePoint = "";
                string authCopointAmountPoint = "";
                string totalPoint = "";

                try
                {
                    authCodePoint = authCodeNodePoint.InnerText;
                }
                catch (Exception)
                {
                    authCodePoint = "";
                }

                if (resultCodeNodePoint != null)
                {
                    resultCodePoint = resultCodeNodePoint.InnerText;
                }
                if (resultDescriptionNodePoint != null)
                {
                    resultDescriptionPoint = resultDescriptionNodePoint.InnerText;
                }

                if (pointAmountNodePoint != null)
                {
                    authCopointAmountPoint = pointAmountNodePoint.InnerText;
                }
                if (totalPointNodePoint != null)
                {
                    totalPoint = totalPointNodePoint.InnerText;
                }


                Logger.LogPayment(LogPaymentType.Information, "VAkıfBank Puan Kullanma Islem Sonucu",
                    " XML Response : " + xmlResponsePoint.InnerXml, PosPaymentValue.PaymentId, "Vakıfbank");
                string paymentId = Guid.NewGuid().ToString();
                string pMaskedCardNumber = PosPaymentValue.CardNumber.Substring(0, 6) + "******" +
                                           PosPaymentValue.CardNumber.Substring(PosPaymentValue.CardNumber.Length - 4, 4);

                DataTable dtInsert = EPayment.Insert(pMaskedCardNumber, PosPaymentValue.NameSurname,
                    PosPaymentValue.ExpMounth, PosPaymentValue.ExpYear, PosPaymentValue.Cvc,
                    PosPaymentValue.TotalPrice + " TL", PosPaymentValue.Installment.ToString(), CurrentCustomer.Id,
                    CurrentCustomer.Code, CurrentCustomer.Name, PosPaymentValue.Bank, PosPaymentValue.TotalPrice,
                    PosPaymentValue.CommissionRate.ToString(), PosPaymentValue.BankId, paymentId, (int)SystemType.Web,
                    PosPaymentValue.PhoneNumber, PosPaymentValue.Explanation, PosPaymentValue.CardType.ToString(),
                    CurrentSalesmanId, PosPaymentValue.ExtraInstallment.ToString(), PosPaymentValue.TotalBonus,
                    PosPaymentValue.ExpendableBonus, PosPaymentValue.UseBonus, PosPaymentValue._3DSecure,
                    PosPaymentValue.InsertId);
                PosPaymentValue.InsertPointId = Convert.ToInt32(dtInsert.Rows[0][0].ToString());
                if (resultCodePoint != "0000")
                {
                    string sonuc = "işlem Başarısız " + resultDescriptionPoint;
                    EPayment.Update(PosPaymentValue.InsertPointId, " ", resultCodePoint, sonuc,
                        PosPaymentValue.IpAddress,
                        string.Empty, string.Empty, paymentId, string.Empty, xmlResponsePoint.InnerXml,
                        PosBankDetails.PosBankName, DateTime.Now, 26);
                    sonuc += "işlem Başarısız Puan Kullanımına Onay Verilmedi Puan kullanmadan işlem deneyiniz!";
                    EPayment.Update(PosPaymentValue.InsertId, " ", resultCodePoint, sonuc,
                        PosPaymentValue.IpAddress,
                        string.Empty, string.Empty, paymentId, string.Empty, xmlResponsePoint.InnerXml,
                        PosBankDetails.PosBankName, DateTime.Now, 26);
                }
                else
                {
                    EPayment.Update(PosPaymentValue.InsertPointId, authCodePoint, "00", resultCodePoint, PosPaymentValue.IpAddress,
                        string.Empty, string.Empty, paymentId, string.Empty, xmlResponsePoint.InnerXml,
                        PosBankDetails.PosBankName, DateTime.Now, 26);

                    if ((PosPaymentValue.TotalPrice - Convert.ToDouble(PosPaymentValue.ExpendableBonus.Replace(".", ","))) > 0)
                    {
                        #region Puan Kullanımından Sonra Kalan Ekstra 
                        XmlDocument xmlDoc = new XmlDocument();

                        XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);

                        XmlElement rootNode = xmlDoc.CreateElement("VposRequest");
                        xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
                        xmlDoc.AppendChild(rootNode);
                        if (installment != "1")
                        {
                            XmlElement NumberOfInstallmentsNode = xmlDoc.CreateElement("NumberOfInstallments");
                            XmlText NumberOfInstallmentsText = xmlDoc.CreateTextNode(installment);
                            rootNode.AppendChild(NumberOfInstallmentsNode);
                            NumberOfInstallmentsNode.AppendChild(NumberOfInstallmentsText);
                        }
                        XmlElement merchantNode = xmlDoc.CreateElement("MerchantId");
                        XmlElement passwordNode = xmlDoc.CreateElement("Password");
                        XmlElement terminalNode = xmlDoc.CreateElement("TerminalNo");
                        XmlElement transactionTypeNode = xmlDoc.CreateElement("TransactionType");
                        XmlElement transactionIdNode = xmlDoc.CreateElement("TransactionId");
                        XmlElement currencyAmountNode = xmlDoc.CreateElement("CurrencyAmount");
                        XmlElement currencyCodeNode = xmlDoc.CreateElement("CurrencyCode");
                        XmlElement panNode = xmlDoc.CreateElement("Pan");
                        XmlElement cvvNode = xmlDoc.CreateElement("Cvv");
                        XmlElement expiryNode = xmlDoc.CreateElement("Expiry");
                        XmlElement ClientIpNode = xmlDoc.CreateElement("ClientIp");
                        XmlElement transactionDeviceSourceNode = xmlDoc.CreateElement("TransactionDeviceSource");
                        XmlElement eciNode = xmlDoc.CreateElement("ECI");
                        XmlElement cavvNode = xmlDoc.CreateElement("CAVV");
                        XmlElement mpiTransactionIdNode = xmlDoc.CreateElement("MpiTransactionId");
                        // XmlElement ThreeDSecureTypeNode = xmlDoc.CreateElement("ThreeDSecureType");

                        XmlText merchantText = xmlDoc.CreateTextNode(merchantId);
                        XmlText passwordtext = xmlDoc.CreateTextNode(password);
                        XmlText terminalNoText = xmlDoc.CreateTextNode(terminal);
                        XmlText transactionTypeText = xmlDoc.CreateTextNode("Sale");
                        XmlText transactionIdText = xmlDoc.CreateTextNode(TransactionId); //uniqe olacak şekilde düzenleyebilirsiniz.
                        XmlText currencyAmountText = xmlDoc.CreateTextNode(amount); //tutarı nokta ile gönderdiğinizden emin olunuz.
                        XmlText currencyCodeText = xmlDoc.CreateTextNode("949");
                        XmlText panText = xmlDoc.CreateTextNode(pan);
                        XmlText cvvText = xmlDoc.CreateTextNode(cvv);
                        XmlText expiryText = xmlDoc.CreateTextNode(expiryDate);
                        XmlText ClientIpText = xmlDoc.CreateTextNode("190.20.13.12");
                        XmlText transactionDeviceSourceText = xmlDoc.CreateTextNode("0");
                        XmlText eciText = xmlDoc.CreateTextNode(eci);
                        XmlText cavvText = xmlDoc.CreateTextNode(cavv);
                        XmlText mpiTransactionIdText = xmlDoc.CreateTextNode(mpiTransactionId);
                        // XmlText ThreeDSecureTypeText = xmlDoc.CreateTextNode("2");


                        rootNode.AppendChild(merchantNode);
                        rootNode.AppendChild(passwordNode);
                        rootNode.AppendChild(terminalNode);
                        rootNode.AppendChild(transactionTypeNode);
                        rootNode.AppendChild(transactionIdNode);
                        rootNode.AppendChild(currencyAmountNode);
                        rootNode.AppendChild(currencyCodeNode);
                        rootNode.AppendChild(panNode);
                        rootNode.AppendChild(cvvNode);
                        rootNode.AppendChild(expiryNode);
                        rootNode.AppendChild(ClientIpNode);
                        rootNode.AppendChild(transactionDeviceSourceNode);
                        rootNode.AppendChild(eciNode);
                        rootNode.AppendChild(cavvNode);
                        rootNode.AppendChild(mpiTransactionIdNode);
                        //   rootNode.AppendChild(ThreeDSecureTypeNode);


                        merchantNode.AppendChild(merchantText);
                        passwordNode.AppendChild(passwordtext);
                        terminalNode.AppendChild(terminalNoText);
                        transactionTypeNode.AppendChild(transactionTypeText);
                        transactionIdNode.AppendChild(transactionIdText);
                        currencyAmountNode.AppendChild(currencyAmountText);
                        currencyCodeNode.AppendChild(currencyCodeText);
                        panNode.AppendChild(panText);
                        cvvNode.AppendChild(cvvText);
                        expiryNode.AppendChild(expiryText);
                        ClientIpNode.AppendChild(ClientIpText);
                        transactionDeviceSourceNode.AppendChild(transactionDeviceSourceText);
                        eciNode.AppendChild(eciText);
                        cavvNode.AppendChild(cavvText);
                        mpiTransactionIdNode.AppendChild(mpiTransactionIdText);
                        //  ThreeDSecureTypeNode.AppendChild(ThreeDSecureTypeText);

                        string xmlMessage = xmlDoc.OuterXml;
                        string responseFromServer = VakıfbankNonSecureWebRequest(xmlMessage);

                        if (string.IsNullOrEmpty(responseFromServer))
                        {
                            return Error(PosPaymentValue.PaymentId);
                        }
                        var xmlResponse = new XmlDocument();
                        xmlResponse.LoadXml(responseFromServer);
                        var resultCodeNode = xmlResponse.SelectSingleNode("VposResponse/ResultCode");
                        var authCodeNode = xmlResponse.SelectSingleNode("VposResponse/AuthCode");
                        var resultDescriptionNode = xmlResponse.SelectSingleNode("VposResponse/ResultDescription");

                        Logger.LogPayment(LogPaymentType.Information, "Vakıfbank Islem Sonucu",
                                " XML Response : " + xmlResponse.InnerXml, PosPaymentValue.PaymentId,
                              "VAKIFBANK");
                        authCode = authCodeNode.InnerText;
                        if (resultCodeNode != null)
                        {
                            resultCode = resultCodeNode.InnerText;
                        }
                        if (resultDescriptionNode != null)
                        {
                            resultDescription = resultDescriptionNode.InnerText;
                        }

                        EPaymentLog log_return = new EPaymentLog(PosPaymentValue, xmlResponse.InnerXml, xmlResponse.InnerXml, authCode, resultCode, resultDescription, TransactionId, 97);//97 işlemin bankadan geldiğini gösterir
                        EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");

                        if (resultCode != "0000")
                        {
                            string sonuc = "işlem Başarısız " + resultDescription;
                            EPayment.Update(PosPaymentValue.InsertId, " ", resultCode, sonuc, PosPaymentValue.IpAddress,
                                string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty,
                                xmlResponse.InnerXml,
                                PosBankDetails.PosBankName, DateTime.Now, 26);


                            string xmlCancelMessage = "";
                            xmlCancelMessage += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                            xmlCancelMessage += "<VposRequest>";
                            xmlCancelMessage += "<MerchantId>" + merchantId + "</MerchantId>";
                            xmlCancelMessage += "<Password>" + password + "</Password>";
                            xmlCancelMessage += "<TransactionType>Cancel</TransactionType>";
                            xmlCancelMessage +=
                                "<ReferenceTransactionId>" + pointTransactionId + "</ReferenceTransactionId>";
                            xmlCancelMessage += "<ClientIp>190.20.13.12</ClientIp>";
                            xmlCancelMessage += "</VposRequest>";

                            string responseFromServerCancel = VakıfbankNonSecureWebRequest(xmlCancelMessage);

                            var xmlResponseCancel = new XmlDocument();
                            xmlResponseCancel.LoadXml(responseFromServerCancel);
                            var resultCodeNodeCancel = xmlResponseCancel.SelectSingleNode("VposResponse/ResultCode");
                            var resultDescriptionNodeCancel =
                                xmlResponseCancel.SelectSingleNode("VposResponse/ResultDetail");
                            var authCodeNodeCancel = xmlResponseCancel.SelectSingleNode("VposResponse/AuthCode");

                            try
                            {
                                authCode = authCodeNodeCancel.InnerText;
                            }
                            catch (Exception)
                            {
                                authCode = "";
                            }

                            if (resultCodeNodeCancel != null)
                            {
                                resultCode = resultCodeNodeCancel.InnerText;
                            }
                            if (resultDescriptionNodeCancel != null)
                            {
                                resultDescription = resultDescriptionNodeCancel.InnerText;
                            }
                            paymentId = Guid.NewGuid().ToString();
                            DataTable dtInsertCancel = EPayment.Insert(pMaskedCardNumber, PosPaymentValue.NameSurname,
                                PosPaymentValue.ExpMounth, PosPaymentValue.ExpYear, PosPaymentValue.Cvc,
                                PosPaymentValue.TotalPrice + " TL", PosPaymentValue.Installment.ToString(),
                                CurrentCustomer.Id,
                                CurrentCustomer.Code, CurrentCustomer.Name, PosPaymentValue.Bank,
                                PosPaymentValue.TotalPrice,
                                PosPaymentValue.CommissionRate.ToString(), PosPaymentValue.BankId, paymentId,
                                (int)SystemType.Web,
                                PosPaymentValue.PhoneNumber, PosPaymentValue.Explanation,
                                PosPaymentValue.CardType.ToString(),
                                CurrentSalesmanId, PosPaymentValue.ExtraInstallment.ToString(),
                                PosPaymentValue.TotalBonus,
                                PosPaymentValue.ExpendableBonus, PosPaymentValue.UseBonus, PosPaymentValue._3DSecure,
                                PosPaymentValue.InsertId);
                            PosPaymentValue.InsertCancelId = Convert.ToInt32(dtInsertCancel.Rows[0][0]);
                            if (resultCode != "0000")
                            {
                                //iptal işlemi başarısız
                                //0707 Manuel İpral Edilmesi Gereken işlem
                                sonuc = "işlem Başarısız " + resultDescription +
                                        " Manuel İptal Gerekli Firma Yetkilisi ile Görüşünüz.";
                                EPayment.Update(PosPaymentValue.InsertCancelId, " ", "0707", sonuc,
                                    PosPaymentValue.IpAddress,
                                    string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty,
                                    xmlResponseCancel.InnerXml,
                                    PosBankDetails.PosBankName, DateTime.Now, 26);
                            }
                            else
                            {
                                EPayment.Update(PosPaymentValue.InsertCancelId, authCode, "00", resultCode,
                                    PosPaymentValue.IpAddress,
                                    string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty,
                                    xmlResponseCancel.InnerXml,
                                    PosBankDetails.PosBankName, DateTime.Now, 26);

                            }
                        }

                        else
                        {
                            EPayment.Update(PosPaymentValue.InsertId, authCode, "00", resultCode,
                                PosPaymentValue.IpAddress,
                                string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty,
                                xmlResponse.InnerXml,
                                PosBankDetails.PosBankName, DateTime.Now, 26);

                        }

                        #endregion
                    }
                    else
                    {
                        EPayment.Update(PosPaymentValue.InsertId, authCode, "00", resultCode, PosPaymentValue.IpAddress,
                          string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty, "",
                          PosBankDetails.PosBankName, DateTime.Now, 26);
                    }
                }
            }
            else
            {



                try
                {
                    XmlDocument xmlDoc = new XmlDocument();

                    XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);

                    XmlElement rootNode = xmlDoc.CreateElement("VposRequest");
                    xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
                    xmlDoc.AppendChild(rootNode);
                    if (installment != "1")
                    {
                        XmlElement NumberOfInstallmentsNode = xmlDoc.CreateElement("NumberOfInstallments");
                        XmlText NumberOfInstallmentsText = xmlDoc.CreateTextNode(installment);
                        rootNode.AppendChild(NumberOfInstallmentsNode);
                        NumberOfInstallmentsNode.AppendChild(NumberOfInstallmentsText);
                    }
                    XmlElement merchantNode = xmlDoc.CreateElement("MerchantId");
                    XmlElement passwordNode = xmlDoc.CreateElement("Password");
                    XmlElement terminalNode = xmlDoc.CreateElement("TerminalNo");
                    XmlElement transactionTypeNode = xmlDoc.CreateElement("TransactionType");
                    XmlElement transactionIdNode = xmlDoc.CreateElement("TransactionId");
                    XmlElement currencyAmountNode = xmlDoc.CreateElement("CurrencyAmount");
                    XmlElement currencyCodeNode = xmlDoc.CreateElement("CurrencyCode");
                    XmlElement panNode = xmlDoc.CreateElement("Pan");
                    XmlElement cvvNode = xmlDoc.CreateElement("Cvv");
                    XmlElement expiryNode = xmlDoc.CreateElement("Expiry");
                    XmlElement ClientIpNode = xmlDoc.CreateElement("ClientIp");
                    XmlElement transactionDeviceSourceNode = xmlDoc.CreateElement("TransactionDeviceSource");
                    XmlElement eciNode = xmlDoc.CreateElement("ECI");
                    XmlElement cavvNode = xmlDoc.CreateElement("CAVV");
                    XmlElement mpiTransactionIdNode = xmlDoc.CreateElement("MpiTransactionId");
                    // XmlElement ThreeDSecureTypeNode = xmlDoc.CreateElement("ThreeDSecureType");

                    XmlText merchantText = xmlDoc.CreateTextNode(merchantId);
                    XmlText passwordtext = xmlDoc.CreateTextNode(password);
                    XmlText terminalNoText = xmlDoc.CreateTextNode(terminal);
                    XmlText transactionTypeText = xmlDoc.CreateTextNode("Sale");
                    XmlText transactionIdText = xmlDoc.CreateTextNode(TransactionId); //uniqe olacak şekilde düzenleyebilirsiniz.
                    XmlText currencyAmountText = xmlDoc.CreateTextNode(amount); //tutarı nokta ile gönderdiğinizden emin olunuz.
                    XmlText currencyCodeText = xmlDoc.CreateTextNode("949");
                    XmlText panText = xmlDoc.CreateTextNode(pan);
                    XmlText cvvText = xmlDoc.CreateTextNode(cvv);
                    XmlText expiryText = xmlDoc.CreateTextNode(expiryDate);
                    XmlText ClientIpText = xmlDoc.CreateTextNode("190.20.13.12");
                    XmlText transactionDeviceSourceText = xmlDoc.CreateTextNode("0");
                    XmlText eciText = xmlDoc.CreateTextNode(eci);
                    XmlText cavvText = xmlDoc.CreateTextNode(cavv);
                    XmlText mpiTransactionIdText = xmlDoc.CreateTextNode(mpiTransactionId);
                    // XmlText ThreeDSecureTypeText = xmlDoc.CreateTextNode("2");


                    rootNode.AppendChild(merchantNode);
                    rootNode.AppendChild(passwordNode);
                    rootNode.AppendChild(terminalNode);
                    rootNode.AppendChild(transactionTypeNode);
                    rootNode.AppendChild(transactionIdNode);
                    rootNode.AppendChild(currencyAmountNode);
                    rootNode.AppendChild(currencyCodeNode);
                    rootNode.AppendChild(panNode);
                    rootNode.AppendChild(cvvNode);
                    rootNode.AppendChild(expiryNode);
                    rootNode.AppendChild(ClientIpNode);
                    rootNode.AppendChild(transactionDeviceSourceNode);
                    rootNode.AppendChild(eciNode);
                    rootNode.AppendChild(cavvNode);
                    rootNode.AppendChild(mpiTransactionIdNode);
                    //   rootNode.AppendChild(ThreeDSecureTypeNode);


                    merchantNode.AppendChild(merchantText);
                    passwordNode.AppendChild(passwordtext);
                    terminalNode.AppendChild(terminalNoText);
                    transactionTypeNode.AppendChild(transactionTypeText);
                    transactionIdNode.AppendChild(transactionIdText);
                    currencyAmountNode.AppendChild(currencyAmountText);
                    currencyCodeNode.AppendChild(currencyCodeText);
                    panNode.AppendChild(panText);
                    cvvNode.AppendChild(cvvText);
                    expiryNode.AppendChild(expiryText);
                    ClientIpNode.AppendChild(ClientIpText);
                    transactionDeviceSourceNode.AppendChild(transactionDeviceSourceText);
                    eciNode.AppendChild(eciText);
                    cavvNode.AppendChild(cavvText);
                    mpiTransactionIdNode.AppendChild(mpiTransactionIdText);
                    //  ThreeDSecureTypeNode.AppendChild(ThreeDSecureTypeText);

                    string xmlMessage = xmlDoc.OuterXml;
                    string responseFromServer = VakıfbankNonSecureWebRequest(xmlMessage);


                    if (string.IsNullOrEmpty(responseFromServer))
                    {
                        return Error(PosPaymentValue.PaymentId);
                    }
                    var xmlResponse = new XmlDocument();
                    xmlResponse.LoadXml(responseFromServer);
                    var resultCodeNode = xmlResponse.SelectSingleNode("VposResponse/ResultCode");
                    var authCodeNode = xmlResponse.SelectSingleNode("VposResponse/AuthCode");
                    var resultDescriptionNode = xmlResponse.SelectSingleNode("VposResponse/ResultDescription");

                    Logger.LogPayment(LogPaymentType.Information, "Vakıfbank Islem Sonucu",
                            " XML Response : " + xmlResponse.InnerXml, PosPaymentValue.PaymentId,
                          "VAKIFBANK");
                    if (authCodeNode != null)
                    {
                        authCode = authCodeNode.InnerText;
                    }
                    if (resultCodeNode != null)
                    {
                        resultCode = resultCodeNode.InnerText;
                    }
                    if (resultDescriptionNode != null)
                    {
                        resultDescription = resultDescriptionNode.InnerText;
                    }

                    EPaymentLog log_return = new EPaymentLog(PosPaymentValue, xmlResponse.InnerXml, xmlResponse.InnerXml, authCode, resultCode, resultDescription, TransactionId, 97);//97 işlemin bankadan geldiğini gösterir
                    EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");
                    if (resultCode != "0000")
                    {
                        string sonuc = "işlem Başarısız";
                        EPayment.Update(PosPaymentValue.InsertId, resultCode, resultCode, sonuc, PosPaymentValue.IpAddress, string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty, resultDescription, "VAKIFBANK", DateTime.Now, 26);
                    }
                    else
                    {
                        EPayment.Update(PosPaymentValue.InsertId, authCode, "00", resultCode, PosPaymentValue.IpAddress, string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty, resultDescription, "VAKIFBANK", DateTime.Now, 26);
                    }
                }
                catch (Exception)
                {
                    return CloseFancybox(PosPaymentValue.PaymentId);
                }
            }

            return CloseFancybox(PosPaymentValue.PaymentId);
        }

        public ActionResult Ykb3D()
        {
            return View();
        }
        public ActionResult Ykb3DP()
        {
            C_PosnetOOSTDS posnetOOSTDSObj = new C_PosnetOOSTDS();
            string merchantPacket = null;
            string bankPacket = null;
            string sign = null;
            string tranType = null;
            string authCode = string.Empty;
            string pProcreturnCode = string.Empty;
            string ErrorMsg = string.Empty;
            string pUseEpayment = Bank.YAPIKREDI.ToString();
            string OrderId = string.Empty;
            string xid = "";

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //Banka tarafından yönlendirilen işlem bilgilerini alınır
            merchantPacket = Request.Form.Get("MerchantPacket");
            bankPacket = Request.Form.Get("BankPacket");
            sign = Request.Form.Get("Sign");
            tranType = Request.Form.Get("TranType");
            xid = Request.Form.Get("XID");

            //Config Dosyasından Firma bilgileri okunur
            posnetOOSTDSObj.SetMid(PosBankDetails.StoreNumber);
            posnetOOSTDSObj.SetTid(PosBankDetails.TerminalNo);
            posnetOOSTDSObj.SetPosnetID(PosBankDetails.PosnetId);
            posnetOOSTDSObj.SetKey(PosBankDetails._3dSecureKey);
            posnetOOSTDSObj.SetURL(PosBankDetails.PosBank.XmlUrl);

            string amounts = PosPaymentValue.TotalPrice.ToString("N2").Replace(".", ",");
            if (amounts.Contains(",") || amounts.Contains("."))
            {
                amounts = amounts.Replace(",", "");
                amounts = amounts.Replace(".", "");
                amounts = amounts.Trim();
            }

            // işleme ait MAC bilgisi üretilerek, bu işlemin 3d secure doğrulaması öncesinde bilgilerle aynı olup olmadığı sunucu tarafında kontrol edilir.
            C_PosnetSession c_PosnetSession = new C_PosnetSession();
            c_PosnetSession.Amount = amounts; // veri tabanındaki işleme ait amount bilgisi
            c_PosnetSession.Currency = "TL"; // veri tabanındaki işleme ait  currency bilgisi
            c_PosnetSession.MerchantNo = PosBankDetails.StoreNumber; // işleme ait MerchantNo
            c_PosnetSession.TermianlNo = PosBankDetails.TerminalNo; // işleme ait MerchantNo
            c_PosnetSession.Xid = xid; // veri tabanındaki işleme ait  currency bilgisi

            String threeDSecureTransactionMac = posnetOOSTDSObj.getMacFor3DSTransaction(c_PosnetSession);


            //İşlemin kredi kartı finansallanın başlatılması
            posnetOOSTDSObj.CheckAndResolveMerchantData(merchantPacket, bankPacket, sign);

            C_PosnetSession responsePosnetSession = new C_PosnetSession();
            responsePosnetSession.Amount = amounts; // veri tabanındaki işleme ait amount bilgisi
            responsePosnetSession.Currency = "TL"; // veri tabanındaki işleme ait  currency bilgisi
            responsePosnetSession.MerchantNo = PosBankDetails.StoreNumber; // işleme ait MerchantNo
            responsePosnetSession.TermianlNo = PosBankDetails.TerminalNo; // işleme ait MerchantNo
            responsePosnetSession.Xid = xid; // veri tabanındaki işleme ait  currency bilgisi
            responsePosnetSession.ResponseHostlogKey = posnetOOSTDSObj.GetHostlogkey();

            String responseMac = posnetOOSTDSObj.getMacFor3DSResponse(responsePosnetSession);


            EPaymentLog log = new EPaymentLog(PosPaymentValue, posnetOOSTDSObj.GetXMLRequest(), "", "", "", "", "", 97);//98 işlemin bankaya gittiğini gösterir
            EPaymentLogger.LogDB(log, "/api/payment/updatestatus");

            if (responseMac.Equals(posnetOOSTDSObj.GetResponseMac()))
            {
                // doğrulama başarılı
            }
            else
            {
                // doğrulama başarısız başarısız mesajı ver

            }

            //İşlem MdStatus 1 ise devam eder. Degil ise hata kaydı atıp işlemi sonlandırır.
            if (posnetOOSTDSObj.GetTDSMDStatus() == "1")
            {
                //3DS Kredi kartı onay İşlemini başlat
                posnetOOSTDSObj.ConnectAndDoTDSTransaction(merchantPacket, bankPacket, sign, threeDSecureTransactionMac);

                /* Finansallaştırma sonrası dönen nesne parçalanarak veriler veritabanına kayıt edilir */
                string responseText = posnetOOSTDSObj.GetResponseText();
                string responseCode = posnetOOSTDSObj.GetResponseCode();
                OrderId = posnetOOSTDSObj.GetXID();
                pProcreturnCode = posnetOOSTDSObj.GetApprovedCode() == "1" ? "00" : posnetOOSTDSObj.GetApprovedCode();

                authCode = posnetOOSTDSObj.GetAuthcode();
                ErrorMsg = pProcreturnCode == "00"
                    ? ""
                    : "İşlem Başarısızdır Hata Kodu" + posnetOOSTDSObj.GetApprovedCode() + " responseCode = " +
                      responseCode + " ResponseText = " + responseText + "";

                Logger.LogPayment(LogPaymentType.Information, "YKB Islem Sonucu", "XML Request : " + posnetOOSTDSObj.GetXMLRequest() + " XML Response : " + posnetOOSTDSObj.GetXMLResponse(), PosPaymentValue.PaymentId, Bank.YAPIKREDI.ToString(),
                   CurrentCustomer.Id, -1, false);

                EPayment.Update(PosPaymentValue.InsertId, authCode, pProcreturnCode, ErrorMsg,
                    PosPaymentValue.IpAddress, OrderId, OrderId, String.Empty, pProcreturnCode,
                    String.Empty, pUseEpayment, DateTime.Now, 27);

                return CloseFancybox(PosPaymentValue.PaymentId);

            }
            else
            {

                Logger.LogPayment(LogPaymentType.Error, "YKB MdStatus Hatası", "MdStatus :" + posnetOOSTDSObj.GetTDSMDStatus() + " ErrorMessage : " + posnetOOSTDSObj.GetResponseText()
                    + " XML Request : " + posnetOOSTDSObj.GetXMLRequest() + " XML Response : " + posnetOOSTDSObj.GetXMLResponse(), PosPaymentValue.PaymentId, Bank.YAPIKREDI.ToString(),
                    CurrentCustomer.Id, -1, false);

                OrderId = posnetOOSTDSObj.GetXID();

                EPayment.Update(PosPaymentValue.InsertId, string.Empty, "07",
                    "İşlem Başarısız " + posnetOOSTDSObj.GetTDSTXStatus() + " - " + posnetOOSTDSObj.GetTDSMDErrorMessage() + " Md Status:" +
                    posnetOOSTDSObj.GetTDSMDStatus().ToString(), PosPaymentValue.IpAddress,
                    PosPaymentValue.PaymentId, PosPaymentValue.PaymentId, OrderId, OrderId,
                    string.Empty, Bank.YAPIKREDI.ToString(), DateTime.Now, 27);

                return CloseFancybox(PosPaymentValue.PaymentId);

            }

        }

        #region NonSecureMethods
        private ContentResult QNBFinansbankPayment()
        {
            string MerchantId = PosBankDetails.StoreNumber;
            string UserCode = PosBankDetails.ApiUser;
            string UserPassword = PosBankDetails.Password;
            String Expiry = PosPaymentValue.ExpMounth.Trim() + PosPaymentValue.ExpYear.Trim();

            string data = string.Empty;
            data += "MbrId=5&";                                                                            //Kurum Kodu
            data += "MerchantID=" + MerchantId + "&";                                                      //Üye işyeri no
            data += "UserCode=" + UserCode + "&";                                                          //Kullanıcı Kodu
            data += "UserPass=" + UserPassword + "&";                                                      //Kullanıcı Şifre
            data += "OrderId=" + PosPaymentValue.PaymentId + "&";                                          //Sipariş Numarası
            data += "SecureType=NonSecure&";                                                               //Güvenlik tipi
            data += "TxnType=Auth&";                                                                       //İşlem  Tipi
            data += "PurchAmount=" + PosPaymentValue.TotalPrice.ToString().Replace(".", ",") + "&";        //Tutar
            data += "Currency=949&";                                                                       //Para Birimi
            data += "Pan=" + PosPaymentValue.CardNumber.Replace(" ", "").Trim() + "&";
            data += "Expiry=" + Expiry + "&";                                                              //Son Kullanma Tarihi (MMYY)
            data += "Cvv2=" + PosPaymentValue.Cvc + "&";                                                   //Guvenlik Kodu (Cvv)
            data += "MOTO=0&";                                                                             //MOTO mu
            data += "Lang=TR&";                                                                            //Dil
            data += "InstallmentCount=" + PosPaymentValue.Installment.ToString() + "&";                    //Taksit sayısı

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://vpos.qnbfinansbank.com/Gateway/Default.aspx");
                byte[] parameters = Encoding.UTF8.GetBytes(data);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = parameters.Length;
                Stream requeststream = request.GetRequestStream();
                requeststream.Write(parameters, 0, parameters.Length);
                requeststream.Close();
                HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
                StreamReader responsereader = new StreamReader(resp.GetResponseStream(), Encoding.UTF8);
                String responseStr = responsereader.ReadToEnd();

                string ProcReturnCode = "99";
                string AuthCode = "";
                string ErrMsg = "İşlem Sırasında Bir Hata Oluştu.";
                string OrderId = "";
                string Extra = "";
                string IpAddress = "";

                if (responseStr != null)
                {
                    string[] paramArr = responseStr.Split(new char[] { ';', ';' }, StringSplitOptions.RemoveEmptyEntries);

                    Hashtable list = new Hashtable();

                    foreach (string item in paramArr)
                    {
                        string[] nameValue = item.Split('=');
                        list.Add(nameValue[0], nameValue[1]);
                    }

                    ProcReturnCode = list.ContainsKey("ProcReturnCode") ? list["ProcReturnCode"].ToString() : "";
                    AuthCode = list.ContainsKey("AuthCode") ? list["AuthCode"].ToString() : "";
                    ErrMsg = list.ContainsKey("ErrMsg") ? list["ErrMsg"].ToString() : "";
                    OrderId = list.ContainsKey("OrderId") ? list["OrderId"].ToString() : "";
                    Extra = list.ContainsKey("BankInternalResponseMessage") ? list["BankInternalResponseMessage"].ToString() : "";
                    IpAddress = list.ContainsKey("RequestIp") ? list["RequestIp"].ToString() : "";
                }

                EPayment.Update(PosPaymentValue.InsertId, AuthCode, ProcReturnCode, ErrMsg, PosPaymentValue.IpAddress, PosPaymentValue.PaymentId, PosPaymentValue.PaymentId, "", OrderId, Extra, PosBankDetails.PosBankName, DateTime.Now, 1);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }

            return CloseFancybox(PosPaymentValue.PaymentId);
        }
        private ContentResult EstPayment()
        {
            string HostAddress = "";
            cc5payment mycc5Pay = new cc5payment();

            mycc5Pay.host = PosBankDetails.PosBank.PostUrl;
            HostAddress = PosBankDetails.PosBank.PostUrl;
            mycc5Pay.name = PosBankDetails.ApiUser;
            mycc5Pay.password = PosBankDetails.Password;
            mycc5Pay.clientid = PosBankDetails.StoreNumber;


            mycc5Pay.orderresult = 0;
            mycc5Pay.oid = "";

            mycc5Pay.cardnumber = PosPaymentValue.CardNumber;
            mycc5Pay.expmonth = PosPaymentValue.ExpMounth;
            mycc5Pay.expyear = PosPaymentValue.ExpYear;
            mycc5Pay.cv2 = PosPaymentValue.Cvc;
            mycc5Pay.subtotal = PosPaymentValue.TotalPrice.ToString();
            mycc5Pay.currency = "949";
            mycc5Pay.chargetype = "Auth";
            if (PosPaymentValue.Installment != 1) mycc5Pay.taksit = PosPaymentValue.Installment.ToString();


            mycc5Pay.bname = PosPaymentValue.CustomerCode + "-" + PosPaymentValue.NameSurname;

            if (PosPaymentValue.UseBonus)
            {
                string ExpendableBonus = PosPaymentValue.ExpendableBonus.ToString();

                switch (PosBankDetails.PosBanks)
                {
                    case PosBanks.Akbank:
                        mycc5Pay.putExtra("CARDHOLDERNAME", PosPaymentValue.NameSurname);
                        mycc5Pay.putExtra("CCBCHIPPARA", ExpendableBonus);
                        break;
                    case PosBanks.Finansbank:
                    case PosBanks.Ziraatbank:
                    case PosBanks.Ingbank:
                    case PosBanks.Teb:
                    case PosBanks.Sekerbank:
                        mycc5Pay.putExtra("KULLANPUAN", ExpendableBonus);
                        break;
                    case PosBanks.IsBank:
                        mycc5Pay.putExtra("KULLANPUAN", ExpendableBonus);
                        break;
                    case PosBanks.Halkbank:
                        mycc5Pay.putExtra("ODUL", ExpendableBonus);
                        break;

                }

            }


            string result = mycc5Pay.processorder();
            string procReturnCode = mycc5Pay.procreturncode;
            string errMsg = mycc5Pay.errmsg;
            string oid1 = mycc5Pay.oid;
            string groupId = mycc5Pay.groupid;
            string appr1 = mycc5Pay.appr;
            string refNo = mycc5Pay.refno;
            string transId = mycc5Pay.transid;
            string extra = mycc5Pay.Extra("HOSTMSG");
            string authCode = mycc5Pay.code;


            //Akbank puan kullanımı sonunda dönen ektra alanlar
            string SETTLEID = mycc5Pay.Extra("SETTLEID");
            string TRXDATE = mycc5Pay.Extra("TRXDATE");
            string ERRORCODE = mycc5Pay.Extra("ERRORCODE");
            string NUMCODE = mycc5Pay.Extra("NUMCODE");
            string CCBCHIPPARABAKIYE = mycc5Pay.Extra("CCBCHIPPARABAKIYE");
            string PCBCHIPPARABAKIYE = mycc5Pay.Extra("PCBCHIPPARABAKIYE");
            string XCBCHIPPARABAKIYE = mycc5Pay.Extra("XCBCHIPPARABAKIYE");
            string CCBCHIPPARA = mycc5Pay.Extra("CCBCHIPPARA");
            string PCBCHIPPARA = mycc5Pay.Extra("PCBCHIPPARA");
            string XCBCHIPPARA = mycc5Pay.Extra("XCBCHIPPARA");
            string ERTELEMEAY = mycc5Pay.Extra("ERTELEMEAY");
            string ARTITAKSIT = mycc5Pay.Extra("ARTITAKSIT");
            string ERTELEMESABITTARIH = mycc5Pay.Extra("ERTELEMESABITTARIH");
            string KAZANILANCEKILISADEDI = mycc5Pay.Extra("KAZANILANCEKILISADEDI");
            string TOPLAMCEKILISBAKIYESI = mycc5Pay.Extra("TOPLAMCEKILISBAKIYESI");
            string CCBCHIPPARAACIKLAMA = mycc5Pay.Extra("CCBCHIPPARAACIKLAMA");
            string PCBCHIPPARAACIKLAMA = mycc5Pay.Extra("CCBCHIPPARAACIKLAMA");
            string XCBCHIPPARAACIKLAMA = mycc5Pay.Extra("CCBCHIPPARAACIKLAMA");
            /*Akbank Puan Bitiş*/

            string KAZANILANPUAN = mycc5Pay.Extra("KAZANILANPUAN");
            string KAZANILKULLANILANPUANANPUAN = mycc5Pay.Extra("KULLANILANPARAPUANTRL");

            string log = Serializer.Serialize(mycc5Pay).InnerXml;
            Logger.LogPayment(LogPaymentType.Information, "EST NONSECURE Islem Başlangıcı",
                        " XML Request : " + log, PosPaymentValue.PaymentId, PosPaymentValue.Bank
                     );
            EPaymentLog log_return = new EPaymentLog(PosPaymentValue, string.Empty, log, authCode, procReturnCode, errMsg, oid1, 97);//97 işlemin bankadan geldiğini gösterir
            EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");

            EPayment.Update(PosPaymentValue.InsertId, authCode, procReturnCode, errMsg, PosPaymentValue.IpAddress, oid1, oid1, groupId, transId, extra, PosBankDetails.PosBankName, DateTime.Now, PosPaymentValue.BankId);


            return CloseFancybox(PosPaymentValue.PaymentId);
        }

        public void EstAkbankPointQuery(cc5payment mycc5Pay)
        {
            mycc5Pay.chargetype = "Query";
            mycc5Pay.putExtra("CARDHOLDERNAME", PosPaymentValue.NameSurname.ToUpper());
            mycc5Pay.putExtra("CHIPPARASORGU", "CHIPPARASORGU");
            string result = mycc5Pay.processorder();
            string CCBCHIPPARABAKIYE = mycc5Pay.Extra("CCBCHIPPARABAKIYE");//Chippara bakiye. Bu chippara kampanya türünde kazanılan chipparalar heryerde harcanır.(Her yerde kazan heryerde harca) Bizde Kullanıan bu olacak
            string strXml = Serializer.Serialize(mycc5Pay).InnerXml;
            LogPointQuery(strXml, PosBanks.Akbank);
            SetTotalPointAddSession(CCBCHIPPARABAKIYE);

        }
        public void EstZiraatPointQuery(cc5payment mycc5Pay)
        {
            mycc5Pay.chargetype = "Query";
            //mycc5Pay.putExtra("CARDHOLDERNAME", PosPaymentValue.NameSurname.ToUpper());
            mycc5Pay.putExtra("PUANSORGU", "PUANSORGU");
            string result = mycc5Pay.processorder();
            string KULLANILABILIRPUAN = mycc5Pay.Extra("KULLANILABILIRPUAN");
            string strXml = Serializer.Serialize(mycc5Pay).InnerXml;
            LogPointQuery(strXml, PosBanks.Ziraatbank);
            SetTotalPointAddSession(KULLANILABILIRPUAN);

        }
        public void EstFinansPointQuery(cc5payment mycc5Pay)
        {
            mycc5Pay.chargetype = "Query";
            //mycc5Pay.putExtra("CARDHOLDERNAME", PosPaymentValue.NameSurname.ToUpper());
            mycc5Pay.putExtra("PUANSORGU", "PUANSORGU");
            string result = mycc5Pay.processorder();
            string KULLANILABILIRBONUS = mycc5Pay.Extra("KULLANILABILIRBONUS");
            string strXml = Serializer.Serialize(mycc5Pay).InnerXml;
            LogPointQuery(strXml, PosBanks.Finansbank);

            SetTotalPointAddSession(KULLANILABILIRBONUS);

        }
        public void EstIngPointQuery(cc5payment mycc5Pay)
        {
            mycc5Pay.chargetype = "Query";
            //mycc5Pay.putExtra("CARDHOLDERNAME", PosPaymentValue.NameSurname.ToUpper());
            mycc5Pay.putExtra("PUANSORGU", "PUANSORGU");
            string result = mycc5Pay.processorder();
            string KULLANILABILIRBONUS = mycc5Pay.Extra("KULLANILABILIRBONUS");
            string strXml = Serializer.Serialize(mycc5Pay).InnerXml;
            LogPointQuery(strXml, PosBanks.Ingbank);
            SetTotalPointAddSession(KULLANILABILIRBONUS);

        }
        public void EstTebPointQuery(cc5payment mycc5Pay)
        {
            mycc5Pay.chargetype = "Query";
            //mycc5Pay.putExtra("CARDHOLDERNAME", PosPaymentValue.NameSurname.ToUpper());
            mycc5Pay.putExtra("PUANSORGU", "PUANSORGU");
            string result = mycc5Pay.processorder();
            string KULLANILABILIRBONUS = mycc5Pay.Extra("KULLANILABILIRBONUS");
            string strXml = Serializer.Serialize(mycc5Pay).InnerXml;
            LogPointQuery(strXml, PosBanks.Teb);
            SetTotalPointAddSession(KULLANILABILIRBONUS);

        }
        public void EstIsbankPointQuery(cc5payment mycc5Pay)
        {
            mycc5Pay.chargetype = "Query";
            //mycc5Pay.putExtra("CARDHOLDERNAME", PosPaymentValue.NameSurname.ToUpper());
            mycc5Pay.putExtra("MAXIPUANSORGU", "MAXIPUANSORGU");
            string result = mycc5Pay.processorder();
            string KULLANILABILIRBONUS = mycc5Pay.Extra("MAXIPUAN");
            string strXml = Serializer.Serialize(mycc5Pay).InnerXml;
            LogPointQuery(strXml, PosBanks.IsBank);
            SetTotalPointAddSession(KULLANILABILIRBONUS);

        }
        public void EstSekerbankPointQuery(cc5payment mycc5Pay)
        {
            mycc5Pay.chargetype = "Query";
            //mycc5Pay.putExtra("CARDHOLDERNAME", PosPaymentValue.NameSurname.ToUpper());
            mycc5Pay.putExtra("SECMELIKAMPANYASORGU", "SECMELIKAMPANYASORGU");
            string result = mycc5Pay.processorder();
            string KULLANILABILIRBONUS = mycc5Pay.Extra("KULLANILABILIRBONUS");
            string strXml = Serializer.Serialize(mycc5Pay).InnerXml;
            LogPointQuery(strXml, PosBanks.IsBank);
            SetTotalPointAddSession(KULLANILABILIRBONUS);

        }
        public void EstHsbcPointQuery(cc5payment mycc5Pay)
        {
            mycc5Pay.chargetype = "Query";
            //mycc5Pay.putExtra("CARDHOLDERNAME", PosPaymentValue.NameSurname.ToUpper());
            mycc5Pay.putExtra("ODULSORGU", "ODULSORGU");
            string result = mycc5Pay.processorder();
            string KULLANILABILIRBONUS = mycc5Pay.Extra("ODUL");
            string strXml = Serializer.Serialize(mycc5Pay).InnerXml;
            LogPointQuery(strXml, PosBanks.Hsbc);
            SetTotalPointAddSession(KULLANILABILIRBONUS);

        }
        public void EstHalbankPointQuery(cc5payment mycc5Pay)
        {
            mycc5Pay.chargetype = "Query";
            //mycc5Pay.putExtra("CARDHOLDERNAME", PosPaymentValue.NameSurname.ToUpper());
            mycc5Pay.putExtra("ODULSORGU", "SOR");
            string result = mycc5Pay.processorder();
            string KULLANILABILIRBONUS = mycc5Pay.Extra("ODUL");
            string strXml = Serializer.Serialize(mycc5Pay).InnerXml;
            LogPointQuery(strXml, PosBanks.Halkbank);
            SetTotalPointAddSession(KULLANILABILIRBONUS);

        }

        public void LogPointQuery(string strXml, PosBanks bank)
        {
            Logger.LogPayment(LogPaymentType.PointQuery, MethodBase.GetCurrentMethod().Name.ToString(), strXml ?? "", "-1",
              bank.ToString(), CurrentCustomer.Id, CurrentSalesmanId);

        }
        private ContentResult TurkiyeFinans()
        {
            #region KodBlogu

            NetProvRemote myProv = new NetProvRemote();

            myProv.OrgNo = Convert.ToInt32(PosBankDetails.ApiUser);//006;
            myProv.FirmNo = Convert.ToInt32(PosBankDetails.StoreNumber);//9626604;
            myProv.TermNo = Convert.ToInt32(PosBankDetails.TerminalNo);

            myProv.CardNo = long.Parse(PosPaymentValue.CardNumber);
            string expriy = "20" + PosPaymentValue.ExpYear.Trim() + PosPaymentValue.ExpMounth.Trim();
            myProv.Expiry = int.Parse(expriy);
            myProv.Cvv2No = int.Parse(PosPaymentValue.Cvc);
            myProv.Amount = Convert.ToDecimal(PosPaymentValue.TotalPrice);
            //  myProv.Taksit = 0;//int.Parse(CurrentPaymentValue.Installment);
            // myProv.BonusAmount = decimal.Parse(0);
            string OrderID = PosPaymentValue.PaymentId.Substring(0, 15);
            myProv.SipNo = OrderID;
            myProv.CurrencyCode = 949;
            myProv.WaitForSaleCompleted = true;

            myProv.MerchantKey = PosBankDetails._3dSecureKey;// "awNv56fY";

            // Burada ayrıca MPI değişkenlerine ilgili dönüş değerleri atanacak
            // myProv.MPI* = Form[“MPI*”];

            myProv.MPIcavv = "";
            myProv.MPIxid = "";
            myProv.MPIeci = "";
            myProv.MPIversion = "";
            myProv.MPImerchantID = "";
            myProv.MPImdStatus = "";
            myProv.MPImdErrorMsg = "";
            myProv.MPItxstatus = "";
            myProv.MPIiReqCode = "";
            myProv.MPIiReqDetail = "";
            myProv.MPIvendorCode = "";
            myProv.MPIcavvAlgorithm = "";
            myProv.MPIPAResVerified = "";
            myProv.MPIPAResSyntaxOK = "0";
            myProv.WaitForSaleCompleted = true;
            string result = "İşlem Başarısız";
            string Code = "007";
            string responseCode;
            string responseDesc;
            string xml;
            var Sonuc = myProv.MessageSend();
            responseCode = myProv.ResponseCode;
            if (Sonuc && responseCode == "00")
            {
                Code = "00";
                result = "İşlem Başarılı";
                responseCode = myProv.ResponseCode;
                string errmsag = myProv.MPImdErrorMsg;
                xml = Serializer.Serialize(myProv).InnerXml;
                responseDesc = myProv.ResponseDescription;
            }
            else
            {
                xml = Serializer.Serialize(myProv).InnerXml;
                responseCode = myProv.ResponseCode;
                responseDesc = myProv.ResponseDescription;
            }



            Logger.LogPayment(LogPaymentType.Information, "TURKIYEFINANS NONSECURE Islem Başlangıcı",
                     " XML Request : " + xml, PosPaymentValue.PaymentId, PosPaymentValue.Bank
                  );
            EPaymentLog log_return = new EPaymentLog(PosPaymentValue, string.Empty, xml, responseCode, Code, responseDesc, OrderID, 97);//97 işlemin bankadan geldiğini gösterir
            EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");
            EPayment.Update(PosPaymentValue.InsertId, responseCode, Code, result + " " + responseDesc, PosPaymentValue.IpAddress, OrderID, OrderID, "", OrderID, xml, PosBankDetails.PosBankName, DateTime.Now, PosPaymentValue.BankId);
            return CloseFancybox(PosPaymentValue.PaymentId);
            #endregion

        }
        public void SetTotalPointAddSession(string point)
        {

            if (!string.IsNullOrEmpty(point))
            {
                point = point.Substring(0, point.Length - 2) + "," + point.Substring(point.Length - 2, 2).Replace(".", "").Replace(",", "");
                double bonus = Convert.ToDouble(point);
                PosPaymentValue.TotalBonus = bonus.ToString();
                PosPaymentValue.TotalBonusPrice = Convert.ToDouble(point.Substring(0, point.Length - 2)).ToString();
                PosPaymentValue.TotalBonusDecimal = point.Substring(point.Length - 2, 2).Replace(".", "").Replace(",", "");
            }
        }
        public cc5payment EstSetDefaultValue()
        {
            string HostAddress = "";
            cc5payment mycc5Pay = new cc5payment();

            mycc5Pay.host = PosBankDetails.PosBank.PostUrl;
            HostAddress = PosBankDetails.PosBank.PostUrl;
            mycc5Pay.name = PosBankDetails.ApiUser;
            mycc5Pay.password = PosBankDetails.Password;
            mycc5Pay.clientid = PosBankDetails.StoreNumber;


            mycc5Pay.orderresult = 0;
            mycc5Pay.oid = "";
            mycc5Pay.cardnumber = PosPaymentValue.CardNumber;
            mycc5Pay.expmonth = PosPaymentValue.ExpMounth;
            mycc5Pay.expyear = PosPaymentValue.ExpYear;
            mycc5Pay.cv2 = PosPaymentValue.Cvc;
            mycc5Pay.subtotal = PosPaymentValue.TotalPrice.ToString("N2");
            mycc5Pay.currency = "949";


            if (PosPaymentValue.Installment != 1) mycc5Pay.taksit = PosPaymentValue.Installment.ToString();

            mycc5Pay.bname = PosPaymentValue.CustomerCode + "-" + PosPaymentValue.NameSurname;
            return mycc5Pay;

        }
        private ContentResult DenizBank()
        {
            string HostAddress = "";

            String expiresval = PosPaymentValue.ExpMounth.Replace(" ", "") + PosPaymentValue.ExpYear.Replace(" ", "");

            String format = "{0}={1}&";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(format, "ShopCode", PosBankDetails.StoreNumber);
            sb.AppendFormat(format, "PurchAmount", PosPaymentValue.TotalPrice.ToString());
            sb.AppendFormat(format, "Currency", "949");
            sb.AppendFormat(format, "OrderId", "");
            sb.AppendFormat(format, "InstallmentCount", PosPaymentValue.Installment);
            sb.AppendFormat(format, "TxnType", "Auth");
            sb.AppendFormat(format, "UserCode", PosBankDetails.ApiUser);//Online Kullanıcı Kodu
            sb.AppendFormat(format, "UserPass", PosBankDetails.Password);// Online Kullanıcı Şifre
            sb.AppendFormat(format, "SecureType", "NonSecure");
            sb.AppendFormat(format, "Pan", PosPaymentValue.CardNumber);
            sb.AppendFormat(format, "Expiry", expiresval);
            sb.AppendFormat(format, "Cvv2", PosPaymentValue.Cvc);
            sb.AppendFormat(format, "CardType", 0);
            sb.AppendFormat(format, "Lang", "TR");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(PosBankDetails.PosBank.XmlUrl);
            byte[] parameters = Encoding.GetEncoding("ISO-8859-9").GetBytes(sb.ToString());
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = parameters.Length;
            Stream requeststream = request.GetRequestStream();
            requeststream.Write(parameters, 0, parameters.Length);
            requeststream.Close();
            Response.Write(sb.ToString());
            HttpWebResponse resp = (HttpWebResponse)request.GetResponse();
            StreamReader responsereader = new StreamReader(resp.GetResponseStream(), Encoding.GetEncoding("ISO-8859-9"));
            String responseStr = responsereader.ReadToEnd();
            {
                string procReturnCode = "";
                string errMsg = "";
                string oid1 = "";
                string groupId = "";
                string transId = "";

                string authCode = "";
                var vDenizBank = new PaymentControlDenizBankTemp();
                string[] paramArr = responseStr.Split(';', ';');

                vDenizBank = paramArr.Select(p => p.Split('=')).Where(nameValue => nameValue.Length > 1).Aggregate(vDenizBank, (current, nameValue) => current.SetValue(current, nameValue[0], nameValue[1]));
                procReturnCode = vDenizBank.ProcReturnCode;
                errMsg = vDenizBank.ErrorMessage;
                oid1 = vDenizBank.OrderId;
                groupId = vDenizBank.OrderId;
                transId = vDenizBank.TransId;
                authCode = vDenizBank.AuthCode;
                Logger.LogPayment(LogPaymentType.Information, "Denizbank NonSecure Islem Sonucu",
                    " XML Response : " + responseStr, PosPaymentValue.PaymentId, "Denizbank");
                EPaymentLog log_return = new EPaymentLog(PosPaymentValue, "", responseStr, authCode, procReturnCode, errMsg, oid1, 97);//97 işlemin bankadan geldiğini gösterir
                EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");
                EPayment.Update(PosPaymentValue.InsertId, authCode, procReturnCode, errMsg, PosPaymentValue.IpAddress, oid1, oid1, transId, "", "", PosBankDetails.PosBankName, DateTime.Now, 34);
                return CloseFancybox(PosPaymentValue.PaymentId);

            }
        }
        private ContentResult GarantiBank()
        {
            string strMode = "PROD";
            string strVersion = "v0.01";
            string strTerminalID = PosBankDetails.TerminalNo;
            string _strTerminalID = "0" + strTerminalID;
            string strProvUserID = "PROVAUT";
            string strProvisionPassword = PosBankDetails.Password;
            string strUserID = CurrentCustomer.Code + CurrentCustomer.Name;
            if (strUserID.Contains("-") || strUserID.Contains(" ") || strUserID.Contains(".") || strUserID.Contains(",") || strUserID.Contains("!") || strUserID.Contains("#") || strUserID.Contains("&") || strUserID.Contains("(") || strUserID.Contains(")") || strUserID.Contains("?"))
            {
                strUserID = strUserID.Replace("-", "");
                strUserID = strUserID.Replace(" ", "");
                strUserID = strUserID.Replace(".", "");
                strUserID = strUserID.Replace(",", "");
                strUserID = strUserID.Replace("!", "");
                strUserID = strUserID.Replace("#", "");
                strUserID = strUserID.Replace("&", "");
                strUserID = strUserID.Replace("(", "");
                strUserID = strUserID.Replace(")", "");
                strUserID = strUserID.Replace("?", "");
            }
            if (strUserID.Length > 32)
            {
                strUserID = strUserID.Substring(0, 32);
            }

            string strMerchantID = PosBankDetails.StoreNumber;
            string strIPAddress = "212.253.125.106";
            string strEmailAddress = "sanalpos@eryaz.net";
            string strOrderID = GenerateOrderId();
            string strNumber = PosPaymentValue.CardNumber;
            string strExpireDate = PosPaymentValue.ExpMounth.Replace(" ", "") + PosPaymentValue.ExpYear.Replace(" ", "");
            string strCVV2 = PosPaymentValue.Cvc;
            string kurus = "";
            string lira = "";
            string cekilenTutar = PosPaymentValue.TotalPrice.ToString();
            string amount = PosPaymentValue.TotalPrice.ToString("N2");

            if (amount.Contains(",") || amount.Length < 3)
            {
                string[] dizi = amount.Split(',');
                if (dizi.Length > 1)
                {
                    kurus = dizi[1].ToString();
                }
                lira = dizi[0].ToString().Replace(".", "");
                if (kurus.Length < 2)
                {
                    while (kurus.Length < 2)
                    {
                        kurus = kurus + "0";
                    }
                }
                if (kurus.Length > 2)
                {
                    kurus = kurus.Substring(0, 2);
                }
                amount = lira + kurus;
            }

            string strAmount = amount; //İşlem Tutarı 1.00 TL için 100 gönderilmeli
            string strType = "sales";
            string strCurrencyCode = "949";
            string strCardholderPresentCode = "0";
            string strMotoInd = "N";
            string strInstallmentCount = "";
            string strHostAddress = PosBankDetails.PosBank.XmlUrl;

            string SecurityData = GetSHA1(strProvisionPassword + _strTerminalID).ToUpper();
            string HashData = GetSHA1(strOrderID + strTerminalID + strNumber + strAmount + SecurityData).ToUpper();


            if (PosPaymentValue.Installment == 1)
            {
                strInstallmentCount = "";
            }
            else
            {
                strInstallmentCount = PosPaymentValue.Installment.ToString();
            }
            string totalPonus = PosPaymentValue.ExpendableBonus;
            if (totalPonus.Contains(",") || totalPonus.Contains("."))
            {
                totalPonus = totalPonus.Replace(",", "");
                totalPonus = totalPonus.Replace(".", "");
                totalPonus = totalPonus.Trim();
            }

            string strXML = null;
            strXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                     + "<GVPSRequest>" +
                     "<Mode>" + strMode + "</Mode>" +
                     "<Version>" + strVersion + "</Version>" +
                     "<Terminal>" +
                     "<ProvUserID>" + strProvUserID + "</ProvUserID>" +
                     "<HashData>" + HashData + "</HashData>" +
                     "<UserID>" + strUserID + "</UserID>" +
                     "<ID>" + strTerminalID + "</ID>" +
                     "<MerchantID>" + strMerchantID + "</MerchantID>" +
                     "</Terminal>" +
                     "<Customer" + ">" +
                     "<IPAddress>" + strIPAddress + "</IPAddress>" +
                     "<EmailAddress>" + strEmailAddress + "</EmailAddress>" +
                     "</Customer>" + "" +
                     "<Card>" +
                     "<Number>" + strNumber + "</Number>" +
                     "<ExpireDate>" + strExpireDate + "</ExpireDate>" +
                     "<CVV2>" + strCVV2 + "</CVV2>" +
                     "</Card>" +
                     "<Order>" +
                     "<OrderID>" + strOrderID + "</OrderID>" +
                     "<GroupID></GroupID>" +
                     "<AddressList>" +
                     "<Address>" +
                     "<Type>S</Type>" +
                     "<Name></Name>" +
                     "<LastName>" +
                     "</LastName>" +
                     "<Company>" +
                     "</Company>" +
                     "<Text>" +
                     "</Text>" +
                     "<District>" +
                     "</District>" +
                     "<City>" +
                     "</City>" +
                     "<PostalCode>" +
                     "</PostalCode>" +
                     "<Country>" +
                     "</Country>" +
                     "<PhoneNumber>" +
                     "</PhoneNumber>" +
                     "</Address>" +
                     "</AddressList>" +
                     "</Order>" +
                     "<Transaction>" +
                     "<Type>" + strType + "</Type>" +
                     "<InstallmentCnt>" + strInstallmentCount + "</InstallmentCnt>" +
                     "<Amount>" + strAmount + "</Amount>" +
                     "<CurrencyCode>" + strCurrencyCode + "</CurrencyCode>" +
                     "<CardholderPresentCode>" + strCardholderPresentCode + "</CardholderPresentCode>" +
                     "<MotoInd>" + strMotoInd + "</MotoInd>";
            if (PosPaymentValue.UseBonus)
            {
                strXML += "<RewardList>" +
                              "<Reward>" +
                                  "<Type>BNS</Type>" +
                                  "<UsedAmount>" + totalPonus + "</UsedAmount>" +
                              "</Reward>" +
                          "</RewardList>";


            }

            strXML += "</Transaction>" + "" +
                "</GVPSRequest>";

            #region Loglama için Eklendi

            string strXML_2 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + "<GVPSRequest>" + "<Mode>" + strMode + "</Mode>" + "<Version>" + strVersion + "</Version>" + "<Terminal><ProvUserID>" + strProvUserID + "</ProvUserID><HashData>" + HashData + "</HashData><UserID>" + strUserID + "</UserID><ID>" + strTerminalID + "</ID><MerchantID>" + strMerchantID + "</MerchantID></Terminal>" + "<Customer><IPAddress>" + strIPAddress + "</IPAddress><EmailAddress>" + strEmailAddress + "</EmailAddress></Customer>" + "<Card><Number>" + strNumber.Substring(0, 6) + "******" + strNumber.Substring(strNumber.Length - 4, 4) + "</Number><ExpireDate>" + strExpireDate + "</ExpireDate><CVV2>" + strCVV2 + "</CVV2></Card>" + "<Order><OrderID>" + strOrderID + "</OrderID><GroupID></GroupID><AddressList><Address><Type>S</Type><Name></Name><LastName></LastName><Company></Company><Text></Text><District></District><City></City><PostalCode></PostalCode><Country></Country><PhoneNumber></PhoneNumber></Address></AddressList></Order>" + "<Transaction>" + "<Type>" + strType + "</Type><InstallmentCnt>" + strInstallmentCount + "</InstallmentCnt><Amount>" + strAmount + "</Amount><CurrencyCode>" + strCurrencyCode + "</CurrencyCode><CardholderPresentCode>" + strCardholderPresentCode + "</CardholderPresentCode><MotoInd>" + strMotoInd + "</MotoInd>" + "</Transaction>" + "</GVPSRequest>";
            #endregion

            String campaingUrl = "";
            string Message = "";
            string AuthCode = "";
            string ReasonCode = "";
            string OrderID = "";
            string ErrorMsg = "";
            string SysErrMsg = "";
            string HostMsgList = "";
            string RetrefNum = "";
            string ProvDate = "";
            String Code = "";
            try
            {
                string data = "data=" + strXML;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                WebRequest _WebRequest = WebRequest.Create(strHostAddress);
                _WebRequest.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                _WebRequest.ContentType = "application/x-www-form-urlencoded";
                _WebRequest.ContentLength = byteArray.Length;
                Stream dataStream = _WebRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse _WebResponse = _WebRequest.GetResponse();
                Console.WriteLine(((HttpWebResponse)_WebResponse).StatusDescription);
                dataStream = _WebResponse.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
                string XML = responseFromServer;
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(XML);
                XmlElement xElement1 = xDoc.SelectSingleNode("//GVPSResponse/Transaction/Response/ReasonCode") as XmlElement;
                XmlElement xElement3 = xDoc.SelectSingleNode("//GVPSResponse/Transaction/Response/ErrorMsg") as XmlElement;
                Response.Write(xElement3.InnerText);
                XmlNodeList list = xDoc.GetElementsByTagName("GVPSResponse");
                String xmlResponse = list[0].InnerText;
                list = xDoc.GetElementsByTagName("Code");
                Code = list[0].InnerText;
                EPaymentLog log = new EPaymentLog(PosPaymentValue, strXML_2, "", "", "", "", "", 98);//98 işlemin bankaya gittiğini gösterir
                EPaymentLogger.LogDB(log, "/api/payment/updatestatus");
                Logger.LogPayment(LogPaymentType.Information, "GARANTI Islem Sonucu",
                        " XML Request : " + strXML_2, PosPaymentValue.PaymentId,
                      PosPaymentValue.Bank);


                if (Code == "00")
                {

                    list = xDoc.GetElementsByTagName("Message");
                    Message = list[0].InnerText;
                    list = xDoc.GetElementsByTagName("AuthCode");
                    AuthCode = list[0].InnerText;
                    list = xDoc.GetElementsByTagName("ReasonCode");
                    ReasonCode = list[0].InnerText;
                    list = xDoc.GetElementsByTagName("OrderID");
                    OrderID = list[0].InnerText;
                }
                list = xDoc.GetElementsByTagName("ErrorMsg");
                ErrorMsg = list[0].InnerText;
                list = xDoc.GetElementsByTagName("SysErrMsg");
                SysErrMsg = list[0].InnerText;
                list = xDoc.GetElementsByTagName("HostMsgList");
                HostMsgList = list[0].InnerText;
                list = xDoc.GetElementsByTagName("OrderID");
                OrderID = list[0].InnerText;
                list = xDoc.GetElementsByTagName("RetrefNum");
                RetrefNum = list[0].InnerText;
                list = xDoc.GetElementsByTagName("ProvDate");
                ProvDate = list[0].InnerText;
                EPaymentLog log_return = new EPaymentLog(PosPaymentValue, "", xDoc.InnerXml, AuthCode, Code, ErrorMsg, OrderID, 97);//97 işlemin bankadan geldiğini gösterir
                EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");
                Logger.LogPayment(LogPaymentType.Information, "GARANTI Islem Sonucu",
                   " XML Response : " + xDoc.InnerXml, PosPaymentValue.PaymentId,
                 PosPaymentValue.Bank);
                if (Code == "00")
                {
                    list = xDoc.GetElementsByTagName("CampaignChooseLink");
                    campaingUrl = list[0].InnerText;

                    campaingUrl = campaingUrl.Replace(" ", "");
                    if (!string.IsNullOrEmpty(campaingUrl))
                    {
                        EPayment.Update(PosPaymentValue.InsertId, AuthCode, Code, (ErrorMsg + SysErrMsg), PosPaymentValue.IpAddress, OrderID, OrderID, "", "", Message, PosBankDetails.PosBankName, DateTime.Now, 34, campaingUrl);
                        return CloseFancybox(PosPaymentValue.PaymentId);

                    }
                    else
                    {
                        EPayment.Update(PosPaymentValue.InsertId, AuthCode, Code, (ErrorMsg + SysErrMsg), PosPaymentValue.IpAddress, OrderID, OrderID, "", "", Message, PosBankDetails.PosBankName, DateTime.Now, 34, campaingUrl);
                        return CloseFancybox(PosPaymentValue.PaymentId);
                    }

                }
                else
                {
                    EPayment.Update(PosPaymentValue.InsertId, AuthCode, Code, (ErrorMsg + SysErrMsg), PosPaymentValue.IpAddress, OrderID, OrderID, "", "", Message, PosBankDetails.PosBankName, DateTime.Now, 34, campaingUrl);
                    return CloseFancybox(PosPaymentValue.PaymentId);
                }

            }
            catch (Exception ex)
            {
                EPayment.Update(PosPaymentValue.InsertId, AuthCode, Code, (ErrorMsg + SysErrMsg), PosPaymentValue.IpAddress, OrderID, OrderID, "", "", Message, PosBankDetails.PosBankName, DateTime.Now, 34, campaingUrl);
                return CloseFancybox(PosPaymentValue.PaymentId);
            }
        }
        private void GarantiBankBonusQuery()
        {
            string strMode = "PROD";
            string strVersion = "v0.01";
            string strTerminalID = PosBankDetails.TerminalNo;
            string _strTerminalID = "0" + strTerminalID;
            string strProvUserID = "PROVAUT";
            string strProvisionPassword = PosBankDetails.Password;
            string strUserID = CurrentCustomer.Code + CurrentCustomer.Name;
            if (strUserID.Contains("-") || strUserID.Contains(" ") || strUserID.Contains(".") || strUserID.Contains(",") || strUserID.Contains("!") || strUserID.Contains("#") || strUserID.Contains("&") || strUserID.Contains("(") || strUserID.Contains(")") || strUserID.Contains("?"))
            {
                strUserID = strUserID.Replace("-", "");
                strUserID = strUserID.Replace(" ", "");
                strUserID = strUserID.Replace(".", "");
                strUserID = strUserID.Replace(",", "");
                strUserID = strUserID.Replace("!", "");
                strUserID = strUserID.Replace("#", "");
                strUserID = strUserID.Replace("&", "");
                strUserID = strUserID.Replace("(", "");
                strUserID = strUserID.Replace(")", "");
                strUserID = strUserID.Replace("?", "");
            }
            if (strUserID.Length > 32)
            {
                strUserID = strUserID.Substring(0, 32);
            }

            string strMerchantID = PosBankDetails.StoreNumber;
            string strIPAddress = "212.253.125.106";
            string strEmailAddress = "sanalpos@eryaz.net";
            string strOrderID = GenerateOrderId();
            string strNumber = PosPaymentValue.CardNumber;
            string strExpireDate = PosPaymentValue.ExpMounth.Replace(" ", "") + PosPaymentValue.ExpYear.Replace(" ", "");
            string strCVV2 = PosPaymentValue.Cvc;
            string kurus = "";
            string lira = "";
            string cekilenTutar = PosPaymentValue.TotalPrice.ToString();
            string amount = PosPaymentValue.TotalPrice.ToString();
            if (amount.Contains(",") || amount.Length < 3)
            {
                string[] dizi = amount.Split(',');
                if (dizi.Length > 1)
                {
                    kurus = dizi[1].ToString();
                }
                lira = dizi[0].ToString();
                if (kurus.Length < 2)
                {
                    while (kurus.Length < 2)
                    {
                        kurus = kurus + "0";
                    }
                }
                if (kurus.Length > 2)
                {
                    kurus = kurus.Substring(0, 2);
                }
                amount = lira + kurus;
            }

            string strAmount = amount; //İşlem Tutarı 1.00 TL için 100 gönderilmeli
                                       //  string strType = "sales";
            string strType = "rewardinq";
            string strCurrencyCode = "949";
            string strCardholderPresentCode = "0";
            string strMotoInd = "N";
            string strInstallmentCount = "";
            string strHostAddress = PosBankDetails.PosBank.XmlUrl;

            string SecurityData = GetSHA1(strProvisionPassword + _strTerminalID).ToUpper();
            string HashData = GetSHA1(strOrderID + strTerminalID + strNumber + strAmount + SecurityData).ToUpper();


            if (PosPaymentValue.Installment == 1)
            {
                strInstallmentCount = "";
            }
            else
            {
                strInstallmentCount = PosPaymentValue.Installment.ToString();
            }

            string strXML = null;
            strXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                + "<GVPSRequest>" +
                    "<Mode>" + strMode + "</Mode>" +
                    "<Version>" + strVersion + "</Version>" +
                    "<Terminal>" +
                    "<ProvUserID>" + strProvUserID + "</ProvUserID>" +
                    "<HashData>" + HashData + "</HashData>" +
                    "<UserID>" + strUserID + "</UserID>" +
                    "<ID>" + strTerminalID + "</ID>" +
                    "<MerchantID>" + strMerchantID + "</MerchantID>" +
                    "</Terminal>" +
                    "<Customer" + ">" +
                        "<IPAddress>" + strIPAddress + "</IPAddress>" +
                        "<EmailAddress>" + strEmailAddress + "</EmailAddress>" +
                    "</Customer>" + "" +
                    "<Card>" +
                        "<Number>" + strNumber + "</Number>" +
                        "<ExpireDate>" + strExpireDate + "</ExpireDate>" +
                        "<CVV2>" + strCVV2 + "</CVV2>" +
                    "</Card>" +
                    "<Order>" +
                        "<OrderID>" + strOrderID + "</OrderID>" +
                        "<GroupID></GroupID>" +
                        "<AddressList>" +
                        "<Address>" +
                        "<Type>S</Type>" +
                        "<Name></Name>" +
                        "<LastName>" +
                        "</LastName>" +
                        "<Company>" +
                        "</Company>" +
                        "<Text>" +
                        "</Text>" +
                        "<District>" +
                        "</District>" +
                        "<City>" +
                        "</City>" +
                        "<PostalCode>" +
                        "</PostalCode>" +
                        "<Country>" +
                        "</Country>" +
                        "<PhoneNumber>" +
                        "</PhoneNumber>" +
                        "</Address>" +
                        "</AddressList>" +
                    "</Order>" +
                    "<Transaction>" +
                        "<Type>" + strType + "</Type>" +
                        "<InstallmentCnt>" + strInstallmentCount + "</InstallmentCnt>" +
                        "<Amount>" + strAmount + "</Amount>" +
                        "<CurrencyCode>" + strCurrencyCode + "</CurrencyCode>" +
                        "<CardholderPresentCode>" + strCardholderPresentCode + "</CardholderPresentCode>" +
                        "<MotoInd>" + strMotoInd + "</MotoInd>" +
                    "</Transaction>" + "" +
                "</GVPSRequest>";

            string BonusTotal = "";
            try
            {
                string data = "data=" + strXML;
                WebRequest _WebRequest = WebRequest.Create(strHostAddress);
                _WebRequest.Method = "POST";
                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                _WebRequest.ContentType = "application/x-www-form-urlencoded";
                _WebRequest.ContentLength = byteArray.Length;
                Stream dataStream = _WebRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse _WebResponse = _WebRequest.GetResponse();
                Console.WriteLine(((HttpWebResponse)_WebResponse).StatusDescription);
                dataStream = _WebResponse.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Console.WriteLine(responseFromServer);
                string XML = responseFromServer;
                XmlDocument xDoc = new XmlDocument();
                xDoc.LoadXml(XML);
                XmlNodeList list = xDoc.GetElementsByTagName("GVPSResponse");
                String xmlResponse = list[0].InnerText;
                try
                {

                    list = xDoc.GetElementsByTagName("TotalAmount");
                    BonusTotal = list[0].InnerText;

                }
                catch (Exception ex)
                {

                }
                Logger.LogPayment(LogPaymentType.PointQuery, MethodBase.GetCurrentMethod().Name.ToString(), xmlResponse, "-1",
           Bank.GARANTIBANK.ToString(), CurrentCustomer.Id, CurrentSalesmanId);
                SetTotalPointAddSession(BonusTotal);


            }
            catch (Exception ex)
            {

            }
        }

        public void YkbPointQuery()
        {
            bool result = false;
            C_Posnet posnetObj = new C_Posnet();
            posnetObj.SetURL(PosBankDetails.PosBank.XmlUrl);
            posnetObj.SetMid(PosBankDetails.StoreNumber);
            posnetObj.SetTid(PosBankDetails.TerminalNo);
            string date = PosPaymentValue.ExpYear.Replace(" ", "") + PosPaymentValue.ExpMounth.Replace(" ", "");
            result = posnetObj.DoPointInquiryTran(PosPaymentValue.CardNumber, date);
            if (result)
            {
                string KULLANILABILIRBONUS = posnetObj.GetTotalPointAmount();
                string strXml = Serializer.Serialize(posnetObj).InnerXml;
                LogPointQuery(strXml, PosBanks.Yapikredi);
                SetTotalPointAddSession(KULLANILABILIRBONUS);
            }
        }
        private ContentResult YkbPayment()
        {
            Boolean result = false;
            C_Posnet posnetObj = new C_Posnet();
            C_PosnetIP ip = new C_PosnetIP();
            C_MerchantInfo mi = new C_MerchantInfo();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            ip.SetMid(PosBankDetails.StoreNumber);
            ip.SetTid(PosBankDetails.TerminalNo);
            posnetObj.SetURL(PosBankDetails.PosBank.XmlUrl);

            posnetObj.SetMid(PosBankDetails.StoreNumber);
            posnetObj.SetTid(PosBankDetails.TerminalNo);

            String pan = PosPaymentValue.CardNumber;

            string amount = PosPaymentValue.TotalPrice.ToString("N2").Replace(".", ",");
            string Installment = PosPaymentValue.Installment.ToString();
            string ErrMsg = "";

            string generat = GenerateOrderId();
            string orderID = "YKB_0000" + generat;

            if (orderID.Length > 24)
            {
                orderID = orderID.Substring(0, 24);
            }
            if (orderID.Length < 24)
            {
                for (int i = orderID.Length; i < 24; i++)
                {
                    Random r = new Random();
                    orderID += r.Next(0, 9).ToString();
                }
            }

            if (amount.Contains(",") || amount.Contains(".") || amount.Contains(" "))
            {
                amount = amount.Replace(",", "");
                amount = amount.Replace(".", "");
                amount = amount.Trim();
            }
            if (Installment == "1")
                Installment = "00";
            string date = PosPaymentValue.ExpYear.Replace(" ", "") + PosPaymentValue.ExpMounth.Replace(" ", "");
            string cvc = PosPaymentValue.Cvc;

            if (Installment != "00") { posnetObj.SetKOICode("1"); }
            EPaymentLog log = new EPaymentLog(PosPaymentValue, posnetObj.GetXMLRequest(), "", "", "", "", "", 98);//98 işlemin bankaya gittiğini gösterir
            EPaymentLogger.LogDB(log, "/api/payment/updatestatus");
            result = posnetObj.DoSaleTran(pan, date, cvc, orderID, amount, "TL", Installment);


            String authCode = "";
            String referansNo = "";
            String app = posnetObj.GetApprovedCode().ToString();
            string pProcreturnCode = "";
            if (app == "1")
            {
                pProcreturnCode = "00";
            }
            else
            {
                pProcreturnCode = app;
            }
            if (posnetObj.GetApprovedCode() == "1" || posnetObj.GetApprovedCode() == "2")
            {
                if (posnetObj.GetApprovedCode() == "2")
                {
                    ErrMsg = "Daha önce yapılmış İşlem.Error Code : " + posnetObj.GetResponseCode() + " Error thisssage : " + posnetObj.GetResponseText();
                }
                if (posnetObj.GetAuthcode().Trim() != "")
                {
                    authCode = posnetObj.GetAuthcode().ToString();
                }
                if (posnetObj.GetHostlogkey().Trim() != "")
                {
                    referansNo = posnetObj.GetHostlogkey().ToString();
                }
            }
            else
            {
                ErrMsg = "Onaylanmadı.Error Code : " + posnetObj.GetResponseCode() + " Error thisssage : " + posnetObj.GetResponseText();
            }
            EPaymentLog log_return = new EPaymentLog(PosPaymentValue, "", posnetObj.GetXMLResponse(), authCode, pProcreturnCode, posnetObj.GetResponseText(), orderID, 97);//97 işlemin bankadan geldiğini gösterir
            EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");
            Logger.LogPayment(LogPaymentType.Information, "YKb NONSECURE Islem Sonucu",
                       " XML Request : " + posnetObj.GetXMLResponse(), PosPaymentValue.PaymentId, PosPaymentValue.Bank
                    );
            EPayment.Update(PosPaymentValue.InsertId, authCode, pProcreturnCode, ErrMsg, PosPaymentValue.IpAddress, orderID, orderID, "", referansNo, "", PosBankDetails.PosBankName, DateTime.Now, 27);
            return CloseFancybox(PosPaymentValue.PaymentId);
        }
        private ContentResult KuveytTurk()
        {

            Decimal Amount = Convert.ToDecimal(PosPaymentValue.TotalPrice.ToString().Replace(",", "").Replace(".", ""));


            string CardHolderName = PosPaymentValue.NameSurname;
            string CardNumber = PosPaymentValue.CardNumber;
            string CardExpireDateMonth = PosPaymentValue.ExpMounth.Replace(" ", "");
            string CardExpireDateYear = PosPaymentValue.ExpYear.Replace(" ", "");
            string CardCVV2 = PosPaymentValue.Cvc;
            string MerchantOrderId = PosPaymentValue.PaymentId;
            string APIVersion = "1.0.0";
            string XType = "Sale";
            string CurrencyCode = "0949"; //TL islemleri için
            string CustomerId = PosBankDetails.TerminalNo; //Müsteri Numarasi->Bizdeki Terminal numarası
            string MerchantId = PosBankDetails.StoreNumber; //Magaza Kodu

            string OkUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/Payment/KuvetyturkPos";
            string FailUrl = Request.Url.Scheme + "://" + Request.Url.Authority + "/Payment/Error?paymentId=" + PosPaymentValue.PaymentId;



            string UserName = PosBankDetails.ApiUser; //  api rollü kullanici
            string Password = PosBankDetails.Password;//  api rollü kullanici sifresi
            SHA1 sha = new SHA1CryptoServiceProvider();
            string XHashedPassword = Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(Password)));
            SHA1 sha2 = new SHA1CryptoServiceProvider();
            string XHashData = Convert.ToBase64String(sha2.ComputeHash(Encoding.UTF8.GetBytes(MerchantId + MerchantOrderId + Amount + OkUrl + FailUrl + UserName + XHashedPassword)));
            string gServer = PosBankDetails.PosBank.XmlUrl;

            string Installment = PosPaymentValue.Installment.ToString();
            Installment = Installment == "1" ? "0" : Installment;

            string postdata = "<KuveytTurkVPosMessage xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema'>";
            postdata = postdata + "<APIVersion>1.0.0</APIVersion>";
            postdata = postdata + "<OkUrl>" + OkUrl + "</OkUrl>";
            postdata = postdata + "<FailUrl>" + FailUrl + "</FailUrl>";
            postdata = postdata + "<HashData>" + XHashData + "</HashData>";
            postdata = postdata + "<MerchantId>" + MerchantId + "</MerchantId>";
            postdata = postdata + "<CustomerId>" + CustomerId + "</CustomerId>";
            postdata = postdata + "<UserName>" + UserName + "</UserName>";
            postdata = postdata + "<CardNumber>" + CardNumber + "</CardNumber>";
            postdata = postdata + "<CardExpireDateYear>" + CardExpireDateYear + "</CardExpireDateYear>";
            postdata = postdata + "<CardExpireDateMonth>" + CardExpireDateMonth + "</CardExpireDateMonth>";
            postdata = postdata + "<CardCVV2>" + CardCVV2 + "</CardCVV2>";
            postdata = postdata + "<CardHolderName>" + CardHolderName + "</CardHolderName>";
            postdata = postdata + "<CardType>" + PosPaymentValue.CardType.ToString() + "</CardType>";
            postdata = postdata + "<BatchID>0</BatchID>";
            postdata = postdata + "<TransactionType>" + "Sale" + "</TransactionType>";
            postdata = postdata + "<InstallmentCount>" + Installment + "</InstallmentCount>";
            postdata = postdata + "<Amount>" + Amount + "</Amount>";
            postdata = postdata + "<DisplayAmount>" + Amount + "</DisplayAmount>";
            postdata = postdata + "<CurrencyCode>" + CurrencyCode + "</CurrencyCode>";
            postdata = postdata + "<MerchantOrderId>" + MerchantOrderId + "</MerchantOrderId>";
            postdata = postdata + "<TransactionSecurity>1</TransactionSecurity>";
            postdata = postdata + "</KuveytTurkVPosMessage>";

            string proxyValue = "";
            byte[] buffer = Encoding.UTF8.GetBytes(postdata);
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(gServer);
            WebReq.Timeout = 5 * 60 * 1000;

            WebReq.Method = "POST";
            WebReq.ContentType = "application/x-www-form-urlencoded";
            WebReq.ContentLength = buffer.Length;

            WebReq.CookieContainer = new CookieContainer();
            Stream ReqStream = WebReq.GetRequestStream();
            ReqStream.Write(buffer, 0, buffer.Length);

            ReqStream.Close();

            WebResponse WebRes = WebReq.GetResponse();
            Stream ResStream = WebRes.GetResponseStream();
            StreamReader ResReader = new StreamReader(ResStream);
            string responseString = ResReader.ReadToEnd();


            string ResponseCode = "";
            string ResponseMessage = "";
            string ProvisionNumber = "";
            string OrderId = "";

            HtmlDocument document = new HtmlDocument();
            string htmlString = responseString;
            document.LoadHtml(htmlString);
            HtmlNodeCollection collection = document.DocumentNode.SelectNodes("//input");
            foreach (HtmlNode link in collection)
            {
                string deger = link.Attributes["name"].Value;
                switch (deger)
                {
                    case "ResponseCode":
                        ResponseCode = link.Attributes["value"].Value;
                        break;
                    case "ResponseMessage":
                        ResponseMessage = link.Attributes["value"].Value;
                        break;
                    case "ProvisionNumber":
                        ProvisionNumber = link.Attributes["value"].Value;
                        break;
                    case "OrderId":
                        OrderId = link.Attributes["value"].Value;
                        break;
                    default:
                        break;
                }

            }
            EPaymentLog log_return = new EPaymentLog(PosPaymentValue, "", responseString, ProvisionNumber, ResponseCode, ResponseMessage, OrderId, 97);//97 işlemin bankadan geldiğini gösterir
            EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");
            Logger.LogPayment(LogPaymentType.Information, "Kuveytturk Islem Sonucu",
                " XML Response : " + responseString, PosPaymentValue.PaymentId, PosPaymentValue.Bank);
            EPayment.Update(PosPaymentValue.InsertId, ProvisionNumber, ResponseCode, ResponseMessage, PosPaymentValue.IpAddress, OrderId, MerchantOrderId, string.Empty, string.Empty, string.Empty, PosBankDetails.PosBankName, DateTime.Now, 37);
            return CloseFancybox(PosPaymentValue.PaymentId);
        }
        private ContentResult Albaraka()
        {
            string installment = PosPaymentValue.Installment == 1 ? "" : PosPaymentValue.Installment.ToString();
            string expdate = (PosPaymentValue.ExpYear + PosPaymentValue.ExpMounth).Replace(" ", "");
            string orderID = PosPaymentValue.PaymentId.Replace("-", "").Substring(0, 24);
            string koiCode = ""; // Ek taksit var ise 1 gönderilecek.
            if (PosPaymentValue.ExtraInstallment > 0)
            {
                koiCode = "1";

            }
            string merchantId = PosBankDetails.StoreNumber;
            string password = PosBankDetails.Password;
            string XML = "";
            XML += "<?xml version='1.0' encoding='iso-8859-9'?>";
            XML += "<posnetRequest>";
            XML += "<mid" + merchantId + "mid>";
            XML += "<tid" + password + "tid>";
            XML += "<sale>";
            XML += "<amount>" + PosPaymentValue.TotalPrice.ToString().Replace(",", "") + "</amount>";
            XML += "<ccno>" + PosPaymentValue.CardNumber + "</ccno>";
            XML += "<cvc>" + PosPaymentValue.Cvc + "</cvc>";
            XML += "<expDate>" + expdate + "</expDate>";
            XML += "<installment>" + installment + "</installment>";
            if (koiCode != "")
                XML += "<koiCode>" + koiCode + "</koiCode>";
            XML += "<orderID>" + PosPaymentValue.PaymentId.Replace("-", "").Substring(0, 24) + "</orderID>";
            XML += "<currencyCode>YT</currencyCode>";
            XML += "<specialMessage>CustomerCode:" + PosPaymentValue.CustomerCode + "</specialMessage>";
            XML += "</sale>";
            XML += "</posnetRequest>";
            WebRequest request = WebRequest.Create(PosBankDetails.PosBank.XmlUrl + XML);
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(XML);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(responseFromServer);
            XmlNodeList list = xDoc.GetElementsByTagName("posnetResponse");
            list = xDoc.GetElementsByTagName("approved");
            String Approved = list[0].InnerText;


            string AuthCode = "";
            string Hostlogkey = "";
            string RespCode = "";
            string RespText = "";
            Logger.LogPayment(LogPaymentType.Information, "Albaraka Islem Sonucu",
             " XML Response : " + xDoc.InnerXml, PosPaymentValue.PaymentId, "Albaraka");

            if (Approved == "1")
            {
                list = xDoc.GetElementsByTagName("authCode");
                AuthCode = list[0].InnerText;
                list = xDoc.GetElementsByTagName("hostlogkey");
                Hostlogkey = list[0].InnerText;
                EPayment.Update(PosPaymentValue.InsertId, AuthCode, "00", (RespText), PosPaymentValue.IpAddress, orderID, orderID, String.Empty, String.Empty, RespText, PosBankDetails.PosBankName, DateTime.Now, 25);
            }
            else
            {
                list = xDoc.GetElementsByTagName("respCode");
                RespCode = list[0].InnerText;
                list = xDoc.GetElementsByTagName("respText");
                RespText = list[0].InnerText;
                EPayment.Update(PosPaymentValue.InsertId, AuthCode, RespCode, (RespCode + RespText), PosPaymentValue.IpAddress, orderID, orderID, String.Empty, String.Empty, RespText, PosBankDetails.PosBankName, DateTime.Now, 25);
            }
            Logger.LogPayment(LogPaymentType.Information, "Albaraka Islem Sonucu",
                " XML Response : " + xDoc.InnerXml, PosPaymentValue.PaymentId, PosPaymentValue.Bank);
            EPaymentLog log_return = new EPaymentLog(PosPaymentValue, "", xDoc.InnerXml, AuthCode, RespCode, RespText, orderID, 97);//97 işlemin bankadan geldiğini gösterir
            EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");
            return CloseFancybox(PosPaymentValue.PaymentId);

        }

        #region ISBANK
        private ContentResult IsBank()
        {
            string MerchantId = PosBankDetails.StoreNumber;
            string Password = PosBankDetails.Password;
            string CurrencyCode = "949";
            String PosURL = PosBankDetails.PosBank.XmlUrl;
            String PosXML = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                           "<VposRequest>\n" +
                           "<MerchantId>" + MerchantId + "</MerchantId>\n" +
                           "<Password>" + Password + "</Password>\n" +
                           "<BankId>1</BankId>\n" +
                           "<TransactionType>Sale</TransactionType>\n" +
                           "<TransactionId>" + PosPaymentValue.PaymentId + "</TransactionId>\n" +
                           "<CurrencyAmount>" + PosPaymentValue.TotalPrice.ToString("0.00").Replace(",", ".") + "</CurrencyAmount>\n" +
                           "<CurrencyCode>" + CurrencyCode + "</CurrencyCode>\n" +
                           "<Pan>" + PosPaymentValue.CardNumber + "</Pan>\n" +
                           "<Cvv>" + PosPaymentValue.Cvc + "</Cvv>\n" +
                           "<InstallmentCount>" + PosPaymentValue.Installment + "</InstallmentCount>\n" +
                           "<Expiry>" + "20" + PosPaymentValue.ExpYear.Trim() + PosPaymentValue.ExpMounth.Trim() + "</Expiry>\n" +
                           "</VposRequest>\n";
            #region Log_XML

            String PosXML_LOG = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                          "<VposRequest>\n" +
                          "<MerchantId>" + MerchantId + "</MerchantId>\n" +
                          "<Password>" + Password + "</Password>\n" +
                          "<BankId>1</BankId>\n" +
                          "<TransactionType>Sale</TransactionType>\n" +
                          "<TransactionId>" + PosPaymentValue.PaymentId + "</TransactionId>\n" +
                          "<CurrencyAmount>" + PosPaymentValue.TotalPrice.ToString("0.00").Replace(",", ".") + "</CurrencyAmount>\n" +
                          "<CurrencyCode>" + CurrencyCode + "</CurrencyCode>\n" +
                          "<Pan>" + PosPaymentValue.CardNumber.Substring(0, 6) + "******" + PosPaymentValue.CardNumber.Substring(PosPaymentValue.CardNumber.Length - 4, 4) + "</Pan>\n" +
                          "<Cvv>" + PosPaymentValue.Cvc + "</Cvv>\n" +
                          "<InstallmentCount>" + PosPaymentValue.Installment + "</InstallmentCount>\n" +
                          "<Expiry>" + "20" + PosPaymentValue.ExpYear.Trim() + PosPaymentValue.ExpMounth.Trim() + "</Expiry>\n" +
                          "</VposRequest>\n";
            #endregion

            string response = GetResponseText(PosURL, "prmstr=" + PosXML);
            XmlDocument xmlDoc;
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            string ResultCode = "";
            string TransactionId = "";
            string ResultDetail = "";
            string AuthCode = "";
            string HostDate = "";
            string Rrn = "";


            XmlNodeList nodeResultCode = xmlDoc.GetElementsByTagName("ResultCode");
            XmlNodeList nodeTransactionId = xmlDoc.GetElementsByTagName("TransactionId");
            XmlNodeList nodeResultDetail = xmlDoc.GetElementsByTagName("ResultDetail");
            XmlNodeList nodeAuthCode = xmlDoc.GetElementsByTagName("AuthCode");
            XmlNodeList nodeHostDate = xmlDoc.GetElementsByTagName("HostDate");
            XmlNodeList nodeRrn = xmlDoc.GetElementsByTagName("Rrn");

            ResultCode = nodeResultCode[0].InnerText;
            TransactionId = nodeTransactionId[0].InnerText;
            ResultDetail = nodeResultDetail[0].InnerText;
            AuthCode = nodeAuthCode[0].InnerText;
            HostDate = nodeHostDate[0].InnerText;
            Rrn = nodeRrn[0].InnerText;
            Logger.LogPayment(LogPaymentType.Information, "Isbank Innova NonSecure Islem Sonucu",
           " XML Response : " + xmlDoc.InnerXml, PosPaymentValue.PaymentId, PosPaymentValue.Bank);
            EPaymentLog log_return = new EPaymentLog(PosPaymentValue, "", PosXML_LOG, AuthCode, ResultCode, ResultDetail, TransactionId, 97);//97 işlemin bankadan geldiğini gösterir
            EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");

            if (ResultCode == "0000")
            {

                EPayment.Update(PosPaymentValue.InsertId, AuthCode, "00", String.Empty, PosPaymentValue.IpAddress, String.Empty, String.Empty, String.Empty, TransactionId, "Giden XML : " + PosXML_LOG.ToString() + "Gelen Sonuc :" + response.ToString(), PosBankDetails.PosBankName, DateTime.Now, 21);

                return CloseFancybox(PosPaymentValue.PaymentId);


            }
            else
            {
                EPayment.Update(PosPaymentValue.InsertId, AuthCode, ResultCode, ResultDetail, PosPaymentValue.IpAddress, String.Empty, String.Empty, String.Empty, TransactionId, "Giden XML : " + PosXML_LOG.ToString() + "Gelen Sonuc :" + response.ToString(), PosBankDetails.PosBankName, DateTime.Now, 21);

                return CloseFancybox(PosPaymentValue.PaymentId);

            }
        }
        protected string GetResponseText(string url, string postData)
        {
            string responseText = String.Empty;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 59000;
                request.Credentials = CredentialCache.DefaultCredentials;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;
                using (StreamWriter sw = new StreamWriter(request.GetRequestStream()))
                {
                    sw.Write(postData);
                }
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    responseText = sr.ReadToEnd();
                    sr.Close();
                }
                response.Close();
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message.ToString());
            }
            return responseText;
        }
        #endregion
        private ContentResult Vakifbank()
        {

            string amount;
            string resultCode = "";
            string resultDescription = "";
            string authCode = "";
            string pointTransactionId = Guid.NewGuid().ToString("N");
            if (PosPaymentValue.UseBonus)//Bonus Kullalanıyor ise normal işlemden bonus tuturını çıkarak gönderiyorum
                amount = (PosPaymentValue.TotalPrice - Convert.ToDouble(PosPaymentValue.ExpendableBonus.Replace(".", ","))).ToString().Replace(",", ".");
            else
                amount = PosPaymentValue.TotalPrice.ToString().Replace(",", ".");

            #region Puan Kullandırım işlemleri 

            if (PosPaymentValue.UseBonus)
            {
                string amountPoint = PosPaymentValue.ExpendableBonus.ToString().Replace(",", ".");
                string xmlMessagePoint = GenerateVakifbankPaymentXmlDocument(amountPoint, TransactionType.PointSale, pointTransactionId);

                string responseFromServerPoint = VakıfbankNonSecureWebRequest(xmlMessagePoint);


                if (string.IsNullOrEmpty(responseFromServerPoint))
                {
                    return CloseFancybox(PosPaymentValue.PaymentId);
                }
                var xmlResponsePoint = new XmlDocument();
                xmlResponsePoint.LoadXml(responseFromServerPoint);
                var resultCodeNodePoint = xmlResponsePoint.SelectSingleNode("VposResponse/ResultCode");
                var resultDescriptionNodePoint = xmlResponsePoint.SelectSingleNode("VposResponse/ResultDetail");
                var authCodeNodePoint = xmlResponsePoint.SelectSingleNode("VposResponse/AuthCode");
                var pointAmountNodePoint = xmlResponsePoint.SelectSingleNode("VposResponse/PointAmount");
                var totalPointNodePoint = xmlResponsePoint.SelectSingleNode("VposResponse/TotalPoint");
                string resultCodePoint = "";
                string resultDescriptionPoint = "";
                string authCodePoint = "";
                string authCopointAmountPoint = "";
                string totalPoint = "";

                try
                {
                    authCodePoint = authCodeNodePoint.InnerText;
                }
                catch (Exception)
                {
                    authCodePoint = "";
                }

                if (resultCodeNodePoint != null)
                {
                    resultCodePoint = resultCodeNodePoint.InnerText;
                }
                if (resultDescriptionNodePoint != null)
                {
                    resultDescriptionPoint = resultDescriptionNodePoint.InnerText;
                }

                if (pointAmountNodePoint != null)
                {
                    authCopointAmountPoint = pointAmountNodePoint.InnerText;
                }
                if (totalPointNodePoint != null)
                {
                    totalPoint = totalPointNodePoint.InnerText;
                }

                Logger.LogPayment(LogPaymentType.Information, "VakıfBank Puan Kullanma Islem Sonucu",
                    " XML Response : " + xmlResponsePoint.InnerXml, PosPaymentValue.PaymentId, "Vakıfbank");
                string paymentId = Guid.NewGuid().ToString();
                string pMaskedCardNumber = PosPaymentValue.CardNumber.Substring(0, 6) + "******" +
                                           PosPaymentValue.CardNumber.Substring(PosPaymentValue.CardNumber.Length - 4, 4);

                DataTable dtInsert = EPayment.Insert(pMaskedCardNumber, PosPaymentValue.NameSurname,
                    PosPaymentValue.ExpMounth, PosPaymentValue.ExpYear, PosPaymentValue.Cvc,
                    PosPaymentValue.TotalPrice + " TL", PosPaymentValue.Installment.ToString(), CurrentCustomer.Id,
                    CurrentCustomer.Code, CurrentCustomer.Name, PosPaymentValue.Bank, PosPaymentValue.TotalPrice,
                    PosPaymentValue.CommissionRate.ToString(), PosPaymentValue.BankId, paymentId, (int)SystemType.Web,
                    PosPaymentValue.PhoneNumber, PosPaymentValue.Explanation, PosPaymentValue.CardType.ToString(),
                    CurrentSalesmanId, PosPaymentValue.ExtraInstallment.ToString(), PosPaymentValue.TotalBonus,
                    PosPaymentValue.ExpendableBonus, PosPaymentValue.UseBonus, PosPaymentValue._3DSecure,
                    PosPaymentValue.InsertId);
                PosPaymentValue.InsertPointId = Convert.ToInt32(dtInsert.Rows[0][0].ToString());
                if (resultCodePoint != "0000")
                {
                    string sonuc = "işlem Başarısız " + resultDescriptionPoint;
                    EPayment.Update(PosPaymentValue.InsertPointId, " ", resultCodePoint, sonuc,
                        PosPaymentValue.IpAddress,
                        string.Empty, string.Empty, paymentId, string.Empty, xmlResponsePoint.InnerXml,
                        PosBankDetails.PosBankName, DateTime.Now, 26);
                    sonuc += "işlem Başarısız Puan Kullanımına Onay Verilmedi Puan kullanmadan işlem deneyiniz!";
                    EPayment.Update(PosPaymentValue.InsertId, " ", resultCodePoint, sonuc,
                        PosPaymentValue.IpAddress,
                        string.Empty, string.Empty, paymentId, string.Empty, xmlResponsePoint.InnerXml,
                        PosBankDetails.PosBankName, DateTime.Now, 26);
                }
                else
                {
                    EPayment.Update(PosPaymentValue.InsertPointId, authCodePoint, "00", resultCodePoint, PosPaymentValue.IpAddress,
                        string.Empty, string.Empty, paymentId, string.Empty, xmlResponsePoint.InnerXml,
                        PosBankDetails.PosBankName, DateTime.Now, 26);

                    if ((PosPaymentValue.TotalPrice - Convert.ToDouble(PosPaymentValue.ExpendableBonus.Replace(".", ","))) > 0)
                    {
                        #region Puan Kullanımından Sonra Kalan Ekstra 

                        string xmlMessage = GenerateVakifbankPaymentXmlDocument(amount, TransactionType.Sale);
                        string responseFromServer = VakıfbankNonSecureWebRequest(xmlMessage);

                        if (string.IsNullOrEmpty(responseFromServer))
                        {
                            return CloseFancybox(PosPaymentValue.PaymentId);
                        }
                        var xmlResponse = new XmlDocument();
                        xmlResponse.LoadXml(responseFromServer);
                        var resultCodeNode = xmlResponse.SelectSingleNode("VposResponse/ResultCode");
                        var resultDescriptionNode = xmlResponse.SelectSingleNode("VposResponse/ResultDescription");
                        var authCodeNode = xmlResponse.SelectSingleNode("VposResponse/AuthCode");

                        Logger.LogPayment(LogPaymentType.Information, "VAkıfBank NonSecure Islem Sonucu",
                            " XML Response : " + xmlResponse.InnerXml, PosPaymentValue.PaymentId, "Vakıfbank");

                        try
                        {
                            authCode = authCodeNode.InnerText;
                        }
                        catch (Exception)
                        {
                            authCode = "";
                        }

                        if (resultCodeNode != null)
                        {
                            resultCode = resultCodeNode.InnerText;
                        }
                        if (resultDescriptionNode != null)
                        {
                            resultDescription = resultDescriptionNode.InnerText;
                        }
                        EPaymentLog log_return = new EPaymentLog(PosPaymentValue, "", xmlResponse.InnerXml, authCode,
                            resultCode, resultDescription, "", 97); //97 işlemin bankadan geldiğini gösterir
                        EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");
                        if (resultCode != "0000")
                        {
                            string sonuc = "işlem Başarısız " + resultDescription;
                            EPayment.Update(PosPaymentValue.InsertId, " ", resultCode, sonuc, PosPaymentValue.IpAddress,
                                string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty,
                                xmlResponse.InnerXml,
                                PosBankDetails.PosBankName, DateTime.Now, 26);


                            string xmlCancelMessage = "";
                            xmlCancelMessage += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
                            xmlCancelMessage += "<VposRequest>";
                            xmlCancelMessage += "<MerchantId>" + PosBankDetails.StoreNumber + "</MerchantId>";
                            xmlCancelMessage += "<Password>" + PosBankDetails.Password + "</Password>";
                            xmlCancelMessage += "<TransactionType>Cancel</TransactionType>";
                            xmlCancelMessage +=
                                "<ReferenceTransactionId>" + pointTransactionId + "</ReferenceTransactionId>";
                            xmlCancelMessage += "<ClientIp>190.20.13.12</ClientIp>";
                            xmlCancelMessage += "</VposRequest>";


                            string responseFromServerCancel = VakıfbankNonSecureWebRequest(xmlCancelMessage);

                            var xmlResponseCancel = new XmlDocument();
                            xmlResponseCancel.LoadXml(responseFromServerCancel);
                            var resultCodeNodeCancel = xmlResponseCancel.SelectSingleNode("VposResponse/ResultCode");
                            var resultDescriptionNodeCancel =
                                xmlResponseCancel.SelectSingleNode("VposResponse/ResultDetail");
                            var authCodeNodeCancel = xmlResponseCancel.SelectSingleNode("VposResponse/AuthCode");

                            try
                            {
                                authCode = authCodeNodeCancel.InnerText;
                            }
                            catch (Exception)
                            {
                                authCode = "";
                            }

                            if (resultCodeNodeCancel != null)
                            {
                                resultCode = resultCodeNodeCancel.InnerText;
                            }
                            if (resultDescriptionNodeCancel != null)
                            {
                                resultDescription = resultDescriptionNodeCancel.InnerText;
                            }
                            paymentId = Guid.NewGuid().ToString();
                            DataTable dtInsertCancel = EPayment.Insert(pMaskedCardNumber, PosPaymentValue.NameSurname,
                                PosPaymentValue.ExpMounth, PosPaymentValue.ExpYear, PosPaymentValue.Cvc,
                                PosPaymentValue.TotalPrice + " TL", PosPaymentValue.Installment.ToString(),
                                CurrentCustomer.Id,
                                CurrentCustomer.Code, CurrentCustomer.Name, PosPaymentValue.Bank,
                                PosPaymentValue.TotalPrice,
                                PosPaymentValue.CommissionRate.ToString(), PosPaymentValue.BankId, paymentId,
                                (int)SystemType.Web,
                                PosPaymentValue.PhoneNumber, PosPaymentValue.Explanation,
                                PosPaymentValue.CardType.ToString(),
                                CurrentSalesmanId, PosPaymentValue.ExtraInstallment.ToString(),
                                PosPaymentValue.TotalBonus,
                                PosPaymentValue.ExpendableBonus, PosPaymentValue.UseBonus, PosPaymentValue._3DSecure,
                                PosPaymentValue.InsertId);
                            PosPaymentValue.InsertCancelId = Convert.ToInt32(dtInsertCancel.Rows[0][0]);
                            if (resultCode != "0000")
                            {
                                //iptal işlemi başarısız
                                //0707 Manuel İpral Edilmesi Gereken işlem
                                sonuc = "işlem Başarısız " + resultDescription +
                                        " Manuel İptal Gerekli Firma Yetkilisi ile Görüşünüz.";
                                EPayment.Update(PosPaymentValue.InsertCancelId, " ", "0707", sonuc,
                                    PosPaymentValue.IpAddress,
                                    string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty,
                                    xmlResponse.InnerXml,
                                    PosBankDetails.PosBankName, DateTime.Now, 26);
                            }
                            else
                            {
                                EPayment.Update(PosPaymentValue.InsertCancelId, authCode, "00", resultCode,
                                    PosPaymentValue.IpAddress,
                                    string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty,
                                    xmlResponse.InnerXml,
                                    PosBankDetails.PosBankName, DateTime.Now, 26);

                            }
                        }

                        else
                        {
                            EPayment.Update(PosPaymentValue.InsertId, authCode, "00", resultCode,
                                PosPaymentValue.IpAddress,
                                string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty,
                                xmlResponse.InnerXml,
                                PosBankDetails.PosBankName, DateTime.Now, 26);

                        }

                        #endregion
                    }
                    else
                    {
                        EPayment.Update(PosPaymentValue.InsertId, authCode, "00", resultCode, PosPaymentValue.IpAddress,
                          string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty, "",
                          PosBankDetails.PosBankName, DateTime.Now, 26);
                    }
                }
            }
            else
            {
                string xmlMessage = GenerateVakifbankPaymentXmlDocument(amount, TransactionType.Sale);

                string responseFromServer = VakıfbankNonSecureWebRequest(xmlMessage);

                if (string.IsNullOrEmpty(responseFromServer))
                {
                    return CloseFancybox(PosPaymentValue.PaymentId);
                }
                var xmlResponse = new XmlDocument();
                xmlResponse.LoadXml(responseFromServer);
                var resultCodeNode = xmlResponse.SelectSingleNode("VposResponse/ResultCode");
                var resultDescriptionNode = xmlResponse.SelectSingleNode("VposResponse/ResultDescription");
                var authCodeNode = xmlResponse.SelectSingleNode("VposResponse/AuthCode");


                Logger.LogPayment(LogPaymentType.Information, "VAkıfBank NonSecure Islem Sonucu",
                    " XML Response : " + xmlResponse.InnerXml, PosPaymentValue.PaymentId, "Vakıfbank");

                try
                {
                    authCode = authCodeNode.InnerText;
                }
                catch (Exception)
                {
                    authCode = "";
                }

                if (resultCodeNode != null)
                {
                    resultCode = resultCodeNode.InnerText;
                }
                if (resultDescriptionNode != null)
                {
                    resultDescription = resultDescriptionNode.InnerText;
                }
                EPaymentLog log_return = new EPaymentLog(PosPaymentValue, "", xmlResponse.InnerXml, authCode,
                    resultCode, resultDescription, "", 97); //97 işlemin bankadan geldiğini gösterir
                EPaymentLogger.LogDB(log_return, "/api/payment/updatereturnstatus");
                if (resultCode != "0000")
                {
                    string sonuc = "işlem Başarısız " + resultDescription;
                    EPayment.Update(PosPaymentValue.InsertId, " ", resultCode, sonuc, PosPaymentValue.IpAddress,
                        string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty, xmlResponse.InnerXml,
                        PosBankDetails.PosBankName, DateTime.Now, 26);

                }
                else
                {
                    EPayment.Update(PosPaymentValue.InsertId, authCode, "00", resultCode, PosPaymentValue.IpAddress,
                        string.Empty, string.Empty, PosPaymentValue.PaymentId, string.Empty, xmlResponse.InnerXml,
                        PosBankDetails.PosBankName, DateTime.Now, 26);
                    //Daha sonra  burayı düzenle bankadan gelen cevaba göre


                }
            }
            #endregion

            return CloseFancybox(PosPaymentValue.PaymentId);
        }

        public string GenerateVakifbankPaymentXmlDocument(string amount, TransactionType transactionType, string transactionId = "")
        {
            string expriy = "20" + PosPaymentValue.ExpYear.Replace(" ", "") + PosPaymentValue.ExpMounth.Replace(" ", "");


            XmlDocument xmlDoc = new XmlDocument();

            XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);

            XmlElement rootNode = xmlDoc.CreateElement("VposRequest");
            xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
            xmlDoc.AppendChild(rootNode);

            if (transactionType == TransactionType.Sale && PosPaymentValue.Installment.ToString() != "1")
            {
                XmlElement NumberOfInstallmentsNode = xmlDoc.CreateElement("NumberOfInstallments");
                XmlText NumberOfInstallmentsText = xmlDoc.CreateTextNode(PosPaymentValue.Installment.ToString());
                rootNode.AppendChild(NumberOfInstallmentsNode);
                NumberOfInstallmentsNode.AppendChild(NumberOfInstallmentsText);

            }

            XmlElement merchantNode = xmlDoc.CreateElement("MerchantId");
            XmlElement passwordNode = xmlDoc.CreateElement("Password");
            XmlElement terminalNode = xmlDoc.CreateElement("TerminalNo");
            XmlElement transactionTypeNode = xmlDoc.CreateElement("TransactionType");
            XmlElement transactionIdNode = xmlDoc.CreateElement("TransactionId");
            XmlElement currencyAmountNode = xmlDoc.CreateElement("CurrencyAmount");
            XmlElement currencyCodeNode = xmlDoc.CreateElement("CurrencyCode");
            XmlElement panNode = xmlDoc.CreateElement("Pan");
            XmlElement cvvNode = xmlDoc.CreateElement("Cvv");
            XmlElement expiryNode = xmlDoc.CreateElement("Expiry");
            XmlElement ClientIpNode = xmlDoc.CreateElement("ClientIp");
            XmlElement transactionDeviceSourceNode = xmlDoc.CreateElement("TransactionDeviceSource");

            XmlText merchantText = xmlDoc.CreateTextNode(PosBankDetails.StoreNumber);
            XmlText passwordtext = xmlDoc.CreateTextNode(PosBankDetails.Password);
            XmlText terminalNoText = xmlDoc.CreateTextNode(PosBankDetails.TerminalNo);
            XmlText transactionTypeText = xmlDoc.CreateTextNode(transactionType.ToString());
            XmlText transactionIdText = xmlDoc.CreateTextNode(transactionId ?? Guid.NewGuid().ToString("N"));
            //uniqe olacak şekilde düzenleyebilirsiniz.
            XmlText currencyAmountText = xmlDoc.CreateTextNode(amount);
            //tutarı nokta ile gönderdiğinizden emin olunuz.
            XmlText currencyCodeText = xmlDoc.CreateTextNode("949");
            XmlText panText = xmlDoc.CreateTextNode(PosPaymentValue.CardNumber);
            XmlText cvvText = xmlDoc.CreateTextNode(PosPaymentValue.Cvc);
            XmlText expiryText = xmlDoc.CreateTextNode(expriy);
            XmlText ClientIpText = xmlDoc.CreateTextNode("190.20.13.12");
            XmlText transactionDeviceSourceText = xmlDoc.CreateTextNode("0");


            rootNode.AppendChild(merchantNode);
            rootNode.AppendChild(passwordNode);
            rootNode.AppendChild(terminalNode);
            rootNode.AppendChild(transactionTypeNode);
            rootNode.AppendChild(transactionIdNode);
            rootNode.AppendChild(currencyAmountNode);
            rootNode.AppendChild(currencyCodeNode);
            rootNode.AppendChild(panNode);
            rootNode.AppendChild(cvvNode);
            rootNode.AppendChild(expiryNode);
            rootNode.AppendChild(ClientIpNode);
            rootNode.AppendChild(transactionDeviceSourceNode);


            merchantNode.AppendChild(merchantText);
            passwordNode.AppendChild(passwordtext);
            terminalNode.AppendChild(terminalNoText);
            transactionTypeNode.AppendChild(transactionTypeText);
            transactionIdNode.AppendChild(transactionIdText);
            currencyAmountNode.AppendChild(currencyAmountText);
            currencyCodeNode.AppendChild(currencyCodeText);
            panNode.AppendChild(panText);
            cvvNode.AppendChild(cvvText);
            expiryNode.AppendChild(expiryText);
            ClientIpNode.AppendChild(ClientIpText);
            transactionDeviceSourceNode.AppendChild(transactionDeviceSourceText);

            return xmlDoc.OuterXml;
        }

        public string VakıfbankNonSecureWebRequest(string xmlMessage)
        {
            string responseFromServer = "";
            byte[] dataStream = Encoding.UTF8.GetBytes("prmstr=" + xmlMessage);
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(PosBankDetails.PosBank.XmlUrl);
            //Vpos adresi
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = dataStream.Length;
            webRequest.KeepAlive = false;
            using (Stream newStream = webRequest.GetRequestStream())
            {
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    responseFromServer = reader.ReadToEnd();
                    reader.Close();
                }

                webResponse.Close();
            }

            return responseFromServer;
        }
        private void VakifbankPointSearch()
        {
            string expriy = "20" + PosPaymentValue.ExpYear.Replace(" ", "") + PosPaymentValue.ExpMounth.Replace(" ", "");

            string xmlMessage = "";
            xmlMessage += "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            xmlMessage += "<VposRequest>";
            xmlMessage += "<MerchantId>" + PosBankDetails.StoreNumber + "</MerchantId>";
            xmlMessage += "<Password>" + PosBankDetails.Password + "</Password>";
            xmlMessage += "<TerminalNo>" + PosBankDetails.TerminalNo + "</TerminalNo>";
            xmlMessage += "<TransactionType>" + TransactionType.PointSearch.ToString() + "</TransactionType>";
            xmlMessage += "<Pan>" + PosPaymentValue.CardNumber + "</Pan>";
            xmlMessage += "<Cvv>" + PosPaymentValue.Cvc + "</Cvv>";
            xmlMessage += "<Expiry>" + expriy + "</Expiry>";
            xmlMessage += "<ClientIp>190.20.13.12</ClientIp>";
            xmlMessage += "<TransactionDeviceSource>0</TransactionDeviceSource>";
            xmlMessage += "</VposRequest>";

            byte[] dataStream = Encoding.UTF8.GetBytes("prmstr=" + xmlMessage);
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(PosBankDetails.PosBank.XmlUrl);//Vpos adresi
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = dataStream.Length;
            webRequest.KeepAlive = false;
            string responseFromServer = "";

            using (Stream newStream = webRequest.GetRequestStream())
            {
                newStream.Write(dataStream, 0, dataStream.Length);
                newStream.Close();
            }

            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    responseFromServer = reader.ReadToEnd();
                    reader.Close();
                }

                webResponse.Close();
            }
            if (string.IsNullOrEmpty(responseFromServer))
            {

            }
            var xmlResponse = new XmlDocument();
            xmlResponse.LoadXml(responseFromServer);
            var resultCodeNode = xmlResponse.SelectSingleNode("VposResponse/ResultCode");
            var resultDescriptionNode = xmlResponse.SelectSingleNode("VposResponse/ResultDetail");
            var authCodeNode = xmlResponse.SelectSingleNode("VposResponse/AuthCode");
            var totalPointNode = xmlResponse.SelectSingleNode("VposResponse/TotalPoint");
            string resultCode = "";
            string resultDescription = "";
            string authCode = "";
            string totalPoint = totalPointNode.InnerText;

            try
            {
                authCode = authCodeNode.InnerText;
            }
            catch (Exception)
            {
                authCode = "";
            }

            if (resultCodeNode != null)
            {
                resultCode = resultCodeNode.InnerText;
            }
            if (resultDescriptionNode != null)
            {
                resultDescription = resultDescriptionNode.InnerText;
            }
            LogPointQuery(xmlResponse.InnerXml, PosBanks.Vakifbank);

            if (resultCode == "0000")
            {
                if (!string.IsNullOrEmpty(totalPoint))
                {
                    PosPaymentValue.TotalBonus = totalPoint.ToString();
                    PosPaymentValue.TotalBonusPrice = Convert.ToDouble(totalPoint.Substring(0, totalPoint.Length - 2)).ToString();
                    PosPaymentValue.TotalBonusDecimal = totalPoint.Substring(totalPoint.Length - 2, 2);
                }

            }


        }
        #endregion

        public ContentResult CloseFancybox(string paymentId)
        {
            string script = "<script type='text/javascript' language='javascript'>window.onload=parent.pageReload('" + paymentId + "');</script>";
            return Content(script);
        }

        #region CustomCheckMethods
        public string GenerateOrderId()
        {
            long i = Guid.NewGuid().ToByteArray().Aggregate<byte, long>(1, (current, b) => current * ((int)b + 1));
            return $"{i - DateTime.Now.Ticks:x}";
        }
        public string GetSHA1(string SHA1Data)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();
            string HashedPassword = SHA1Data;
            byte[] hashbytes = Encoding.GetEncoding("ISO-8859-9").GetBytes(HashedPassword);
            byte[] inputbytes = sha.ComputeHash(hashbytes);
            return GetHexaDecimal(inputbytes);
        }

        public string GetHexaDecimal(byte[] bytes)
        {
            StringBuilder s = new StringBuilder();
            int length = bytes.Length;
            for (int n = 0; n <= length - 1; n++)
            {
                s.Append($"{bytes[n],2:x}".Replace(" ", "0"));
            }
            return s.ToString();
        }
        private PosCardType CartTypeControl(string pType)
        {
            switch (pType)
            {
                case "visa":
                    return PosCardType.Visa;
                case "mastercard":
                    return PosCardType.Master;
                case "maestro":
                    return PosCardType.Maestro;
                case "amex":
                    return PosCardType.Amex;
                default:
                    return PosCardType.None;
            }
        }
        #endregion




    }
}