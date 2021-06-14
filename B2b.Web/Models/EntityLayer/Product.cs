using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using System.Data;
using System.Linq;
using System.Reflection;
using B2b.Web.v4.Models.Helper;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class Product : DataAccess
    {
        #region Constructors
        public Product()
        {

            PriceNetCustomer = new Price(0, string.Empty);
            PriceNet = new Price(0, string.Empty);
            PriceNetWithVat = new Price(0, string.Empty);
            Campaign = new Campaign();
            Rule = new Rule();
            CampaignList = new List<Campaign>();
            RuleAdditional = new RuleAdditional();
            CustomerCurrency = "TL";
            Price = 0;
            PriceCurrency = "TL";
            PriceCurrencyRate = 1;
            Name = string.Empty;
            SystemNotes = new List<string>();
            CouponItem = new CouponCs();
            DiscountStr = string.Empty;
        }
        #endregion

        #region Properties

        public List<string> SystemNotes { get; set; }
        public CouponCs CouponItem { get; set; }
        public string CustomerCurrency { get; set; }
        public Customer Customer { get; set; }
        public int Id { get; set; }
        public int EntegreId { get; set; }
        public int GroupId { get; set; }
        public int OldGruopId { get; set; }
        public double MinSpecialDiscount { get; set; }
        public double MaxSpecialDiscount { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameShort
        {
            get
            {
                return Name.Length > 30 ? Name.Substring(0, 30) + "..." : Name;
            }
        }
        public string Name2 { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerCode { get; set; }
        public string Unit { get; set; }
        public double VatRate { get; set; }
        public double MinOrder { get; set; }
        public bool IsPackIncrease { get; set; }
        public string ShelfAddress { get; set; }
        public string ProductGroup1 { get; set; }
        public string ProductGroup2 { get; set; }
        public string ProductGroup3 { get; set; }
        public int ComparisonId { get; set; }
        public int FollowId { get; set; }
        public double Width { get; set; }
        public double Lenght { get; set; }
        public double Height { get; set; }
        public double GrossWeight { get; set; }
        public double NetWeight { get; set; }
        public string SpecialCode1 { get; set; }
        public string SpecialCode2 { get; set; }

        [JsonIgnore]
        public AvailabilityStatus AvailabilityStatus { get; set; }
        public string AvailabilityCss { get; set; }//Var Yok Classı gelecek
        public string AvailabilityText { get; set; }//Miktar veya var yok yazacak

        public Campaign Campaign { get; set; }
        public List<Campaign> CampaignList { get; set; } // Kademli Kampanyalar için eklendi
        public string CampaignStr
        {
            get
            {
                string retStr = "-";

                if (Campaign == null)
                {
                    return retStr;
                }
                switch (Campaign.Type)
                {
                    case CampaignType.NetPrice:
                    case CampaignType.GrossPrice:
                    case CampaignType.Discount:
                        retStr = string.Format("({1}) {0}", CampaignPriceStr, Campaign.MinOrder);
                        break;
                    case CampaignType.GradualNetPrice:
                    case CampaignType.GradualDiscount:
                        retStr = "Detayı inceleyiniz";
                        break;
                    case CampaignType.PromotionProduct:
                        break;
                    default:
                        break;
                }

                return retStr;
            }
        }
        public bool IsActive { get; set; }
        public bool NewProduct { get; set; }
        public bool HavePicture { get; set; }
        public bool BannerStatu { get; set; }
        public string PicturePath { get; set; }
        public Rule Rule { get; set; }
        public RuleAdditional RuleAdditional { get; set; }
        public double TotalQuantity { get; set; }
        public double TotalQuantityOnWay { get; set; }
        public int WarehouseCount { get; set; }
        List<WarehouseQuantity> QuantityList { get; set; }
        public double PriceValue { get; set; }
        public double Price { get; set; }
        public string PriceCurrency { get; set; }
        public double PriceCurrencyRate { get; set; }
        public string Notes { get; set; }
        public double SpecialDiscount { get; set; }
        public double CriticalLevel { get; set; }
        public int QuantityType { get; set; }
        public double Cost { get; set; }
        public double CostWithVAT { get; set; }
        public double Bonus { get; set; }
        public string RuleCode { get; set; }
        public string Notes5 { get; set; }
        public string Notes4 { get; set; }
        public string Notes3 { get; set; }
        public string Notes2 { get; set; }
        public string Notes1 { get; set; }
        public string T9Text { get; set; }
        public double ProfitRate { get; set; }
        public string PurchasePrice { get; set; }
        public string SalesPrice5 { get; set; }
        public string SalesPrice4 { get; set; }
        public string SalesPrice3 { get; set; }
        public string SalesPrice2 { get; set; }
        public string SalesPrice1 { get; set; }
        public int LinkedId { get; set; }
        public double QuantityInPackage { get; set; }
        public Price PriceList { get { return new Price(Price, PriceCurrency); } }
        public Price PriceListCustomer { get { return new Price(Price, CustomerCurrency, PriceCurrencyRate); } }
        public Price PriceGross { get; set; }
        public Price PriceGrossTL { get; set; }
        public Price PriceNet { get; set; }
        public Price PriceNetShow { get; set; }
        private Price priceNetCustomer;
        public Price PriceNetCustomer { get { return new Price(PriceNet.Value, CustomerCurrency, PriceCurrencyRate); } set { priceNetCustomer = value; } }
        [JsonIgnore]
        public Price PriceNetWithVat { get; set; }
        [JsonIgnore]
        public Price PriceNetWithVatCustomer { get { return new Price(PriceNetWithVat.Value, CustomerCurrency, PriceCurrencyRate); } }
        public Price CampaignPrice
        {
            get
            {
                if (Campaign == null)
                {
                    return new Price(0, "TL");
                }
                switch (Campaign.Type)
                {
                    case CampaignType.NetPrice:
                        {
                            return new Price(Campaign.Price, Campaign.Currency);
                        }
                    case CampaignType.GrossPrice:
                        {
                            double c = Campaign.Price;
                            c *= (1 - (Rule.Disc1 / 100));
                            c *= (1 - (Rule.Disc2 / 100));
                            c *= (1 - (Rule.Disc3 / 100));
                            c *= (1 - (Rule.Disc4 / 100));
                            return new Price(c, Campaign.Currency);
                        }
                    case CampaignType.Discount:
                        {
                            double c = Price;
                            c *= (1 - (Rule.Disc1 / 100));
                            c *= (1 - (Rule.Disc2 / 100));
                            c *= (1 - (Rule.Disc3 / 100));
                            c *= (1 - (Rule.Disc4 / 100));
                            c *= (1 - (Campaign.Discount / 100));
                            return new Price(c, PriceCurrency);
                        }
                    case CampaignType.PromotionProduct:
                        Campaign.CurrencyRate = PriceCurrencyRate;
                        return new Price(Price, PriceCurrency);
                    case CampaignType.None:
                    case CampaignType.GradualNetPrice:
                    case CampaignType.GradualDiscount:
                    default:
                        return new Price(0, "TL");
                }
            }
        }
        public Price CampaignPriceCustomer { get { return new Price(CampaignPrice.Value, CustomerCurrency, Campaign.CurrencyRate); } }
        public Price CampaignPriceWithVat { get { return new Price(CampaignPrice.Value * (100 + VatRate) / 100, CampaignPrice.Currency); } }
        public Price CampaignPriceWithVatCustomer { get { return new Price(CampaignPrice.Value * (100 + VatRate) / 100, CustomerCurrency, Campaign.CurrencyRate); } }
        public string CampaignPriceCustomerStr { get { return CampaignPriceCustomer.ToString(); } }
        public string CampaignPriceStr { get { return CampaignPrice.ToString(); } }
        public string CampaignPriceWithVatStr { get { return CampaignPriceWithVat.ToString(); } }
        public string CampaignPriceWithVatCustomerStr { get { return CampaignPriceWithVatCustomer.ToString(); } }
        public string PriceListStr { get { return PriceList.ToString(); } }
        public string PriceListCustomerStr { get { try { return PriceListCustomer.ToString(); } catch { return string.Empty; } } }
        public string PriceNetStr { get { try { return PriceNet.ToString(); } catch { return string.Empty; } } }
        public string PriceNetShowStr { get { try { return PriceNetShow.ToString(); } catch { return string.Empty; } } }
        public string PriceNetWithVatStr { get { try { return PriceNetWithVat.ToString(); } catch { return string.Empty; } } }
        public string PriceNetCustomerStr { get { try { return PriceNetCustomer.ToString(); } catch { return string.Empty; } } }
        public string PriceNetWithVatCustomerStr { get { try { return PriceNetWithVatCustomer.ToString(); } catch { return string.Empty; } } }
        public string DiscountStr { get; set; }
        public LoginType LoginType { get; set; }

        public bool IsKitMain { get; set; }
        public List<Product> KitDetailList { get; set; }
        #endregion

        #region Methods

        #region Calculate

        public AvailabilityStatus CalculateAvaibilityStatus(double quantity)
        {
            double totalQuantity = quantity;
            double criticalLevel = CriticalLevel;

            switch (QuantityType)
            {
                case 0:
                    if (totalQuantity > 0 && totalQuantity <= criticalLevel)
                        return AvailabilityStatus.LittleAvailable;
                    else if (totalQuantity > criticalLevel)
                        return AvailabilityStatus.Available;
                    else
                        return AvailabilityStatus.Unavailable;
                case 1:
                    if (totalQuantity > 0 && totalQuantity <= criticalLevel)
                        return AvailabilityStatus.LittleAvailable;
                    else
                        return AvailabilityStatus.Available;

                case 2:
                    return AvailabilityStatus.Unavailable;

                case 3:
                    if (totalQuantity > 0 && totalQuantity <= criticalLevel)
                        return AvailabilityStatus.LittleAvailable;
                    else if (totalQuantity > criticalLevel)
                        return AvailabilityStatus.Available;
                    else
                        return AvailabilityStatus.InCustoms;

                case 4:
                    if (totalQuantity > 0 && totalQuantity <= criticalLevel)
                        return AvailabilityStatus.LittleAvailable;
                    else if (totalQuantity > criticalLevel)
                        return AvailabilityStatus.Available;
                    else
                        return AvailabilityStatus.OnOrder;

                case 5:
                    if (totalQuantity > 0 && totalQuantity <= criticalLevel)
                        return AvailabilityStatus.LittleAvailable;
                    else if (totalQuantity > criticalLevel)
                        return AvailabilityStatus.Available;
                    else
                        return AvailabilityStatus.OnOrder;

                default:
                    return AvailabilityStatus.None;
            }
        }

        public void CalculateDetailInformation(bool itemCmpStatu, bool campaignStatu, double quantity, double specialDiscount = 0, bool isFixedPrice = false, double fixedPrice = 0, string fixedCurrency = "TL", double fixedCurrencyRate = 0, double userRate = 0, bool notes = true)
        {

            if (notes)
                DiscountStr += "%";

            if (RuleAdditional.SalesPrice > 0)
            {
                Price = RuleAdditional.SalesPrice;
                PriceCurrency = RuleAdditional.Currency;
                PriceCurrencyRate = RuleAdditional.CurrenctyRate;
            }

            if (RuleAdditional.MainDiscountPassive)
            {
                Rule.Disc1 = RuleAdditional.Disc1;
                Rule.Disc2 = RuleAdditional.Disc2;
                Rule.Disc3 = RuleAdditional.Disc3;
                Rule.Disc4 = RuleAdditional.Disc4;
                Rule.Rate = RuleAdditional.Rate;
                if (notes)
                    SystemNotes.Add("Ana koşullar iptal edilmiştir.");
            }
            else
            {
                if (!Rule.IsRuleAdditional && RuleAdditional.Disc1 > 0)
                {
                    Rule.IsRuleAdditional = true;
                    if (Rule.Disc1 == 0)
                    {
                        Rule.Disc1 = RuleAdditional.Disc1;
                        Rule.Disc2 = RuleAdditional.Disc2;
                        Rule.Disc3 = RuleAdditional.Disc3;
                        Rule.Disc4 = RuleAdditional.Disc4;
                    }
                    else if (Rule.Disc2 == 0)
                    {
                        Rule.Disc2 = RuleAdditional.Disc1;
                        Rule.Disc3 = RuleAdditional.Disc2;
                        Rule.Disc4 = RuleAdditional.Disc3;
                    }
                    else if (Rule.Disc3 == 0)
                    {
                        Rule.Disc3 = RuleAdditional.Disc1;
                        Rule.Disc4 = RuleAdditional.Disc2;
                    }
                    else if (Rule.Disc4 == 0)
                    {
                        Rule.Disc4 = RuleAdditional.Disc1;
                    }
                }
            }


            #region DiscountCalculation
            if (isFixedPrice)
            {
                Rule.Disc1 = 0;
                Rule.Disc2 = 0;
                Rule.Disc3 = 0;
                Rule.Disc4 = 0;
                Rule.Rate = 0;
                RuleAdditional.Disc1 = 0;
                RuleAdditional.Disc2 = 0;
                RuleAdditional.Disc3 = 0;
                RuleAdditional.Disc4 = 0;
                RuleAdditional.Rate = 0;
                if (notes)
                    SystemNotes.Add("Özel fiyat sebebiyle koşullar iptal edilmiştir.");



            }

            if (CouponItem.Id > 0 && CouponItem.IsCancelMainDisc)
            {
                Rule.Disc1 = 0;
                Rule.Disc2 = 0;
                Rule.Disc3 = 0;
                Rule.Disc4 = 0;
                Rule.Rate = 0;
                SystemNotes.Add(CouponItem.Header + " kampanyası sebebiyle uygulanan ana koşullar iptal edilmiştir.");
                DiscountStr = "%";
            }

            if (CouponItem.Id > 0 && CouponItem.IsCancelAdditionalDisc)
            {
                RuleAdditional.Disc1 = 0;
                RuleAdditional.Disc2 = 0;
                RuleAdditional.Disc3 = 0;
                RuleAdditional.Disc4 = 0;
                RuleAdditional.Rate = 0;
                SystemNotes.Add(CouponItem.Header + " kampanyası sebebiyle uygulanan ek koşullar iptal edilmiştir.");
            }

            if (CouponItem.Id > 0 && CouponItem.IsCancelManuelDisc)
            {
                specialDiscount = 0;
                SystemNotes.Add(CouponItem.Header + " kampanyası sebebiyle uygulanan manuel iskonto iptal edilmiştir.");
            }

            #endregion

            PriceValue = Price;
            Cost = PriceValue;
            Cost *= (1 - (Rule.Disc1 / 100));
            Cost *= (1 - (Rule.Disc2 / 100));
            Cost *= (1 - (Rule.Disc3 / 100));
            Cost *= (1 - (Rule.Disc4 / 100));
            Cost *= (1 - (specialDiscount / 100));

            if (campaignStatu && Campaign.Id > 0 && (Cost * PriceCurrencyRate) > 0 && (Cost * PriceCurrencyRate) < (Campaign.Price * Campaign.CurrencyRate))
            {
                itemCmpStatu = true;
                if (notes)
                    SystemNotes.Add("Müşterinin iskonto kazanımı daha ucuz olduğu için kampanya otomatik iptal edilmiştir");

                BannerStatu = false;
                Campaign = new Campaign();
            }

            #region CampaignListPrice


            switch (campaignStatu && !itemCmpStatu)
            {
                case true:
                    switch (Campaign.Type)
                    {
                        case CampaignType.None:
                            break;

                        case CampaignType.NetPrice:
                            if (quantity >= Campaign.MinOrder)
                            {
                                Rule.Disc1 = 0;
                                Rule.Disc2 = 0;
                                Rule.Disc3 = 0;
                                Rule.Disc4 = 0;
                                Rule.Rate = 0;
                                RuleAdditional.Disc1 = 0;
                                RuleAdditional.Disc2 = 0;
                                RuleAdditional.Disc3 = 0;
                                RuleAdditional.Disc4 = 0;
                                RuleAdditional.Rate = 0;
                                if (notes)
                                {
                                    DiscountStr = "%";
                                    SystemNotes.Add("Kampanya sebebiyle koşullar iptal edilmiştir.");
                                }

                            }
                            break;

                        case CampaignType.GrossPrice:
                            break;

                        case CampaignType.Discount:
                            if (Campaign.DiscountPassive)
                            {
                                Rule.Disc1 = 0;
                                Rule.Disc2 = 0;
                                Rule.Disc3 = 0;
                                Rule.Disc4 = 0;
                                Rule.Rate = 0;
                                RuleAdditional.Disc1 = 0;
                                RuleAdditional.Disc2 = 0;
                                RuleAdditional.Disc3 = 0;
                                RuleAdditional.Disc4 = 0;
                                RuleAdditional.Rate = 0;
                                if (notes)
                                {
                                    DiscountStr = "%";
                                    SystemNotes.Add("Kampanya sebebiyle koşullar iptal edilmiştir.");
                                }

                            }
                            break;

                        case CampaignType.GradualNetPrice:
                            if (quantity >= Campaign.MinOrder)
                            {
                                Rule.Disc1 = 0;
                                Rule.Disc2 = 0;
                                Rule.Disc3 = 0;
                                Rule.Disc4 = 0;
                                Rule.Rate = 0;
                                RuleAdditional.Disc1 = 0;
                                RuleAdditional.Disc2 = 0;
                                RuleAdditional.Disc3 = 0;
                                RuleAdditional.Disc4 = 0;
                                RuleAdditional.Rate = 0;
                                if (notes)
                                {
                                    DiscountStr = "%";
                                    SystemNotes.Add("Kampanya sebebiyle koşullar iptal edilmiştir.");
                                }

                            }
                            break;

                        case CampaignType.GradualDiscount:
                            if (Campaign.DiscountPassive)
                            {
                                Rule.Disc1 = 0;
                                Rule.Disc2 = 0;
                                Rule.Disc3 = 0;
                                Rule.Disc4 = 0;
                                Rule.Rate = 0;
                                RuleAdditional.Disc1 = 0;
                                RuleAdditional.Disc2 = 0;
                                RuleAdditional.Disc3 = 0;
                                RuleAdditional.Disc4 = 0;
                                RuleAdditional.Rate = 0;
                                if (notes)
                                {
                                    DiscountStr = "%";
                                    SystemNotes.Add("Kampanya sebebiyle koşullar iptal edilmiştir.");
                                }

                            }

                            //if (quantity >= Campaign.MinOrder)
                            //{
                            //CampaignList = Campaign.GetListByHeaderId(Campaign.Id, Customer);
                            //if (quantity < CampaignList[0].MinOrder)
                            //{
                            //    Campaign.Discount = 0.0;
                            //}
                            //else if (quantity >= CampaignList[CampaignList.Count - 1].MinOrder)
                            //{
                            //    Campaign = CampaignList[CampaignList.Count - 1];
                            //}
                            //else
                            //{
                            //    for (int i = 0; i < CampaignList.Count; i++)
                            //    {
                            //        if (quantity >= CampaignList[i].MinOrder && quantity < CampaignList[i + 1].MinOrder)
                            //        {
                            //            Campaign = CampaignList[i];
                            //            break;
                            //        }
                            //    }
                            //}
                            //}
                            break;

                        case CampaignType.PromotionProduct:
                            break;
                    }
                    break;

                case false:
                    // Herşey Aynı
                    break;
            }

            double normalRate = 1.0;
            double campaignRate = 1.0;
            string currency = PriceCurrency;
            if (isFixedPrice)
            {
                PriceValue = fixedPrice;
                currency = fixedCurrency;
                PriceCurrencyRate = fixedCurrencyRate;
                if (notes)
                    SystemNotes.Add("Özel fiyat tanımı " + PriceValue + " " + currency + " olarak güncellenmiştir");
            }

            switch (campaignStatu && !itemCmpStatu)
            {
                case false:
                    if (!isFixedPrice)
                        PriceValue = Price * normalRate;
                    break;

                case true:
                    double campaignPrice = Campaign.Price;

                    switch (Campaign.Type)
                    {
                        case CampaignType.None:
                            if (!isFixedPrice)
                                PriceValue = Price * normalRate;
                            break;

                        case CampaignType.NetPrice:
                            if (quantity >= Campaign.MinOrder)
                            {
                                PriceValue = campaignPrice * campaignRate;
                                currency = Campaign.Currency;
                                PriceCurrencyRate = Campaign.CurrencyRate;
                                if (notes)
                                    SystemNotes.Add("Kampanya koşulları sağlandığı için fiyat " + PriceValue + " " + currency + " olarak güncellenmiştir");
                            }
                            else
                            {
                                PriceValue = Price * normalRate;
                                currency = PriceCurrency;
                            }
                            break;

                        case CampaignType.GrossPrice:
                            if (quantity >= Campaign.MinOrder)
                            {
                                PriceValue = campaignPrice * campaignRate;
                                currency = Campaign.Currency;
                                PriceCurrencyRate = Campaign.CurrencyRate;
                                if (notes)
                                    SystemNotes.Add("Kampanya koşulları sağlandığı için fiyat " + PriceValue + " " + currency + " olarak güncellenmiştir");
                            }
                            else
                                PriceValue = Price * normalRate;
                            break;

                        case CampaignType.Discount:
                            PriceValue = Price * normalRate;
                            break;

                        case CampaignType.GradualNetPrice:
                            CampaignList = Campaign.GetListByHeaderId(Campaign.Id, Customer);
                            if (CampaignList.Any() && quantity < CampaignList[0].MinOrder)
                            {
                                PriceValue = Price * normalRate;
                            }
                            else
                            {
                                if (CampaignList.Any() && quantity >= CampaignList[CampaignList.Count - 1].MinOrder)
                                {
                                    Campaign = CampaignList[CampaignList.Count - 1];
                                }
                                else
                                {
                                    for (int i = 0; i < CampaignList.Count; i++)
                                    {
                                        if (CampaignList.Any() && quantity >= CampaignList[i].MinOrder && quantity < CampaignList[i + 1].MinOrder)
                                        {
                                            Campaign = CampaignList[i];
                                            break;
                                        }
                                    }
                                }

                                PriceValue = Campaign.Price * campaignRate;
                                currency = Campaign.Currency;
                                PriceCurrencyRate = Campaign.CurrencyRate;
                                if (notes)
                                    SystemNotes.Add("Kampanya koşulları sağlandığı için fiyat " + PriceValue + " " + currency + " olarak güncellenmiştir");
                            }
                            break;

                        case CampaignType.GradualDiscount:
                            CampaignList = Campaign.GetListByHeaderId(Campaign.Id, Customer);
                            if (quantity < CampaignList[0].MinOrder)
                            {
                                Campaign.Discount = 0.0;
                            }
                            else if (quantity >= CampaignList[CampaignList.Count - 1].MinOrder)
                            {
                                Campaign = CampaignList[CampaignList.Count - 1];
                            }
                            else
                            {
                                for (int i = 0; i < CampaignList.Count; i++)
                                {
                                    if (quantity >= CampaignList[i].MinOrder && quantity < CampaignList[i + 1].MinOrder)
                                    {
                                        Campaign = CampaignList[i];
                                        break;
                                    }
                                }
                            }
                            //PriceValue = Price * normalRate;

                            break;

                        case CampaignType.PromotionProduct:
                            PriceValue = Price * normalRate;
                            break;
                    }
                    break;
            }

            PriceValue = PriceValue * ((Rule.Rate / 100) + 1);
            if (!RuleAdditional.MainDiscountPassive)
                PriceValue = PriceValue * ((RuleAdditional.Rate / 100) + 1);


            PriceGross = new Price(PriceValue, currency);
            #endregion

            #region NoTaxPrice
            //CalculateCost(product);
            Cost = PriceValue;
            Cost *= (1 - (Rule.Disc1 / 100));
            Cost *= (1 - (Rule.Disc2 / 100));
            Cost *= (1 - (Rule.Disc3 / 100));
            Cost *= (1 - (Rule.Disc4 / 100));
            if (Rule.Disc1 > 0 && notes)
            {
                SystemNotes.Add(Rule.Disc1 + "% iskonto uygulanmıştır");
                DiscountStr += DiscountStr == "%" ? Rule.Disc1.ToString() : "+" + Rule.Disc1.ToString();
            }
            if (Rule.Disc2 > 0 && notes)
            {
                SystemNotes.Add(Rule.Disc2 + "% iskonto uygulanmıştır");
                DiscountStr += DiscountStr == "%" ? Rule.Disc2.ToString() : "+" + Rule.Disc2.ToString();
            }
            if (Rule.Disc3 > 0 && notes)
            {
                SystemNotes.Add(Rule.Disc3 + "% iskonto uygulanmıştır");
                DiscountStr += DiscountStr == "%" ? Rule.Disc3.ToString() : "+" + Rule.Disc3.ToString();
            }
            if (Rule.Disc4 > 0 && notes)
            {
                SystemNotes.Add(Rule.Disc4 + "% iskonto uygulanmıştır");
                DiscountStr += DiscountStr == "%" ? Rule.Disc4.ToString() : "+" + Rule.Disc4.ToString();
            }


            if (!itemCmpStatu && campaignStatu && quantity >= Campaign.MinOrder && Campaign.Id > 0 && Campaign.Discount > 0)
            {
                Cost *= (1 - (Campaign.Discount / 100));
                if (notes)
                {
                    DiscountStr += DiscountStr == "%" ? Campaign.Discount.ToString() : "+" + Campaign.Discount.ToString();
                    SystemNotes.Add("Kampanya koşulları sağlandığı için " + Campaign.Discount + "% iskonto uygulanmıştır");
                }

            }

            Cost *= (1 - (specialDiscount / 100));

            if (specialDiscount > 0)
            {
                if (notes)
                    SystemNotes.Add(specialDiscount + "% özel iskonto uygulanmıştır");

                DiscountStr += DiscountStr == "%" ? specialDiscount.ToString() : "+" + specialDiscount.ToString();
            }


            if (CouponItem.Id > 0 && !itemCmpStatu)
            {
                if (CouponItem.Type == 0)
                {
                    double _cost = Cost;
                    Cost *= (1 - (CouponItem.Discount / 100));
                    Cost *= (1 - (CouponItem.Discount1 / 100));
                    Cost *= (1 - (CouponItem.Discount2 / 100));
                    CouponItem.UsedDiscountTl = (_cost - Cost) * PriceCurrencyRate;
                    string discCouponStr = string.Empty;
                    if (CouponItem.Discount != 0)
                        discCouponStr += CouponItem.Discount + "+";
                    if (CouponItem.Discount1 != 0)
                        discCouponStr += CouponItem.Discount1 + "+";
                    if (CouponItem.Discount2 != 0)
                        discCouponStr += CouponItem.Discount2 + "+";

                    SystemNotes.Add(CouponItem.Header + " kampanyasından " + discCouponStr.Substring(0, discCouponStr.Length - 1) + "% iskonto uygulanmıştır");

                    DiscountStr += DiscountStr == "%" ? discCouponStr.Substring(0, discCouponStr.Length - 1) : "+" + discCouponStr.Substring(0, discCouponStr.Length - 1);
                }
                else if (CouponItem.Type == 1)
                {
                    Cost = Cost - (CouponItem.Price / PriceCurrencyRate);
                    CouponItem.UsedDiscountTl = (CouponItem.Price / PriceCurrencyRate);
                    SystemNotes.Add(CouponItem.Header + " kampanyasından " + CouponItem.Price.ToString("N2") + " TL indirim uygulanmıştır");
                }
            }

            if (DiscountStr == "%")
                DiscountStr = "-";

            PriceNet = new Price(Cost, PriceGross.Currency);
            PriceNetShow = new Price((PriceNet.ValueFinal * (1 + (userRate / 100))), PriceNet.Currency);
            #endregion

            #region TaxPrice
            //CalculateCostWithVAT(product);
            CostWithVAT = Cost * (1 + VatRate / 100);
            PriceNetWithVat = new Price(CostWithVAT, PriceGross.Currency);
            #endregion
        }

        public void SetAvailabilityClass(LoginType loginType, double quantity, AvailabilityStatus availabilityStatus, out string availabilityCss, out string availabilityText)
        {
            switch (availabilityStatus)
            {
                case AvailabilityStatus.Available:
                    availabilityCss = "fa fa-check-circle fa-stack-2x   text-success";
                    availabilityText = loginType == LoginType.Customer ? "Var" : quantity.ToString();
                    break;
                case AvailabilityStatus.Unavailable:
                    availabilityCss = "fa fa-times-circle fa-stack-2x   text-danger";
                    availabilityText = loginType == LoginType.Customer ? "Yok" : quantity.ToString();
                    break;
                case AvailabilityStatus.LittleAvailable:
                    availabilityCss = "fa fa-adjust fa-flip-horizontal fa-stack-2x   text-success";
                    availabilityText = loginType == LoginType.Customer ? "Az Var" : quantity.ToString();
                    break;
                case AvailabilityStatus.OnOrder:
                    availabilityCss = "fa fa-rocket text-primary fa-stack-2x";
                    availabilityText = loginType == LoginType.Customer ? "Yolda" : quantity.ToString();
                    break;
                case AvailabilityStatus.InCustoms:
                    availabilityCss = "fa fa-question-circle fa-stack-2x   text-info";
                    availabilityText = loginType == LoginType.Customer ? "Sipariş Üzerine" : quantity.ToString();
                    break;
                case AvailabilityStatus.AskOnPhone:
                    availabilityCss = "fa fa-circle fa-stack-2x   text-success";
                    availabilityText = loginType == LoginType.Customer ? "Telefonla Sorunuz" : quantity.ToString();
                    break;
                case AvailabilityStatus.None:
                    availabilityCss = "fa fa-circle fa-stack-2x   text-success";
                    availabilityText = loginType == LoginType.Customer ? "Belirsiz" : quantity.ToString();
                    break;
                default:
                    availabilityCss = "fa fa-circle fa-stack-2x   text-success";
                    availabilityText = loginType == LoginType.Customer ? "Belirsiz" : quantity.ToString();
                    break;
            }



        }


        #endregion

        public static Product GetById(int productId, LoginType loginType, Customer customer)
        {

            Product product = new Product();
            DataTable dt = DAL.GetProductById(productId, customer.Id, customer.Users.Id, (int)loginType);

            foreach (DataRow row in dt.Rows)
            {
                product.Customer = customer;
                product.CustomerCurrency = customer.CurrencyType;
                product.LoginType = loginType;
                product.Id = row.Field<int>("Id");
                product.GroupId = row.Field<int>("GroupId");
                product.Code = row.Field<string>("Code");
                product.Name = row.Field<string>("Name");
                product.Manufacturer = row.Field<string>("Manufacturer");
                product.ManufacturerCode = NullControl(row.Field<string>("ManufacturerCode"));
                product.Unit = row.Field<string>("Unit");
                product.TotalQuantity = row.Field<double>("TotalQuantity");
                product.TotalQuantityOnWay = row.Field<double>("TotalQuantityOnWay");
                product.WarehouseCount = row.Field<int>("WarehouseCount");
                product.NewProduct = row.Field<bool>("NewProduct");
                product.HavePicture = row.Field<bool>("HavePicture");
                product.VatRate = Convert.ToDouble(row["VatRate"]);
                product.MinOrder = row.Field<double>("MinOrder");
                product.Price = row.Field<double>("Price");
                product.PriceCurrency = row.Field<string>("PriceCurrency");
                product.CriticalLevel = row.Field<double>("CriticalLevel");
                product.QuantityType = row.Field<int>("QuantityType");
                product.IsPackIncrease = row.Field<bool>("IsPackIncrease");
                product.PriceCurrencyRate = row.Field<double>("PriceCurrencyRate");
                product.Notes = row.Field<string>("Notes");
                product.Width = row.Field<double>("Width");
                product.Height = row.Field<double>("Height");
                product.Lenght = row.Field<double>("Lenght");
                product.GrossWeight = row.Field<double>("GrossWeight");
                product.NetWeight = row.Field<double>("NetWeight");
                product.PicturePath = row.Field<string>("PicturePath") == string.Empty ? "../Content/images/nophoto.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath");
                product.Rule = new Rule()
                {
                    Customer = row.Field<string>("Customer"),
                    PaymentType = row.Field<int>("PaymentType"),
                    Disc1 = row.Field<double>("Disc1"),
                    Disc2 = row.Field<double>("Disc2"),
                    Disc3 = row.Field<double>("Disc3"),
                    Disc4 = row.Field<double>("Disc4"),
                    PriceNumber = row.Field<int>("PriceNumber"),
                    Rate = row.Field<double>("Rate")
                };
                product.Campaign = new Campaign()
                {
                    Id = Convert.ToInt32(row["CampaignId"]),
                    Type = (CampaignType)Convert.ToInt32(row.Field<long>("CampaignType")),
                    Price = Convert.ToDouble(row["CampaignPrice"]),
                    Currency = row.Field<string>("CampaignCurrency"),
                    CurrencyRate = Convert.ToDouble(row["CampaignCurrencyRate"]),
                    Discount = Convert.ToDouble(row["Discount"]),
                    MinOrder = Convert.ToDouble(row["CampaignMinOrder"]),
                    TotalQuantity = Convert.ToDouble(row["CampaignTotalQuantity"]),
                    SaledQuantity = Convert.ToDouble(row["SaledQuantity"]),
                    PromotionProductCode = row.Field<string>("PromotionProductCode"),
                    PromotionProductQuantity = Convert.ToDouble(row["PromotionProductQuantity"]),
                    DiscountPassive = Convert.ToBoolean(row["DiscountPassive"]),
                    StartDate = row.Field<DateTime>("StartDate"),
                    FinishDate = row.Field<DateTime>("FinishDate")
                };
                if (row.Field<string>("CustomerRuleAdditional") != "Null")
                {
                    string[] splitData = row.Field<string>("CustomerRuleAdditional").Split('-');

                    product.RuleAdditional = new RuleAdditional()
                    {
                        Manufacturer = splitData[0],
                        ProductGroup1 = splitData[1],
                        ProductGroup2 = splitData[2],
                        ProductGroup3 = splitData[3],
                        Disc1 = Convert.ToDouble(splitData[4].Replace(".", ",")),
                        Disc2 = Convert.ToDouble(splitData[5].Replace(".", ",")),
                        Disc3 = Convert.ToDouble(splitData[6].Replace(".", ",")),
                        Disc4 = Convert.ToDouble(splitData[7].Replace(".", ",")),
                        Rate = Convert.ToDouble(splitData[8].Replace(".", ",")),
                        MainDiscountPassive = Convert.ToBoolean(Convert.ToInt32(splitData[9])),
                        SalesPrice = Convert.ToDouble(splitData[10].Replace(".", ",")),
                        Currency = Convert.ToString(splitData[11]),
                        CurrenctyRate = Convert.ToDouble(splitData[12].Replace(".", ",")),

                    };

                }

            }

            product.CalculateDetailInformation(false, (customer.CampaignStatu && customer.Users.AuthorityUser._CampaignStatu), -1, 0, false, 0, "TL", 0, customer.Users.Rate);
            product.AvailabilityStatus = product.CalculateAvaibilityStatus(product.TotalQuantity);
            string availabilityText, availabilityCss;

            product.SetAvailabilityClass(product.LoginType, product.TotalQuantity, product.AvailabilityStatus, out availabilityCss, out availabilityText);
            product.AvailabilityCss = availabilityCss;
            product.AvailabilityText = availabilityText;

            return product;

        }


        public static Product GetById(int productId)
        {

            Product product = new Product();
            DataTable dt = DAL.GetProductByOnlyId(productId);

            foreach (DataRow row in dt.Rows)
            {
                product.Id = row.Field<int>("Id");
                product.GroupId = row.Field<int>("GroupId");
                product.Code = row.Field<string>("Code");
                product.Name = row.Field<string>("Name");
                product.Manufacturer = row.Field<string>("Manufacturer");
                product.ManufacturerCode = NullControl(row.Field<string>("ManufacturerCode"));
                product.Unit = row.Field<string>("Unit");
                product.TotalQuantity = row.Field<double>("TotalQuantity");
                product.TotalQuantityOnWay = row.Field<double>("TotalQuantityOnWay");
                product.WarehouseCount = row.Field<int>("WarehouseCount");
                product.NewProduct = row.Field<bool>("NewProduct");
                product.HavePicture = row.Field<bool>("HavePicture");
                product.VatRate = Convert.ToDouble(row["VatRate"]);
                product.MinOrder = row.Field<double>("MinOrder");
                product.CriticalLevel = row.Field<double>("CriticalLevel");
                product.QuantityType = row.Field<int>("QuantityType");
                product.IsPackIncrease = row.Field<bool>("IsPackIncrease");
                product.Notes = row.Field<string>("Notes");
                product.Width = row.Field<double>("Width");
                product.Height = row.Field<double>("Height");
                product.Lenght = row.Field<double>("Lenght");
                product.GrossWeight = row.Field<double>("GrossWeight");
                product.NetWeight = row.Field<double>("NetWeight");
                product.PicturePath = row.Field<string>("PicturePath") == string.Empty ? "../Content/images/nophoto.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath");

                product.Campaign = new Campaign()
                {
                    Id = Convert.ToInt32(row["CampaignId"]),
                    Type = (CampaignType)Convert.ToInt32(row.Field<long>("CampaignType")),
                    Price = Convert.ToDouble(row["CampaignPrice"]),
                    Currency = row.Field<string>("CampaignCurrency"),
                    CurrencyRate = Convert.ToDouble(row["CampaignCurrencyRate"]),
                    Discount = Convert.ToDouble(row["Discount"]),
                    MinOrder = Convert.ToDouble(row["CampaignMinOrder"]),
                    TotalQuantity = Convert.ToDouble(row["CampaignTotalQuantity"]),
                    SaledQuantity = Convert.ToDouble(row["SaledQuantity"]),
                    PromotionProductCode = row.Field<string>("PromotionProductCode"),
                    PromotionProductQuantity = Convert.ToDouble(row["PromotionProductQuantity"]),
                    DiscountPassive = Convert.ToBoolean(row["DiscountPassive"])
                };

            }

            product.AvailabilityStatus = product.CalculateAvaibilityStatus(product.TotalQuantity);
            string availabilityText, availabilityCss;

            product.SetAvailabilityClass(product.LoginType, product.TotalQuantity, product.AvailabilityStatus, out availabilityCss, out availabilityText);
            product.AvailabilityCss = availabilityCss;
            product.AvailabilityText = availabilityText;

            return product;

        }

        public static List<Product> GetListByKitMainId(int mainProductId, int customerId)
        {
            List<Product> list = new List<Product>();
            DataTable dt = DAL.GetProductListByKitMainId(mainProductId, customerId);

            foreach (DataRow row in dt.Rows)
            {
                Product obj = new Product();

                obj.Id = row.Field<int>("Id");
                obj.GroupId = row.Field<int>("GroupId");
                obj.Code = row.Field<string>("Code");
                obj.Name = row.Field<string>("Name");
                obj.Manufacturer = row.Field<string>("Manufacturer");
                obj.ManufacturerCode = NullControl(row.Field<string>("ManufacturerCode"));
                obj.Unit = row.Field<string>("Unit");
                obj.TotalQuantity = row.Field<double>("TotalQuantity");
                obj.TotalQuantityOnWay = row.Field<double>("TotalQuantityOnWay");
                obj.WarehouseCount = row.Field<int>("WarehouseCount");
                obj.NewProduct = row.Field<bool>("NewProduct");
                obj.HavePicture = row.Field<bool>("HavePicture");
                obj.VatRate = Convert.ToDouble(row["VatRate"]);
                obj.MinOrder = row.Field<double>("MinOrder");
                obj.CriticalLevel = row.Field<double>("CriticalLevel");
                obj.QuantityType = row.Field<int>("QuantityType");
                obj.IsPackIncrease = row.Field<bool>("IsPackIncrease");
                obj.Notes = row.Field<string>("Notes");
                obj.Width = row.Field<double>("Width");
                obj.Height = row.Field<double>("Height");
                obj.Lenght = row.Field<double>("Lenght");
                obj.GrossWeight = row.Field<double>("GrossWeight");
                obj.NetWeight = row.Field<double>("NetWeight");
                obj.PicturePath = row.Field<string>("PicturePath") == string.Empty ? "../Content/images/nophoto.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath");
                obj.IsKitMain = row.Field<bool>("IsKitMain");

                obj.Price = row.Field<double>("Price");
                obj.PriceCurrency = row.Field<string>("PriceCurrency");
                obj.PriceCurrencyRate = row.Field<double>("PriceCurrencyRate");

                list.Add(obj);
            }

            return list;
        }



        public static Product GetByCode(string code, LoginType loginType, Customer customer)
        {
            Product product = new Product();
            DataTable dt = DAL.GetProductByCode(code, customer.Id, customer.Users.Id, (int)loginType);

            foreach (DataRow row in dt.Rows)
            {
                product.CustomerCurrency = customer.CurrencyType;
                product.Customer = customer;
                product.LoginType = loginType;
                product.Id = row.Field<int>("Id");
                product.GroupId = row.Field<int>("GroupId");
                product.Code = row.Field<string>("Code");
                product.Name = row.Field<string>("Name");
                product.Manufacturer = row.Field<string>("Manufacturer");
                product.ManufacturerCode = NullControl(row.Field<string>("ManufacturerCode"));
                product.Unit = row.Field<string>("Unit");
                product.TotalQuantity = row.Field<double>("TotalQuantity");
                product.TotalQuantityOnWay = row.Field<double>("TotalQuantityOnWay");
                product.WarehouseCount = row.Field<int>("WarehouseCount");
                product.NewProduct = row.Field<bool>("NewProduct");
                product.HavePicture = row.Field<bool>("HavePicture");
                product.VatRate = Convert.ToDouble(row["VatRate"]);
                product.MinOrder = row.Field<double>("MinOrder");
                product.Price = row.Field<double>("Price");
                product.PriceCurrency = row.Field<string>("PriceCurrency");
                product.CriticalLevel = row.Field<double>("CriticalLevel");
                product.QuantityType = row.Field<int>("QuantityType");
                product.IsPackIncrease = row.Field<bool>("IsPackIncrease");
                product.PriceCurrencyRate = row.Field<double>("PriceCurrencyRate");
                product.Notes = row.Field<string>("Notes");
                product.Width = row.Field<double>("Width");
                product.Height = row.Field<double>("Height");
                product.Lenght = row.Field<double>("Lenght");
                product.GrossWeight = row.Field<double>("GrossWeight");
                product.NetWeight = row.Field<double>("NetWeight");
                product.PicturePath = row.Field<string>("PicturePath") == string.Empty ? "../Content/images/nophoto.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath");
                product.Rule = new Rule()
                {
                    Customer = row.Field<string>("Customer"),
                    PaymentType = row.Field<int>("PaymentType"),
                    Disc1 = row.Field<double>("Disc1"),
                    Disc2 = row.Field<double>("Disc2"),
                    Disc3 = row.Field<double>("Disc3"),
                    Disc4 = row.Field<double>("Disc4"),
                    PriceNumber = row.Field<int>("PriceNumber"),
                    Rate = row.Field<double>("Rate")
                };
                product.Campaign = new Campaign()
                {
                    Id = Convert.ToInt32(row["CampaignId"]),
                    Type = (CampaignType)Convert.ToInt32(row.Field<long>("CampaignType")),
                    Price = Convert.ToDouble(row["CampaignPrice"]),
                    Currency = row.Field<string>("CampaignCurrency"),
                    CurrencyRate = Convert.ToDouble(row["CampaignCurrencyRate"]),
                    Discount = Convert.ToDouble(row["Discount"]),
                    MinOrder = Convert.ToDouble(row["CampaignMinOrder"]),
                    TotalQuantity = Convert.ToDouble(row["CampaignTotalQuantity"]),
                    SaledQuantity = Convert.ToDouble(row["SaledQuantity"]),
                    PromotionProductCode = row.Field<string>("PromotionProductCode"),
                    PromotionProductQuantity = Convert.ToDouble(row["PromotionProductQuantity"]),
                    DiscountPassive = Convert.ToBoolean(row["DiscountPassive"])
                };
                if (row.Field<string>("CustomerRuleAdditional") != "Null")
                {
                    string[] splitData = row.Field<string>("CustomerRuleAdditional").Split('-');

                    product.RuleAdditional = new RuleAdditional()
                    {
                        Manufacturer = splitData[0],
                        ProductGroup1 = splitData[1],
                        ProductGroup2 = splitData[2],
                        ProductGroup3 = splitData[3],
                        Disc1 = Convert.ToDouble(splitData[4].Replace(".", ",")),
                        Disc2 = Convert.ToDouble(splitData[5].Replace(".", ",")),
                        Disc3 = Convert.ToDouble(splitData[6].Replace(".", ",")),
                        Disc4 = Convert.ToDouble(splitData[7].Replace(".", ",")),
                        Rate = Convert.ToDouble(splitData[8].Replace(".", ",")),
                        MainDiscountPassive = Convert.ToBoolean(Convert.ToInt32(splitData[9])),

                    };

                }

            }

            product.CalculateDetailInformation(false, (customer.CampaignStatu && customer.Users.AuthorityUser._CampaignStatu), -1, 0, false, 0, "TL", 0, customer.Users.Rate);
            product.AvailabilityStatus = product.CalculateAvaibilityStatus(product.TotalQuantity);
            string availabilityText, availabilityCss;

            product.SetAvailabilityClass(product.LoginType, product.TotalQuantity, product.AvailabilityStatus, out availabilityCss, out availabilityText);
            product.AvailabilityCss = availabilityCss;
            product.AvailabilityText = availabilityText;

            return product;
        }
        public static Product GetByCode(string productCode)
        {
            Product product = new Product();
            DataTable dt = DAL.GetProductByCode(productCode);

            foreach (DataRow row in dt.Rows)
            {
                product.Id = row.Field<int>("Id");
                product.GroupId = row.Field<int>("GroupId");
                product.Code = row.Field<string>("Code");
                product.Name = row.Field<string>("Name");
                product.Manufacturer = row.Field<string>("Manufacturer");
                product.ManufacturerCode = NullControl(row.Field<string>("ManufacturerCode"));
                product.Unit = row.Field<string>("Unit");
                product.TotalQuantity = row.Field<double>("TotalQuantity");
                product.TotalQuantityOnWay = row.Field<double>("TotalQuantityOnWay");
                product.WarehouseCount = row.Field<int>("WarehouseCount");
                product.NewProduct = Convert.ToBoolean(row["NewProduct"]);
                product.HavePicture = Convert.ToBoolean(row["HavePicture"]);
                product.VatRate = row.Field<double>("VatRate");
                product.MinOrder = row.Field<double>("MinOrder");
                product.CriticalLevel = row.Field<double>("CriticalLevel");
                product.QuantityType = row.Field<int>("QuantityType");
                product.IsPackIncrease = Convert.ToBoolean(row["IsPackIncrease"]);
                product.Notes = row.Field<string>("Notes");
                product.Width = row.Field<double>("Width");
                product.Height = row.Field<double>("Height");
                product.Lenght = row.Field<double>("Lenght");
                product.GrossWeight = row.Field<double>("GrossWeight");
                product.NetWeight = row.Field<double>("NetWeight");
                product.PicturePath = row.Field<string>("PicturePath") == string.Empty ? "../Content/images/nophoto.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath");

                product.Campaign = new Campaign()
                {
                    Id = Convert.ToInt32(row["CampaignId"]),
                    Type = (CampaignType)Convert.ToInt32(row["CampaignType"]),

                    Discount = Convert.ToDouble(row["Discount"]),
                    TotalQuantity = Convert.ToDouble(row["CampaignTotalQuantity"]),
                    SaledQuantity = Convert.ToDouble(row["SaledQuantity"]),
                    PromotionProductCode = row.Field<string>("PromotionProductCode"),
                    PromotionProductQuantity = Convert.ToDouble(row["PromotionProductQuantity"]),
                    DiscountPassive = Convert.ToBoolean(row["DiscountPassive"])
                };

            }

            product.AvailabilityStatus = product.CalculateAvaibilityStatus(product.TotalQuantity);
            string availabilityText, availabilityCss;

            product.SetAvailabilityClass(product.LoginType, product.TotalQuantity, product.AvailabilityStatus, out availabilityCss, out availabilityText);
            product.AvailabilityCss = availabilityCss;
            product.AvailabilityText = availabilityText;

            return product;

        }

        public static List<Product> Search(int startIndex, int finishIndex, LoginType loginType, Customer customer, int salesmanId, string manufacturer, string t9Text, string productGroup1, string productGroup2, string productGroup3, int newProduct, int comparsionProduct, int campaignProduct, int newArrival, string vehicleBrand, string vehicleModel, string vehicleType, int groupId = -1, int onQty = 0, int onWay = 0, int picture = 0, int bannerStatu = 0, string pOrderBy = "")
        {
            List<Product> productList = new List<Product>();
            string[] t9Array = GenerateT9Search(t9Text);
            DataTable dt = DAL.GetProductListBySearch(startIndex, finishIndex, customer.Id, customer.Users.Id, salesmanId, manufacturer,
           productGroup1, productGroup2, productGroup3, newProduct, comparsionProduct, campaignProduct, newArrival, vehicleBrand, vehicleModel, vehicleType, groupId,
           t9Array[0], t9Array[1], t9Array[2], t9Array[3], t9Array[4], t9Array[5], t9Array[6], t9Array[7], t9Array[8], t9Array[9], onQty, onWay, picture, bannerStatu, pOrderBy);

            foreach (DataRow row in dt.Rows)
            {
                Product product = new Product();
                {
                    product.Customer = customer;
                    product.CustomerCurrency = customer.CurrencyType;
                    product.LoginType = loginType;
                    product.Id = row.Field<int>("Id");
                    product.GroupId = row.Field<int>("GroupId");
                    product.Code = row.Field<string>("Code");
                    product.Name = row.Field<string>("Name");
                    product.Manufacturer = row.Field<string>("Manufacturer");
                    product.ManufacturerCode = NullControl(row.Field<string>("ManufacturerCode"));
                    product.Unit = row.Field<string>("Unit");
                    product.TotalQuantity = row.Field<double>("TotalQuantity");
                    product.TotalQuantityOnWay = row.Field<double>("TotalQuantityOnWay");
                    product.WarehouseCount = row.Field<int>("WarehouseCount");
                    product.NewProduct = row.Field<bool>("NewProduct");
                    product.HavePicture = row.Field<bool>("HavePicture");
                    product.VatRate = Convert.ToDouble(row["VatRate"]);
                    product.MinOrder = row.Field<double>("MinOrder");
                    product.Price = row.Field<double>("Price");
                    product.PriceCurrency = row.Field<string>("PriceCurrency");
                    product.FollowId = Convert.ToInt32(row["FollowId"]);
                    product.ComparisonId = Convert.ToInt32(row["ComparisonId"]);
                    //product.SpecialDiscount = row.Field<double>("SpecialDiscount");
                    product.CriticalLevel = row.Field<double>("CriticalLevel");
                    product.QuantityType = row.Field<int>("QuantityType");
                    product.IsPackIncrease = row.Field<bool>("IsPackIncrease");
                    product.HavePicture = row.Field<bool>("HavePicture");
                    product.PicturePath = row.Field<string>("PicturePath") == string.Empty ? GlobalSettings.B2bAddress + "Content/images/nophoto.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath");

                    product.Rule = new Rule()
                    {
                        Customer = row.Field<string>("Customer"),
                        PaymentType = row.Field<int>("PaymentType"),
                        Disc1 = row.Field<double>("Disc1"),
                        Disc2 = row.Field<double>("Disc2"),
                        Disc3 = row.Field<double>("Disc3"),
                        Disc4 = row.Field<double>("Disc4"),
                        PriceNumber = row.Field<int>("PriceNumber"),
                        Rate = row.Field<double>("Rate")
                    };

                    product.Campaign = new Campaign()
                    {
                        Id = Convert.ToInt32(row["CampaignId"]),
                        Type = (CampaignType)Convert.ToInt32(row.Field<long>("CampaignType")),
                        Price = Convert.ToDouble(row["CampaignPrice"]),
                        Currency = row.Field<string>("CampaignCurrency"),
                        CurrencyRate = Convert.ToDouble(row["CampaignCurrencyRate"]),
                        Discount = Convert.ToDouble(row["Discount"]),
                        MinOrder = Convert.ToDouble(row["CampaignMinOrder"]),
                        TotalQuantity = Convert.ToDouble(row["CampaignTotalQuantity"]),
                        SaledQuantity = Convert.ToDouble(row["SaledQuantity"]),
                        PromotionProductCode = row.Field<string>("PromotionProductCode"),
                        PromotionProductQuantity = Convert.ToDouble(row["PromotionProductQuantity"]),
                        DiscountPassive = Convert.ToBoolean(row["DiscountPassive"])
                    };

                    //MATCH (product.T9Text) AGAINST (pT9Text IN BOOLEAN MODE) as SCORE

                    product.PriceCurrencyRate = row.Field<double>("PriceCurrencyRate");

                    if (row.Field<string>("CustomerRuleAdditional") != "Null")
                    {
                        string[] splitData = row.Field<string>("CustomerRuleAdditional").Split('-');

                        product.RuleAdditional = new RuleAdditional()
                        {
                            Manufacturer = splitData[0],
                            ProductGroup1 = splitData[1],
                            ProductGroup2 = splitData[2],
                            ProductGroup3 = splitData[3],
                            Disc1 = Convert.ToDouble(splitData[4].Replace(".", ",")),
                            Disc2 = Convert.ToDouble(splitData[5].Replace(".", ",")),
                            Disc3 = Convert.ToDouble(splitData[6].Replace(".", ",")),
                            Disc4 = Convert.ToDouble(splitData[7].Replace(".", ",")),
                            Rate = Convert.ToDouble(splitData[8].Replace(".", ",")),
                            MainDiscountPassive = Convert.ToBoolean(Convert.ToInt32(splitData[9])),
                            SalesPrice = Convert.ToDouble(splitData[10].Replace(".", ",")),
                            Currency = Convert.ToString(splitData[11]),
                            CurrenctyRate = Convert.ToDouble(splitData[12].Replace(".", ",")),
                        };

                    }

                };


                product.CalculateDetailInformation(false, (customer.CampaignStatu && customer.Users.AuthorityUser._CampaignStatu), -1, 0, false, 0, "TL", 0, customer.Users.Rate);
                product.AvailabilityStatus = product.CalculateAvaibilityStatus(product.TotalQuantity);
                string availabilityText, availabilityCss;

                product.SetAvailabilityClass(product.LoginType, product.TotalQuantity, product.AvailabilityStatus, out availabilityCss, out availabilityText);
                product.AvailabilityCss = availabilityCss;
                product.AvailabilityText = availabilityText;

                productList.Add(product);
            }

            return productList;
        }

        #region AdminArea

        public static List<Product> GetProductSpecialCode()
        {
            DataTable dt = new DataTable();
            List<Product> list = new List<Product>();

            dt = DAL.GetProductSpecialCode();

            foreach (DataRow row in dt.Rows)
            {
                Product item = new Product()
                {
                    SpecialCode1 = row.Field<string>("SpecialCode1")
                };
                list.Add(item);
            }

            return list;
        }

        public static List<Product> AdminGetProductListBySearch(SearchCriteria searchCriteria)
        {
            searchCriteria.Code = searchCriteria.Code ?? "*";
            searchCriteria.Name = searchCriteria.Name ?? "*";
            searchCriteria.Manufacturer = searchCriteria.Manufacturer ?? "*";
            searchCriteria.ManufacturerCode = searchCriteria.ManufacturerCode ?? "*";
            searchCriteria.ShelfAdress = searchCriteria.ShelfAdress ?? "*";
            searchCriteria.RuleCode = searchCriteria.RuleCode ?? "*";
            searchCriteria.T9Text = searchCriteria.T9Text ?? "*";
            List<Product> list = new List<Product>();
            string[] t9Array = GenerateT9Search(searchCriteria.T9Text);
            DataTable dt = DAL.AdminGetProductListBySearch(searchCriteria.Code, searchCriteria.Name, searchCriteria.Manufacturer, searchCriteria.ManufacturerCode, searchCriteria.ShelfAdress, searchCriteria.RuleCode, t9Array[0], t9Array[1], t9Array[2], t9Array[3], t9Array[4], t9Array[5], t9Array[6], t9Array[7], t9Array[8], t9Array[9]);

            foreach (DataRow row in dt.Rows)
            {
                Product product = new Product();
                {
                    product.Id = row.Field<int>("Id");
                    product.GroupId = row.Field<int>("GroupId");
                    product.Code = row.Field<string>("Code");
                    product.Name = row.Field<string>("Name");
                    product.Manufacturer = row.Field<string>("Manufacturer");
                    product.ManufacturerCode = row.Field<string>("ManufacturerCode");
                    product.Unit = row.Field<string>("Unit");
                    product.ProductGroup1 = row.Field<string>("ProductGroup1");
                    product.ProductGroup2 = row.Field<string>("ProductGroup2");
                    product.ProductGroup3 = row.Field<string>("ProductGroup3");
                    product.NewProduct = row.Field<bool>("NewProduct");
                    product.HavePicture = row.Field<bool>("HavePicture");
                    product.VatRate = row.Field<double>("VatRate");
                    product.MinOrder = row.Field<double>("MinOrder");
                    product.SalesPrice1 = new Price(Convert.ToDouble(row["SalesPrice1"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType1")).ToString();
                    //product.SalesPrice2 = new Price(Convert.ToDouble(row["SalesPrice2"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType2")).ToString();
                    //product.SalesPrice3 = new Price(Convert.ToDouble(row["SalesPrice3"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType3")).ToString();
                    //product.SalesPrice4 = new Price(Convert.ToDouble(row["SalesPrice4"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType4")).ToString();
                    //product.SalesPrice5 = new Price(Convert.ToDouble(row["SalesPrice5"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType5")).ToString();
                    product.PurchasePrice = new Price(Convert.ToDouble(row["PurchasePrice"]), row.Field<string>("PurchaseType")).ToString();
                    product.ProfitRate = Convert.ToDouble(row["ProfitRate"]);
                    product.QuantityType = row.Field<int>("QuantityType");
                    product.SpecialDiscount = row.Field<double>("MaxSpecialDiscount");
                    product.T9Text = row.Field<string>("T9Text");
                    product.CriticalLevel = row.Field<double>("CriticalLevel");
                    product.ShelfAddress = row.Field<string>("ShelfAddress");
                    product.Notes = row.Field<string>("Notes");
                    product.Notes1 = row.Field<string>("Notes1");
                    product.Notes2 = row.Field<string>("Notes2");
                    product.Notes3 = row.Field<string>("Notes3");
                    product.Notes4 = row.Field<string>("Notes4");
                    product.Notes5 = row.Field<string>("Notes5");
                    product.RuleCode = row.Field<string>("RuleCode");
                    product.IsActive = row.Field<bool>("IsActive");
                    product.Bonus = row.Field<double>("Bonus");
                    product.IsPackIncrease = row.Field<bool>("IsPackIncrease");
                    product.Name2 = row.Field<string>("Name2");
                    product.QuantityInPackage = row.Field<double>("QuantityInPackage");
                    product.Width = row.Field<double>("Width");
                    product.Lenght = row.Field<double>("Lenght");
                    product.Height = row.Field<double>("Height");
                    product.GrossWeight = row.Field<double>("GrossWeight");
                    product.NetWeight = row.Field<double>("NetWeight");
                }

                list.Add(product);
            }

            return list;
        }
        public static List<Product> AdminGetProductById(int pProductId)
        {

            DataTable dt = DAL.AdminGetProductById(pProductId);
            List<Product> list = new List<Product>();
            foreach (DataRow row in dt.Rows)
            {
                Product product = new Product();
                {
                    product.Id = row.Field<int>("Id");
                    product.GroupId = row.Field<int>("GroupId");
                    product.Code = row.Field<string>("Code");
                    product.Name = row.Field<string>("Name");
                    product.Manufacturer = row.Field<string>("Manufacturer");
                    product.ManufacturerCode = row.Field<string>("ManufacturerCode");
                    product.Unit = row.Field<string>("Unit");
                    product.ProductGroup1 = row.Field<string>("ProductGroup1");
                    product.ProductGroup2 = row.Field<string>("ProductGroup2");
                    product.ProductGroup3 = row.Field<string>("ProductGroup3");
                    product.NewProduct = row.Field<bool>("NewProduct");
                    product.HavePicture = row.Field<bool>("HavePicture");
                    product.VatRate = row.Field<double>("VatRate");
                    product.MinOrder = row.Field<double>("MinOrder");
                    product.SalesPrice1 = new Price(Convert.ToDouble(row["SalesPrice1"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType1")).ToString();
                    product.SalesPrice2 = new Price(Convert.ToDouble(row["SalesPrice2"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType2")).ToString();
                    product.SalesPrice3 = new Price(Convert.ToDouble(row["SalesPrice3"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType3")).ToString();
                    product.SalesPrice4 = new Price(Convert.ToDouble(row["SalesPrice4"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType4")).ToString();
                    product.SalesPrice5 = new Price(Convert.ToDouble(row["SalesPrice5"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType5")).ToString();
                    product.PurchasePrice = new Price(Convert.ToDouble(row["PurchasePrice"]), row.Field<string>("PurchaseType")).ToString();
                    product.ProfitRate = Convert.ToDouble(row["ProfitRate"]);
                    product.QuantityType = row.Field<int>("QuantityType");
                    product.SpecialDiscount = row.Field<double>("MaxSpecialDiscount");
                    product.T9Text = row.Field<string>("T9Text");
                    product.CriticalLevel = row.Field<double>("CriticalLevel");
                    product.ShelfAddress = row.Field<string>("ShelfAddress");
                    product.Notes = row.Field<string>("Notes");
                    product.Notes1 = row.Field<string>("Notes1");
                    product.Notes2 = row.Field<string>("Notes2");
                    product.Notes3 = row.Field<string>("Notes3");
                    product.Notes4 = row.Field<string>("Notes4");
                    product.Notes5 = row.Field<string>("Notes5");
                    product.RuleCode = row.Field<string>("RuleCode");
                    product.IsActive = row.Field<bool>("IsActive");
                    product.Bonus = row.Field<double>("Bonus");
                    product.IsPackIncrease = row.Field<bool>("IsPackIncrease");
                    product.Name2 = row.Field<string>("Name2");
                    product.QuantityInPackage = row.Field<double>("QuantityInPackage");
                    product.Width = row.Field<double>("Width");
                    product.Lenght = row.Field<double>("Lenght");
                    product.Height = row.Field<double>("Height");
                    product.GrossWeight = row.Field<double>("GrossWeight");
                    product.NetWeight = row.Field<double>("NetWeight");
                }

                list.Add(product);
            }

            return list;
        }
        public static List<Product> AdminGetNextProduct(string pCode, int pType)
        {

            List<Product> list = new List<Product>();

            DataTable dt = DAL.AdminGetNextProductList(pCode, pType);

            foreach (DataRow row in dt.Rows)
            {
                Product product = new Product();
                {
                    product.Id = row.Field<int>("Id");
                    product.GroupId = row.Field<int>("GroupId");
                    product.Code = row.Field<string>("Code");
                    product.Name = row.Field<string>("Name");
                    product.Manufacturer = row.Field<string>("Manufacturer");
                    product.ManufacturerCode = row.Field<string>("ManufacturerCode");
                    product.Unit = row.Field<string>("Unit");
                    product.ProductGroup1 = row.Field<string>("ProductGroup1");
                    product.ProductGroup2 = row.Field<string>("ProductGroup2");
                    product.ProductGroup3 = row.Field<string>("ProductGroup3");
                    product.NewProduct = row.Field<bool>("NewProduct");
                    product.HavePicture = row.Field<bool>("HavePicture");
                    product.VatRate = row.Field<double>("VatRate");
                    product.MinOrder = row.Field<double>("MinOrder");

                    product.SalesPrice1 = new Price(Convert.ToDouble(row["SalesPrice1"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType1")).ToString();
                    product.SalesPrice2 = new Price(Convert.ToDouble(row["SalesPrice2"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType2")).ToString();
                    product.SalesPrice3 = new Price(Convert.ToDouble(row["SalesPrice3"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType3")).ToString();
                    product.SalesPrice4 = new Price(Convert.ToDouble(row["SalesPrice4"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType4")).ToString();
                    product.SalesPrice5 = new Price(Convert.ToDouble(row["SalesPrice5"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType5")).ToString();
                    product.PurchasePrice = new Price(Convert.ToDouble(row["PurchasePrice"]), row.Field<string>("PurchaseType")).ToString();
                    product.ProfitRate = Convert.ToDouble(row["ProfitRate"]);

                    product.QuantityType = row.Field<int>("QuantityType");
                    product.SpecialDiscount = row.Field<double>("MaxSpecialDiscount");
                    product.T9Text = row.Field<string>("T9Text");
                    product.CriticalLevel = row.Field<double>("CriticalLevel");
                    product.ShelfAddress = row.Field<string>("ShelfAddress");
                    product.Notes = row.Field<string>("Notes");
                    product.Notes1 = row.Field<string>("Notes1");
                    product.Notes2 = row.Field<string>("Notes2");
                    product.Notes3 = row.Field<string>("Notes3");
                    product.Notes4 = row.Field<string>("Notes4");
                    product.Notes5 = row.Field<string>("Notes5");
                    product.RuleCode = row.Field<string>("RuleCode");
                    product.IsActive = row.Field<bool>("IsActive");
                    product.Bonus = row.Field<double>("Bonus");
                    product.IsPackIncrease = row.Field<bool>("IsPackIncrease");
                    product.Name2 = row.Field<string>("Name2");
                    product.QuantityInPackage = row.Field<double>("QuantityInPackage");
                    product.Width = row.Field<double>("Width");
                    product.Lenght = row.Field<double>("Lenght");
                    product.Height = row.Field<double>("Height");
                    product.GrossWeight = row.Field<double>("GrossWeight");
                    product.NetWeight = row.Field<double>("NetWeight");
                }

                list.Add(product);
            }

            return list;
        }
        public static bool UpdateGroupId(int oldGroupId, int newGroupId, int editId)
        {
            return DAL.UpdateProductGroupId(oldGroupId, newGroupId, editId);
        }
        public static bool UpdateGroupIdByProductId(int productId, int newId, int editId)
        {
            return DAL.UpdateProductGroupIdByProductId(productId, newId, editId);
        }
        public static bool InsertLinkedProduct(int pLinkedId, int pMainProductId, int pCreateId)
        {
            return DAL.InsertLinkedProduct(pLinkedId, pMainProductId, pCreateId);
        }
        public static bool DeleteLinkedProduct(int pId, int pEditId)
        {
            return DAL.DeleteLinkedProduct(pId, pEditId);
        }
        public static bool UpdateProduct(Product product, int pEditId)
        {
            #region NullControl
            product.Code = product.Code ?? string.Empty;
            product.Name = product.Name ?? string.Empty;
            product.Manufacturer = product.Manufacturer ?? string.Empty;
            product.ManufacturerCode = product.ManufacturerCode ?? string.Empty;
            product.Unit = product.Unit ?? string.Empty;
            product.ProductGroup1 = product.ProductGroup1 ?? string.Empty;
            product.ProductGroup2 = product.ProductGroup2 ?? string.Empty;
            product.ProductGroup3 = product.ProductGroup3 ?? string.Empty;
            product.ShelfAddress = product.ShelfAddress ?? string.Empty;
            product.Notes = product.Notes ?? string.Empty;
            product.Notes1 = product.Notes1 ?? string.Empty;
            product.Notes2 = product.Notes2 ?? string.Empty;
            product.Notes3 = product.Notes3 ?? string.Empty;
            product.Notes4 = product.Notes4 ?? string.Empty;
            product.Notes5 = product.Notes5 ?? string.Empty;
            product.RuleCode = product.RuleCode ?? string.Empty;
            product.Name2 = product.Name2 ?? string.Empty;
            #endregion
            return DAL.UpdateProduct(product.Id, product.GroupId, product.Code, product.Name, product.Manufacturer, product.ManufacturerCode, product.Unit, product.ProductGroup1, product.ProductGroup2, product.ProductGroup3, product.NewProduct, product.HavePicture, product.VatRate, product.MinOrder, product.ProfitRate, product.SpecialDiscount, product.CriticalLevel, product.ShelfAddress, product.Notes, product.Notes1, product.Notes2, product.Notes3, product.Notes4, product.Notes5, product.RuleCode, product.IsActive, product.Bonus, product.IsPackIncrease, product.Name2, product.QuantityInPackage, product.Width, product.Height, product.Lenght, product.GrossWeight, product.NetWeight, pEditId);


        }
        public static List<Product> GetProductListByGroupId(int groupId)
        {
            List<Product> list = new List<Product>();
            DataTable dt = DAL.AdminGetProductListByGroupId(groupId);

            foreach (DataRow row in dt.Rows)
            {
                Product product = new Product();
                {
                    product.Id = row.Field<int>("Id");
                    product.GroupId = row.Field<int>("GroupId");
                    product.Code = row.Field<string>("Code");
                    product.Name = row.Field<string>("Name");
                    product.Manufacturer = row.Field<string>("Manufacturer");
                    product.ManufacturerCode = row.Field<string>("ManufacturerCode");
                    product.Unit = row.Field<string>("Unit");
                    product.SalesPrice1 = row["SalesPrice1"].ToString();// new Price(Convert.ToDouble(row["SalesPrice1"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType1")).ToString();
                    product.PriceCurrency = row.Field<string>("CurrencyType1");// new Price(Convert.ToDouble(row["SalesPrice1"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType1")).ToString();
                }
                list.Add(product);
            }

            return list;
        }
        public static List<Product> GetProductListLinked(int productId)
        {
            List<Product> list = new List<Product>();
            DataTable dt = DAL.AdminGetProductListLinked(productId);

            foreach (DataRow row in dt.Rows)
            {
                Product product = new Product();
                {
                    product = Product.GetById(row.Field<int>("LinkedId"));
                    product.LinkedId = row.Field<int>("LinkedId");
                    product.Id = row.Field<int>("Id");
                    product.GroupId = row.Field<int>("GroupId");
                    product.Code = row.Field<string>("Code");
                    product.Name = row.Field<string>("Name");
                    product.Manufacturer = row.Field<string>("Manufacturer");
                    product.ManufacturerCode = row.Field<string>("ManufacturerCode");
                    product.Unit = row.Field<string>("Unit");
                    product.SalesPrice1 = new Price(Convert.ToDouble(row["SalesPrice1"].ToString().Replace(".", ",")), row.Field<string>("CurrencyType1")).ToString();

                }
                list.Add(product);
            }

            return list;
        }
        public static List<Product> GetProductListLinked(int productId, LoginType loginType, Customer currentCustomer)
        {
            List<Product> list = new List<Product>();
            DataTable dt = DAL.AdminGetProductListLinked(productId);

            foreach (DataRow row in dt.Rows)
            {
                Product product = new Product();
                {
                    product.LinkedId = row.Field<int>("LinkedId");
                    product = Product.GetById(product.LinkedId, loginType, currentCustomer);
                }
                list.Add(product);
            }

            return list;
        }
        #endregion

        public static List<Product> GetByIdProductCodes(string productCodes, LoginType loginType, Customer customer)
        {

            List<Product> list = new List<Product>();

            DataTable dt = DAL.GetByIdProductCodes(productCodes, customer.Id, customer.Users.Id, (int)loginType);

            foreach (DataRow row in dt.Rows)
            {
                Product product = new Product();
                {
                    product.Customer = customer;
                    product.CustomerCurrency = customer.CurrencyType;
                    product.LoginType = loginType;
                    product.Id = row.Field<int>("Id");
                    product.GroupId = row.Field<int>("GroupId");
                    product.Code = row.Field<string>("Code");
                    product.Name = row.Field<string>("Name");
                    product.Manufacturer = row.Field<string>("Manufacturer");
                    product.ManufacturerCode = NullControl(row.Field<string>("ManufacturerCode"));
                    product.Unit = row.Field<string>("Unit");
                    product.TotalQuantity = row.Field<double>("TotalQuantity");
                    product.TotalQuantityOnWay = row.Field<double>("TotalQuantityOnWay");
                    product.WarehouseCount = row.Field<int>("WarehouseCount");
                    product.NewProduct = row.Field<bool>("NewProduct");
                    product.HavePicture = row.Field<bool>("HavePicture");
                    product.VatRate = Convert.ToDouble(row["VatRate"]);
                    product.MinOrder = row.Field<double>("MinOrder");
                    product.Price = row.Field<double>("Price");
                    product.PriceCurrency = row.Field<string>("PriceCurrency");
                    product.CriticalLevel = row.Field<double>("CriticalLevel");
                    product.QuantityType = row.Field<int>("QuantityType");
                    product.IsPackIncrease = row.Field<bool>("IsPackIncrease");
                    product.PriceCurrencyRate = row.Field<double>("PriceCurrencyRate");
                    product.Notes = row.Field<string>("Notes");
                    product.Width = row.Field<double>("Width");
                    product.Height = row.Field<double>("Height");
                    product.Lenght = row.Field<double>("Lenght");
                    product.GrossWeight = row.Field<double>("GrossWeight");
                    product.NetWeight = row.Field<double>("NetWeight");
                    product.PicturePath = row.Field<string>("PicturePath") == string.Empty ? "../Content/images/nophoto.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath");
                    product.Rule = new Rule()
                    {
                        Customer = row.Field<string>("Customer"),
                        PaymentType = row.Field<int>("PaymentType"),
                        Disc1 = row.Field<double>("Disc1"),
                        Disc2 = row.Field<double>("Disc2"),
                        Disc3 = row.Field<double>("Disc3"),
                        Disc4 = row.Field<double>("Disc4"),
                        PriceNumber = row.Field<int>("PriceNumber"),
                        Rate = row.Field<double>("Rate")
                    };
                    product.Campaign = new Campaign()
                    {
                        Id = Convert.ToInt32(row["CampaignId"]),
                        Type = (CampaignType)Convert.ToInt32(row.Field<long>("CampaignType")),
                        Price = Convert.ToDouble(row["CampaignPrice"]),
                        Currency = row.Field<string>("CampaignCurrency"),
                        CurrencyRate = Convert.ToDouble(row["CampaignCurrencyRate"]),
                        Discount = Convert.ToDouble(row["Discount"]),
                        MinOrder = Convert.ToDouble(row["CampaignMinOrder"]),
                        TotalQuantity = Convert.ToDouble(row["CampaignTotalQuantity"]),
                        SaledQuantity = Convert.ToDouble(row["SaledQuantity"]),
                        PromotionProductCode = row.Field<string>("PromotionProductCode"),
                        PromotionProductQuantity = Convert.ToDouble(row["PromotionProductQuantity"]),
                        DiscountPassive = Convert.ToBoolean(row["DiscountPassive"]),
                        StartDate = row.Field<DateTime>("StartDate"),
                        FinishDate = row.Field<DateTime>("FinishDate")
                    };
                    if (row.Field<string>("CustomerRuleAdditional") != "Null")
                    {
                        string[] splitData = row.Field<string>("CustomerRuleAdditional").Split('-');

                        product.RuleAdditional = new RuleAdditional()
                        {
                            Manufacturer = splitData[0],
                            ProductGroup1 = splitData[1],
                            ProductGroup2 = splitData[2],
                            ProductGroup3 = splitData[3],
                            Disc1 = Convert.ToDouble(splitData[4].Replace(".", ",")),
                            Disc2 = Convert.ToDouble(splitData[5].Replace(".", ",")),
                            Disc3 = Convert.ToDouble(splitData[6].Replace(".", ",")),
                            Disc4 = Convert.ToDouble(splitData[7].Replace(".", ",")),
                            Rate = Convert.ToDouble(splitData[8].Replace(".", ",")),
                            MainDiscountPassive = Convert.ToBoolean(Convert.ToInt32(splitData[9])),
                            SalesPrice = Convert.ToDouble(splitData[10].Replace(".", ",")),
                            Currency = Convert.ToString(splitData[11]),
                            CurrenctyRate = Convert.ToDouble(splitData[12].Replace(".", ",")),

                        };
                    }

                }
                product.CalculateDetailInformation(false, (customer.CampaignStatu && customer.Users.AuthorityUser._CampaignStatu), -1, 0, false, 0, "TL", 0, customer.Users.Rate);
                product.AvailabilityStatus = product.CalculateAvaibilityStatus(product.TotalQuantity);
                string availabilityText, availabilityCss;

                product.SetAvailabilityClass(product.LoginType, product.TotalQuantity, product.AvailabilityStatus, out availabilityCss, out availabilityText);
                product.AvailabilityCss = availabilityCss;
                product.AvailabilityText = availabilityText;

                list.Add(product);
            }





            return list;

        }

        #endregion
    }


    public enum AvailabilityStatus
    {
        Available,
        Unavailable,
        LittleAvailable,
        OnOrder,
        InCustoms,
        AskOnPhone,
        None
    }


    public partial class DataAccessLayer
    {
        public DataTable GetByIdProductCodes(string pProductCodes, int pCustomerId, int pUserId, int pLoginType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ProductByCodes", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductCodes, pCustomerId, pUserId, pLoginType });
        }

        public DataTable GetProductListBySearch(int pStartIndex, int pFinishIndex, int pCustomerId, int pUserId, int pSalesmanId,
         string pManufacturer, string pProductGroup1, string pProductGroup2, string pProductGroup3,
         int? pNewProduct, int? pIsComparison, int? pCampaignProduct, int? pNewArrival, string pVehicleBrand, string pVehicleBrandModel, string pVehicleType, int? pGroupId,
         string pT9Text1, string pT9Text2, string pT9Text3, string pT9Text4, string pT9Text5, string pT9Text6,
        string pT9Text7, string pT9Text8, string pT9Text9, string pT9Text10, int pOnQty, int pOnWay, int pPicture, int pBannerStatu, string pOrderBy)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_GeneralSearch", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pStartIndex, pFinishIndex, pCustomerId, pUserId, pSalesmanId, pManufacturer, pProductGroup1, pProductGroup2, pProductGroup3, pNewProduct, pIsComparison, pCampaignProduct, pNewArrival, pVehicleBrand, pVehicleBrandModel, pVehicleType, pGroupId, pT9Text1, pT9Text2, pT9Text3, pT9Text4, pT9Text5, pT9Text6, pT9Text7, pT9Text8, pT9Text9, pT9Text10, pOnQty, pOnWay, pPicture, pBannerStatu, pOrderBy });
        }



        public DataTable GetProductById(int pProductId, int pCustomerId, int pUserId, int pLoginType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_ProductById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId, pCustomerId, pUserId, pUserId, pLoginType });
        }

        public DataTable GetProductListByKitMainId(int pMainProductId, int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ProductByKitMainId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pMainProductId, pCustomerId });
        }

        public DataTable GetProductByOnlyId(int pProductId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetItem_ProductByOnlyId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId });
        }


        public bool UpdateProduct(int pId, int pGroupId, string pCode, string pName, string pManufacturer, string pManufacturerCode, string pUnit, string pProductGroup1, string pProductGroup2, string pProductGroup3, bool pNewProduct, bool pHavePicture, double pTaxRate, double pMinOrder, double pProfitRate, double pSpecialDiscount, double pCriticalLevel, string pShelfAddress, string pNotes, string pNotes1, string pNotes2, string pNotes3, string pNotes4, string pNotes5, string pRuleCode, bool pIsActive, double pBonus, bool pIsPackIncrease, string pName2, double pQuantityInPackage, double pWidth, double pHeight, double pLenght, double pGrossWeight, double pNetWeight, int pEditId)
        {

            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Product", MethodBase.GetCurrentMethod().GetParameters(), new object[] {  pId,pGroupId,pCode,pName , pManufacturer , pManufacturerCode , pUnit ,pProductGroup1,
                    pProductGroup2 ,  pProductGroup3 , pNewProduct,pHavePicture ,pTaxRate , pMinOrder, pProfitRate , pSpecialDiscount, pCriticalLevel , pShelfAddress, pNotes, pNotes1,  pNotes2, pNotes3, pNotes4, pNotes5, pRuleCode , pIsActive,
                    pBonus, pIsPackIncrease,pName2,pQuantityInPackage,pWidth,pHeight,pLenght,pGrossWeight,pNetWeight,pEditId
         });
        }

        public DataTable GetProductByCode(string pProductCode, int pCustomerId, int pUserId, int pLoginType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_ProductByCode", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductCode, pCustomerId, pUserId, pLoginType });
        }
        public DataTable GetProductByCode(string pProductCode)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetItem_ProductByCode", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductCode });
        }



        #region AdminArea
        public DataTable AdminGetProductListBySearch(string pCode, string pName, string pManufacturer,
           string pManufacturerCode, string pShelfAddress, string pRule, string pT9Text, string pT9Text2, string pT9Text3, string pT9Text4, string pT9Text5, string pT9Text6, string pT9Text7, string pT9Text8, string pT9Text9, string pT9Text10)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_Product_Search", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCode, pName, pManufacturer, pManufacturerCode, pShelfAddress, pRule, pT9Text, pT9Text2, pT9Text3, pT9Text4, pT9Text5, pT9Text6, pT9Text7, pT9Text8, pT9Text9, pT9Text10 });
        }
        public DataTable AdminGetProductById(int pProductId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetItem_ProductById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId });
        }

        public DataTable GetProductSpecialCode()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_ProductSpecialCode");
        }

        public DataTable AdminGetNextProductList(string pCode, int pType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetItem_Product_Next", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCode, pType });
        }
        public DataTable AdminGetProductListByGroupId(int pGroupId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_ProductByGroupId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pGroupId });
        }
        public DataTable AdminGetProductListLinked(int pProductId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_Product_Linked", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId });
        }
        public bool UpdateProductGroupId(int pOldGroupId, int pNewGroupId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Product_GroupId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pOldGroupId, pNewGroupId, pEditId });
        }
        public bool UpdateProductGroupIdByProductId(int pProductId, int pNewGroupId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Product_GroupIdByProductId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId, pNewGroupId, pEditId });
        }
        public bool InsertLinkedProduct(int pLinkedProductId, int pMainProductId, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Product_Linked", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pLinkedProductId, pMainProductId, pCreateId });
        }
        public bool DeleteLinkedProduct(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_Product_Linked", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }

        #endregion



    }

}