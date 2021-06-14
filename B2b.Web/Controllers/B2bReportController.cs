using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Controllers
{
    [CustomAuthorize(Name = "Salesman")]
    public class B2bReportController : BaseController
    {
        // GET: B2bReport
        List<AdminReports> ReportList
        {
            get { return (List<AdminReports>)Session["B2bReportsList"]; }
            set { Session["B2bReportsList"] = value; }
        }

        List<OrderHeaderSmall> OrderHeaderList
        {
            get { return (List<OrderHeaderSmall>)Session["OrderHeaderListReport"]; }
            set { Session["OrderHeaderListReport"] = value; }
        }

        // GET: B2bReport
        public ActionResult Index(int id = 0)
        {
            ViewBag.ReportId = id;
            return View();
        }

        public ActionResult OrderReport()
        {
            ViewBag.CurrentSalesman = CurrentSalesman;
            return View();
        }

        [HttpPost]
        public JsonResult GetMenuList(int reportIsActive = 1)
        {
            ReportList = AdminReports.GetReportMenuList(reportIsActive, CurrentSalesman != null ? CurrentSalesman.AuthoritySalesman.CustomerType : false);
            return Json(ReportList);
        }

        [HttpPost]
        public string GetOrderHeaderList(string dateStart, string datetEnd, string searchText, int salesmanId)
        {
            Salesman s = salesmanId == -1 ? CurrentSalesman : new Salesman { Id = salesmanId };
            OrderHeaderList = OrderHeader.GetOrderHeaderListBySalesman(s, Convert.ToDateTime(dateStart), Convert.ToDateTime(datetEnd), searchText);

            return JsonConvert.SerializeObject(OrderHeaderList);
        }

        [HttpPost]
        public string GetOrderDetailList(int orderId)
        {
            List<OrderDetail> list = OrderDetail.GetOrderDetail(orderId);

            return JsonConvert.SerializeObject(list);
        }


        [HttpPost]
        public JsonResult ReportDetails(int reportId)
        {
            AdminReports r = AdminReports.GetReport_ByReportId(reportId, CurrentSalesman.Id);
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