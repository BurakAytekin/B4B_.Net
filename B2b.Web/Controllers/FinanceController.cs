using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Log;
using B2b.Web.v4.Areas.Admin.Models;
using System.Data;
using B2b.Web.v4.Models.ErpLayer;
using Newtonsoft.Json;

namespace B2b.Web.v4.Controllers
{
    public class FinanceController : BaseController
    {
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


        // GET: Finance
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UnClosedInvoice()
        {
            return View();
        }

        public ActionResult InvoiceDetail(string DocumentNo)
        {
            List<CustomerInvoice> list = Session["InvoiceList"] as List<CustomerInvoice>;
            if (list == null || list.Count == 0 || list.Where(x => x.DocumentNo == DocumentNo).Count() == 0)
                return RedirectToAction("Unauthorized", "Login");

            ViewBag.CustomerInvoice = list.Where(x => x.DocumentNo == DocumentNo).First();
            ViewBag.CurrentCustomer = CurrentCustomer;
            return View();
        }

        public ActionResult CheckDetail()
        {
            return View();
        }

        #region HttpPost Methods

        [HttpPost]
        public JsonResult CheckAuthorized()
        {
            bool result = false;
            result = CurrentLoginType == Models.Helper.LoginType.Salesman ? false : CurrentCustomer.IsCurrentAccountStatu;

            return Json(result);
        }

        [HttpPost]
        public string GetFinanceYear()
        {
            FinanceYearList = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.Finance);

            return JsonConvert.SerializeObject(FinanceYearList);
        }

        [HttpPost]
        public string GetFinanceDetail(string DocumentNo)
        {
            ErpFunctionDetail erpDetail = Session["ErpFunctionDetail"] as ErpFunctionDetail;

            GeneralParameters parametres = new GeneralParameters();
            parametres.CommandType = erpDetail.FunctionType == 0 ? CommandType.StoredProcedure : CommandType.Text;
            parametres.ParameterNames = (new string[2] { "pDocumantNumber", "pCustomerCode" });
            parametres.ParameterValues = (new string[2] { DocumentNo , CurrentCustomer.Code });
          
            parametres.CommandText = erpDetail.FunctionDetailInvoice;

            DataTable dt = ErpHelper.FireServiceMethod(parametres, erpDetail.Settings).ConvertResponseDataTable();
            List<CustomerInvoiceDetail> list = new List<CustomerInvoiceDetail>();
            list = dt.DataTableToList<CustomerInvoiceDetail>();


            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string GetUnClosedData()
        {
            List<ErpFunctionDetail> yearList = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.Finance);

            ErpFunctionDetail erpDetail = yearList.Count() > 0 ? yearList.First() : new ErpFunctionDetail();

            GeneralParameters parametres = new GeneralParameters();
            parametres.CommandType = erpDetail.FunctionType == 0 ? CommandType.StoredProcedure : CommandType.Text;
            parametres.ParameterNames = (new string[1] { "CustomerCode" });
            parametres.ParameterValues = (new string[1] { CurrentCustomer.Code });
            parametres.CommandText = erpDetail.FunctionName;

            DataTable dt = ErpHelper.FireServiceMethod(parametres, erpDetail.Settings).ConvertResponseDataTable();
            List<CustomerInvoice> list = new List<CustomerInvoice>();
            list = dt.DataTableToList<CustomerInvoice>();


            double balance = 0;
            double debt = 0;
            double totalDebt = 0;
            List<CustomerInvoice> unclosedList = new List<CustomerInvoice>();
            List<CustomerInvoice> filteredCurrAccTranList = new List<CustomerInvoice>();

            if (list.Count > 0)
            {

                foreach (CustomerInvoice item in list)
                {
                    balance += item.Debt;
                    balance -= item.Credit;
                    item.Balance = balance;
                    item.DebtStr = item.Debt.ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();
                    item.CreditStr = item.Credit.ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();
                    item.BalanceStr = item.Balance.ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();

                }


                totalDebt = list.Last().Balance;

                list.Reverse();


                foreach (CustomerInvoice item in list)
                {
                    if (item.TransactionType == "Satış faturası" || item.TransactionType == "Açılış fişi")
                    {
                        debt += item.Debt;
                        filteredCurrAccTranList.Add(item);
                        if (debt >= totalDebt) break;
                    }
                }

                filteredCurrAccTranList.Reverse();
                for (int i = 0; i < filteredCurrAccTranList.Count; i++)
                {
                    CustomerInvoice obj = new CustomerInvoice();
                    obj.Date = filteredCurrAccTranList[i].Date;
                    obj.DueDate = (filteredCurrAccTranList[i].DueDate);
                    obj.DocumentNo = filteredCurrAccTranList[i].DocumentNo;
                    obj.TransactionType = filteredCurrAccTranList[i].TransactionType;
                    obj.Explanation = filteredCurrAccTranList[i].Explanation;
                    obj.Debt = filteredCurrAccTranList[i].Debt;
                    obj.Closed = i == 0 ? debt - totalDebt : 0;
                    obj.Remaining = obj.Debt - obj.Closed;
                    obj.RemainingTotal = i == 0 ? obj.Remaining : (unclosedList[i - 1].RemainingTotal + obj.Remaining);
                    TimeSpan ts = DateTime.Now.Subtract(obj.Date);
                    obj.Day = -(ts.Days);

                    obj.ClosedStr = obj.Closed.ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();
                    obj.RemainingTotalStr = obj.RemainingTotal.ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();

                    unclosedList.Add(obj);
                }
            }



            return JsonConvert.SerializeObject(unclosedList);
        }


