using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.Log;
using B2b.Web.v4.Areas.Admin.Models;
using System.Data;
using B2b.Web.v4.Models.ErpLayer;
using Newtonsoft.Json;

namespace B2b.Web.v4.Controllers
{
    public class OrderController : BaseController
    {
        List<OrderHeader> OrderHeaderList
        {
            get { return (List<OrderHeader>)Session["OrderHeaderList"]; }
            set { Session["OrderHeaderList"] = value; }
        }

        OrderHeader SelectedOrderHeader
        {
            get { return (OrderHeader)Session["SelectedOrderHeader"]; }
            set { Session["SelectedOrderHeader"] = value; }
        }

        List<ErpFunctionDetail> BackOrderYearList
        {
            get { return (List<ErpFunctionDetail>)Session["BackOrderYearList"]; }
            set { Session["BackOrderYearList"] = value; }
        }


        // GET: Order
        public ActionResult Index()
        {
             return View();
        }

        public ActionResult OrderDetail()
        {
            if (SelectedOrderHeader == null)
                return null;

            ViewBag.OrderHeader = SelectedOrderHeader;
            ViewBag.CurrentCustomer = CurrentCustomer;

            ViewBag.OrderDetaillist = Models.EntityLayer.OrderDetail.GetOrderDetail(SelectedOrderHeader.Id);
            ViewBag.OrderPaymentDetaillist = OrderHeaderPayment.GetListByOrderId(SelectedOrderHeader.Id);

            //ViewBag.OrderDetaillist = Models.EntityLayer.OrderDetail.GetOrderDetail(CurrentLoginType, CurrentCustomer, SelectedOrderHeader.Id);
            return View();
        }


        public ActionResult BackOrder()
        {
            return View();
        }

        #region HttpPost Methods
        [HttpPost]
        public JsonResult DeleteBackOrder(BackOrder item, int yearItemId)
        {
            ErpFunctionDetail yearItem = BackOrderYearList.Where(x => x.Id == yearItemId).First();
            GeneralParameters parametres = new GeneralParameters();
            parametres.CommandType = yearItem.FunctionType == 0 ? CommandType.StoredProcedure : CommandType.Text;
            parametres.ParameterNames = (new string[1] { "Id" });
            parametres.ParameterValues = (new string[1] { item.Id.ToString() });
            parametres.CommandText = yearItem.FunctionDetailInvoice;
            bool result = ErpHelper.FireServiceMethod(parametres, yearItem.Settings).ConvertResponseBoolen();

            string message = !result ? "İşleminizde Hata Gerçekleşmiştir" : "İşleminiz gerçekleştirilmiştir.";
            return Json("{\"statu\":\"" + (result ? "success" : "error") + "\",\"message\":\"" + message + "\"}");
        }

        [HttpPost]
        public JsonResult AddBasket(BackOrder item)
        {
            Product product = Product.GetByCode(item.ProductCode, CurrentLoginType, CurrentCustomer);
            List<Basket> basketAvailableList = Basket.GetAvailableInBasket(CurrentCustomer,CurrentLoginType, product.Id, CurrentCustomer.Id, (int)CurrentLoginType, CurrentCustomer.Users.Id);

            double qty = item.QuantityRemaining;

            if (qty < product.MinOrder)
            {

                return Json("{\"statu\":\"error\",\"message\":\"Minimum sipariş adedinden daha az sipariş veremezsiniz.\"}");

            }
            else
            {
                if (product.IsPackIncrease && product.MinOrder != 1)
                {
                    double k = qty / product.MinOrder;

                    if (qty % product.MinOrder == 0)
                        qty = Convert.ToInt32(k) * product.MinOrder;
                    else
                        qty = (Convert.ToInt32(Math.Floor(k)) + 1) * product.MinOrder;
                }
                if (basketAvailableList.Count > 0)
                {
                    Basket basket = basketAvailableList[0];
                    basket.Quantity = basketAvailableList[0].Quantity + qty;
                    basket.DiscSpecial = 0;
                    basket.Update();
                }
                else
                {
                    Basket basket = new Basket();
                    {
                        basket.Product = product;
                        basket.CustomerId = CurrentCustomer.Id;
                        basket.SalesmanId = CurrentSalesmanId;
                        basket.Quantity = qty;
                        basket.DiscSpecial = 0;
                        basket.RecordDate = DateTime.Now;
                        basket.AddType = (int)CurrentLoginType;
                        basket.ClientNumber = -1;
                        basket.UserId = CurrentCustomer.Users.Id;
                        basket.UserCode = CurrentCustomer.Users.Code;
                        basket.CustomerCode = CurrentCustomer.Code;
                        basket.ProductCode = product.Code;
                        basket.LogId = -1;
                    }
                    basket.Add();
                }
            }

            string message = $"Sepetinize {product.Code} ürününden {qty} {product.Unit} eklendi.";
            return Json("{\"statu\":\"success\",\"message\":\"" + message + "\"}");

        }

        [HttpPost]
        public string GetBackOrderYear()
        {
            BackOrderYearList = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.BackOrder);

            return JsonConvert.SerializeObject(BackOrderYearList);
        }
        [HttpPost]
        public string GetBackOrderData(int yearItemId)
        {
            ErpFunctionDetail yearItem = BackOrderYearList.Where(x => x.Id == yearItemId).First();

            GeneralParameters parametres = new GeneralParameters();
            parametres.CommandType = yearItem.FunctionType == 0 ? CommandType.StoredProcedure : CommandType.Text;
            parametres.ParameterNames = (new string[1] { "CustomerCode" });
            parametres.ParameterValues = (new string[1] { CurrentCustomer.Code });
            parametres.CommandText = yearItem.FunctionName;

            DataTable dt = ErpHelper.FireServiceMethod(parametres, yearItem.Settings).ConvertResponseDataTable();
            List<BackOrder> list = new List<BackOrder>();
            list = dt.DataTableToList<BackOrder>();

            foreach (BackOrder item in list)
            {
                string availabilityCss, availabilityText;
                Product product = new Product();
                product.SetAvailabilityClass(CurrentLoginType, item.Quantity, (item.Quantity > 0 ? AvailabilityStatus.Available : AvailabilityStatus.Unavailable), out availabilityCss, out availabilityText);
                item.AvailabilityCss = availabilityCss;
                item.AvailabilityText = availabilityText;
            }

            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string GetOrderHeaderList(string dateStart, string datetEnd, string searchText)
        {
            OrderHeaderList = OrderHeader.GetOrderHeaderList(CurrentLoginType, CurrentCustomer, Convert.ToDateTime(dateStart), Convert.ToDateTime(datetEnd), searchText);
            return JsonConvert.SerializeObject(OrderHeaderList);
        }

        [HttpPost]
        public JsonResult ResponseOrderDetail(int orderId)
        {
            SelectedOrderHeader = OrderHeaderList.First(x => x.Id == orderId);
            return null;
        }

        #endregion
    }
}