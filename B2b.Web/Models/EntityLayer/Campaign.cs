using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class Campaign : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public int HeaderId { get; set; }
        public string Code { get; set; }
        public string Code2 { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductManufacturer { get; set; }
        public CampaignType Type { get; set; }
        public string TypeStr
        {
            get
            {
                switch (Type)
                {
                    case CampaignType.Discount:
                        return "İskonto Kampanyası";
                    case CampaignType.GradualDiscount:
                        return "Kademeli İskonto Kampanyası";
                    case CampaignType.GradualNetPrice:
                        return "Kademeli Net Fiyat Kampanyası";
                    case CampaignType.GrossPrice:
                        return "Bürüt Fiyat Kampanyası";
                    case CampaignType.NetPrice:
                        return "Net Fiyat Kampanyası";
                    case CampaignType.PromotionProduct:
                        return "Promosyon Ürün Kampanyası";
                    default:
                        return "-";

                }

            }
        }
        public double Price { get; set; }
        public double PriceP { get; set; }
        public double PriceV { get; set; }
        public string Currency { get; set; }
        public string CurrencyP { get; set; }
        public string CurrencyV { get; set; }
        public double CurrencyRate { get; set; }
        public double Discount { get; set; }
        public double MinOrder { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public double TotalQuantity { get; set; }
        public double EndQuantity { get; set; }

        public double SaledQuantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string PromotionProductCode { get; set; }
        public double PromotionProductQuantity { get; set; }
        public bool PromotionAllProduct { get; set; }
        public bool Picture { get; set; }
        public string PicturePath { get; set; }
        public bool DiscountPassive { get; set; }
        public bool IsActive { get; set; }
        public bool BannerStatu { get; set; }
        public string PriceStr { get { return ((PriceP == 0 ? PriceV : PriceP) + (CurrencyP == "" ? CurrencyV : CurrencyP)); } }
        public string Message { get; set; }
        public bool Checked { get; set; }
        #endregion

        public Campaign()
        {
            FinishDate = DateTime.Now;
            Code = string.Empty;
        }

        #region Methods
        public static List<Campaign> GetCampaigHeaderByType(int type, string t9Text)
        {
            List<Campaign> list = new List<Campaign>();
            DataTable dt = DAL.GetCampaigHeaderByType(type, t9Text);

            foreach (DataRow row in dt.Rows)
            {
                Campaign campaign = new Campaign()
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Code2 = row.Field<string>("Code2"),

                    ProductId = row.Field<int>("ProductId"),
                    Type = (CampaignType)Convert.ToInt32(row.Field<int>("Type")),
                    PriceP = row.Field<double>("PriceP"),
                    CurrencyP = row.Field<string>("CurrencyP"),
                    PriceV = row.Field<double>("PriceV"),
                    CurrencyV = row.Field<string>("CurrencyV"),
                    Discount = row.Field<double>("Discount"),
                    MinOrder = row.Field<double>("MinOrder"),
                    TotalQuantity = row.Field<double>("TotalQuantity"),
                    EndQuantity = row.Field<double>("EndQuantity"),
                    SaledQuantity = row.Field<double>("SaledQuantity"),
                    PromotionProductCode = row.Field<string>("PromotionProductCode"),
                    PromotionProductQuantity = row.Field<double>("PromotionProductQuantity"),
                    PromotionAllProduct = row.Field<bool>("PromotionAllProduct"),
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductName = row.Field<string>("ProductName"),
                    ProductManufacturer = row.Field<string>("ProductManufacturer"),
                    IsActive = row.Field<bool>("IsActive"),
                    BannerStatu = row.Field<bool>("BannerStatu"),
                    DiscountPassive = row.Field<bool>("DiscountPassive"),
                    Notes = row.Field<string>("Notes"),
                    StartDate = row.Field<DateTime>("StartDate"),
                    FinishDate = row.Field<DateTime>("FinishDate")
                };
                list.Add(campaign);
            }

            return list;
        }

        public static List<Product> GetCampaigPromotionProduct(string code)
        {
            List<Product> list = new List<Product>();
            DataTable dt = DAL.GetCampaigPromotionProduct(code);

            foreach (DataRow row in dt.Rows)
            {
                Product campaimgProduct = new Product()
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Manufacturer = row.Field<string>("Manufacturer"),
                    Name = row.Field<string>("Name"),
                };

                list.Add(campaimgProduct);
            }

            return list;
        }
        public static List<Campaign> GetCampaigDetail(int headerId)
        {
            List<Campaign> list = new List<Campaign>();
            DataTable dt = DAL.GetCampaignDetail(headerId);

            foreach (DataRow row in dt.Rows)
            {
                Campaign campaign = new Campaign()
                {

                    Code = row.Field<string>("Code"),
                    Id = row.Field<int>("Id"),
                    HeaderId = row.Field<int>("HeaderId"),
                    CurrencyV = row.Field<string>("CurrencyV"),
                    CurrencyP = row.Field<string>("CurrencyP"),
                    PriceP = row.Field<double>("PriceP"),
                    PriceV = row.Field<double>("PriceV"),
                    Discount = row.Field<double>("Discount"),
                    MinOrder = row.Field<double>("MinOrder")

                };
                list.Add(campaign);
            }

            return list;
        }


        public int Add()
        {
            DataTable dt = DAL.InsertCampaign(ProductId, Code, Code2, (int)Type, PriceP, PriceV, CurrencyP, CurrencyV, Discount, MinOrder, TotalQuantity, EndQuantity, PromotionProductCode, PromotionProductQuantity, PromotionAllProduct, BannerStatu, DiscountPassive, Notes, StartDate, FinishDate.AddDays(1).AddMinutes(-1), CreateId);
            return Convert.ToInt32(dt.Rows[0][0]);
        }
        public bool AddDetail()
        {
            return DAL.InsertCampaignDetail(HeaderId, Code, (int)Type, PriceP, PriceV, CurrencyP, CurrencyV, Discount, MinOrder, StartDate, FinishDate, CreateId);
        }


        public bool Update()
        {
            return DAL.UpdateCampaign(Id, ProductId, Code, Code2, (int)Type, PriceP, PriceV, CurrencyP, CurrencyV, Discount, MinOrder, TotalQuantity, EndQuantity, PromotionProductCode, PromotionProductQuantity, PromotionAllProduct, BannerStatu, DiscountPassive, Notes, StartDate, FinishDate.AddDays(1).AddMinutes(-1), EditId);
        }

        public static bool Delete(string DeleteIds, int DeleteId)
        {
            return DAL.DeleteCampaign(DeleteIds, DeleteId);
        }

        public static bool DeleteleDetail(int id, int headerId, int editId)
        {
            return DAL.DeleteCampaignDetail(id, headerId);
        }
        public static List<Campaign> GetListByHeaderId(int pHeaderId, Customer customer)
        {
            List<Campaign> list = new List<Campaign>();
            DataTable dt = DAL.GetCampignListByHeaderId(pHeaderId, customer.Id);

            foreach (DataRow row in dt.Rows)
            {
                Campaign campaign = new Campaign()
                {
                    Code = row.Field<string>("Code"),
                    Type = (CampaignType)Convert.ToInt32(row.Field<long>("CampaignType")),
                    Price = row.Field<double>("CampaignPriceP"),
                    PriceP = row.Field<double>("CampaignPriceP"),
                    PriceV = row.Field<double>("CampaignPriceV"),
                    Currency = row.Field<string>("CampaignCurrencyV"),
                    CurrencyP = row.Field<string>("CampaignCurrencyP"),
                    CurrencyV = row.Field<string>("CampaignCurrencyV"),
                    CurrencyRate = row.Field<double>("CampaignCurrencyRate"),
                    Discount = row.Field<double>("Discount"),
                    MinOrder = row.Field<double>("MinOrder"),
                    TotalQuantity = row.Field<double>("TotalQuantity"),
                    SaledQuantity = row.Field<double>("SaledQuantity"),
                    PromotionProductCode = row.Field<string>("PromotionProductCode"),
                    PromotionProductQuantity = row.Field<double>("PromotionProductQuantity"),
                };
                list.Add(campaign);
            }

            return list;
        }
        #endregion
    }
    public partial class DataAccessLayer
    {
        public DataTable InsertCampaign(int pProductId, string pCode, string pCode2, int pType, double pPriceP, double pPriceV, string pCurrencyP, string pCurrencyV, double pDiscount, double pMinOrder, double pTotalQuantity, double pEndQuantity, string pPromotionProductCode, double pPromotionProductQuantity, bool pPromotionAllProduct, bool pBannerStatu, bool pDiscountPassive, string pNotes, DateTime pStartDate, DateTime pFinishDate, int pCreateId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_Insert_Campaign2", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId, pCode, pCode2, pType, pPriceP, pPriceV, pCurrencyP, pCurrencyV, pDiscount, pMinOrder, pTotalQuantity, pEndQuantity, pPromotionProductCode, pPromotionProductQuantity, pPromotionAllProduct, pBannerStatu, pDiscountPassive, pNotes, pStartDate, pFinishDate, pCreateId });
        }


        public bool InsertCampaignDetail(int pHeaderId, string pCode, int pType, double pPriceP, double pPriceV, string pCurrencyP, string pCurrencyV, double pDiscount, double pMinOrder, DateTime pStartDate, DateTime pFinishDate, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Campaign_Detail_2", MethodBase.GetCurrentMethod().GetParameters(), new object[]
            {
                pHeaderId,pCode,pType,pPriceP,pPriceV,
                pCurrencyP,pCurrencyV,pDiscount,pMinOrder,pStartDate,pFinishDate,pCreateId
            });

        }

        public bool UpdateCampaign(int pId, int pProductId, string pCode, string pCode2, int pType, double pPriceP, double pPriceV, string pCurrencyP, string pCurrencyV, double pDiscount, double pMinOrder, double pTotalQuantity, double pEndQuantity, string pPromotionProductCode, double pPromotionProductQuantity, bool pPromotionAllProduct, bool pBannerStatu, bool pDiscountPassive, string pNotes, DateTime pStartDate, DateTime pFinishDate, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Campaign2", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pProductId, pCode, pCode2, pType, pPriceP, pPriceV, pCurrencyP, pCurrencyV, pDiscount, pMinOrder, pTotalQuantity, pEndQuantity, pPromotionProductCode, pPromotionProductQuantity, pPromotionAllProduct, pBannerStatu, pDiscountPassive, pNotes, pStartDate, pFinishDate, pEditId });
        }
        public bool DeleteCampaign(string pDeleteIds, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_Campaign", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pDeleteIds, pEditId });
        }
        public bool DeleteCampaignDetail(int pId, int pHeaderId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_CampaignDetail", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pHeaderId });
        }

        public DataTable GetCampaigHeaderByType(int pType, string pT9Text)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_CampaignHeader", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pType, pT9Text });
        }

        public DataTable GetCampaigPromotionProduct(string pCodes)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_CampaignPromotionProduct", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCodes });
        }
        public DataTable GetCampaignDetail(int pHeaderId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_CampaignDetail", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pHeaderId });
        }
        public DataTable GetCampignListByHeaderId(int pHeaderId, int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_CampaignDetail", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pHeaderId, pCustomerId });
        }

    }

    public enum CampaignType
    {
        None = 0,
        NetPrice = 1,
        GrossPrice = 2,
        Discount = 3,
        GradualNetPrice = 4,
        GradualDiscount = 5,
        PromotionProduct = 6
    }
}