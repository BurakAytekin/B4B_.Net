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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Mail;

namespace B2b.Web.v4.Controllers
{
    public class CartController : BaseController
    {
        #region Fields
        List<Basket> BasketList
        {
            get { return (List<Basket>)Session["BasketList"]; }
            set { Session["BasketList"] = value; }
        }
        List<OrderHeaderPayment> OrderHeaderPaymentList
        {
            get { return (List<OrderHeaderPayment>)Session["OrderHeaderPaymentList"]; }
            set { Session["OrderHeaderPaymentList"] = value; }
        }
        List<CartGiveOrderNoQty> ReasonList
        {
            get { return (List<CartGiveOrderNoQty>)Session["ReasonList"]; }
            set { Session["ReasonList"] = value; }
        }
        protected Basket SelectedBasket
        {
            get { return (Basket)Session["SelectedBasket"]; }
            set { Session["SelectedBasket"] = value; }
        }
        double TotalPrice
        {
            get { return (double)Session["TotalPrice"]; }
            set { Session["TotalPrice"] = value; }
        }
        double TotalDiscount
        {
            get { return (double)Session["TotalDiscount"]; }
            set { Session["TotalDiscount"] = value; }
        }
        double TotalCost
        {
            get { return (double)Session["TotalCost"]; }
            set { Session["TotalCost"] = value; }
        }
        double TotalVAT
        {
            get { return (double)Session["TotalVAT"]; }
            set { Session["TotalVAT"] = value; }
        }
        double TotalPriceCustomerCurrency
        {
            get { return (double)Session["TotalPriceCustomerCurrency"]; }
            set { Session["TotalPriceCustomerCurrency"] = value; }
        }
        double TotalDiscountCustomerCurrency
        {
            get { return (double)Session["TotalDiscountCustomerCurrency"]; }
            set { Session["TotalDiscountCustomerCurrency"] = value; }
        }
        double TotalCostCustomerCurrency
        {
            get { return (double)Session["TotalCostCustomerCurrency"]; }
            set { Session["TotalCostCustomerCurrency"] = value; }
        }
        double TotalVATCustomerCurrency
        {
            get { return (double)Session["TotalVATCustomerCurrency"]; }
            set { Session["TotalVATCustomerCurrency"] = value; }
        }
        double GeneralTotal
        {
            get { return TotalCostCustomerCurrency + TotalVATCustomerCurrency; }
            set { Session["TotalVATCustomerCurrency"] = value; }
        }

        #endregion
        // GET: Cart
        public ActionResult Index()
        {

            OrderHeaderPaymentList = new List<OrderHeaderPayment>();
            if (Session["PosInsertId"] != null && Session["OrderForPayment"] != null)
            {
                OrderForPayment ofp = Session["OrderForPayment"] as OrderForPayment;
                ofp.OrderHeader.PaymentId = Session["PosInsertId"].ToString();
                Session["PosInsertId"] = null;
                SendOrder(ofp.OrderHeader, ofp.BasketList);
                OrderHeaderPaymentList = ofp.OrderHeaderPaymentList;
            }

            return View();
        }

        #region   HttpPost Methods

        [HttpPost]
        public JsonResult AddOrderPayment(OrderHeaderPayment payment)
        {
            if (OrderHeaderPaymentList == null)
                OrderHeaderPaymentList = new List<OrderHeaderPayment>();
            OrderHeaderPaymentList.Add(payment);
            int index = GeneralTotal.ToString().IndexOf(",");
            double general = Convert.ToDouble(GeneralTotal.ToString().Substring(0, index + 2));
            if (OrderHeaderPaymentList.Sum(x => x.Total) >= general)
                OrderHeaderPaymentList.ForEach(x => x.IsPaymentOk = true);
            else
                OrderHeaderPaymentList.ForEach(x => x.IsPaymentOk = false);
            return Json(OrderHeaderPaymentList);
        }
        [HttpPost]
        public JsonResult RemoveOrderPayment(int payment)
        {
            OrderHeaderPaymentList.Remove(OrderHeaderPaymentList[payment]);
            return Json(OrderHeaderPaymentList);
        }
        [HttpPost]
        public string LoadBasketDataJsonResult(int basketType, bool coupon, string couponCode)
        {
            basketType = basketType == -1 ? (int)CurrentLoginType : basketType;
            GetActualCurrencies();
            BasketList = LoadBasketData(basketType, coupon, couponCode);
            return JsonConvert.SerializeObject(BasketList);
        }

        [HttpPost]
        public JsonResult LoadGeneralTotals()
        {
            BasketList = CalculateBasketTotals(BasketList, false, string.Empty, false);


            BasketList = CalculateBasketTotals(BasketList, false, string.Empty, false);

            BasketTotal basketTotal = new BasketTotal();

            basketTotal.TotalPriceCustomerCurrency = new Price(TotalPriceCustomerCurrency, CurrentCustomer.CurrencyType).ToString();
            basketTotal.TotalDiscountCustomerCurrency = new Price(TotalDiscountCustomerCurrency, CurrentCustomer.CurrencyType).ToString();
            basketTotal.TotalCostCustomerCurrency = new Price(TotalCostCustomerCurrency, CurrentCustomer.CurrencyType).ToString();
            basketTotal.TotalVATCustomerCurrency = new Price(TotalVATCustomerCurrency, CurrentCustomer.CurrencyType).ToString();
            basketTotal.GeneralTotal = new Price(GeneralTotal, CurrentCustomer.CurrencyType).ToString();
            basketTotal.GeneralTotalDouble = GeneralTotal;

            return Json(basketTotal);

        }

        [HttpPost]
        public JsonResult DeleteBasketItem(int id)
        {
            Basket item = BasketList.First(x => x.Id == id);
            item.Status = 2;
            item.EditId = CurrentEditId;

            MessageBox message = null;
            bool result = item.Update();
            message = result ? new MessageBox(MessageBoxType.Success, "Ürün sepetinizden silinmiştir.") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return Json(message);

        }

