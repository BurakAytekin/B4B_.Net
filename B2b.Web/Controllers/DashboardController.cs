using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Log;
using B2b.Web.v4.Models.ErpLayer;
using B2b.Web.v4.Areas.Admin.Models;
using System.Data;
using Newtonsoft.Json;

namespace B2b.Web.v4.Controllers
{
    public class DashboardController : BaseController
    {
        List<OrderHeader> OrderHeaderList
        {
            get { return (List<OrderHeader>)Session["OrderHeaderList"]; }
            set { Session["OrderHeaderList"] = value; }
        }
        List<ErpFunctionDetail> FinanceYearList
        {
            get { return (List<ErpFunctionDetail>)Session["FinanceYearList"]; }
            set { Session["FinanceYearList"] = value; }
        }

        List<CustomerInvoice> FinanceList
        {
            get { return (List<CustomerInvoice>)Session["FinanceList"]; }
            set { Session["FinanceList"] = value; }
        }

        // GET: Dashboard
        public ActionResult Index()
        {
            FinanceYearList = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.Finance);
            return View();
        }

        

        [HttpPost]
        public string GetOrderList()
        {
            OrderHeaderList = OrderHeader.GetDashhBoardOrder(CurrentLoginType, CurrentCustomer);
            return JsonConvert.SerializeObject(OrderHeaderList);
        }

        [HttpPost]
        public JsonResult GetPaymentTotal()
        {
            double paymentTotal = EPayment.GetSuccessPaymentTotal(CurrentCustomer);
            return Json(paymentTotal.ToString("N2") + CurrentCustomer.CurrencyType.CurrencyHtml());
        }

        [HttpPost]
        public string GetInvoiceList()
        {
            List<CustomerInvoice> financeListTmp = new List<CustomerInvoice>();
            try
            {
                ErpFunctionDetail yearItem = FinanceYearList.Where(x => x.Settings.IsActiveCompany).First();

                GeneralParameters parametres = new GeneralParameters();
                parametres.CommandType = yearItem.FunctionType == 0 ? CommandType.StoredProcedure : CommandType.Text;
                parametres.ParameterNames = (new string[1] { "CustomerCode" });
                parametres.ParameterValues = (new string[1] { CurrentCustomer.Code });
                parametres.CommandText = yearItem.FunctionName;

                DataTable dt = ErpHelper.FireServiceMethod(parametres, yearItem.Settings).ConvertResponseDataTable();
                // List<CustomerInvoice> list = new List<CustomerInvoice>();
                FinanceList = dt.DataTableToList<CustomerInvoice>();
                FinanceList = FinanceList.OrderByDescending(x => x.Date).ToList();
                if (FinanceList.Count >= 5)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        financeListTmp.Add(FinanceList[i]);
                    }
                }
            }
            catch (Exception)
            {

            }

            return JsonConvert.SerializeObject(financeListTmp);
        }

        [HttpPost]
        public string GetBalanceInformation()
        {
            CustomerBalanceInformation item = new CustomerBalanceInformation();

            try
            {
                List<CustomerInvoice> financeListTmp = new List<CustomerInvoice>();
                ErpFunctionDetail yearItem = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.CustomerDashboard).Where(x => x.Settings.IsActiveCompany).First();

                GeneralParameters parametres = new GeneralParameters();
                parametres.CommandType = yearItem.FunctionType == 0 ? CommandType.StoredProcedure : CommandType.Text;
                parametres.ParameterNames = (new string[1] { "CustomerCode" });
                parametres.ParameterValues = (new string[1] { CurrentCustomer.Code });
                parametres.CommandText = yearItem.FunctionName;

                DataTable dt = ErpHelper.FireServiceMethod(parametres, yearItem.Settings).ConvertResponseDataTable();
                // List<CustomerInvoice> list = new List<CustomerInvoice>();
                item = dt.DataTableToItem<CustomerBalanceInformation>();
            }
            catch (Exception)
            {

            }

            item.UnClosedBalanceStr = item.UnClosedBalance.ToString("N2") + CurrentCustomer.CurrencyType.CurrencyHtml();
            item.BalanceStr = item.Balance.ToString("N2") + CurrentCustomer.CurrencyType.CurrencyHtml();

            return JsonConvert.SerializeObject(item);
        }


    }
}