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
    public class RuleDefinitionController : AdminBaseController
    {
        // GET: Admin/RuleDefinition
        public ActionResult Index()
        {
              return View();
        }

        #region   HttpPost Methods
        [HttpPost]
        public string ChangeStartScreen(string startScreen)
        {

            bool result = B2bRule.ChangeStartScreen(startScreen);
            MessageBox messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }

        [HttpPost]
        public string GetB2BRulelist()
        {
            return JsonConvert.SerializeObject(B2bRule.GetGeneralRuleList());
        }
        [HttpPost]
        public string MinOrderTotalUpdate(double total)
        {

            bool result = B2bRule.UpdateMinOrderTotal(total);
            MessageBox messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string CustomerWebLogin(bool status)
        {

            bool result = B2bRule.UpdateBoolen("CustomerWebLogin", status);
            MessageBox messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string SalesmanWebLogin(bool status)
        {

            bool result = B2bRule.UpdateBoolen("SalesmanWebLogin", status);
            MessageBox messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string OrderAutomaticTransfer(bool status)
        {

            bool result = B2bRule.UpdateBoolen("OrderAutomaticTransfer", status);
            MessageBox messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }


        [HttpPost]
        public string UpdateMaintenanceStatus(bool status)
        {
            B2bRule.UpdateBoolen("Maintenance", status);
            HttpRuntime.UnloadAppDomain();
            return String.Empty;
        }
        #endregion
    }
}