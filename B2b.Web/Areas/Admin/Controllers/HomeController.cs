using System.Web.Mvc;
using B2b.Web.v4.Areas.Admin.Models;
using System.Collections.Generic;
using System.Reflection;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;
using RestSharp;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
             return View();
        }

        #region   HttpPost Methods
        [HttpPost]
        public string GetManufacturerReport(int type)
        {
            List<DashboardAdmin> list = DashboardAdmin.GetManufacturerReport(type);
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public JsonResult GetSalesman()
        {
            return Json(AdminCurrentSalesman);
        }
        [HttpPost]
        public string GetHeaderInformation()
        {
            DashboardAdmin item = DashboardAdmin.GetHeaderInformation();
            return JsonConvert.SerializeObject(item);
        }


        [HttpPost]
        public string GetSalesmanNote()
        {
            List<SalesmanNotes> list = SalesmanNotes.GetSalesmanNoteList(AdminCurrentSalesman.Id);
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string AddSalesmanNote(string salesmanNote)
        {
            SalesmanNotes item = new SalesmanNotes()
            {
                Notes = salesmanNote,
                CreateId = AdminCurrentSalesman.Id
            };
            item.Add();

            return JsonConvert.SerializeObject(string.Empty);
        }


        [HttpPost]
        public string UpdateSalesmanNote(SalesmanNotes item)
        {
            item.Update();
            return JsonConvert.SerializeObject(string.Empty);
        }

        [HttpPost]
        public string GetOrderPaymentCross()
        {
            List<DashboardAdmin> list = DashboardAdmin.GetOrderPaymentCross();
            return JsonConvert.SerializeObject(list);
        }

        #endregion
    }
}