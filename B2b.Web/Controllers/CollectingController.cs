using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Log;
using B2b.Web.v4.Models.ErpLayer;
using Newtonsoft.Json;

namespace B2b.Web.v4.Controllers
{
    public class CollectingController : BaseController
    {
        public List<Collecting> CollectingHeaderList
        {
            get { return Session["CollectingHeaderList"] as List<Collecting>; }
            set { Session["CollectingHeaderList"] = value; }
        }
        public CollectingHeader CollectingHeader
        {
            get { return Session["CollectingHeader"] as CollectingHeader; }
            set { Session["CollectingHeader"] = value; }
        }
        // GET: Collecting
        public ActionResult Index()
        {
               return View();
        }

        public ActionResult CollectingList()
        {
            if (CurrentLoginType == LoginType.Customer)
            {
                RedirectToAction("Index", "Home");
            }
            return View();
        }

        #region HttpPost Methods
        [HttpPost]
        public JsonResult SaveCollecting(int collectingType, Collecting collecting)
        {
             collecting.Type = collectingType;
            collecting.Status = 0;
            collecting.Amount = Convert.ToDouble(collecting.Amount + "," + collecting.AmountKrs);
            collecting.CreateDate = DateTime.Now;
            collecting.Customer = CurrentCustomer;
            collecting.Salesman = CurrentSalesman;
            Collecting Item = collecting;

            Item.Save();
            MessageBox message = new MessageBox(MessageBoxType.Success, "Kaydınız Alınmıştır");
            return Json(message);
        }

        [HttpPost]
        public string LoadInsertedLines()
        {
             CollectingHeaderList = Collecting.GetListBySalesmanId(CurrentSalesman.Id, CurrentCustomer.Id, CurrentCustomer.Users.Id);
            return JsonConvert.SerializeObject(CollectingHeaderList);
        }
        [HttpPost]
        public JsonResult Delete(int id)
        {
           Collecting.DeleteById(id);
            MessageBox message = new MessageBox(MessageBoxType.Success, "Kaydınız Silinmiştir.");
            return Json(message);
        }
        [HttpPost]
        public JsonResult SendCollecting()
        {
            CollectingHeader = new CollectingHeader()
            {
                CustomerId = CurrentCustomer.Id,
                UserId = CurrentCustomer.Users.Id,
                SalesmanId = CurrentSalesman.Id,
                DocumentNo = CurrentSalesman.Code,
                Status = 99
            };
            CollectingHeader.Save();

            string ids = CollectingHeaderList.Aggregate(string.Empty, (current, item) => current + (item.Id + ","));
            ids = ids.Remove(ids.Length - 1);

            Collecting.UpdateCollectingHeaderId(ids, CollectingHeader.Id, CollectingHeader.DocumentNo, CreatePrnFile(CollectingHeader.Id));

            MessageBox message = new MessageBox(MessageBoxType.Success, "Tahsilatınız başarıyla oluşturulmuştur. Tahsilat Numaranız: " + CollectingHeader.DocumentNo);
            return Json(message);
        }

        [HttpPost]
        public JsonResult GetListSalesman()
        {

            var list = Salesman.GetList();
            list.Insert(0, new Salesman { Id = -1, Name = "Seçiniz" });
            return Json(list);
        }
        [HttpPost]
        public string GetListCollecting(int salesmanId, DateTime? startDate, DateTime? endDate)
        {
             List<Collecting> list = Collecting.GetListByHeaderBySalesmanAndDate(salesmanId,
             Convert.ToDateTime(startDate), Convert.ToDateTime(endDate).AddDays(1));

            return JsonConvert.SerializeObject(list);
        }
        [HttpPost]
        public string GetBankList()
        {
            return JsonConvert.SerializeObject(CollectingBank.GetList());
        }
        [HttpPost]
        public string GetCollectingDetail(int id)
        {
             List<Collecting> list = Collecting.GetListByHeaderId(id);

            return JsonConvert.SerializeObject(list);
        }
        #endregion