        [HttpPost]
        public string GetShipmentTypeList()
        {
            List<ShipmentType> list = ShipmentType.GetShipmentTypeList();
            list.Insert(0, new ShipmentType() { Name = "Seçiniz" });
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string LoadBasketNotes()
        {
            List<BasketNotes> list = BasketNotes.GetBasketNoteList(CurrentCustomer.Id, CurrentCustomer.Users.Id, CurrentSalesmanId);
            list.Insert(0, new BasketNotes() { Header = "Seçiniz" });
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public JsonResult UpdateItemFixedPrice(int id, string fixedPriceValue, string fixedPriceDecimal, bool isActive)
        {
            Basket item = BasketList.First(x => x.Id == id);
            item.FixedPrice = Convert.ToDouble(fixedPriceValue + "," + fixedPriceDecimal);
            item.IsFixedPrice = isActive;
            item.FixedCurrency = CurrentCustomer.CurrencyType;
            item.EditId = CurrentEditId;


            MessageBox message = null;
            bool result = item.Update();
            message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir.") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return Json(message);

        }

        [HttpPost]
        public JsonResult UpdateItemDiscSpecial(Basket basket)
        {
            basket.EditId = CurrentEditId;
            MessageBox message = null;
            bool result = false;
            if (!CurrentSalesman.AuthoritySalesman.IsSpecDiscount)
                message = new MessageBox(MessageBoxType.Error, "Bu işlemi yapmaya yetkiniz bulunmamaktadır.");
            else if (basket.DiscSpecial < basket.Product.MinSpecialDiscount)
                message = new MessageBox(MessageBoxType.Error, "Belirtilen ürüne en az %" + basket.Product.MinSpecialDiscount + " iskonto uygulanabilir.");
            else if (basket.DiscSpecial > basket.Product.MaxSpecialDiscount)
                message = new MessageBox(MessageBoxType.Error, "Belirtilen ürüne en fazla %" + basket.Product.MaxSpecialDiscount + " iskonto uygulanabilir.");
            else
            {
                result = basket.Update();
                message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir.") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            }
            return Json(message);

        }


        [HttpPost]
        public JsonResult DeleteSelected()
        {
            MessageBox message = null;
            List<Basket> tmp = BasketList.Where(p => p.Checked).ToList();
            if (tmp.Any())
            {
                foreach (var item in tmp)
                {
                    item.Delete();
                }
                message = new MessageBox(MessageBoxType.Success, "Seçilenler silindi.");
            }
            else
                message = new MessageBox(MessageBoxType.Error, "Seçili satır yok.");
            return Json(message);
        }
        [HttpPost]
        public JsonResult UpdateItemDeliveryDate(int id, DateTime deliveryDate)
        {

            MessageBox message = null;
            bool result = Basket.UpdateDeliveryDate(id, deliveryDate);
            message = result ? new MessageBox(MessageBoxType.Success, "Tarih Güncellendi") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return Json(message);
        }
        [HttpPost]
        public JsonResult UpdateBasketAddType(Basket basketItem, int basketType)
        {

            MessageBox message = null;
            basketItem.AddType = basketType;
            bool result = basketItem.Update();
            message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Başarılı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return Json(message);
        }


        [HttpPost]
        public JsonResult DeleteAll()
        {

            foreach (var item in BasketList)
            {
                item.Delete();
            }
            MessageBox message = new MessageBox(MessageBoxType.Success, "Sepetteki tüm ürünler silindi.");
            return Json(message);
        }



        [HttpPost]
        public JsonResult BasketItemChecked(int id, bool checkedValue)
        {
            BasketList.First(x => x.Id == id).Checked = checkedValue;
            if (BasketList.Any(x => x.PromotionId == id))
                BasketList.First(x => x.PromotionId == id).Checked = checkedValue;

            Basket item = BasketList.First(x => x.Id == id);
            item.Checked = checkedValue;
            item.Update();

            MessageBox message = new MessageBox(MessageBoxType.Success, "");
            return Json(message);
        }

        [HttpPost]
        public JsonResult UpdateQuantityValue(int id, double quantity)
        {
            Basket item = BasketList.First(x => x.Id == id);
            if (quantity < item.Product.MinOrder)
            {
                MessageBox message = new MessageBox(MessageBoxType.Error, "Minimum sipariş adedinden daha az sipariş veremezsiniz.");
                return Json(message);
            }
            else
            {
                if (item.Product.IsPackIncrease && item.Product.MinOrder != 1)
                {
                    double k = quantity / item.Product.MinOrder;

                    if (quantity % item.Product.MinOrder == 0)
                        quantity = Convert.ToInt32(k) * item.Product.MinOrder;
                    else
                        quantity = (Convert.ToInt32(Math.Floor(k)) + 1) * item.Product.MinOrder;
                }

                item.Quantity = quantity;

                double cmpAvailableQuantity = 0;

                if (item.Product.Campaign.Type > 0 && item.Quantity < item.Product.Campaign.MinOrder)
                    cmpAvailableQuantity = item.Product.Campaign.MinOrder - item.Quantity;

                MessageBox message = null;
                bool result = item.Update();
                message = result ? new MessageBox(MessageBoxType.Success, "Miktar güncellenmiştir.", Convert.ToInt32(cmpAvailableQuantity)) : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
                return Json(message);

            }
        }

        [HttpPost]
        public JsonResult UpdateItemNotes(int id, string note)
        {
            Basket item = BasketList.First(x => x.Id == id);
            item.EditId = CurrentEditId;
            item.Notes = note;
            BasketList.First(x => x.Id == id).Notes = note;

            MessageBox message = null;
            bool result = item.Update();
            message = result ? new MessageBox(MessageBoxType.Success, "Satır Notunuz Eklenmiştir.") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return Json(message);

        }


        [HttpPost]
        public JsonResult UpdateBasketItem(Basket basket)
        {
            basket.EditId = CurrentEditId;

            MessageBox message = null;
            bool result = basket.Update();
            message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir.") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return Json(message);

        }

        [HttpPost]
        public JsonResult BasketAllChecked(bool checkedValue)
        {
            foreach (var basketitem in BasketList)
            {
                BasketList.First(x => x.Id == basketitem.Id).Checked = checkedValue;
                if (BasketList.Any(x => x.PromotionId == basketitem.Id))
                    BasketList.First(x => x.PromotionId == basketitem.Id).Checked = checkedValue;

                Basket item = BasketList.First(x => x.Id == basketitem.Id);
                item.Checked = checkedValue;
                item.Update();
            }

            MessageBox message = new MessageBox(MessageBoxType.Success, "");
            return Json(message);
        }

        [HttpPost]
        public JsonResult SaveBasketNote(int id, string note)
        {
            BasketNotes item = new BasketNotes()
            {
                Id = id == -1 ? -1 : id,
                Header = note.Length > 20 ? note.Substring(0, 20) : note,
                Note = note,
                EditId = id == -1 ? -1 : CurrentEditId,
                CustomerId = CurrentCustomer.Id,
                UserId = CurrentCustomer.Users.Id,
                SalesmanId = CurrentSalesmanId
            };
            bool result = false;
            if (id == -1 || id == 0)
                result = item.Add();
            else
                result = item.Update();


            MessageBox message = null;
            message = result ? new MessageBox(MessageBoxType.Success, "Notunuz Eklenmiştir.") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return Json(message);


        }

        [HttpPost]
        public JsonResult DeleteBasketNote(int id, string note)
        {
            BasketNotes item = new BasketNotes()
            {
                Id = id,
                Header = note.Length > 20 ? note.Substring(0, 20) : note,
                Note = note,
                EditId = CurrentEditId,
                Deleted = true,
                CustomerId = CurrentCustomer.Id,
                UserId = CurrentCustomer.Users.Id,
                SalesmanId = CurrentSalesmanId
            };


            MessageBox message = null;
            bool result = item.Update();
            message = result ? new MessageBox(MessageBoxType.Success, "Notunuz Silinmiştir.") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return Json(message);

        }

        [HttpPost]
        public JsonResult ReasonItemChange(int id, int value)
        {
            ReasonList.First(x => x.BasketId == id).Status = value;
            MessageBox message = new MessageBox(MessageBoxType.Success, "");
            return Json(message);
        }
        [HttpPost]
        public JsonResult CheckPayment(int shipmentValue, string note, string paymentNote, string shipmentPerson, string shipmentAddress, string shipmentTel, string shipmentCity, string shipmentTown, bool freeShipping, string shipmentInformation)
        {

            MessageBox message = new MessageBox(MessageBoxType.Error, "Sipariş oluşturulacak kalem bulunamadı");
            List<Basket> senderList = BasketList.Where(x => x.Checked).ToList();
            foreach (CartGiveOrderNoQty item in ReasonList)
            {
                switch (item.Status)
                {
                    case 0:
                        break;
                    case 1:
                        if (item.QtyAvbl > 0)
                            senderList.First(x => x.Id == item.BasketId).Quantity = item.QtyAvbl;
                        else
                            senderList.Remove(senderList.First(x => x.Id == item.BasketId));
                        break;
                    case 2:
                        senderList.Remove(senderList.First(x => x.Id == item.BasketId));
                        break;
                    default:
                        break;
                }
            }

            int basketCount = senderList.Count;
            if (basketCount <= 0)
                return Json(message);

            ClearTotalValues();
            senderList = CalculateBasketTotals(senderList, false, string.Empty, false);

            if (B2BRuleItem.MinOrderTotal != 0 && B2BRuleItem.MinOrderTotal >= GeneralTotal)
            {
                message = new MessageBox(MessageBoxType.Warning, $"Minumum Sepet Tutarı En Düşük {B2BRuleItem.MinOrderTotal.ToString("N2")} {CurrentCustomer.CurrencyType} Olmalıdır.");
                return Json(message);
            }


            OrderHeader header = new OrderHeader();
            {
                header.CustomerId = CurrentCustomer.Id;
                header.UserId = CurrentCustomer.Users.Id;
                header.SalesmanId = CurrentSalesmanId;
                header.GeneralTotal = TotalPriceCustomerCurrency;
                header.Discount = TotalDiscountCustomerCurrency;
                header.NetTotal = TotalCostCustomerCurrency;
                header.Vat = TotalVATCustomerCurrency;
                header.Total = GeneralTotal;
                header.Notes = CurrentLoginType == LoginType.Customer ? note : string.Empty;
                header.SalesmanNotes = CurrentLoginType == LoginType.Customer ? string.Empty : note;
                header.SendingTypeId = shipmentValue;
                header.NumberOfProduct = basketCount;
                header.PaymentId = string.Empty;
                header.Status = B2BRuleItem.OrderAutomaticTransfer ? OrderStatus.AutomaticTransfer : OrderStatus.UnKnown;
                header.Currency = CurrentCustomer.CurrencyType;
                header.TotalAvailable = senderList.Sum(x => x.TotalCostWithVATAvailableCustomer);
                header.ShipmentPerson = shipmentPerson;
                header.ShipmentAddress = shipmentAddress;
                header.ShipmentTel = shipmentTel;
                header.ShipmentCity = shipmentCity;
                header.ShipmentTown = shipmentTown;
                header.PaymentNotes = paymentNote;
                header.FreeShipping = freeShipping;
                header.ShipmentInfo = shipmentInformation;
            };

            bool redirectToPayment = OrderHeaderPaymentList.Where(p => p.Type == 5).Sum(p => p.Total) >= Math.Round(header.Total);

            if (CurrentCustomer.PaymentOnOrder || redirectToPayment)
            {

                OrderForPayment orderForPayment = new OrderForPayment(header, senderList, OrderHeaderPaymentList);
                Session["OrderForPayment"] = orderForPayment;

                if (redirectToPayment)
                    Session["PaymentTotal"] = OrderHeaderPaymentList.Where(p => p.Type == 5).Sum(p => p.Total);
                else
                    Session["PaymentTotal"] = Convert.ToDouble(orderForPayment.OrderHeader.Total);

                message = new MessageBox(MessageBoxType.Payment, "Ödeme Sayfasına Yönlendirliyorsunuz.");
                return Json(message);
            }
            else
            {
                return SendOrder(header, senderList);
            }


        }

        [HttpPost]
        public JsonResult CheckOrderForSend()
        {
            ReasonList = new List<CartGiveOrderNoQty>();
            foreach (Basket item in BasketList)
            {
                if (item.Product.TotalQuantity < item.Quantity && item.Checked && !item.IsPromotion)
                {
                    bool isCampaign = (CurrentCustomer.CampaignStatu && CurrentCustomer.Users.AuthorityUser._CampaignStatu && item.Product.Campaign.Type > 0 && item.Quantity > item.Product.Campaign.MinOrder);
                    ReasonList.Add(new CartGiveOrderNoQty(item.Id, item.Product.Code, item.Product.Name, item.Quantity, item.Product.TotalQuantity <= 0 ? 0 : item.Product.TotalQuantity, CurrentLoginType, isCampaign));
                }
            }
            return Json(ReasonList);
        }

        [HttpPost]
        public JsonResult CheckCouponForCustomer(string couponCode)
        {
            CouponCs couponItem = CouponCs.GetCustomerCouponByCode(CurrentCustomer.Id, couponCode);

            var message = couponItem.Id > 0 ? new MessageBox(MessageBoxType.Success, couponCode + " Kodlu Kuponunuz Uygulanmıştır .", couponItem.Id) : new MessageBox(MessageBoxType.Error, couponCode + " Kodlu Kuponunuz Tanımlı Değildir.", 0);
            return Json(message);
        }

        [HttpPost]
        public JsonResult UploadExcelJson(List<CartExcelUploadItem> excelData, int basketType)
        {
            basketType = basketType == -1 ? (int)CurrentLoginType : basketType;

            dynamic returnData = new System.Dynamic.ExpandoObject();
            returnData.Status = true;
            returnData.Message = string.Empty;


            if (excelData.Count == 0)
            {
                returnData.Status = false;
                returnData.Message = "Seçtiğiniz dosyadan satır okunamadı.";
            }
            else
            {
                List<Basket> blist = Basket.GetBasketList(CurrentCustomer, CurrentLoginType, -1, basketType);
                List<string> successList = new List<string>();
                List<string> errorList = new List<string>();
                List<CartExcelUploadItem> notFoundList = new List<CartExcelUploadItem>();

                int emptyCodeRows = excelData.Where(p => string.IsNullOrEmpty(p.Code)).Count();
                if (emptyCodeRows > 0)
                {
                    errorList.Add(emptyCodeRows + " adet satırda STOK KODU yok.");

                    excelData = excelData.Where(p => !string.IsNullOrEmpty(p.Code)).ToList();
                }

                int qtyZeroRows = excelData.Where(p => p.Quantity <= 0).Count();
                if (qtyZeroRows > 0)
                {
                    errorList.Add(qtyZeroRows + " adet satırda MİKTAR yok veya sıfır.");

                    excelData = excelData.Where(p => p.Quantity > 0).ToList();
                }

                if (excelData.Count == 0)
                {
                    returnData.Status = false;
                    returnData.Message = "Tüm satırlar hatalı";
                    returnData.ErrorList = errorList;
                    returnData.NotFoundList = new List<CartExcelUploadItem>();
                    returnData.SuccessList = successList;
                }
                else
                {
                    foreach (CartExcelUploadItem item in excelData)
                    {
                        if (blist.Any(bsk => bsk.Product.Code == item.Code))
                        {
                            Basket basket = blist.First(bsk => bsk.Product.Code == item.Code);
                            string msg = item.Code + " miktar güncellendi. Eski= " + basket.Quantity;
                            basket.Quantity += item.Quantity;
                            basket.DiscSpecial = 0;
                            if (basket.Quantity % basket.Product.MinOrder != 0)
                                basket.Quantity = basket.Product.MinOrder * (((int)basket.Quantity / basket.Product.MinOrder) + 1);
                            msg += " Yeni= " + basket.Quantity;

                            basket.Update();
                            successList.Add(msg);
                        }
                        else
                        {
                            Product product = Product.GetByCode(item.Code, CurrentLoginType, CurrentCustomer);
                            if (product != null && product.Id > 0)
                            {
                                Basket basket = new Basket();
                                {
                                    basket.Product = product;
                                    basket.CustomerId = CurrentCustomer.Id;
                                    basket.SalesmanId = CurrentSalesmanId;
                                    basket.Quantity = item.Quantity;
                                    basket.DiscSpecial = 0;
                                    basket.RecordDate = DateTime.Now;
                                    basket.AddType = basketType;
                                    basket.ClientNumber = -1;
                                    basket.UserId = CurrentCustomer.Users.Id;
                                    basket.UserCode = CurrentCustomer.Users.Code;
                                    basket.CustomerCode = CurrentCustomer.Code;
                                    basket.ProductCode = product.Code;

                                }

                                if (basket.Quantity % basket.Product.MinOrder != 0)
                                    basket.Quantity = basket.Product.MinOrder * (((int)basket.Quantity / basket.Product.MinOrder) + 1);

                                basket.Add();
                                successList.Add(item.Code + " sepete eklendi. Miktar=" + basket.Quantity);
                            }
                            else
                            {
                                notFoundList.Add(new CartExcelUploadItem { Code = item.Code, Quantity = item.Quantity });
                                errorList.Add(item.Code + " stok yok veya pasif veya cari koşulu yok");
                            }
                        }
                    }

                    returnData.Status = true;
                    returnData.Message = errorList.Count() > 0 ? "Bazı satırlarda hata alındı." : "Excel Yükleme işlem başarılı";
                    returnData.ErrorList = errorList;
                    returnData.NotFoundList = notFoundList;
                    returnData.SuccessList = successList;

                }
            }


            return Json(returnData);
        }
        #endregion

        public JsonResult SendOrder(OrderHeader header, List<Basket> senderList)
        {
            //return Json(new MessageBox(MessageBoxType.Warning, "Siparişiniz iletilmiştir. Sipariş İle ilgili lütfen bölge müdürünüz ile görüşünüz"));

            MessageBox message = new MessageBox(MessageBoxType.Error, "Sipariş oluşturulacak kalem bulunamadı");
            int orderId = header.Save();
            bool detailSaveControl = true;

            List<Basket> senderListNew = new List<Basket>();
            if (senderList.Any(p => p.Product.IsKitMain))
            {
                foreach (var item in senderList)
                {
                    if (item.Product.IsKitMain)
                    {
                        List<Product> productKitList = Product.GetListByKitMainId(item.Product.Id, CurrentCustomer.Id);
                        foreach (var kitItem in productKitList)
                        {
                            Basket bNew = (Basket)item.Clone();
                            bNew.Product = kitItem;
                            bNew.Product.Rule = item.Product.Rule;
                            // bNew.Product.Campaign ??

                            bNew.Product.CalculateDetailInformation(bNew.IsCancelCampaign, (CurrentCustomer.CampaignStatu && CurrentCustomer.Users.AuthorityUser._CampaignStatu), bNew.Quantity, bNew.DiscSpecial, bNew.IsFixedPrice, bNew.FixedPrice, bNew.FixedCurrency, bNew.FixedCurrencyRate, 0, true);

                            senderListNew.Add(bNew);
                        }

                    }
                    else
                    {
                        senderListNew.Add(item);
                    }
                }
            }
            else
            {
                senderListNew = senderList;
            }

            foreach (Basket item in senderListNew)
            {
                bool result = false;
                OrderDetail detailItem = new OrderDetail();
                {
                    bool isCampaign = (CurrentCustomer.CampaignStatu && CurrentCustomer.Users.AuthorityUser._CampaignStatu && item.Product.Campaign.Type > 0 && item.Quantity >= item.Product.Campaign.MinOrder) ? true : false;
                    detailItem.OrderId = orderId;
                    detailItem.ProductId = item.Product.Id;
                    detailItem.ProductCode = item.Product.Code;
                    detailItem.ListPrice = item.Product.PriceList.Value;
                    detailItem.ListPriceCurrency = item.Product.PriceList.Currency;
                    detailItem.ListPriceCurrencyRate = item.Product.PriceCurrencyRate;
                    detailItem.Currency = CurrentCustomer.CurrencyType;
                    detailItem.CurrencyRate = CurrencyList.First(x => x.Type == CurrentCustomer.CurrencyType).Rate;
                    detailItem.CurrencyLocal = "TL";
                    detailItem.Disc1 = item.Product.Rule.Disc1;
                    detailItem.Disc2 = item.Product.Rule.Disc2;
                    detailItem.Disc3 = item.Product.Rule.Disc3;
                    detailItem.Disc4 = item.Product.Rule.Disc4;
                    detailItem.DiscSpecial = item.DiscSpecial;
                    detailItem.DueDay = item.Product.Rule.DueDay;
                    detailItem.IsCampaign = isCampaign;
                    detailItem.CampaignId = isCampaign ? item.Product.Campaign.Id : -1;
                    detailItem.CampaignCode = isCampaign ? item.Product.Campaign.Code : string.Empty;
                    detailItem.DiscCampaign = isCampaign ? item.Product.Campaign.Discount : 0;
                    detailItem.Price = (isCampaign && (item.Product.Campaign.Type == CampaignType.NetPrice || item.Product.Campaign.Type == CampaignType.GradualNetPrice)) ? item.Product.CampaignPriceCustomer.ValueFinal : item.Product.PriceListCustomer.ValueFinal;
                    detailItem.NetPrice = (item.Product.PriceNetCustomer.ValueFinal);
                    detailItem.Amount = (item.TotalPrice * item.Product.PriceCurrencyRate);
                    detailItem.NetAmount = (item.Product.PriceNetCustomer.ValueFinal * item.Quantity);
                    detailItem.VatAmount = (item.TotalVAT * item.Product.PriceCurrencyRate);
                    detailItem.Quantity = item.Quantity;
                    detailItem.CouponId = item.Product.CouponItem.UsedDiscountTl > 0 ? item.Product.CouponItem.Id : -1;
                    detailItem.CouponTotal = item.Product.CouponItem.UsedDiscountTl > 0 ? item.Product.CouponItem.UsedDiscountTl : 0;
                    detailItem.DiscCoupon = item.Product.CouponItem.UsedDiscountTl > 0 ? item.Product.CouponItem.Discount : 0;
                    detailItem.DiscCoupon1 = item.Product.CouponItem.UsedDiscountTl > 0 ? item.Product.CouponItem.Discount1 : 0;
                    detailItem.DiscCoupon2 = item.Product.CouponItem.UsedDiscountTl > 0 ? item.Product.CouponItem.Discount2 : 0;
                    detailItem.CustomerId = CurrentCustomer.Id;
                    string ItemExplanationStr = string.Empty;
                    foreach (var value in item.Product.SystemNotes)
                    {
                        ItemExplanationStr += value + " | ";
                    }
                    detailItem.ItemExplanation = ItemExplanationStr;

                }

                result = detailItem.Add();
                if (!result)
                {
                    detailSaveControl = false;
                    break;
                }
            }

            if (detailSaveControl)
            {
                string ids = senderList.Aggregate(string.Empty, (current, item) => current + (item.Id + ","));
                ids = ids.Remove(ids.Length - 1);
                OrderStatus orderStatus = (B2BRuleItem.OrderAutomaticTransfer) ? OrderStatus.AutomaticTransfer : OrderStatus.OnHold;

                if (header.Total > CurrentCustomer.RiskLimit)
                {
                    SendOrderMail(header, senderList);
                    orderStatus = OrderStatus.RegionalDirector;
                    message = new MessageBox(MessageBoxType.Warning, "Siparişiniz iletilmiştir. Sipariş İle ilgili lütfen bölge müdürünüz ile görüşünüz");


                }
                else
                {
                    SendOrderMail(header, senderList);
                    message = new MessageBox(MessageBoxType.Success, "Siparişiniz iletilmiştir. Sipariş numaranız: " + orderId);

                }
                Basket.UpdateBasketWithOrder(ids, orderId, (int)orderStatus);

                foreach (var orderHeaderPaymentItem in OrderHeaderPaymentList)
                {

                    if (orderHeaderPaymentItem.Base64 != null)
                    {
                        orderHeaderPaymentItem.Image = SaveImage(orderHeaderPaymentItem.Base64);

                    }
                    orderHeaderPaymentItem.OrderId = orderId;
                    orderHeaderPaymentItem.CreateId = CurrentEditId;
                    orderHeaderPaymentItem.Insert();
                }

            }
            else
            {
                message = new MessageBox(MessageBoxType.Error, "Sipariş gönderilmesinde hata oluşmuştur.");
            }


            return Json(message);
        }
        private void SendOrderMail(OrderHeader header, List<Basket> senderList)
        {
            try
            {
                string contentHtml = "";
                contentHtml = System.IO.File.ReadAllText(Server.MapPath("/files/mailtemplate/order.html"));
                string companyName = CompanyInformation.GetAll()[0].Title.ToString();
                contentHtml = contentHtml.Replace("{CustomerCode}", CurrentCustomer.Code).
                     Replace("{CustomerName}", CurrentCustomer.Name)
                    .Replace("{CustomerCity}", CurrentCustomer.City)
                    .Replace("{CustomerTown}", CurrentCustomer.Town)
                    .Replace("{CustomerAddress}", CurrentCustomer.Address)
                    .Replace("{OrderNo}", header.Id.ToString())
                    .Replace("{OrderDate}", DateTime.Now.ToShortDateString())
                    .Replace("{Salesman}", CurrentLoginType == LoginType.Salesman ? CurrentSalesman.Name : " ")
                    .Replace("{CustomerTel}", CurrentCustomer.Tel1)
                    .Replace("{CustomerFax}", CurrentCustomer.Fax1)
                    .Replace("{SenderName}", header.SalesmanId == -1 ? "M" : "P")
                    .Replace("{SenderType}", header.SalesmanId == -1 ? "Müşteri" : "Plasiyer")
                    .Replace("{Date}", DateTime.Now.ToString())
                    .Replace("{Total}", header.TotalStr)
                    .Replace("{NetTotal}", header.NetTotalStr)
                    .Replace("{VatTotal}", header.VatStr)
                    .Replace("{GeneralTotal}", header.TotalStr)
                    .Replace("{OrderNote}", header.Notes + " " + header.SalesmanNotes)
                    .Replace("{CompanyName}", companyName);

                string repeatItem = contentHtml.Split('|')[1];

                string repeatItemDetail = string.Empty;

                foreach (var item in senderList)
                {
                    repeatItemDetail += repeatItem.Replace("{ProductCode}", item.Product.Code)
                        .Replace("{ProductName}", item.Product.Name)
                        .Replace("{Manufacturer}", item.Product.Manufacturer)
                        .Replace("{ShelfAddress}", item.Product.ShelfAddress)
                        .Replace("{OrderQuantity}", item.Quantity.ToString())
                        .Replace("{Quantity}", item.Quantity.ToString())
                        .Replace("{Discount}", item.Product.DiscountStr)
                        .Replace("{NetPrice}", item.Product.PriceNetStr)
                        .Replace("{ItemTotal}", item.TotalCostWithVAT.ToString());
                }

                contentHtml = contentHtml.Replace(repeatItem, repeatItemDetail).Replace("|", "");
                MailMessage mail = new MailMessage();
                mail.To.Add("r.yalcin@hantech.com.tr");
                if (CurrentCustomer.RegionCode == "MARMARA")
                {
                    //mail.To.Add("hacer.dogan@eryaz.net");
                    //mail.CC.Add("birol.gur@eryaz.net");
                    mail.To.Add("adil.gundogdu@hantech-turkey.com.tr");
                    mail.CC.Add("metin.uzun@hantech-turkey.com.tr");
                }
                else if (CurrentCustomer.RegionCode == "ANTALYA")
                {
                    mail.CC.Add("metin.uzun@hantech-turkey.com.tr");
                    mail.To.Add("a.kumsal@hantech.com.tr");
                }
                else if (CurrentCustomer.RegionCode.Contains("ÇUKUROVA - G.DOĞU - D.ANAD"))
                {
                    //mail.To.Add("hacer.dogan@eryaz.net");
                    //mail.CC.Add("birol.gur@eryaz.net");
                    mail.To.Add("m.cakmak@hantech.com.tr");
                    mail.CC.Add("metin.uzun@hantech-turkey.com.tr");

                }
                else if (CurrentCustomer.RegionCode == "EGE")
                {
                    mail.To.Add("t.cakir@hantech.com.tr");
                    mail.CC.Add("metin.uzun@hantech-turkey.com.tr");
                }
                else if (CurrentCustomer.RegionCode.Contains("İÇ ANADOLU - KARADENİZ") || CurrentCustomer.RegionCode == "KKTC")
                {
                    mail.To.Add("metin.uzun@hantech-turkey.com.tr");
                }
                else
                {
                    mail.To.Add("satis@hantech.com.tr");
                }
                mail.Subject = "Yeni Siparişiniz Var";
                mail.Body = contentHtml;
                mail.IsBodyHtml = true;
                EmailHelper.Send(mail);


            }
            catch (Exception ex)
            {

            }

        }

        public string SaveImage(Base64Input base64)
        {
            byte[] imageBytes = Convert.FromBase64String(base64.base64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            string name = "";
            if (base64.filetype == "image/jpeg" || base64.filetype == "image/jpg")
            {
                name = Guid.NewGuid().ToString() + ".jpg";
                string FilePath = Server.MapPath("~/Content/images/payments/") + name;
                image.Save(FilePath, ImageFormat.Jpeg);
            }
            else if (base64.filetype == "image/png")
            {
                name = Guid.NewGuid().ToString() + ".png";
                string FilePath = Server.MapPath("~/Content/images/payments/") + name;
                image.Save(FilePath, ImageFormat.Png);
            }

            return name;
        }
        public class CartExcelUploadItem
        {
            public string Code { get; set; }
            public double Quantity { get; set; }
        }

        private void ClearFields()
        {
            ClearTotalValues();
            BasketList = new List<Basket>();
        }

        private void ClearTotalValues()
        {
            TotalPrice = 0;
            TotalDiscount = 0;
            TotalCost = 0;
            TotalVAT = 0;
            TotalPriceCustomerCurrency = 0;
            TotalDiscountCustomerCurrency = 0;
            TotalCostCustomerCurrency = 0;
            TotalVATCustomerCurrency = 0;
        }

        private List<Basket> LoadBasketData(int basketType, bool coupon, string couponCode)
        {
            ClearFields();
            List<Basket> _basketList = Basket.GetBasketList(CurrentCustomer, CurrentLoginType, CurrentSalesmanId, basketType);

            List<Basket> tmpBasketList = _basketList;
            int promotionCount = 0;
            List<Basket> tmp2BasketList = new List<Basket>();
            foreach (Basket basketItem in tmpBasketList)
            {
                Basket basket = basketItem;
                if (basket.IsFixedPrice && basket.Product.Campaign.Type > 0)
                {
                    basket.Product.Campaign = new Campaign();
                    basket.Product.SystemNotes.Add("Özel fiyat sebebiyle kampanya iptal edilmiştir.");
                }



                tmp2BasketList.Add(basket);
                if (CurrentCustomer.CampaignStatu && CurrentCustomer.Users.AuthorityUser._CampaignStatu && basket.Product.Campaign.Type == CampaignType.PromotionProduct &&
                    basket.Quantity >= basket.Product.Campaign.MinOrder && !basket.IsCancelCampaign)
                {

                    string[] promArray = basket.Product.Campaign.PromotionProductCode.Split(',');
                    List<PromotionList> promList = new List<PromotionList>();
                    string promotionCodes = "";

                    foreach (var item in promArray)
                    {
                        promList.Add(new PromotionList
                        {
                            Code = item.Substring(0, item.IndexOf(':')) + "'",
                            Quantity = double.Parse(item.Substring(item.IndexOf(':') + 1, (item.IndexOf(';') - 1 - item.IndexOf(':')))),
                            MinOrder = double.Parse(item.Substring(item.IndexOf(';') + 1, item.Length - item.IndexOf(';') - 2))
                        });
                        promotionCodes += (item.Substring(0, item.IndexOf(':')) + "',");
                    }
                    promotionCodes = promotionCodes.Substring(0, promotionCodes.Length - 1);

                    int promCount = promArray.Length;
                    foreach (var item in Product.GetByIdProductCodes(promotionCodes, CurrentLoginType, CurrentCustomer))
                    {


                        Basket promotionBasket = new Basket();
                        {
                            promotionBasket.Id = basket.Id * -promCount;
                            promotionBasket.IsPromotion = true;
                            promotionBasket.Product = item;
                            promotionBasket.Quantity = promList.Where(x => x.Code.Replace("'", "") == item.Code).First().Quantity * (int)(basket.Quantity / (basket.Product.Campaign.MinOrder + promList.Where(x => x.Code.Replace("'", "") == item.Code).First().MinOrder));
                            promotionBasket.Product.Campaign.Type = 0;
                            promotionBasket.Product.Rule.Disc1 = 100;
                            promotionBasket.Product.Rule.Disc2 = 0;
                            promotionBasket.Product.Rule.Disc3 = 0;
                            promotionBasket.Product.Rule.Disc4 = 0;
                            promotionBasket.Product.RuleAdditional.Disc1 = 0;
                            promotionBasket.Product.RuleAdditional.Disc1 = 0;
                            promotionBasket.Product.RuleAdditional.Disc3 = 0;
                            promotionBasket.Product.RuleAdditional.Disc4 = 0;
                            promotionBasket.Checked = basket.Checked;
                            promotionBasket.PromotionId = basket.Id;
                            promotionBasket.Product.SystemNotes.Add("Promosyon ürün olduğu için 100% iskonto ile eklenmiştir.");
                            promotionBasket.RowCss = "promotion";
                            promotionBasket.Product.DiscountStr = string.Empty;
                        }
                        basket.Product.SystemNotes.Add("İlgili ürüne hediye olarak " + promotionBasket.Product.Code + " kodlu ürün sepete eklenmiştir");
                        basket.RowCmpText = "Promosyon";
                        promotionCount++;
                        if (basket.Quantity >= promList.Where(x => x.Code.Replace("'", "") == item.Code).First().MinOrder + basket.Product.Campaign.MinOrder)
                        {
                            if (!basket.Product.Campaign.PromotionAllProduct)
                            {
                                if (tmp2BasketList.Where(x => x.PromotionId == basket.Id).Count() > 0 &&
                                     promList.Where(r => r.Code.Replace("'", "") == tmp2BasketList.Where(x => x.PromotionId == basket.Id).First().Product.Code).First().MinOrder < promList.Where(x => x.Code.Replace("'", "") == item.Code).First().MinOrder
                                    )
                                    tmp2BasketList.Remove(tmp2BasketList.Where(x => x.PromotionId == basket.Id).First());

                                if (tmp2BasketList.Where(x => x.PromotionId == basket.Id).Count() == 0)
                                {
                                    tmp2BasketList.Add(promotionBasket);
                                    basketItem.LineInBasket = true;
                                }
                            }
                            else
                            {
                                tmp2BasketList.Add(promotionBasket);
                                basketItem.LineInBasket = true;
                            }
                        }
                        else
                            promCount -= 1;
                    }
                    basketItem.PromotionProductCount = !basket.Product.Campaign.PromotionAllProduct ? 2 : promCount + 1;
                }
                else if (!basket.IsCancelCampaign && basketItem.Product.Campaign.Type > 0 && basketItem.Product.Campaign.TotalQuantity > 0 && (basketItem.Quantity > (basketItem.Product.Campaign.TotalQuantity - basketItem.Product.Campaign.SaledQuantity)))
                {
                    basketItem.PromotionProductCount = 2;
                    Basket campaignExtra = new Basket();
                    {
                        campaignExtra.Id = basket.Id * -1;
                        campaignExtra.IsCmpExtra = true;
                        campaignExtra.Product = Product.GetByCode(basket.Product.Code, CurrentLoginType, CurrentCustomer);
                        campaignExtra.Quantity = (basketItem.Quantity - (basketItem.Product.Campaign.TotalQuantity - basketItem.Product.Campaign.SaledQuantity));
                        campaignExtra.Product.Campaign = new Campaign();
                        campaignExtra.Checked = basket.Checked;
                        campaignExtra.PromotionId = basket.Id;
                        campaignExtra.Product.SystemNotes.Add("Kampanya fazlası olarak otomatik eklenmiştir");
                        campaignExtra.Product.DiscountStr = string.Empty;
                    }
                    basket.RowCmpText = "Kmp. Extra";
                    basket.Product.SystemNotes.Add("İlgili üründe kampanya fazlası bölünmüştür");
                    tmp2BasketList.Add(campaignExtra);
                    basketItem.LineInBasket = true;
                    basketItem.CampaignLineInBasket = true;
                    basketItem.Quantity = (basketItem.Product.Campaign.TotalQuantity - basketItem.Product.Campaign.SaledQuantity);
                }
                else
                    basketItem.PromotionId = basketItem.Id;
            }
            _basketList = tmp2BasketList;

            _basketList = CalculateBasketTotals(_basketList, coupon, couponCode);
            return _basketList;
        }

        private List<Basket> CalculateBasketTotals(List<Basket> basketList, bool coupon = false, string couponCode = "", bool notes = true)
        {
            ClearTotalValues();
            foreach (var basket in basketList)
            {
                basket.Product.CalculateDetailInformation(basket.IsCancelCampaign, (CurrentCustomer.CampaignStatu && CurrentCustomer.Users.AuthorityUser._CampaignStatu), basket.Quantity, basket.DiscSpecial, basket.IsFixedPrice, basket.FixedPrice, basket.FixedCurrency, basket.FixedCurrencyRate, 0, notes);
                basket.FixedPrice = basket.FixedPrice == 0 ? basket.Product.PriceNetCustomer.ValueFinal : basket.FixedPrice;
                basket.Product.AvailabilityStatus = basket.Product.CalculateAvaibilityStatus(basket.Product.TotalQuantity);
                string availabilityText, availabilityCss;
                basket.Product.SetAvailabilityClass(basket.Product.LoginType, basket.Product.TotalQuantity, basket.Product.AvailabilityStatus, out availabilityCss, out availabilityText);
                basket.Product.AvailabilityCss = availabilityCss;
                basket.Product.AvailabilityText = availabilityText;
                if (notes)
                    basket.Product.SystemNotes.Add("Sepete eklenme tarihi :" + basket.RecordDate.ToString());
                if (basket.Checked)
                {
                    TotalPrice += basket.TotalPrice;
                    TotalDiscount += basket.TotalDiscount;
                    TotalCost += basket.TotalCost;
                    TotalVAT += basket.TotalVAT;

                    TotalPriceCustomerCurrency += basket.TotalPrice * basket.Product.PriceCurrencyRate;
                    TotalDiscountCustomerCurrency += basket.TotalDiscount * basket.Product.PriceCurrencyRate;
                    TotalCostCustomerCurrency += basket.TotalCost * basket.Product.PriceCurrencyRate;
                    TotalVATCustomerCurrency += basket.TotalVAT * basket.Product.PriceCurrencyRate;
                }

                basket.RowCss = basket.Product.NewProduct ? "new-product" : "";
                basket.RowCss = (basket.Product.Campaign.Type > 0 && !basket.IsCancelCampaign) ? "campaign" : basket.RowCss;
                basket.RowCss = (basket.IsFixedPrice && basket.Product.Campaign.Type == CampaignType.None) ? "fixed-price" : basket.RowCss;
            }
            List<CouponCs> couponList = CouponCs.GetBasketAutoCoupon(CurrentCustomer.Id);
            if (coupon)
            {
                CouponCs couponItem = CouponCs.GetCustomerCouponByCode(CurrentCustomer.Id, couponCode);
                if (couponItem.Id > 0)
                    couponList.Add(couponItem);
            }

            couponList = couponList.Where(x => x.UserType == 0 || x.UserType == (CurrentLoginType == LoginType.Salesman ? 2 : 1)).ToList();


            if (couponList.Count > 0 && CurrentCustomer.CampaignStatu && CurrentCustomer.Users.AuthorityUser._CampaignStatu)
            {
                List<Basket> promotionList = new List<Basket>();

                foreach (CouponCs item in couponList.OrderBy(x => x.Priority))
                {
                    double basketTotal = 0, basketCounTotal = 0, itemCounter = 0;

                    #region GetTotals
                    foreach (var basket in basketList)
                    {
                        if (basket.Checked && (basket.Product.Campaign.Type == CampaignType.None || item.IsIgnoreCampaign) && basket.Product.CouponItem.Id == 0 && !basket.IsPromotion)
                        {
                            basket.Product.CouponItem = new CouponCs();
                            if (item.IsOnlySelectedItemTotal)
                            {
                                bool controlContainsManufacturer = true;
                                bool controlContainsProductGroups = true;
                                bool controlContainsRules = true;
                                bool controlContainsSpecialCodes = true;

                                // filtrelemeye göre topla
                                if (!String.IsNullOrEmpty(item.Manufacturers))
                                {
                                    if (item.Manufacturers.Contains("<" + basket.Product.Manufacturer + ">"))
                                        controlContainsManufacturer = true;
                                    else
                                        controlContainsManufacturer = false;
                                }

                                if (!String.IsNullOrEmpty(item.ProductGroups))
                                {
                                    if (item.ProductGroups.Contains("<" + basket.Product.ProductGroup1 + ">"))
                                        controlContainsProductGroups = true;
                                    else
                                        controlContainsProductGroups = false;
                                }

                                if (!String.IsNullOrEmpty(item.Rules))
                                {
                                    if (item.Rules.Contains("<" + basket.Product.RuleCode + ">"))
                                        controlContainsRules = true;
                                    else
                                        controlContainsRules = false;
                                }

                                if (!String.IsNullOrEmpty(item.SpecialCodes))
                                {
                                    if (item.SpecialCodes.Contains("<" + basket.Product.SpecialCode1 + ">"))
                                        controlContainsSpecialCodes = true;
                                    else
                                        controlContainsSpecialCodes = false;
                                }

                                if (controlContainsManufacturer && controlContainsProductGroups && controlContainsRules && controlContainsSpecialCodes)
                                {
                                    if (item.IsJustAvailable)
                                    {
                                        basketCounTotal += basket.QuantityAvailable;
                                        basketTotal += (basket.TotalCostAvailable * basket.Product.PriceCurrencyRate);
                                    }
                                    else
                                    {
                                        basketCounTotal += basket.Quantity;
                                        basketTotal += (basket.TotalCost * basket.Product.PriceCurrencyRate);
                                    }


                                    itemCounter += 1;
                                }
                            }
                            else
                            {
                                // filtreleme olmadan topla
                                if (item.IsJustAvailable)
                                {
                                    basketCounTotal += basket.QuantityAvailable;
                                    basketTotal += (basket.TotalCostAvailable * basket.Product.PriceCurrencyRate);
                                }
                                else
                                {
                                    basketCounTotal += basket.Quantity;
                                    basketTotal += (basket.TotalCost * basket.Product.PriceCurrencyRate);
                                }

                                itemCounter += 1;
                            }
                        }
                    }

                    #endregion

                    #region checkDiscount
                    ClearTotalValues();
                    double itemDiscountPrice = item.Price;

                    foreach (var basket in basketList)
                    {
                        if (basket.Checked && (basket.Product.Campaign.Type == CampaignType.None || item.IsIgnoreCampaign) && basket.Product.CouponItem.Id == 0 && !basket.IsPromotion)
                        {
                            if ((item.CalculateType == 0 && basketTotal >= item.MinPrice) || (item.CalculateType == 1 && basketCounTotal >= item.MinQuantity))
                            {
                                bool controlContainsManufacturer = true;
                                bool controlContainsProductGroups = true;
                                bool controlContainsRules = true;
                                bool controlContainsSpecialCodes = true;

                                // filtrelemeye göre topla
                                if (!String.IsNullOrEmpty(item.Manufacturers))
                                {
                                    if (item.Manufacturers.Contains("<" + basket.Product.Manufacturer + ">"))
                                        controlContainsManufacturer = true;
                                    else
                                        controlContainsManufacturer = false;
                                }

                                if (!String.IsNullOrEmpty(item.ProductGroups))
                                {
                                    if (item.ProductGroups.Contains("<" + basket.Product.ProductGroup1 + ">"))
                                        controlContainsProductGroups = true;
                                    else
                                        controlContainsProductGroups = false;
                                }

                                if (!String.IsNullOrEmpty(item.Rules))
                                {
                                    if (item.Rules.Contains("<" + basket.Product.RuleCode + ">"))
                                        controlContainsRules = true;
                                    else
                                        controlContainsRules = false;
                                }

                                if (!String.IsNullOrEmpty(item.SpecialCodes))
                                {
                                    if (item.SpecialCodes.Contains("<" + basket.Product.SpecialCode1 + ">"))
                                        controlContainsSpecialCodes = true;
                                    else
                                        controlContainsSpecialCodes = false;
                                }


                                if (item.Type == 0 && (controlContainsManufacturer && controlContainsProductGroups && controlContainsRules && controlContainsSpecialCodes))
                                {
                                    basket.ItemAvailableCoupon = true;
                                    if (!basket.IsCancelCampaign)
                                    {
                                        basket.Product.CouponItem = item;
                                        // iskonto uygulaması
                                        basket.RowCss = "coupon";
                                    }

                                }
                                else if (item.Type == 1 && (controlContainsManufacturer && controlContainsProductGroups && controlContainsRules && controlContainsSpecialCodes))
                                {
                                    basket.ItemAvailableCoupon = true;
                                    if (!basket.IsCancelCampaign)
                                    {
                                        basket.Product.CouponItem = item;
                                        basket.Product.CouponItem.Price = itemDiscountPrice / itemCounter;
                                        basket.RowCss = "coupon";
                                    }
                                    // Tutar uygulaması
                                }
                                else if (item.Type == 2 && (controlContainsManufacturer && controlContainsProductGroups && controlContainsRules && controlContainsSpecialCodes) && basketCounTotal > 0)
                                {
                                    basket.ItemAvailableCoupon = true;
                                    if (!basket.IsCancelCampaign)
                                    {
                                        basket.Product.CouponItem = item;
                                        Basket promotionBasket = new Basket();
                                        {

                                            int counterValue = 1;
                                            if ((item.IsCounter && item.MinQuantity > 0))
                                                counterValue = (int)(basketCounTotal / item.MinQuantity);
                                            else if ((item.IsCounter && item.MinPrice > 0))
                                                counterValue = (int)(basketTotal / item.MinPrice);


                                            promotionBasket.Id = basket.Id * -1;
                                            promotionBasket.IsPromotion = true;
                                            promotionBasket.Product = Product.GetByCode(item.ProductCode, CurrentLoginType, CurrentCustomer);
                                            promotionBasket.Quantity = item.ProductQuantity * counterValue;
                                            promotionBasket.Product.Campaign.Type = 0;
                                            promotionBasket.Product.Rule.Disc1 = 100;
                                            promotionBasket.Product.Rule.Disc2 = 0;
                                            promotionBasket.Product.Rule.Disc3 = 0;
                                            promotionBasket.Product.Rule.Disc4 = 0;
                                            promotionBasket.Product.RuleAdditional.Disc1 = 0;
                                            promotionBasket.Product.RuleAdditional.Disc1 = 0;
                                            promotionBasket.Product.RuleAdditional.Disc3 = 0;
                                            promotionBasket.Product.RuleAdditional.Disc4 = 0;
                                            promotionBasket.Checked = basket.Checked;
                                            promotionBasket.PromotionId = basket.Id;
                                            promotionBasket.Product.SystemNotes.Add("Promosyon ürün olduğu için 100% iskonto ile eklenmiştir.");
                                            promotionBasket.RowCss = "promotion";
                                            promotionBasket.Product.DiscountStr = string.Empty;
                                        }
                                        basket.Product.SystemNotes.Add("Üründe " + item.Header + " kampanyası uygulanmıştır");

                                        basket.RowCmpText = "Promosyon";
                                        promotionBasket.Product.CalculateDetailInformation(basket.IsCancelCampaign, (CurrentCustomer.CampaignStatu && CurrentCustomer.Users.AuthorityUser._CampaignStatu), basket.Quantity, basket.DiscSpecial, basket.IsFixedPrice, basket.FixedPrice, basket.FixedCurrency, basket.FixedCurrencyRate, 0, true);
                                        promotionList.Add(promotionBasket);
                                        basket.RowCss = "coupon";
                                        basket.LineInBasket = true;
                                        basketCounTotal = 0;
                                        basketTotal = 0;
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }
                List<Basket> tmpList = new List<Basket>();
                List<Basket> tmpList2 = new List<Basket>();
                tmpList.AddRange(basketList);

                foreach (var basket in tmpList)
                {
                    tmpList2.Add(basket);
                    if (!basket.IsCancelCampaign && promotionList.Count > 0 && promotionList.Where(x => (x.Id * -1) == basket.Id).Count() > 0)
                    {
                        basket.Product.SystemNotes.Add("İlgili ürüne hediye olarak " + promotionList.Where(x => (x.Id * -1) == basket.Id).First().Product.Code + " kodlu ürün sepete eklenmiştir");
                        tmpList2.Add(promotionList.Where(x => (x.Id * -1) == basket.Id).First());
                    }

                    basket.Product.CalculateDetailInformation(basket.IsCancelCampaign, (CurrentCustomer.CampaignStatu && CurrentCustomer.Users.AuthorityUser._CampaignStatu), basket.Quantity, basket.DiscSpecial, basket.IsFixedPrice, basket.FixedPrice, basket.FixedCurrency, basket.FixedCurrencyRate, 0, false);
                    if (basket.Checked)
                    {
                        TotalPrice += basket.TotalPrice;
                        TotalDiscount += basket.TotalDiscount;
                        TotalCost += basket.TotalCost;
                        TotalVAT += basket.TotalVAT;

                        TotalPriceCustomerCurrency += basket.TotalPrice * basket.Product.PriceCurrencyRate;
                        TotalDiscountCustomerCurrency += basket.TotalDiscount * basket.Product.PriceCurrencyRate;
                        TotalCostCustomerCurrency += basket.TotalCost * basket.Product.PriceCurrencyRate;
                        TotalVATCustomerCurrency += basket.TotalVAT * basket.Product.PriceCurrencyRate;
                    }
                }
                basketList = new List<Basket>();
                basketList.AddRange(tmpList2);
            }

            return basketList;

        }
    }
    public class PromotionList
    {
        public string Code { get; set; }
        public double Quantity { get; set; }
        public double MinOrder { get; set; }
    }

    public class BasketTotal
    {
        public string TotalPriceCustomerCurrency { get; set; }
        public string TotalDiscountCustomerCurrency { get; set; }
        public string TotalCostCustomerCurrency { get; set; }
        public string TotalVATCustomerCurrency { get; set; }
        public string GeneralTotal { get; set; }
        public double GeneralTotalDouble { get; set; }
    }
}