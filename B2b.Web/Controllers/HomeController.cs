using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;
using JsonSerializer = B2b.Web.v4.Models.Helper.JsonSerializer;
using System.Web.Security;


namespace B2b.Web.v4.Controllers
{
    public class HomeController : BaseController
    {

        public ActionResult Index()
        {
            if (CurrentLoginType == LoginType.Customer)
            {
                FormsAuthentication.SetAuthCookie("0&&" + (int)LoginType.Customer + "&&" + CurrentCustomer.Code + "&&" + CurrentCustomer.Name, false);
            }
            else
            {
                FormsAuthentication.SetAuthCookie("0&&" + (int)LoginType.Salesman + "&&" + CurrentSalesman.Code + "&&" + CurrentSalesman.Name, false);
            }

            Tuple<List<Announcements>> datList = new Tuple<List<Announcements>>
                (
                Announcements.GetAllByType(AnnouncementsType.Picture)
                );

            return View(datList);
        }

        #region Dosyalar
        public ActionResult FileList()
        {
            return View();
        }

        #endregion

        [HttpPost]
        public string GetCurrencyList(string pageName)
        {
            List<Currency> currencyList = (List<Currency>)Session["CurrencyList"] != null ? (List<Currency>)Session["CurrencyList"] : new List<Currency>();

            if (pageName == "cart" || pageName == "search")
            {
                currencyList = Currency.GetList();
            }

            return JsonConvert.SerializeObject(currencyList);
        }

        [HttpPost]
        public string GetCustomerWaitingInBasket()
        {
            return Basket.CustomerWaitingInBasket(CurrentCustomer.Id, CurrentCustomer.Users.Id).ToString();
        }