        #region Create Prn
        private string CreatePrnFile(int id)
        {
            try
            {
                CollectingHeader vCollectingHeader = CollectingHeader.GetById(id);
                //CompanyInformation compInfo = CompanyInformation.GetByStatus(0);

                //Customer cust = Customer.GetById(collectingHeader.CustomerId, collectingHeader.UserId);
                //Salesman sals = Salesman.GetById(collectingHeader.SalesmanId);

                string pdfId = Guid.NewGuid().ToString();
                string prn = "";
                #region StaticParameters
                prn += "SIZE 69.9 mm, [DOT] dot";
                prn += "DIRECTION 0,0";
                prn += "REFERENCE 0,0";
                prn += "OFFSET 0 mm";
                prn += "SET PEEL OFF";
                prn += "SET CUTTER OFF";
                prn += "SET PARTIAL_CUTTER OFF";
                prn += "SET TEAR ON";
                prn += "CLS";
                prn += "CODEPAGE 1254";
                prn += "TEXT 1,45,\"4\",0,1,1,\"   TAHSİLAT MAKBUZU\"";
                prn += "TEXT 1,105,\"2\",0,1,1,\"TARİH     :[Date]\"";
                prn += "TEXT 1,145,\"2\",0,1,1,\"MAKBUZ NO :[MakbuzNo]\"";
                prn += "TEXT 1,185,\"2\",0,1,1,\"PLASİYER  :[SalesmanName]\"";
                prn += "TEXT 1,225,\"2\",0,1,1,\"----------------------------------------\"";
                prn += "TEXT 1,265,\"2\",0,1,1,\"       Eryaz Bilgi Teknolojileri        \"";
                prn += "TEXT 1,305,\"2\",0,1,1,\"Kandiş Plaza Fevzi Paşa Cd. No:59 Kat:3 \"";
                prn += "TEXT 1,345,\"2\",0,1,1,\" Küçükbakkalköy / Ataşehir / İSTANBUL   \"";
                prn += "TEXT 1,385,\"2\",0,1,1,\"          TEL:0 216 295 09 09\"";
                prn += "TEXT 1,425,\"2\",0,1,1,\"          FAX:0 216 445 09 51\"";
                prn += "TEXT 1,465,\"2\",0,1,1,\"Vergi Dairesi  : ATAŞEHİR\"";
                prn += "TEXT 1,505,\"2\",0,1,1,\"Vergi Hesap No : 333 333 33 33\"";
                prn += "TEXT 1,545,\"2\",0,1,1,\"       www.eryazsoftware.com.tr\"";
                prn += "TEXT 1,585,\"2\",0,1,1,\"----------------------------------------\"";
                prn += "TEXT 1,625,\"3\",0,1,1,\"          CARİ BİLGİLERİ\"";
                prn += "TEXT 1,665,\"2\",0,1,1,\"Cari Kodu : [CustCode]\"";
                prn += "TEXT 1,705,\"2\",0,1,1,\"Ünvanı    : [CustTittle]\"";
                prn += "TEXT 1,745,\"2\",0,1,1,\"Adres     : [Address]\"";
                prn += "TEXT 1,785,\"2\",0,1,1,\"İlçe      : [Town]\"";
                prn += "TEXT 1,825,\"2\",0,1,1,\"İl        : [City]\"";
                prn += "TEXT 1,865,\"2\",0,1,1,\"Vergi D.  : [TaxOffice]\"";
                prn += "TEXT 1,905,\"2\",0,1,1,\"Vergi HN  : [TaxNumber]\"";
                prn += "TEXT 1,945,\"2\",0,1,1,\"----------------------------------------\"";
                prn += "TEXT 1,985,\"3\",0,1,1,\"          TAHSİLAT ÖZETİ\"";
                prn += "TEXT 1,1025,\"3\",0,1,1,\"Nakit    :[TO_NAKTOP]\"";
                prn += "TEXT 1,1065,\"3\",0,1,1,\"Çek      :[TO_CEKTOP]\"";
                prn += "TEXT 1,1105,\"3\",0,1,1,\"Senet    :[TO_SENTOP]\"";
                prn += "TEXT 1,1145,\"3\",0,1,1,\"Toplam   :[TO_TAHTOP]\"";
                prn += "TEXT 1,1185,\"2\",0,1,1,\"----------------------------------------\"";
                prn += "TEXT 1,1225,\"3\",0,1,1,\"          TAHSİLAT DETAYI\"";
                #endregion
                string vPRN = prn;

                #region Firma Bilgileri

                //vPRN = vPRN.Replace(Collecting.FIRMA, CompanyInformationItem.Title);
                vPRN = vPRN.Replace(Collecting.MAKBUZNO, vCollectingHeader.DocumentNo);
                vPRN = vPRN.Replace(Collecting.TARIH, DateTime.Now.ToString("dd.MM.yy HH:mm:ss"));
                vPRN = vPRN.Replace(Collecting.PLASIYER, CurrentSalesman.Code + "  " + CurrentSalesman.Name);
                //vPRN = vPRN.Replace(Collecting.TEL, CompanyInformationItem.Phone1);
                //vPRN = vPRN.Replace(Collecting.FAX, CompanyInformationItem.Fax);
                //vPRN = vPRN.Replace(Collecting.ADRES, CompanyInformationItem.Address);
                //vPRN = vPRN.Replace(Collecting.EMAIL, CompanyInformationItem.Email1);
                #endregion

                #region Cari Bilgileri
                vPRN = vPRN.Replace(Collecting.KODU, CurrentCustomer.Code);
                vPRN = vPRN.Replace(Collecting.UNVANI, CurrentCustomer.Name);
                vPRN = vPRN.Replace(Collecting.ADRES, CurrentCustomer.Address);
                vPRN = vPRN.Replace(Collecting.ILCE, CurrentCustomer.Town);
                vPRN = vPRN.Replace(Collecting.IL, CurrentCustomer.City);
                vPRN = vPRN.Replace(Collecting.VERGIDAIRESI, CurrentCustomer.TaxOffice);
                vPRN = vPRN.Replace(Collecting.VERGINO, CurrentCustomer.TaxNumber);
                #endregion

                #region Tahsilat
                vPRN = vPRN.Replace(Collecting.TO_NAKTOP, AlignRight(vCollectingHeader.CollectingList.Where(c => c.CollectingType == 0).Sum(c => c.Amount).ToString("N2") + " TL", Collecting.MAXROWCOUNT - 10));
                //vPRN = vPRN.Replace(Collecting.KREDIKARTI, vCollectingHeader.CollectingList.Where(c => c.CollectingType == 3).Sum(c => c.Amount).ToString("N2") + " TL");
                //vPRN = vPRN.Replace(Collecting.MAILORDER, vCollectingHeader.CollectingList.Where(c => c.CollectingType == 4).Sum(c => c.Amount).ToString("N2") + " TL");
                vPRN = vPRN.Replace(Collecting.TO_CEKTOP, AlignRight(vCollectingHeader.CollectingList.Where(c => c.CollectingType == 1).Sum(c => c.Amount).ToString("N2") + " TL", Collecting.MAXROWCOUNT - 10));
                vPRN = vPRN.Replace(Collecting.TO_SENTOP, AlignRight(vCollectingHeader.CollectingList.Where(c => c.CollectingType == 2).Sum(c => c.Amount).ToString("N2") + " TL", Collecting.MAXROWCOUNT - 10));
                vPRN = vPRN.Replace(Collecting.TO_TAHTOP, AlignRight(vCollectingHeader.CollectingList.Sum(c => c.Amount).ToString("N2") + " TL", Collecting.MAXROWCOUNT - 10));
                #endregion

                #region Açıklama
                int index = 1;
                int vRow = 0;
                foreach (var item in vCollectingHeader.CollectingList)
                {
                    vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                    switch (item.CollectingType)
                    {
                        case 0:
                            vPRN += Environment.NewLine +
                                    Collecting.TD_TUR.Replace("[TD_TUR]", "NAKİT").Replace("@ROW", vRow.ToString());
                            break;
                        case 1:
                            vPRN += Environment.NewLine +
                                    Collecting.TD_TUR.Replace("[TD_TUR]", "ÇEK").Replace("@ROW", vRow.ToString());
                            break;
                        case 2:
                            vPRN += Environment.NewLine +
                                    Collecting.TD_TUR.Replace("[TD_TUR]", "SENET").Replace("@ROW", vRow.ToString());
                            break;
                        case 4:
                            vPRN += Environment.NewLine +
                                    Collecting.TD_TUR.Replace("[TD_TUR]", "MOR").Replace("@ROW", vRow.ToString());
                            break;
                        default:
                            break;
                    }

                    if (item.CollectingType != 0 && item.CollectingType != 2)
                    {
                        index++;
                        vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                        vPRN += Environment.NewLine +
                                Collecting.TD_BANKA.Replace("[TD_BANKA]", item.Bank).Replace("@ROW", vRow.ToString());
                    }


                    var str = "";
                    if (item.AccountNo.Length > 10)
                        str = item.AccountNo.Substring(0, 10);
                    else
                        str = item.AccountNo;

                    if (item.CollectingType != 0 && item.CollectingType != 2)
                    {
                        index++;
                        vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                        vPRN += Environment.NewLine +
                                Collecting.TD_EVRAKNO.Replace("[TD_EVRAKNO]", str).Replace("@ROW", vRow.ToString());
                    }
                    if (item.CollectingType != 0)
                    {
                        index++;
                        vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                        vPRN += Environment.NewLine +
                                Collecting.TD_VADE.Replace("[TD_VADE]",
                                    item.DueDate.HasValue && item.DueDate.Value != DateTime.MinValue
                                        ? item.DueDate.Value.ToString("dd.MM.yyyy")
                                        : "").Replace("@ROW", vRow.ToString());
                    }

                    index++;
                    vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                    vPRN += Environment.NewLine + Collecting.TD_TUTAR.Replace("[TD_TUTAR]", item.Amount.ToString("N2") + " TL").Replace("@ROW", vRow.ToString());

                    index++;
                    index++;
                }

                #endregion

                #region Döküm
                index++;
                vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                vPRN += Environment.NewLine + Collecting.PARSELINE.Replace("@ROW", vRow.ToString());

                index++;
                vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);

                List<CustomerInvoice> financeList = new List<CustomerInvoice>();
                //Ramazan

                double totalDebt = financeList.Sum(p => p.Debt);
                double totalCredit = financeList.Sum(p => p.Credit);
                string tmp = Collecting.CBD_ESKIBAKIYE.Replace("@ROW", vRow.ToString()).Split(':')[0] + ":" + "[CBD_ESKIBAKIYE]";
                vPRN += Environment.NewLine + tmp.Replace("[CBD_ESKIBAKIYE]", AlignRight((totalDebt - totalCredit).ToString("N2") + " TL\"", Collecting.MAXROWCOUNT - 18));
                index += 1;
                vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                tmp = Collecting.CBD_TAHTOP.Replace("@ROW", vRow.ToString()).Split(':')[0] + ":" + "[CBD_TAHTOP]";
                vPRN += Environment.NewLine + tmp.Replace("[CBD_TAHTOP]", AlignRight(vCollectingHeader.CollectingList.Sum(c => c.Amount).ToString("N2") + " TL\"", Collecting.MAXROWCOUNT - 18));
                index += 1;
                vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                tmp = Collecting.CBD_SONBAKIYE.Replace("@ROW", vRow.ToString()).Split(':')[0] + ":" + "[CBD_SONBAKIYE]";
                vPRN += Environment.NewLine + tmp.Replace("[CBD_SONBAKIYE]", AlignRight(((totalDebt - totalCredit) - vCollectingHeader.CollectingList.Sum(c => c.Amount)).ToString("N2") + " TL\"", Collecting.MAXROWCOUNT - 18));
                #endregion

                #region Onay
                index += 2;
                vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                vPRN += Environment.NewLine + Collecting.PARSELINE.Replace("@ROW", vRow.ToString());
                index++;

                vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                vPRN += Environment.NewLine + Collecting.ONAYCARILINE.Replace("@ROW", vRow.ToString());
                index += 4;
                vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                vPRN += Environment.NewLine + Collecting.PARSELINE.Replace("@ROW", vRow.ToString());
                index++;
                vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                vPRN += Environment.NewLine + Collecting.ONAYPLASIYERLINE.Replace("@ROW", vRow.ToString());
                #endregion
                #region Endline
                vPRN += Environment.NewLine + Collecting.PREVENDLINE;

                index += 4;
                vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                vPRN = vPRN.Replace(Collecting.PAGEHEIGHT, vRow.ToString());
                vPRN += "\n";
                index += 4;
                vRow = (Collecting.YCOOR + Collecting.ROWHEIGHT * index);
                vPRN = vPRN.Replace("[DOT]", vRow.ToString());
                #endregion

                return vPRN;
            }
            catch (Exception ex)
            {

                return ex.Message;
            }


        }
        private string AlignRight(string text, int sumcell, int leftSub = 0)
        {
            text = text.Substring(leftSub, text.Length - leftSub);
            int spaceCount = sumcell - text.Length;
            string rtrn = string.Empty;
            for (int i = 0; i < spaceCount; i++)
            {
                rtrn += " ";
            }
            return rtrn += text;
        }
        #endregion
    }
}