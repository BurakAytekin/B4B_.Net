using B2b.Web.v4.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class BatchProcessingController : AdminBaseController
    {
        // GET: Admin/BatchProcessing
        public ActionResult Index()
        {
             return View();
        }

      

        public ActionResult Rule()
        {
               return View();
        }
        #region HttpPost Methods
        [HttpPost]
        public string UploadRule(List<ExcelUpload> uploadList)
        {
        
            List<Rule> ruleList = new List<Rule>();
            foreach (var uploadItem in uploadList)
            {

                Rule rule = new Rule();
                rule.Customer = uploadItem.Customer;
                rule.Product = uploadItem.Product;
                rule.PaymentType = uploadItem.PaymentType;
                rule.Disc1 = uploadItem.Disc1;
                rule.Disc2 = uploadItem.Disc2;
                rule.Disc3 = uploadItem.Disc3;
                rule.Disc4 = uploadItem.Disc4;
                rule.PriceNumber = uploadItem.PriceNumber;
                rule.Rate = uploadItem.Rate;
                ruleList.Add(rule);

            }
            return JsonConvert.SerializeObject(ruleList);

        }

        [HttpPost]
        public string SaveRuleExcel(List<Rule> ruleList)
        {
             bool result = false;
            foreach (var rule in ruleList)
            {

                rule.CreateId = AdminCurrentSalesman.Id;
                result = rule.Add();

            }
            MessageBox messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }

        #endregion
        public class ExcelUpload
        {
            public string Product { get; set; }
            public string Customer{ get; set; }
            public int PaymentType{ get; set; }
            public double Disc1{ get; set; }
            public double Disc2{ get; set; }
            public double Disc3{ get; set; }
            public double Disc4{ get; set; }
            public int PriceNumber { get; set; }
            public double Rate { get; set; }
          
        }

       
    }
}