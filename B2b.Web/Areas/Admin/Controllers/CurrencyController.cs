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
using System.Xml;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class CurrencyController : AdminBaseController
    {
        // GET: Admin/Currency
        public ActionResult Index()
        {
            return View();
        }

        #region   HttpPost Methods
        [HttpPost]
        public JsonResult GetCurrencyList()
        {
            List<Currency> list = Currency.GetList();
            return Json(list);
        }

        [HttpPost]
        public JsonResult UpdateCurrencyBist()
        {
            bool result = false;

            var curencyList = Currency.GetList().Where(x => x.CheckBist && x.Type != "TL");
            var xmlDoc = new XmlDocument();
            xmlDoc.Load("http://www.tcmb.gov.tr/kurlar/today.xml");
            foreach (var currency in curencyList)
            {
                string value = xmlDoc.SelectSingleNode("Tarih_Date/Currency[@Kod='" + currency.Type + "']/ForexSelling").InnerXml.Replace(".", ",");
                currency.Rate = Convert.ToDouble(value);
                result = currency.AddOrUpdate();
            }

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        [HttpPost]
        public JsonResult AddOrUpdate(Currency currency)
        {
            bool result = false;
            if (currency != null)
            {
                if (currency.Id < 1)
                {//ekleniyorsa

                    currency.CreateId = AdminCurrentSalesman.Id;
                }
                else//güncelleniyorsa
                {
                    currency.EditId = AdminCurrentSalesman.Id;
                }

                result = currency.AddOrUpdate();
            }

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        [HttpPost]
        public JsonResult DeleteCurrency(int id)
        {
            bool result = false;
            Currency item = new Currency()
            {
                Id = id
            };
            result = item.Delete();


            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        } 
        #endregion
    }
}