        [HttpPost]
        public string GetDetailSubTotals(List<CustomerInvoiceDetail> list)
        {
            CustomerInvoiceDetail item = new CustomerInvoiceDetail();
            foreach (var test in list)
            {
                double vats = test.Vat / 100;
                test.Vat = 0.0;
                test.Vat += (test.Total) * vats;
               
            }
            if (list != null)
            {
                item.TotalStr = list.Sum(x => x.Total).ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();
                item.VatTotalStr = list.Sum(x => x.Vat).ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();
                double cost = list.Sum(x => x.Total);
                cost -= cost * item.Discount1 / 100;
                cost -= cost * item.Discount2 / 100;
                cost -= cost * item.Discount3 / 100;
                cost -= cost * item.Discount4 / 100;
                item.SubTotalStr = cost.ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();
                item.DiscTotalStr = (list.Sum(x => x.Total) - cost).ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();
                item.GeneralTotalStr = (list.Sum(x => x.Total) + list.Sum(x => x.Vat)).ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();
               
            }
            return JsonConvert.SerializeObject(item);
        }


        [HttpPost]
        public JsonResult GetSubTotals(List<CustomerInvoice> list)
        {
            CustomerInvoice item = new CustomerInvoice();
            if (list != null)
            {
                item.DebtStr = list.Sum(x => x.Debt).ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();
                item.CreditStr = list.Sum(x => x.Credit).ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();
                item.BalanceStr = list.Last().Balance.ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();
            }
            return Json(item);
        }




        [HttpPost]
        public string GetFinanceList(DateTime startDate, DateTime endDate, int year, string password)
        {
            if (((CurrentCustomer.IsCurrentAccountStatu && CurrentCustomer.CurrentAccountPassword == MD5Sifreleme(password)) || !CurrentCustomer.IsCurrentAccountStatu) || CurrentLoginType == Models.Helper.LoginType.Salesman)
            {

                ErpFunctionDetail yearItem = FinanceYearList.Where(x => x.Id == year).First();

                GeneralParameters parametres = new GeneralParameters();
                parametres.CommandType = yearItem.FunctionType == 0 ? CommandType.StoredProcedure : CommandType.Text;
                parametres.ParameterNames = (new string[1] { "pCustomerCode" });
                parametres.ParameterValues = (new string[1] { CurrentCustomer.Code });
                parametres.CommandText = yearItem.FunctionName;

                DataTable dt = ErpHelper.FireServiceMethod(parametres, yearItem.Settings).ConvertResponseDataTable();
                // List<CustomerInvoice> list = new List<CustomerInvoice>();
                FinanceList = dt.DataTableToList<CustomerInvoice>();

                double balance = 0;

                foreach (CustomerInvoice item in FinanceList)
                {
                    balance += item.Debt;
                    balance -= item.Credit;
                    item.Balance = balance;
                    item.DebtStr = item.Debt.ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();
                    item.CreditStr = item.Credit.ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();
                    item.BalanceStr = item.Balance.ToString("N2") + " " + CurrentCustomer.CurrencyType.CurrencyHtml();


                    if (item.TransactionType == "Satış Faturası")
                        item.Detail = 1;

                    item.Href = "Finance/InvoiceDetail?DocumentNo=" + item.DocumentNo;
                }

                List<CustomerInvoice> filteredList = new List<CustomerInvoice>();
                List<CustomerInvoice> tmp1 = FinanceList.Where(cat => cat.Date < startDate).ToList();
                List<CustomerInvoice> tmp2 = FinanceList.Where(cat => cat.Date >= startDate && cat.Date <= endDate).ToList();
                List<CustomerInvoice> tmp3 = FinanceList.Where(cat => cat.Date > endDate).ToList();

                if (tmp1.Count > 0)
                {
                    CustomerInvoice firstCat = new CustomerInvoice();
                    firstCat.Date = tmp1.Min(cat => cat.Date);
                    firstCat.DueDate = tmp1.Max(cat => cat.Date); ;
                    firstCat.DocumentNo = "";
                    firstCat.TransactionType = "Önceki Hareketler";
                    firstCat.Debt = tmp1.Sum(cat => cat.Debt);
                    firstCat.Credit = tmp1.Sum(cat => cat.Credit);
                    firstCat.Balance = firstCat.Debt - firstCat.Credit;

                    filteredList.Add(firstCat);
                }
                filteredList.AddRange(tmp2);

                if (tmp3.Count > 0)
                {
                    CustomerInvoice lastCat = new CustomerInvoice();
                    lastCat.Date = tmp3.Min(cat => cat.Date);
                    lastCat.DueDate = tmp3.Max(cat => cat.Date); ;
                    lastCat.DocumentNo = "";
                    lastCat.TransactionType = "Sonraki Hareketler";
                    lastCat.Debt = tmp3.Sum(cat => cat.Debt);
                    lastCat.Credit = tmp3.Sum(cat => cat.Credit);
                    lastCat.Balance = lastCat.Debt - lastCat.Credit;

                    filteredList.Add(lastCat);
                }

                Session["ErpFunctionDetail"] = yearItem;
                Session["InvoiceList"] = filteredList;
                MyTuple list = new MyTuple(filteredList, true);
                return JsonConvert.SerializeObject(list);
            }
            else
            {
                FinanceList = new List<CustomerInvoice>();
                MyTuple list = new MyTuple(FinanceList, false);
                return JsonConvert.SerializeObject(list);
            }
        }


        #endregion
    }

    public class MyTuple : Tuple<List<CustomerInvoice>, bool>
    {
        public MyTuple(List<CustomerInvoice> mList1, bool mList2) : base(mList1, mList2) { }
        public List<CustomerInvoice> List => Item1;
        public bool Result => Item2;
    }
}