using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class OrderDetail : DataAccess
    {
        public OrderDetail()
        {
            ProductCode = string.Empty;
            CampaignCode = string.Empty;
        }

        #region Properties
        public int Id { get; set; }
        public int OrderId { get; set; }
        public double VatRate { get; set; }
        public int OldOrderId { get; set; }
        public string ShelfAddress { get; set; }
        public double TotalQuantity { get; set; }
        public int ProductId { get; set; }
        public double Quantity { get; set; }
        public double ShippedQty { get; set; }
        public double ListPrice { get; set; }
        public string ListPriceCurrency { get; set; }
        public double ListPriceCurrencyRate { get; set; }
        public double Price { get; set; }
        public string PriceStr { get { return new Price(Price, Currency).ToString(); } }
        public double NetPrice { get; set; }
        public string NetPriceStr { get { return new Price(NetPrice, Currency).ToString(); } }
        public double Amount { get; set; }
        public string AmountStr { get { return new Price(Amount, Currency).ToString(); } }
        public double NetAmount { get; set; }
        public string NetAmountStr { get { return new Price(NetAmount, Currency).ToString(); } }
        public string NetAmountWithVat { get { return new Price((NetAmount + VatAmount), "").ToString(); } }
        public string NetAmountWithVatStr { get { return new Price((NetAmount + VatAmount), Currency).ToString(); } }

        public double VatAmount { get; set; }
        public string VatAmountStr { get { return new Price(VatAmount, Currency).ToString(); } }
        public string Currency { get; set; }
        public double CurrencyRate { get; set; }
        public double PriceLocal { get; set; }
        public double NetPriceLocal { get; set; }
        public double AmountLocal { get; set; }
        public double NetAmountLocal { get; set; }
        public double VatAmountLocal { get; set; }
        public string CurrencyLocal { get; set; }
        public double Disc1 { get; set; }
        public double Disc2 { get; set; }
        public double Disc3 { get; set; }
        public double Disc4 { get; set; }
        public double DiscCampaign { get; set; }
        public double DiscSpecial { get; set; }
        public string DiscountStr
        {
            get
            {
                if (Disc1 == 0)
                    return "-";
                else
                {
                    string disc = "%" + Disc1.ToString();
                    if (Disc2 > 0) disc += "+" + Disc2.ToString();
                    if (Disc3 > 0) disc += "+" + Disc3.ToString();
                    if (Disc4 > 0) disc += "+" + Disc4.ToString();
                    if (DiscCampaign > 0) disc += "+" + DiscCampaign.ToString();
                    if (DiscSpecial > 0) disc += "+" + DiscSpecial.ToString();
                    return disc;
                }
            }
        }
        public int DueDay { get; set; }
        public int Status { get; set; }
        public int CampaignId { get; set; }
        public string CampaignCode { get; set; }
        public bool IsCampaign { get; set; }
        public int CouponId { get; set; }
        public double CouponTotal { get; set; }
        public string ItemExplanation { get; set; }
        public double DiscCoupon { get; set; }
        public double DiscCoupon1 { get; set; }
        public double DiscCoupon2 { get; set; }
        public int CustomerId { get; set; }
        #endregion

        #region orderDetailsProperties
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductShortName { get { return (ProductName.Length > 35 ? ProductName.Substring(0, 35) : ProductName); } }
        public string Manufacturer { get; set; }
        public string Unit { get; set; }
        #endregion

        #region Methods

        public bool Add()
        {
            return DAL.InsertOrderDetail(OrderId, ProductId, Quantity, ListPrice, ListPriceCurrency, ListPriceCurrencyRate, Price, NetPrice, Amount, NetAmount, VatAmount, Currency, CurrencyRate, Disc1, Disc2, Disc3, Disc4, DiscCampaign, DiscSpecial, DueDay, CampaignId, CampaignCode, ProductCode, IsCampaign, CouponId, CouponTotal, ItemExplanation, DiscCoupon, DiscCoupon1, DiscCoupon2, CustomerId);
        }

        internal bool Delete()
        {
            return DAL.DeleteOrderDetail(Id);
        }

        public static List<OrderDetail> GetOrderDetail(int orderId)
        {
            List<OrderDetail> list = new List<OrderDetail>();
            DataTable dt = DAL.GetOrderDetail(orderId);
            foreach (DataRow row in dt.Rows)
            {
                OrderDetail obj = new OrderDetail()
                {
                    Id = row.Field<int>("Id"),
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductId = row.Field<int>("ProductId"),
                    ListPriceCurrencyRate = row.Field<double>("ListPriceCurrencyRate"),
                    ProductName = row.Field<string>("ProductName"),
                    Manufacturer = row.Field<string>("Manufacturer"),
                    Unit = row.Field<string>("Unit"),
                    Quantity = row.Field<double>("Quantity"),
                    ShippedQty = row.Field<double>("ShippedQty"),
                    Disc1 = row.Field<double>("Disc1"),
                    Disc2 = row.Field<double>("Disc2"),
                    Disc3 = row.Field<double>("Disc3"),
                    Disc4 = row.Field<double>("Disc4"),
                    DiscCampaign = row.Field<double>("DiscCampaign"),
                    DiscSpecial = row.Field<double>("DiscSpecial"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Amount = row.Field<double>("Amount"),
                    NetAmount = row.Field<double>("NetAmount"),
                    Price = row.Field<double>("Price"),
                    NetPrice = row.Field<double>("NetPrice"),
                    Currency = row.Field<string>("Currency"),
                    IsCampaign = row.Field<bool>("IsCampaign"),
                    Status = row.Field<int>("Status"),
                    VatAmount = row.Field<double>("VatAmount"),
                    DiscCoupon = row.Field<double>("DiscCoupon"),
                    DiscCoupon1 = row.Field<double>("DiscCoupon1"),
                    DiscCoupon2 = row.Field<double>("DiscCoupon2"),
                    PriceLocal = Convert.ToDouble(row["PriceLocal"]),
                    NetPriceLocal = Convert.ToDouble(row["NetPriceLocal"]),
                    AmountLocal = Convert.ToDouble(row["AmountLocal"]),
                    NetAmountLocal = Convert.ToDouble(row["NetAmountLocal"]),
                    VatAmountLocal = Convert.ToDouble(row["VatAmountLocal"]),
                    ItemExplanation = row.Field<string>("ItemExplanation").Replace("|", "</br>"),
                    ShelfAddress = row.Field<string>("ShelfAddress"),
                    TotalQuantity = row.Field<double>("TotalQuantity"),
                    VatRate = row.Field<double>("VatRate"),

                };
                list.Add(obj);
            }
            return list;
        }
        public static void Calculate(OrderHeader header, OrderDetail detailRow, bool isDelete = false)
        {

            Customer customer = Customer.GetById(header.Customer.Id, header.Customer.Users.Id);
            List<Currency> CurrencyList = B2b.Web.v4.Models.EntityLayer.Currency.GetList();
            OrderHeader newHeader = new OrderHeader();
            newHeader.Currency = customer.CurrencyType;
            newHeader.Id = header.Id;

            List<OrderDetail> odList = OrderDetail.GetOrderDetail(header.Id);
            OrderDetail orjDetail = new OrderDetail();


            foreach (var item in odList)
            {
                if (item.Id != detailRow.Id)
                {
                    Product prod = new Product
                    {
                        Price = item.Price,
                        PriceCurrency = item.Currency,
                        PriceCurrencyRate = item.ListPriceCurrencyRate,
                        VatRate = item.VatRate,
                        TotalQuantity = item.Quantity,
                        SpecialDiscount = item.DiscSpecial
                    };
                    prod.Campaign = new Campaign
                    {
                        Discount = item.DiscCampaign
                    };
                    prod.Rule = new Rule
                    {
                        Disc1 = item.Disc1,
                        Disc2 = item.Disc2,
                        Disc3 = item.Disc3,
                        Disc4 = item.Disc4,
                    };
                    prod.CalculateDetailInformation(false, (customer.CampaignStatu && customer.Users.AuthorityUser._CampaignStatu), item.Quantity, item.DiscSpecial);
                    newHeader.GeneralTotal += (prod.PriceValue * prod.TotalQuantity);
                    newHeader.Discount += ((prod.PriceValue * prod.TotalQuantity) - (prod.Cost * prod.TotalQuantity));
                    newHeader.NetTotal += (prod.PriceNet.Value * prod.TotalQuantity);
                    newHeader.Vat += ((prod.PriceNet.Value * prod.TotalQuantity) * (prod.VatRate / 100));
                    newHeader.Total += ((prod.PriceNet.Value * prod.TotalQuantity) + (prod.PriceNet.Value * prod.TotalQuantity) * (prod.VatRate / 100));
                }
                else
                    orjDetail = detailRow;
            }

            if (isDelete)
            {
                newHeader.CurrencyRate = CurrencyList.First(x => x.Type == customer.CurrencyType).Rate;
                newHeader.Update();
                return;
            }

            double vat = 0;

            Product product = new Product();
            if (detailRow.Id == 0)
            {
                detailRow.OrderId = header.Id;
                product = Product.GetById( detailRow.ProductId, LoginType.Salesman, customer);
                product.CalculateDetailInformation(false, (customer.CampaignStatu && customer.Users.AuthorityUser._CampaignStatu), detailRow.Quantity, detailRow.DiscSpecial);
                bool isCampaign = (customer.CampaignStatu && customer.Users.AuthorityUser._CampaignStatu && product.Campaign.Type > 0 && product.TotalQuantity >= product.Campaign.MinOrder) ? true : false;
                detailRow.NetPrice = (product.PriceNetCustomer.ValueFinal * detailRow.Quantity);
                detailRow.Amount = ((product.PriceValue * detailRow.Quantity) * product.PriceCurrencyRate);
                detailRow.NetAmount = (product.PriceNetCustomer.ValueFinal * detailRow.Quantity);
                double totalVat = (product.Cost * detailRow.Quantity) * (product.VatRate / 100);
                detailRow.VatAmount = (totalVat * product.PriceCurrencyRate);
                detailRow.Currency = customer.CurrencyType;
                detailRow.CurrencyRate = CurrencyList.First(x => x.Type == customer.CurrencyType).Rate;
                detailRow.Add();
                newHeader.GeneralTotal = (newHeader.GeneralTotal + ((product.PriceList.ValueFinal * product.PriceCurrencyRate) * detailRow.Quantity));
                newHeader.Discount = ConvertStringToDouble((newHeader.Discount + (((product.PriceList.ValueFinal * product.PriceCurrencyRate) * detailRow.Quantity) - ((product.PriceNet.ValueFinal * product.PriceCurrencyRate) * detailRow.Quantity))));
                newHeader.NetTotal = (newHeader.NetTotal + ((product.PriceNet.ValueFinal * product.PriceCurrencyRate) * detailRow.Quantity));
                newHeader.Vat = (newHeader.Vat + detailRow.VatAmount);
                newHeader.Total = (newHeader.Total + ConvertStringToDouble(((product.PriceNet.ValueFinal * product.PriceCurrencyRate * detailRow.Quantity) + detailRow.VatAmount)));
                newHeader.CurrencyRate = CurrencyList.First(x => x.Type == customer.CurrencyType).Rate;
                newHeader.Update();
            }
            else
            {
                product = Product.GetById(detailRow.ProductId, LoginType.Salesman, customer);
                product.Campaign.Discount = detailRow.DiscCampaign;
                product.Rule.Disc1 = detailRow.Disc1;
                product.Rule.Disc2 = detailRow.Disc2;
                product.Rule.Disc3 = detailRow.Disc3;
                product.Rule.Disc4 = detailRow.Disc4;
                product.RuleAdditional.Disc1 = 0;
                product.RuleAdditional.Disc2 = 0;
                product.RuleAdditional.Disc3 = 0;
                product.RuleAdditional.Disc4 = 0;
                product.Price = detailRow.Price * detailRow.Quantity;
                product.PriceCurrency = detailRow.Currency;
                product.TotalQuantity = detailRow.Quantity;
                product.SpecialDiscount = detailRow.DiscSpecial;
                product.PriceCurrencyRate = detailRow.ListPriceCurrencyRate;
                product.CouponItem.Discount = detailRow.DiscCoupon;
                product.CalculateDetailInformation(false, (customer.CampaignStatu && customer.Users.AuthorityUser._CampaignStatu), detailRow.Quantity, detailRow.DiscSpecial);
                foreach (PropertyInfo productPropertyInfo in product.GetType().GetProperties())
                {
                    if (productPropertyInfo.Name == "TotalQuantity")
                    {
                        if (orjDetail.Quantity != product.TotalQuantity)
                            detailRow.ItemExplanation = "Adet Alanı Güncellendi. Eski Değer:" + orjDetail.Quantity + " Yeni Değer:" + product.TotalQuantity + " - " + DateTime.Now + "| " + detailRow.ItemExplanation;
                    }
                    else if (productPropertyInfo.Name == "Rule")
                    {
                        Rule value = (Rule)productPropertyInfo.GetValue(product, null);
                        foreach (PropertyInfo rulePropertyInfo in value.GetType().GetProperties())
                        {
                            if (rulePropertyInfo.Name == "Disc1")
                            {
                                if (orjDetail.Disc1 != product.Rule.Disc1)
                                    detailRow.ItemExplanation = "İskonto1 Alanı Güncellendi. Eski Değer:" + orjDetail.Disc1 + " Yeni Değer:" + product.Rule.Disc1 + " - " + DateTime.Now + "| " + detailRow.ItemExplanation;
                            }
                            if (rulePropertyInfo.Name == "Disc2")
                            {
                                if (orjDetail.Disc2 != product.Rule.Disc2)
                                    detailRow.ItemExplanation = "İskonto2 Alanı Güncellendi. Eski Değer:" + orjDetail.Disc2 + " Yeni Değer:" + product.Rule.Disc2 + " - " + DateTime.Now + "| " + detailRow.ItemExplanation;
                            }
                            if (rulePropertyInfo.Name == "Disc3")
                            {
                                if (orjDetail.Disc3 != product.Rule.Disc3)
                                    detailRow.ItemExplanation = "İskonto3 Alanı Güncellendi. Eski Değer:" + orjDetail.Disc3 + " Yeni Değer:" + product.Rule.Disc3 + " - " + DateTime.Now + "| " + detailRow.ItemExplanation;
                            }
                            if (rulePropertyInfo.Name == "Disc4")
                            {
                                if (orjDetail.Disc4 != product.Rule.Disc4)
                                    detailRow.ItemExplanation = "İskonto4 Alanı Güncellendi. Eski Değer:" + orjDetail.Disc4 + " Yeni Değer:" + product.Rule.Disc4 + " - " + DateTime.Now + "| " + detailRow.ItemExplanation;
                            }
                        }
                    }
                    else if (productPropertyInfo.Name == "Campaign")
                    {
                        Campaign value = (Campaign)productPropertyInfo.GetValue(product, null);
                        foreach (PropertyInfo campaignPropertyInfo in value.GetType().GetProperties())
                        {
                            if (campaignPropertyInfo.Name == "Discount")
                            {
                                if (orjDetail.DiscCampaign != product.Campaign.Discount)
                                    detailRow.ItemExplanation = "Kampanya İskonto Alanı Güncellendi. Eski Değer:" + orjDetail.DiscCampaign + " Yeni Değer:" + product.Campaign.Discount + " - " + DateTime.Now + "| " + detailRow.ItemExplanation;
                            }
                        }
                    }
                    else if (productPropertyInfo.Name == "SpecialDiscount")
                    {
                        if (orjDetail.DiscSpecial != product.SpecialDiscount)
                            detailRow.ItemExplanation = "Özel İskonto Alanı Güncellendi. Eski Değer:" + orjDetail.DiscSpecial + " Yeni Değer:" + product.SpecialDiscount + " - " + DateTime.Now + "| " + detailRow.ItemExplanation;
                    }
                    else if (productPropertyInfo.Name == "CouponItem")
                    {
                        CouponCs value = (CouponCs)productPropertyInfo.GetValue(product, null);
                        foreach (PropertyInfo couponPropertyInfo in value.GetType().GetProperties())
                        {
                            if (couponPropertyInfo.Name == "Discount")
                            {
                                if (orjDetail.DiscCoupon != product.CouponItem.Discount)
                                    detailRow.ItemExplanation = "Kupon İskonto Alanı Güncellendi. Eski Değer:" + orjDetail.DiscCoupon + " Yeni Değer:" + product.CouponItem.Discount + " - " + DateTime.Now + "| " + detailRow.ItemExplanation;
                            }
                        }
                    }
                }
                vat = ConvertStringToDouble(product.Cost * (product.VatRate / 100));
                //Netvat = ConvertStringToDouble((vat * product.PriceCurrencyRate));
                detailRow.Price = detailRow.Price;

                detailRow.NetPrice = product.PriceNet.ValueFinal;
                detailRow.Amount = product.PriceList.ValueFinal;
                detailRow.NetAmount = product.PriceNet.ValueFinal;
                detailRow.VatAmount = vat;
                detailRow.Currency = customer.CurrencyType;
                detailRow.CurrencyRate = CurrencyList.First(x => x.Type == customer.CurrencyType).Rate;
                detailRow.Disc1 = detailRow.Disc1;
                detailRow.Disc2 = detailRow.Disc2;
                detailRow.Disc3 = detailRow.Disc3;
                detailRow.Disc4 = detailRow.Disc4;
                detailRow.Update();
                newHeader.GeneralTotal = (newHeader.GeneralTotal + product.PriceList.ValueFinal);
                newHeader.Discount = ConvertStringToDouble((newHeader.Discount + (product.PriceList.ValueFinal - (product.PriceNet.ValueFinal))));
                newHeader.NetTotal = (newHeader.NetTotal + product.PriceNet.ValueFinal);
                newHeader.Vat = (newHeader.Vat + vat);
                newHeader.Total = (newHeader.Total + ConvertStringToDouble((product.PriceNet.ValueFinal + vat)));
                newHeader.CurrencyRate = CurrencyList.First(x => x.Type == customer.CurrencyType).Rate;
                newHeader.Update();
            }
        }
        public bool Update()
        {
            return DAL.UpdateOrderDetail(Quantity, Price, NetPrice, Amount, NetAmount, VatAmount, PriceLocal,
                NetPriceLocal, AmountLocal, NetAmountLocal, VatAmountLocal, Disc1, Disc2, Disc3, Disc4, DiscSpecial,
                DiscCampaign, Id, ShippedQty);
        }

        public static bool ChangeOrderId(int orderId, int newOrderId)
        {
            return DAL.ChangeOrderId(orderId, newOrderId);
        }

        public static double ConvertStringToDouble(double item)
        {
            return Convert.ToDouble(item.ToString("N4"));
        }
        #endregion

    }


    public partial class DataAccessLayer
    {

        public DataTable GetOrderDetail(int pOrderId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_OrderDetailByOrderId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pOrderId });
        }


        public bool InsertOrderDetail(int pOrderId, int pProductId, double pQuantity, double pListPrice, string pListPriceCurrency, double pListPriceCurrencyRate, double pPrice, double pNetPrice, double pAmount, double pNetAmount, double pVatAmount, string pCurrency, double pCurrencyRate, double pDisc1, double pDisc2, double pDisc3, double pDisc4, double pDiscCampaign, double pDiscSpecial, int pDueDay, int pCampaignId, string pCampaignCode, string pProductCode, bool pIsCampaign, int pCouponId, double pCouponTotal, string pItemExplanation, double pDiscCoupon, double pDiscCoupon1, double pDiscCoupon2, int pCustomerId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_OrderDetail", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pOrderId, pProductId, pQuantity, pListPrice, pListPriceCurrency, pListPriceCurrencyRate, pPrice, pNetPrice, pAmount, pNetAmount, pVatAmount, pCurrency, pCurrencyRate, pDisc1, pDisc2, pDisc3, pDisc4, pDiscCampaign, pDiscSpecial, pDueDay, pCampaignId, pCampaignCode, pProductCode, pIsCampaign, pCouponId, pCouponTotal, pItemExplanation, pDiscCoupon, pDiscCoupon1, pDiscCoupon2, pCustomerId });

        }
        public bool DeleteOrderDetail(int pOrderDetailId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Delete_OrderDetail", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pOrderDetailId });

        }
        public bool UpdateOrderDetail(double pQuantity, double pPrice, double pNetPrice, double pAmount, double pNetAmount, double pVatAmount, double pPriceLocal, double pNetPriceLocal, double pAmountLocal, double pNetAmountLocal, double pVatAmountLocal, double pDisc1, double pDisc2, double pDisc3, double pDisc4, double pDiscSpecial, double pDiscCampaign, int pDetailId, double pShippedQty)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_Order_Detail", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pQuantity, pPrice, pNetPrice, pAmount, pNetAmount, pVatAmount, pPriceLocal, pNetPriceLocal, pAmountLocal, pNetAmountLocal, pVatAmountLocal, pDisc1, pDisc2, pDisc3, pDisc4, pDiscSpecial, pDiscCampaign, pDetailId, pShippedQty });

        }

        public bool ChangeOrderId(int pOrderId, int pNewOrderId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_ChangeOrderIdOrderDetail", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pOrderId, pNewOrderId });

        }

    }
}