        [HttpPost]
        public string GetProductOfDayList(int type)
        {
            List<ProductOfDayCs> list = new List<ProductOfDayCs>();

            if (Session["ProductOfDayList"] == null || type == 1)
            {
                list = ProductOfDayCs.GetProductOfDayByCustomer(CurrentCustomer, CurrentLoginType);
                Session["ProductOfDayList"] = list;
            }
            else
            {
                list = Session["ProductOfDayList"] as List<ProductOfDayCs>;
                if (list.Count > 0 && (DateTime.Now - list.First().SelectTime).TotalMinutes > 15)
                {
                    list = ProductOfDayCs.GetProductOfDayByCustomer(CurrentCustomer, CurrentLoginType);
                    Session["ProductOfDayList"] = list;
                }
            }


            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string GetNotificationsList(int type)
        {
            List<Notifications> list = new List<Notifications>();

            if (Session["NotificationsList"] == null || type == 1)
            {
                list = Notifications.GetNotifications(CurrentCustomer, (CurrentLoginType == LoginType.Salesman ? CurrentSalesman : new Salesman()), CurrentLoginType);
                Session["NotificationsList"] = list;
            }
            else
            {
                list = Session["NotificationsList"] as List<Notifications>;
                if (list.Count > 0 && (DateTime.Now - list.First().SelectTime).TotalMinutes > 15)
                {
                    list = Notifications.GetNotifications(CurrentCustomer, (CurrentLoginType == LoginType.Salesman ? CurrentSalesman : new Salesman()), CurrentLoginType);
                    Session["NotificationsList"] = list;
                }
            }
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string CloseNotification(Notifications item)
        {

            item.Delete();
            return JsonConvert.SerializeObject(string.Empty);
        }

        public JsonResult SendProductOfDayOrder(ProductOfDayCs productOfDay)
        {
            bool result = false;


            OrderHeader header = new OrderHeader()
            {
                CustomerId = CurrentCustomer.Id,
                UserId = CurrentCustomer.Users.Id,
                SalesmanId = CurrentSalesmanId,
                GeneralTotal = productOfDay.GeneralTotal,
                Discount = 0,
                NetTotal = productOfDay.NetTotal,
                Vat = (productOfDay.GeneralTotal - productOfDay.NetTotal),
                Total = productOfDay.GeneralTotal,
                Notes = CurrentLoginType == LoginType.Customer ? "Günün Ürünü" : string.Empty,
                SalesmanNotes = CurrentLoginType == LoginType.Customer ? string.Empty : "Günün Ürünü",
                SendingTypeId = 1,
                NumberOfProduct = 1,
                PaymentId = string.Empty,
                Status = B2BRuleItem.OrderAutomaticTransfer ? OrderStatus.AutomaticTransfer : OrderStatus.OnHold,
                Currency = CurrentCustomer.CurrencyType,
                TotalAvailable = productOfDay.GeneralTotal,

            };
            int orderId = header.Save();

            OrderDetail detailItem = new OrderDetail();
            {
                bool isCampaign = true;
                detailItem.OrderId = orderId;
                detailItem.ProductId = productOfDay.Product.Id;
                detailItem.ProductCode = productOfDay.Product.Code;
                detailItem.ListPrice = productOfDay.Product.PriceList.Value;
                detailItem.ListPriceCurrency = productOfDay.Product.PriceList.Currency;
                detailItem.ListPriceCurrencyRate = productOfDay.Product.PriceCurrencyRate;
                detailItem.Currency = CurrentCustomer.CurrencyType;
                detailItem.CurrencyRate = CurrencyList.First(x => x.Type == CurrentCustomer.CurrencyType).Rate;
                detailItem.CurrencyLocal = "TL";
                detailItem.Disc1 = 0;
                detailItem.Disc2 = 0;
                detailItem.Disc3 = 0;
                detailItem.Disc4 = 0;
                detailItem.DiscSpecial = 0;
                detailItem.DueDay = 0;
                detailItem.IsCampaign = isCampaign;
                detailItem.CampaignId = isCampaign ? productOfDay.Id : -1;
                detailItem.CampaignCode = string.Empty;
                detailItem.DiscCampaign = 0;
                detailItem.Price = productOfDay.PriceCustomer.ValueFinal;
                detailItem.NetPrice = productOfDay.PriceCustomer.ValueFinal;
                detailItem.Amount = productOfDay.NetTotal;
                detailItem.NetAmount = productOfDay.NetTotal;
                detailItem.VatAmount = (productOfDay.GeneralTotal - productOfDay.NetTotal);
                detailItem.Quantity = productOfDay.Quantity;
                detailItem.CustomerId = CurrentCustomer.Id;
                string ItemExplanationStr = "Günün Ürünü";
                detailItem.ItemExplanation = ItemExplanationStr;

            }

            result = result = detailItem.Add();

            Models.EntityLayer.LogProductOfDay item = new Models.EntityLayer.LogProductOfDay()
            {
                ProductOfDayId = productOfDay.Id,
                CustomerId = CurrentCustomer.Id,
                SalesmanId = CurrentLoginType == LoginType.Customer ? -1 : CurrentSalesman.Id
            };
            result = item.Add();

            MessageBox message = null;
            message = result ? new MessageBox(MessageBoxType.Success, "Siparişiniz gönderilmiştir.") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return Json(message);
        }



        [HttpPost]
        public string CheckKvkkContract()
        {
            if (CurrentCustomer.IsConfirmKvkk == 1 && CurrentLoginType == LoginType.Customer)
            {
                LogContract item = LogContract.CheckKvkkContract(CurrentCustomer.Id);
                if (item == null || item.Id == 0)
                {
                    // string button = "<a class='btn btn-success' onClick='saveKvkkContract()' >Tüm Koşulları ONAYLIYORUM</a> <a href='Login/Logout' class='btn btn-danger'>Onaylamıyorum</a>";
                    return JsonConvert.SerializeObject(FooterInformationItem.KvkkContract);
                }
                else
                    return JsonConvert.SerializeObject(string.Empty);

            }
            else
                return JsonConvert.SerializeObject(string.Empty);
        }

        [HttpPost]
        public string SaveKvkkContract()
        {
            bool result = false;
            LogContract item = new LogContract()
            {
                CustomerId = CurrentCustomer.Id,
                UserId = CurrentCustomer.Users.Id,
                SalesmanId = CurrentEditId,
                Contract = FooterInformationItem.KvkkContract,
                Type = ContractType.KvkkContract,

            };
            result = item.Add();

            CurrentCustomer.IsConfirmKvkk = 2;

            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);

        }

        //Text duyuru
        public PartialViewResult AnnouncementTypeTextView()
        {
            return PartialView("AnnouncementTypeTextView", Announcements.GetAllByType(AnnouncementsType.Article));
        }

        #region HttpPost Methods
        [HttpPost]
        public string AnnouncementTypeTextById(int id)
        {
            var announcements = Announcements.GetAllByType((AnnouncementsType)id);

            return JsonConvert.SerializeObject(announcements);
        }

        [HttpPost]
        public JsonResult GetCampaignList()
        {
            List<Product> list = new List<Product>();
            if (Session["HomeCampaignList"] == null)
                list = Product.Search(0, 15, CurrentLoginType, CurrentCustomer, -1, "*", "*", "*", "*", "*", 0, 0, 1, 0, "*", "*", "*", -1, 0, 0, 0, 1, "ORDER BY RAND() LIMIT 15");
            else
                list = Session["HomeCampaignList"] as List<Product>;
            Session["HomeCampaignList"] = list;
            return Json(list);
        }

        [HttpPost]
        public JsonResult GetBannerList()
        {
            List<Product> list = new List<Product>();
            if (Session["HomeBannerList"] == null)
            {
                list = Product.Search(0, 15, CurrentLoginType, CurrentCustomer, -1, "*", "*", "*", "*", "*", 0, 0, 0, 0, "*", "*", "*", -1, 0, 0, 0, 1, "ORDER BY RAND() LIMIT 15");
            }
            else
            {
                list = Session["HomeBannerList"] as List<Product>;
                Session["HomeBannerList"] = list;
            }
            return Json(list);
        }

        [HttpPost]
        public JsonResult GetNewStockArrivalList()
        {
            List<Product> list = new List<Product>();
            if (Session["HomeArrivalList"] == null)
                list = Product.Search(0, 15, CurrentLoginType, CurrentCustomer, -1, "*", "*", "*", "*", "*", 0, 0, 0, 1, "*", "*", "*", -1, 0, 0, 0, 0, "ORDER BY RAND() LIMIT 15");
            else
                list = Session["HomeArrivalList"] as List<Product>;
            Session["HomeArrivalList"] = list;
            return Json(list);
        }

        [HttpPost]
        public string GetFileListType()
        {
            List<Files> fileType = Files.GetFileListType();
            return JsonConvert.SerializeObject(fileType);
        }

        [HttpPost]
        public string GetFileList()
        {
            List<Files> fileList = Files.GetFileList();
            return JsonConvert.SerializeObject(fileList);
        }

        [HttpPost]
        public string GetBasketCount()
        {
            BasketCount basketCount = BasketCount.GetCount(CurrentCustomer.Id, CurrentCustomer.Users.Id, CurrentLoginType);
            return JsonConvert.SerializeObject(basketCount);
        }

        [HttpPost]
        public string GetSuggestionProducts(string productCodes)
        {

            List<Product> suggestionProductList = (List<Product>)Session["SuggestionProductList"] != null ? (List<Product>)Session["SuggestionProductList"] : new List<Product>();

            if (suggestionProductList.Count > 1)
            {
                productCodes = productCodes.Substring(0, productCodes.Length - 1);

                suggestionProductList = Product.GetByIdProductCodes(productCodes, CurrentLoginType, CurrentCustomer);
                Session["SuggestionProductList"] = suggestionProductList;
            }

            return JsonConvert.SerializeObject(suggestionProductList);
        }


        #endregion

    }
}