using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class Basket : DataAccess, ICloneable
    {
        #region Constructors
        public Basket()
        {
            OrderId = -1;
            Status = 0;
            IsPromotion = false;
            AddType = 1;
            Product = new Product();
            Product.PriceNetCustomer = new Price(0, string.Empty);
            Product.PriceNet = new Price(0, string.Empty);
            Product.Campaign = new Campaign();
            Notes = string.Empty;
            FixedPrice = 0;
            IsFixedPrice = false;
            FixedCurrency = "TL";
            RowCss = string.Empty;
            RowCmpText = string.Empty;
            LogId = -1;

        }
        #endregion

        #region Properties
        public int Id { get; set; }
        public bool IsPromotion { get; set; }
        public int PromotionProductCount { get; set; }
        public bool IsCmpExtra { get; set; }
        public Product Product { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int SalesmanId { get; set; }
        public double Quantity { get; set; }
        public double QuantityAvailable { get { return (Quantity > Product.TotalQuantity ? Product.TotalQuantity : Quantity); } }
        public double QuantityOrj { get; set; }
        public double DiscSpecial { get; set; }
        public bool ItemAvailableCoupon { get; set; }
        public DateTime RecordDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int Status { get; set; }
        public int AddType { get; set; }
        public int ClientNumber { get; set; }
        public int ProgramType { get; set; }
        public bool IsFixedPrice { get; set; }
        public double FixedPrice { get; set; }
        public double FixedCurrencyRate { get; set; }
        public string FixedPriceValue { get { return (Math.Truncate(Math.Round(FixedPrice, 2))).ToString(); } }
        public string FixedPriceDecimal { get { return ((Math.Round(FixedPrice, 2) % 1) * 100).ToString("00"); } }
        public string FixedCurrency { get; set; }
        public bool Checked { get; set; }
        public bool IsCancelCampaign { get; set; }
        public string UserCode { get; set; }
        public string CustomerCode { get; set; }
        public string ProductCode { get; set; }
        public string Notes { get; set; }
        public int PromotionId { get; set; }
        public int LogId { get; set; }
        public bool LineInBasket { get; set; }
        public bool CampaignLineInBasket { get; set; }
        public string RowCss { get; set; }
        public string RowCmpText { get; set; }
        //public string DiscountStr
        //{
        //    get
        //    {
        //        string disc = string.Empty;
        //        if ((Product.Rule == null || Product.Rule.Disc1 == 0) && Product.Campaign.Type == 0)
        //            return "-";
        //        if (Product.Rule.Disc1 != 0)
        //            disc += Product.Rule.Disc1.ToString() + "+";
        //        if (Product.Rule.Disc2 != 0)
        //            disc += Product.Rule.Disc2.ToString() + "+";
        //        if (Product.Rule.Disc3 != 0)
        //            disc += Product.Rule.Disc2.ToString() + "+";
        //        if (Product.Rule.Disc4 != 0)
        //            disc += Product.Rule.Disc2.ToString() + "+";
        //        if (Product.Campaign.Discount != 0 && Quantity >= Product.Campaign.MinOrder)
        //            disc += Product.Campaign.Discount.ToString() + "+";

        //        return disc == string.Empty ? "-" :  "% " + disc.Substring(0, disc.Length - 1);

        //    }
        //}

        public double TotalPrice
        {
            get { return Product.PriceValue * Quantity; }
        }

        public double TotalDiscount
        {
            get { return TotalPrice - TotalCost; }
        }

        public double TotalCost
        {
            get { return Product.Cost * Quantity; }
        }

        public double TotalCostAvailable
        {
            get { return Product.Cost * (Quantity > Product.TotalQuantity ? Product.TotalQuantity : Quantity); }
        }
        public double TotalCostWithVATAvailableCustomer
        {
            get { return (TotalCostAvailable + TotalVATAvailable) * Product.PriceCurrencyRate; }
        }
        public double TotalVAT
        {
            get { return TotalCost * (Product.VatRate / 100); }
        }

        public double TotalVATAvailable
        {
            get { return TotalCostAvailable * (Product.VatRate / 100); }
        }

        public double TotalCostWithVAT
        {
            get { return TotalCost + TotalVAT; }
        }

        public double TotalCostWithVATAvailable
        {
            get { return TotalCostAvailable + TotalVATAvailable; }
        }

        public string PriceTotalStr
        {
            get { return (new Price(TotalPrice * Product.PriceCurrencyRate, Product.CustomerCurrency)).ToString(); }
        }

        public string TotalCostStr
        {
            get { return (new Price(TotalCost * Product.PriceCurrencyRate, Product.CustomerCurrency)).ToString(); }
        }

        public string PriceCostWithVatStr
        {
            get { return (new Price(TotalCostWithVAT * Product.PriceCurrencyRate, Product.CustomerCurrency)).ToString(); }
        }
        #endregion

        #region Methods
        public static List<Basket> GetAvailableInBasket(Customer customer, LoginType loginType, int productId, int customerId, int addType, int userId)
        {
            List<Basket> list = new List<Basket>();
            DataTable dt = DAL.GetAvailableInBasket(productId, customerId, addType, userId);

            foreach (DataRow row in dt.Rows)
            {
                Basket item = Basket.GetBasketById(customer, loginType, row.Field<int>("Id"));
                list.Add(item);
            }

            return list;
        }


        public static bool CustomerWaitingInBasket(int customerId, int userId)
        {

            DataTable dt = DAL.CustomerWaitingInBasket(customerId, userId);

            return dt.Rows.Count > 0;
        }

        public static Basket GetBasketById(Customer customer, LoginType loginType, int pId)
        {
            Basket list = new Basket();
            DataTable dt = DAL.GetBasketById(customer.Id, customer.Users.Id, (int)loginType, pId);

            foreach (DataRow row in dt.Rows)
            {
                Basket item = new Basket();
                {
                    item.Id = row.Field<int>("Id");
                    item.DiscSpecial = row.Field<double>("DiscSpecial");
                    item.Quantity = row.Field<double>("Quantity");
                    item.QuantityOrj = row.Field<double>("Quantity");
                    item.OrderId = row.Field<int>("OrderId");
                    item.Checked = row.Field<bool>("Checked");
                    item.Notes = row.Field<string>("Notes");
                    item.FixedPrice = row.Field<double>("FixedPrice");
                    item.IsFixedPrice = row.Field<bool>("IsFixedPrice");
                    item.FixedCurrency = row.Field<string>("FixedCurrency");
                    item.FixedCurrencyRate = row.Field<double>("FixedCurrencyRate");
                    item.RecordDate = row.Field<DateTime>("RecordDate");
                    item.IsCancelCampaign = row.Field<bool>("IsCancelCampaign");
                    item.DeliveryDate = row.Field<DateTime?>("DeliveryDate");
                    item.AddType = row.Field<int>("AddType");
                    item.Product = new Product();
                    {
                        item.Product.CustomerCurrency = customer.CurrencyType;
                        item.Product.Customer = customer;
                        item.Product.LoginType = loginType;
                        item.Product.Id = row.Field<int>("ProductId");
                        item.Product.GroupId = row.Field<int>("GroupId");
                        item.Product.Code = row.Field<string>("Code");
                        item.Product.Name = row.Field<string>("Name");
                        item.Product.Manufacturer = row.Field<string>("Manufacturer");
                        item.Product.ProductGroup1 = row.Field<string>("ProductGroup1");
                        item.Product.RuleCode = row.Field<string>("RuleCode");
                        item.Product.SpecialCode1 = row.Field<string>("SpecialCode1");
                        item.Product.ManufacturerCode = NullControl(row.Field<string>("ManufacturerCode"));
                        item.Product.Unit = row.Field<string>("Unit");
                        item.Product.TotalQuantity = row.Field<double>("TotalQuantity");
                        item.Product.TotalQuantityOnWay = row.Field<double>("TotalQuantityOnWay");
                        item.Product.WarehouseCount = row.Field<int>("WarehouseCount");
                        item.Product.NewProduct = row.Field<bool>("NewProduct");
                        item.Product.HavePicture = row.Field<bool>("HavePicture");
                        item.Product.VatRate = row.Field<double>("VatRate");
                        item.Product.MinOrder = row.Field<double>("MinOrder");
                        item.Product.Price = row.Field<double>("Price");
                        item.Product.PriceCurrency = row.Field<string>("PriceCurrency");
                        item.Product.CriticalLevel = row.Field<double>("CriticalLevel");
                        item.Product.QuantityType = row.Field<int>("QuantityType");
                        item.Product.IsPackIncrease = row.Field<bool>("IsPackIncrease");
                        item.Product.PriceCurrencyRate = row.Field<double>("PriceCurrencyRate");
                        item.Product.Notes = row.Field<string>("Notes");
                        item.Product.Width = row.Field<double>("Width");
                        item.Product.Height = row.Field<double>("Height");
                        item.Product.Lenght = row.Field<double>("Lenght");
                        item.Product.GrossWeight = row.Field<double>("GrossWeight");
                        item.Product.NetWeight = row.Field<double>("NetWeight");
                        item.Product.PicturePath = row.Field<string>("PicturePath") == string.Empty ? "../Content/images/nophoto.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath");
                        item.Product.MinSpecialDiscount = row.Field<double>("MinSpecialDiscount");
                        item.Product.MaxSpecialDiscount = row.Field<double>("MaxSpecialDiscount");
                        item.Product.Rule = new Rule()
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
                        item.Product.Campaign = new Campaign()
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
                            PromotionAllProduct = Convert.ToBoolean(row["PromotionAllProduct"]),
                        };
                        if (row.Field<string>("CustomerRuleAdditional") != "Null")
                        {
                            string[] splitData = row.Field<string>("CustomerRuleAdditional").Split('-');

                            item.Product.RuleAdditional = new RuleAdditional()
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
                }

                list = (item);
            }
            return list;
        }



        public static List<Basket> GetBasketList(Customer customer, LoginType loginType, int salesmanId, int basketType)
        {
            List<Basket> list = new List<Basket>();
            DataTable dt = DAL.GetBasketList(customer.Id, customer.Users.Id, (int)loginType, salesmanId, basketType);

            foreach (DataRow row in dt.Rows)
            {
                Basket item = new Basket();
                {
                    item.Id = row.Field<int>("Id");
                    item.DiscSpecial = row.Field<double>("DiscSpecial");
                    item.Quantity = row.Field<double>("Quantity");
                    item.QuantityOrj = row.Field<double>("Quantity");
                    item.OrderId = row.Field<int>("OrderId");
                    item.Checked = row.Field<bool>("Checked");
                    item.Notes = row.Field<string>("Notes");
                    item.FixedPrice = row.Field<double>("FixedPrice");
                    item.IsFixedPrice = row.Field<bool>("IsFixedPrice");
                    item.FixedCurrency = row.Field<string>("FixedCurrency");
                    item.FixedCurrencyRate = row.Field<double>("FixedCurrencyRate");
                    item.RecordDate = row.Field<DateTime>("RecordDate");
                    item.IsCancelCampaign = row.Field<bool>("IsCancelCampaign");
                    item.DeliveryDate = row.Field<DateTime?>("DeliveryDate");
                    item.AddType = row.Field<int>("AddType");
                    item.Product = new Product();
                    {
                        item.Product.CustomerCurrency = customer.CurrencyType;
                        item.Product.Customer = customer;
                        item.Product.LoginType = loginType;
                        item.Product.Id = row.Field<int>("ProductId");
                        item.Product.GroupId = row.Field<int>("GroupId");
                        item.Product.Code = row.Field<string>("Code");
                        item.Product.Name = row.Field<string>("Name");
                        item.Product.Manufacturer = row.Field<string>("Manufacturer");
                        item.Product.ProductGroup1 = row.Field<string>("ProductGroup1");
                        item.Product.RuleCode = row.Field<string>("RuleCode");
                        item.Product.SpecialCode1 = row.Field<string>("SpecialCode1");
                        item.Product.ManufacturerCode = NullControl(row.Field<string>("ManufacturerCode"));
                        item.Product.Unit = row.Field<string>("Unit");
                        item.Product.TotalQuantity = row.Field<double>("TotalQuantity");
                        item.Product.TotalQuantityOnWay = row.Field<double>("TotalQuantityOnWay");
                        item.Product.WarehouseCount = row.Field<int>("WarehouseCount");
                        item.Product.NewProduct = row.Field<bool>("NewProduct");
                        item.Product.HavePicture = row.Field<bool>("HavePicture");
                        item.Product.VatRate = row.Field<double>("VatRate");
                        item.Product.MinOrder = row.Field<double>("MinOrder");
                        item.Product.Price = row.Field<double>("Price");
                        item.Product.PriceCurrency = row.Field<string>("PriceCurrency");
                        item.Product.CriticalLevel = row.Field<double>("CriticalLevel");
                        item.Product.QuantityType = row.Field<int>("QuantityType");
                        item.Product.IsPackIncrease = row.Field<bool>("IsPackIncrease");
                        item.Product.PriceCurrencyRate = row.Field<double>("PriceCurrencyRate");
                        item.Product.Notes = row.Field<string>("Notes");
                        item.Product.Width = row.Field<double>("Width");
                        item.Product.Height = row.Field<double>("Height");
                        item.Product.Lenght = row.Field<double>("Lenght");
                        item.Product.GrossWeight = row.Field<double>("GrossWeight");
                        item.Product.NetWeight = row.Field<double>("NetWeight");
                        item.Product.PicturePath = row.Field<string>("PicturePath") == string.Empty ? "../Content/images/nophoto.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath");
                        item.Product.MinSpecialDiscount = row.Field<double>("MinSpecialDiscount");
                        item.Product.MaxSpecialDiscount = row.Field<double>("MaxSpecialDiscount");
                        item.Product.Rule = new Rule()
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
                        item.Product.Campaign = new Campaign()
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
                            PromotionAllProduct = Convert.ToBoolean(row["PromotionAllProduct"]),
                        };
                        if (row.Field<string>("CustomerRuleAdditional") != "Null")
                        {
                            string[] splitData = row.Field<string>("CustomerRuleAdditional").Split('-');

                            item.Product.RuleAdditional = new RuleAdditional()
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

                        item.Product.IsKitMain = row.Field<bool>("IsKitMain");
                    };
                }

                list.Add(item);
            }
            return list;
        }


        public bool Update()
        {
            return DAL.UpdateBasket(Id, Quantity, DiscSpecial, Status, OrderId, Checked, EditId, Notes, IsFixedPrice, FixedPrice, FixedCurrency, IsCancelCampaign, AddType);
        }
        public bool Delete()
        {
            return DAL.UpdateBasket(Id, Quantity, DiscSpecial, 2, OrderId, Checked, EditId, Notes, IsFixedPrice, FixedPrice, FixedCurrency, IsCancelCampaign, AddType);
        }
        public void Add()
        {
            DAL.InsertBasket(Product.Id, CustomerId, SalesmanId, Quantity, DiscSpecial, RecordDate, AddType, ClientNumber, ProgramType, UserId, UserCode, CustomerCode, ProductCode, LogId);
        }

        public static bool UpdateBasketWithOrder(string ids, int orderId, int pStatus)
        {
            return DAL.UpdateBasketWithOrderByIds(ids, orderId, pStatus);
        }

        public static bool UpdateDeliveryDate(int id, DateTime deliveryDate)
        {
            return DAL.UpdateBasketWithDeliveryDate(id, deliveryDate);
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }

    public class CartGiveOrderNoQty
    {
        public int BasketId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public double QtyReq { get; set; }
        public string QtyReqStr { get { return (LoginType == LoginType.Salesman ? QtyReq.ToString() : "-"); } }
        public double QtyAvbl { get; set; }
        public string QtyAvblStr { get { return (LoginType == LoginType.Salesman ? QtyAvbl.ToString() : "-"); } }
        public int Status { get; set; }
        public bool IsCampaign { get; set; }
        public LoginType LoginType { get; set; }

        public CartGiveOrderNoQty(int basketId, string code, string name, double qtyReq, double qtyAvbl, LoginType loginType, bool campaign)
        {
            BasketId = basketId;
            Code = code;
            Name = name;
            QtyReq = qtyReq;
            QtyAvbl = qtyAvbl;
            Status = 0;
            LoginType = loginType;
            IsCampaign = campaign;
        }
    }

    public partial class DataAccessLayer
    {
        public DataTable GetBasketById(int pCustomerId, int pUserId, int pLoginType, int pId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_BasketById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId, pLoginType, pId });
        }
        public bool UpdateBasketWithOrderByIds(string pIds, int pOrderId, int pStatus)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_BasketWithOrderByIds", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pIds, pOrderId, pStatus });
        }
        public bool UpdateBasketWithDeliveryDate(int pId, DateTime pDeliveryDate)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_BasketWithDeliveryDate", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pDeliveryDate });
        }
        public DataTable GetBasketList(int pCustomerId, int pUserId, int pLoginType, int pSalesmanId, int pBasketType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_BasketByCustomer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId, pLoginType, pSalesmanId, pBasketType });
        }
        public DataTable GetAvailableInBasket(int pProductId, int pCustomerId, int pAddType, int pUserId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_AvailableInBasket", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId, pCustomerId, pAddType, pUserId });
        }
        public DataTable CustomerWaitingInBasket(int pCustomerId, int pUserId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Getlist_CustomerWaitingInBasket", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId });
        }
        public bool UpdateBasket(int pId, double pQuantity, double pDiscSpecial, int pStatus, int pOrderId, bool pChecked, int pEditId, string pNotes, bool pIsFixedPrice, double pFixedPrice, string pFixedCurrency, bool pIsCancelCampaign, int pAddType)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_Basket", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pQuantity, pDiscSpecial, pStatus, pOrderId, pChecked, pEditId, pNotes, pIsFixedPrice, pFixedPrice, pFixedCurrency, pIsCancelCampaign, pAddType });
        }
        public void InsertBasket(int pProductId, int pCustomerId, int pSalesmanId, double pQuantity, double pDiscSpecial, DateTime pRecordDate, int pAddType, int pClientNumber, int pProgramType, int pUserId, string pUserCode, string pCustomerCode, string pProductCode, int pLogId)
        {
            DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_Basket", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId, pCustomerId, pSalesmanId, pQuantity, pDiscSpecial, pRecordDate, pAddType, pClientNumber, pProgramType, pUserId, pUserCode, pCustomerCode, pProductCode, pLogId });
        }
    }
}