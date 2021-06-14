using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class ReportSettingsController : AdminBaseController
    {
        // GET: Admin/ReportSettings
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string SaveReportSettings(AdminReports aReport, string aParameters)
        {
            Logger.LogNavigation(-1, -1, AdminCurrentSalesman.Id,
                  GetControllerName() + MethodBase.GetCurrentMethod().Name, ClientType.Admin, GetUserIpAddress());
            aReport.ReportCreateName = AdminCurrentSalesman.Id.ToString();
            aReport.ReportCreateDate = DateTime.Now;
            aReport.ReportEditName = AdminCurrentSalesman.Id.ToString();
            aReport.ReportEditDate = DateTime.Now;
            aReport.ParametersStr = aParameters;
            int result = aReport.SaveAndUpdate();
            MessageBox messageBox = result == -2 ? new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu", result) : new MessageBox(MessageBoxType.Success, (result == -1 ? "Güncelleme " : "Kayıt ") + "İşleminiz Tamamlandı", result);
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string DeleteReportSettings(int id)
        {
            Logger.LogNavigation(-1, -1, AdminCurrentSalesman.Id,
                  GetControllerName() + MethodBase.GetCurrentMethod().Name, ClientType.Admin, GetUserIpAddress());
            AdminReports report = new AdminReports();
            report.Id = id;
            bool result = report.Delete();

            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı", 1) : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return JsonConvert.SerializeObject(messageBox);
        }
    }
}