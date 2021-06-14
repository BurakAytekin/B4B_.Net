using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log.EPayment;
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
    public class CompanySettingsController : AdminBaseController
    {
        // GET: Admin/CompanySettings
        public ActionResult Index()
        {
             return View();
        }

        #region  HttpPost Methods
        [HttpPost]
        public string GetSettingList()
        {
            List<Settings> list = Settings.GetSettingsList();
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public JsonResult SaveSettings(Settings settings)
        {
            bool result = false;

            if (settings.Id == 0)
            {
                settings.DbUser = Token.Encrypt(settings.DbUser, GlobalSettings.EncryptKey);
                settings.DbPassword = Token.Encrypt(settings.DbPassword, GlobalSettings.EncryptKey);
                settings.ErpUserName = Token.Encrypt(settings.ErpUserName, GlobalSettings.EncryptKey);
                settings.ErpPassword = Token.Encrypt(settings.ErpPassword, GlobalSettings.EncryptKey);
                settings.Database = Token.Encrypt(settings.Database, GlobalSettings.EncryptKey);
                settings.ServiceUserName = Token.Encrypt(settings.ServiceUserName, GlobalSettings.EncryptKey);
                settings.ServicePassword = Token.Encrypt(settings.ServicePassword, GlobalSettings.EncryptKey);
                settings.ServiceAddress = Token.Encrypt(settings.ServiceAddress, GlobalSettings.EncryptKey);
                settings.ServiceAddressLocal = Token.Encrypt(settings.ServiceAddressLocal, GlobalSettings.EncryptKey);


                settings.CreateId = AdminCurrentSalesman.Id;
                result = settings.Add();
            }
            else
            {
                settings.DbUser = Token.Encrypt(settings.DbUser, GlobalSettings.EncryptKey);
                settings.DbPassword = Token.Encrypt(settings.DbPassword, GlobalSettings.EncryptKey);
                settings.ErpUserName = Token.Encrypt(settings.ErpUserName, GlobalSettings.EncryptKey);
                settings.ErpPassword = Token.Encrypt(settings.ErpPassword, GlobalSettings.EncryptKey);
                settings.Database = Token.Encrypt(settings.Database, GlobalSettings.EncryptKey);
                settings.ServiceUserName = Token.Encrypt(settings.ServiceUserName, GlobalSettings.EncryptKey);
                settings.ServicePassword = Token.Encrypt(settings.ServicePassword, GlobalSettings.EncryptKey);
                settings.ServiceAddress = Token.Encrypt(settings.ServiceAddress, GlobalSettings.EncryptKey);
                settings.ServiceAddressLocal = Token.Encrypt(settings.ServiceAddressLocal, GlobalSettings.EncryptKey);

                settings.EditId = AdminCurrentSalesman.Id;
                result = settings.Update();
            }


            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        } 
        #endregion
    }
}