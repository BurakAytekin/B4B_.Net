using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using System.Reflection;
using System.Data;
using System.Web.Script.Serialization;
using B2b.Web.v4.Areas.Admin.Models;
using Newtonsoft.Json;

namespace B2b.Web.v4.Areas.Report.Controllers
{
    public class HomeController : AdminBaseController
    {
        List<AdminReports> ReportList
        {
            get { return (List<AdminReports>)Session["ReportsList"]; }
            set { Session["ReportsList"] = value; }
        }

        // GET: Report/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewReport(int id)
        {
            ViewBag.ReportId = id;
            return View();
        }


        [HttpPost]
        public JsonResult GetMenuList(int reportIsActive = 1)
        {
            ReportList = AdminReports.GetReportMenuList(reportIsActive, AdminCurrentSalesman.AuthoritySalesman.CustomerType);
            return Json(ReportList);
        }

        [HttpPost]
        public JsonResult ReportDetails(int reportId)
        {
            AdminReports r = AdminReports.GetReport_ByReportId(reportId, AdminCurrentSalesman.Id);
            if (r.Customers != null)
                r.Customers.Insert(0, new Custo { Code = "TÜMÜ", Name = "TÜMÜ" });
            return Json(r);
        }



        [HttpPost]
        public string GetReport(string procName, string[] @params, string[] types, string[] values, int exportViewType, Settings set)
        {
            DataTable dt = AdminReports.GetReport(procName, @params, types, values, exportViewType, set);
            return DataTableToJSON(dt);
        }


        public string DataTableToJSON(DataTable table)
        {
            var list = new List<Dictionary<string, object>>();
            var dict = new Dictionary<string, object>();

            if (table.Rows.Count < 1)
            {
                if (table.Columns.Count > 0)
                    foreach (DataColumn col in table.Columns)
                    {
                        if (col.Ordinal == 0)
                            dict[col.ColumnName] = "Kayıt Bulunamadı.";
                        else
                            dict[col.ColumnName] = "";
                    }
                else
                {
                    table.Columns.Add(new DataColumn { ColumnName = "Tablo" });
                    dict["Tablo"] = "Kayıt Bulunamadı...";
                }
                list.Add(dict);
            }

            foreach (DataRow row in table.Rows)
            {
                dict = new Dictionary<string, object>();

                foreach (DataColumn col in table.Columns)
                {
                    dict[col.ColumnName] = (Convert.ToString(row[col]));
                }
                list.Add(dict);
            }
            return JsonConvert.SerializeObject(list);
        }

    }
}