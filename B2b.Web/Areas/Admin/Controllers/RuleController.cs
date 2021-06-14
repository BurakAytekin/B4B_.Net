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

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class RuleController : AdminBaseController
    {
        // GET: Admin/Rule
        public ActionResult Index()
        {
              return View();
        }

        #region   HttpPost Methods
        [HttpPost]
        public string GetRuleList()
        {
            List<Rule> list = Rule.GetRuleList();
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public JsonResult UpdateRule(int id, string product, string customer, int paymentType, double disc1, double disc2, double disc3, double disc4, int priceNumber, double rate)
        {
            bool result = false;
            if (id == 0)
            {
                Rule item = new Rule()
                {
                    Product = product,
                    Customer = customer,
                    PaymentType = paymentType,
                    Disc1 = disc1,
                    Disc2 = disc2,
                    Disc3 = disc3,
                    Disc4 = disc4,
                    PriceNumber = priceNumber,
                    CreateId = AdminCurrentSalesman.Id,
                    Rate = rate
                };
                result = item.Add();
            }
            else
            {
                Rule item = new Rule()
                {
                    Id = id,
                    EditId = AdminCurrentSalesman.Id,
                    Product = product,
                    Customer = customer,
                    PaymentType = paymentType,
                    Disc1 = disc1,
                    Disc2 = disc2,
                    Disc3 = disc3,
                    Disc4 = disc4,
                    PriceNumber = priceNumber,
                    CreateId = AdminCurrentSalesman.Id,
                    Rate = rate
                };
                result = item.Update();
            }
            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        [HttpPost]
        public JsonResult DeleteRule(int id)
        {
             bool result = false;
            Rule item = new Rule()
            {
                Id = id,
                EditId = AdminCurrentSalesman.Id
            };
            result = item.Delete();


            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        #endregion


    }
}