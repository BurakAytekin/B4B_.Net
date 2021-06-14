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
    public class ReturnProductController : BaseController
    {
        // GET: ReturnProduct
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ReturnProductList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveReturnForm(ReturnForm retrunForm)
        {
            bool result = false;
            if (!String.IsNullOrEmpty(retrunForm.InvoiceNumber))
            {
                retrunForm.CreateId = CurrentEditId;
                retrunForm.UserId = CurrentCustomer.Users.Id;
                retrunForm.CustomerId = CurrentCustomer.Id;
                retrunForm.SalesmanId = CurrentLoginType == LoginType.Salesman ? CurrentSalesman.Id : -1;
                retrunForm.CurrencyType = CurrentCustomer.CurrencyType;
                retrunForm.AddType = (int)CurrentLoginType;

                result = retrunForm.Insert();
            }


            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return Json(messageBox);
        }



        [HttpPost]
        public JsonResult FindInvoice(string manufacturer, string productCode)
        {
            List<ProductOrder> financeListTmp = new List<ProductOrder>();
            if (String.IsNullOrEmpty(manufacturer) && String.IsNullOrEmpty(productCode))
            {

                return Json(financeListTmp);
            }
            else
            {
                try
                {

                    ErpFunctionDetail yearItem = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.ReturnProduct).Where(x => x.Settings.IsActiveCompany).First();

                    GeneralParameters parametres = new GeneralParameters();
                    parametres.CommandType = yearItem.FunctionType == 0 ? CommandType.StoredProcedure : CommandType.Text;
                    parametres.ParameterNames = (new string[3] { "CustomerCode", "Manufacturer", "ProductCode" });
                    parametres.ParameterValues = (new string[3] { CurrentCustomer.Code, manufacturer, productCode });
                    parametres.CommandText = yearItem.FunctionName;

                    DataTable dt = ErpHelper.FireServiceMethod(parametres, yearItem.Settings).ConvertResponseDataTable();
                    // List<CustomerInvoice> list = new List<CustomerInvoice>();
                    financeListTmp = dt.DataTableToList<ProductOrder>();
                }
                catch (Exception ex)
                {

                }


                return Json(financeListTmp);
            }


        }


        [HttpPost]
        public string GetManufacturerList()
        {
            List<ProductOrder> financeListTmp = new List<ProductOrder>();
            try
            {

                ErpFunctionDetail yearItem = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.ReturnProductManufacturer).Where(x => x.Settings.IsActiveCompany).First();

                GeneralParameters parametres = new GeneralParameters();
                parametres.CommandType = yearItem.FunctionType == 0 ? CommandType.StoredProcedure : CommandType.Text;
                parametres.ParameterNames = (new string[1] { "CustomerCode" });
                parametres.ParameterValues = (new string[1] { CurrentCustomer.Code.ToString() });
                parametres.CommandText = yearItem.FunctionName;

                DataTable dt = ErpHelper.FireServiceMethod(parametres, yearItem.Settings).ConvertResponseDataTable();
                financeListTmp = dt.DataTableToList<ProductOrder>();
            }
            catch (Exception ex)
            {

            }


            return JsonConvert.SerializeObject(financeListTmp);
        }


        [HttpPost]
        public string GetReturnProductList()
        {
            List<ReturnForm> list = ReturnForm.GetList(CurrentCustomer.Id);

            return JsonConvert.SerializeObject(list);
        }


    }
}