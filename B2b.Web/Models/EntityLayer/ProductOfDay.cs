using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class ProductOfDayCs : DataAccess
    {
        public ProductOfDayCs()
        {
            Customer = new Customer();
        }

        #region Properties
        public int Id { get; set; }
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public double MinOrder { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public string PriceStr { get { return (Price.ToString("N2") + " " + Currency); } }
        public double CurrencyRate { get; set; }
        public string Explanation { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public bool IsActive { get; set; }
        public string PicturePath { get; set; }
        public double TotalQuantity { get; set; }
        public double SaledQuantity { get; set; }
        public double RisingQuantity { get; set; }
        public bool IsUseProductPicture { get; set; }
        public string PicturePathShow { get { return (GlobalSettings.FtpServerAddressFull + PicturePath); } }
        public DateTime SelectTime { get; set; }
        public double Quantity { get; set; }
        public bool SendDisabled { get; set; }
        public bool IsOnlyOneOrder { get; set; }
        public Customer Customer { get; set; }

        public Price PriceCustomer { get { return new Price(Price, Customer.CurrencyType, CurrencyRate); } }
        public Price PriceWithWatCustomer { get { return new Price((Price * (1 + Product.VatRate / 100)), Customer.CurrencyType, CurrencyRate); } }
        public double NetTotal { get { return (PriceCustomer.ValueFinal * Quantity); } }
        public double GeneralTotal { get { return (PriceWithWatCustomer.ValueFinal * Quantity); } }

        #endregion

        #region Methods

        public static List<ProductOfDayCs> GetproductOfDay(string t9Text)
        {
            List<ProductOfDayCs> list = new List<ProductOfDayCs>();
            DataTable dt = DAL.GetproductOfDay(t9Text);

            foreach (DataRow row in dt.Rows)
            {

                ProductOfDayCs campaign = new ProductOfDayCs()
                {
                    Customer = new Customer(),
                    Id = row.Field<int>("Id"),
                    ProductId = row.Field<int>("ProductId"),
                    Product = new Product()
                    {
                        Id = row.Field<int>("ProductId"),
                        Code = row.Field<string>("ProductCode"),
                        Name = row.Field<string>("ProductName"),
                        Manufacturer = row.Field<string>("Manufacturer"),
                        ManufacturerCode = row.Field<string>("ManufacturerCode"),
                    },
                    IsOnlyOneOrder = row.Field<bool>("IsOnlyOneOrder"),
                    Price = row.Field<double>("Price"),
                    Currency = row.Field<string>("Currency"),
                    MinOrder = row.Field<double>("MinOrder"),
                    Explanation = row.Field<string>("Explanation"),
                    StartDate = row.Field<DateTime>("StartDate"),
                    FinishDate = row.Field<DateTime>("FinishDate"),
                    IsActive = row.Field<bool>("IsActive"),
                    PicturePath = row.Field<string>("PicturePath") == string.Empty ? "nophoto.png" : row.Field<string>("PicturePath"),
                    TotalQuantity = row.Field<double>("TotalQuantity"),
                    SaledQuantity = row.Field<double>("SaledQuantity"),
                    RisingQuantity = row.Field<double>("RisingQuantity"),
                    IsUseProductPicture = row.Field<bool>("IsUseProductPicture")
                };
                list.Add(campaign);
            }

            return list;
        }

        public static List<ProductOfDayCs> GetProductOfDayByCustomer(Customer customer, LoginType loginType)
        {
            List<ProductOfDayCs> list = new List<ProductOfDayCs>();
            DataTable dt = DAL.GetProductOfDayByCustomer(customer.Id, customer.Users.Id, (int)loginType);

            foreach (DataRow row in dt.Rows)
            {
                ProductOfDayCs campaign = new ProductOfDayCs()
                {
                    Customer = customer,
                    SelectTime = DateTime.Now,
                    Id = row.Field<int>("Id"),
                    ProductId = row.Field<int>("ProductId"),
                    Product = new Product()
                    {
                        Id = row.Field<int>("ProductId"),
                        Code = row.Field<string>("ProductCode"),
                        Name = row.Field<string>("ProductName"),
                        Manufacturer = row.Field<string>("Manufacturer"),
                        ManufacturerCode = row.Field<string>("ManufacturerCode"),
                        CustomerCurrency = customer.CurrencyType,
                        PriceCurrencyRate = row.Field<double>("PriceCurrencyRate"),
                        VatRate = row.Field<double>("VatRate"),
                        Price = row.Field<double>("ProductPrice"),
                        PriceCurrency = row.Field<string>("ProductCurrency")
                    },
                    CurrencyRate = row.Field<double>("PriceCurrencyRate"),
                    Price = row.Field<double>("Price"),
                    Currency = row.Field<string>("Currency"),
                    MinOrder = row.Field<double>("MinOrder"),
                    Explanation = row.Field<string>("Explanation"),
                    StartDate = row.Field<DateTime>("StartDate"),
                    FinishDate = row.Field<DateTime>("FinishDate"),
                    IsActive = row.Field<bool>("IsActive"),
                    PicturePath = row.Field<string>("PicturePath") == string.Empty ? "nophoto.png" : row.Field<string>("PicturePath"),
                    TotalQuantity = row.Field<double>("TotalQuantity"),
                    SaledQuantity = row.Field<double>("SaledQuantity"),
                    RisingQuantity = row.Field<double>("RisingQuantity"),
                    IsUseProductPicture = row.Field<bool>("IsUseProductPicture"),
                    Quantity = row.Field<double>("MinOrder")
                };
                list.Add(campaign);
            }

            return list;
        }

        public bool Add()
        {
            return DAL.InsertProductOfDay(ProductId, MinOrder, Price, Currency, Explanation, StartDate, FinishDate, PicturePath, TotalQuantity, RisingQuantity, IsUseProductPicture, IsOnlyOneOrder, CreateId);
        }

        public bool Update()
        {
            return DAL.UpdateProductOfDay(Id, ProductId, MinOrder, Price, Currency, Explanation, StartDate, FinishDate, PicturePath, TotalQuantity, RisingQuantity, IsUseProductPicture, IsActive, IsOnlyOneOrder, EditId);
        }

        public static bool Delete(string DeleteIds, int DeleteId)
        {
            return DAL.DeleteProductOfDay(DeleteIds, DeleteId);
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetproductOfDay(string pT9Text)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_ProductOfDay", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pT9Text });
        }

        public DataTable GetProductOfDayByCustomer(int pCustomerId, int pUserId, int pLoginType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ProductOfDayList", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId, pLoginType });
        }

        public bool InsertProductOfDay(int pProductId, double pMinOrder, double pPrice, string pCurrency, string pExplanation, DateTime pStartDate, DateTime pFinishDate, string pPicturePath, double pTotalQuantity, double pRisingQuantity, bool pIsUseProductPicture, bool pIsOnlyOneOrder, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_ProductOfDay", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId, pMinOrder, pPrice, pCurrency, pExplanation, pStartDate, pFinishDate, pPicturePath, pTotalQuantity, pRisingQuantity, pIsUseProductPicture, pIsOnlyOneOrder, pCreateId });
        }

        public bool UpdateProductOfDay(int pId, int pProductId, double pMinOrder, double pPrice, string pCurrency, string pExplanation, DateTime pStartDate, DateTime pFinishDate, string pPicturePath, double pTotalQuantity, double pRisingQuantity, bool pIsUseProductPicture, bool pIsActive, bool pIsOnlyOneOrder, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_ProductOfDay", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pProductId, pMinOrder, pPrice, pCurrency, pExplanation, pStartDate, pFinishDate, pPicturePath, pTotalQuantity, pRisingQuantity, pIsUseProductPicture, pIsActive, pIsOnlyOneOrder, pEditId });
        }

        public bool DeleteProductOfDay(string pDeleteIds, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_ProductOfDay", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pDeleteIds, pEditId });
        }
    }

}