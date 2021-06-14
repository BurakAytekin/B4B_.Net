using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class CouponCs : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Header { get; set; }
        public string Code { get; set; }
        public double Price { get; set; }
        public string PriceLeft { get { return (Math.Truncate(Math.Round(Price, 2))).ToString(); } }
        public string PriceRight { get { return ((Math.Round(Price, 2) % 1) * 100).ToString("00"); } }
        public string Currency { get; set; }
        public double Discount { get; set; }
        public double Discount1 { get; set; }
        public double Discount2 { get; set; }
        public int Type { get; set; }
        public bool IsIgnoreCampaign { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Manufacturers { get; set; }
        public string ProductGroups { get; set; }
        public double MinPrice { get; set; }
        public string Explanation1 { get; set; }
        public string Explanation2 { get; set; }
        public bool IsOneUsed { get; set; }
        public bool IsAutoUse { get; set; }
        public int CalculateType { get; set; }
        public double MinQuantity { get; set; }
        public bool IsCancelMainDisc { get; set; }
        public bool IsCancelAdditionalDisc { get; set; }
        public bool IsCancelManuelDisc { get; set; }
        public string Rules { get; set; }
        public string SpecialCodes { get; set; }
        public string ProductCode { get; set; }
        public int CustomerCount { get; set; }
        public int ActiveCustomerCount { get; set; }
        public int UsedCount { get; set; }
        public int UnUsedCount { get; set; }
        public double CouponTotal { get; set; }
        public bool IsUsedForCustomer { get; set; }
        public Product Product { get; set; }
        public string CssClassAdmin { get { return ((EndDate < DateTime.Now || IsActive == false) ? "default" : (StartDate < DateTime.Now ? "success" : "danger")); } }
        public bool IsOnlySelectedItemTotal { get; set; }
        public int ProductQuantity { get; set; }
        public bool IsActive { get; set; }
        public double UsedDiscountTl { get; set; }
        public bool IsCounter { get; set; }
        public bool IsJustAvailable { get; set; }
        public int Priority { get; set; }
        public int UserType { get; set; }
        #endregion

        #region Methods

        public static CouponCs GetCouponStatistics(int pCuponId)
        {
            CouponCs list = new CouponCs();
            DataTable dt = DAL.GetCouponStatistics(pCuponId);

            foreach (DataRow row in dt.Rows)
            {
                CouponCs obj = new CouponCs()
                {
                    CustomerCount = Convert.ToInt32(row["CustomerCount"]),
                    ActiveCustomerCount = Convert.ToInt32(row["ActiveCustomerCount"]),
                    UsedCount = Convert.ToInt32(row["UsedCount"]),
                    UnUsedCount = Convert.ToInt32(row["UnUsedCount"]),
                    CouponTotal = Convert.ToDouble(row["CouponTotal"]),
                };
                list = obj;
            }
            return list;
        }

        public static List<CouponCs> GetBasketAutoCoupon(int pCustomerId)
        {
            List<CouponCs> list = new List<CouponCs>();
            DataTable dt = DAL.GetBasketAutoCoupon(pCustomerId);

            foreach (DataRow row in dt.Rows)
            {
                CouponCs obj = new CouponCs()
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Header = row.Field<string>("Header"),
                    Discount = row.Field<double>("Discount"),
                    Discount1 = row.Field<double>("Discount1"),
                    Discount2 = row.Field<double>("Discount2"),
                    Currency = row.Field<string>("Currency"),
                    EndDate = row.Field<DateTime>("EndDate"),
                    Explanation1 = row.Field<string>("Explanation1"),
                    Explanation2 = row.Field<string>("Explanation2"),
                    IsIgnoreCampaign = row.Field<bool>("IsIgnoreCampaign"),
                    IsJustAvailable = row.Field<bool>("IsJustAvailable"),
                    IsOneUsed = row.Field<bool>("IsOneUsed"),
                    IsAutoUse = row.Field<bool>("IsAutoUse"),
                    Manufacturers = row.Field<string>("Manufacturers"),
                    MinPrice = row.Field<double>("MinPrice"),
                    Price = row.Field<double>("Price"),
                    ProductGroups = row.Field<string>("ProductGroups"),
                    StartDate = row.Field<DateTime>("StartDate"),
                    Type = row.Field<int>("Type"),
                    IsCancelMainDisc = row.Field<bool>("IsCancelMainDisc"),
                    IsCancelAdditionalDisc = row.Field<bool>("IsCancelAdditionalDisc"),
                    IsCancelManuelDisc = row.Field<bool>("IsCancelManuelDisc"),
                    CalculateType = row.Field<int>("CalculateType"),
                    MinQuantity = row.Field<double>("MinQuantity"),
                    Rules = row.Field<string>("Rules"),
                    SpecialCodes = row.Field<string>("SpecialCodes"),
                    IsOnlySelectedItemTotal = row.Field<bool>("IsOnlySelectedItemTotal"),
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductQuantity = row.Field<int>("ProductQuantity"),
                    IsCounter = row.Field<bool>("IsCounter"),
                    Priority = row.Field<int>("Priority"),
                    UserType = row.Field<int>("UserType")
                };
                list.Add(obj);
            }
            return list;
        }

        public static CouponCs GetCustomerCouponByCode(int pCustomerId, string pCode)
        {
            CouponCs list = new CouponCs();
            DataTable dt = DAL.GetCustomerCouponByCode(pCustomerId, pCode);

            foreach (DataRow row in dt.Rows)
            {
                CouponCs obj = new CouponCs()
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Header = row.Field<string>("Header"),
                    Discount = row.Field<double>("Discount"),
                    Discount1 = row.Field<double>("Discount1"),
                    Discount2 = row.Field<double>("Discount2"),
                    Currency = row.Field<string>("Currency"),
                    EndDate = row.Field<DateTime>("EndDate"),
                    Explanation1 = row.Field<string>("Explanation1"),
                    Explanation2 = row.Field<string>("Explanation2"),
                    IsIgnoreCampaign = row.Field<bool>("IsIgnoreCampaign"),
                    IsJustAvailable = row.Field<bool>("IsJustAvailable"),
                    IsOneUsed = row.Field<bool>("IsOneUsed"),
                    IsAutoUse = row.Field<bool>("IsAutoUse"),
                    Manufacturers = row.Field<string>("Manufacturers"),
                    MinPrice = row.Field<double>("MinPrice"),
                    Price = row.Field<double>("Price"),
                    ProductGroups = row.Field<string>("ProductGroups"),
                    StartDate = row.Field<DateTime>("StartDate"),
                    Type = row.Field<int>("Type"),
                    IsCancelMainDisc = row.Field<bool>("IsCancelMainDisc"),
                    IsCancelAdditionalDisc = row.Field<bool>("IsCancelAdditionalDisc"),
                    IsCancelManuelDisc = row.Field<bool>("IsCancelManuelDisc"),
                    CalculateType = row.Field<int>("CalculateType"),
                    MinQuantity = row.Field<double>("MinQuantity"),
                    Rules = row.Field<string>("Rules"),
                    SpecialCodes = row.Field<string>("SpecialCodes"),
                    IsOnlySelectedItemTotal = row.Field<bool>("IsOnlySelectedItemTotal"),
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductQuantity = row.Field<int>("ProductQuantity"),
                    IsCounter = row.Field<bool>("IsCounter"),
                    UserType = row.Field<int>("UserType")
                };
                list = (obj);
            }
            return list;
        }


        public static CouponCs GenerateCouponCode()
        {
            CouponCs list = new CouponCs();
            DataTable dt = DAL.GenerateCouponCode();

            foreach (DataRow row in dt.Rows)
            {
                CouponCs obj = new CouponCs()
                {
                    Code = row.Field<string>("Code")
                };
                list = obj;
            }
            return list;
        }
        public static List<CouponCs> GetCouponList()
        {
            List<CouponCs> list = new List<CouponCs>();
            DataTable dt = DAL.GetCouponList();

            foreach (DataRow row in dt.Rows)
            {
                CouponCs obj = new CouponCs()
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Header = row.Field<string>("Header"),
                    Discount = row.Field<double>("Discount"),
                    Discount1 = row.Field<double>("Discount1"),
                    Discount2 = row.Field<double>("Discount2"),
                    Currency = row.Field<string>("Currency"),
                    EndDate = row.Field<DateTime>("EndDate"),
                    Explanation1 = row.Field<string>("Explanation1"),
                    Explanation2 = row.Field<string>("Explanation2"),
                    IsIgnoreCampaign = row.Field<bool>("IsIgnoreCampaign"),
                    IsJustAvailable = row.Field<bool>("IsJustAvailable"),
                    IsOneUsed = row.Field<bool>("IsOneUsed"),
                    IsAutoUse = row.Field<bool>("IsAutoUse"),
                    Manufacturers = row.Field<string>("Manufacturers"),
                    MinPrice = row.Field<double>("MinPrice"),
                    Price = row.Field<double>("Price"),
                    ProductGroups = row.Field<string>("ProductGroups"),
                    StartDate = row.Field<DateTime>("StartDate"),
                    Type = row.Field<int>("Type"),
                    IsCancelMainDisc = row.Field<bool>("IsCancelMainDisc"),
                    IsCancelAdditionalDisc = row.Field<bool>("IsCancelAdditionalDisc"),
                    IsCancelManuelDisc = row.Field<bool>("IsCancelManuelDisc"),
                    CalculateType = row.Field<int>("CalculateType"),
                    MinQuantity = row.Field<double>("MinQuantity"),
                    Rules = row.Field<string>("Rules"),
                    SpecialCodes = row.Field<string>("SpecialCodes"),
                    IsOnlySelectedItemTotal = row.Field<bool>("IsOnlySelectedItemTotal"),
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductQuantity = row.Field<int>("ProductQuantity"),
                    IsActive = row.Field<bool>("IsActive"),
                    IsCounter = row.Field<bool>("IsCounter"),
                    Priority = row.Field<int>("Priority"),
                    UserType = row.Field<int>("UserType")
                };
                list.Add(obj);
            }
            return list;
        }

        public static List<CouponCs> GetCouponListByCustomerId(int customerId, int type)
        {
            List<CouponCs> list = new List<CouponCs>();
            DataTable dt = DAL.GetCouponListByCustomerId(customerId, type);

            foreach (DataRow row in dt.Rows)
            {
                CouponCs obj = new CouponCs()
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Header = row.Field<string>("Header"),
                    Discount = row.Field<double>("Discount"),
                    Discount1 = row.Field<double>("Discount1"),
                    Discount2 = row.Field<double>("Discount2"),
                    Currency = row.Field<string>("Currency"),
                    EndDate = row.Field<DateTime>("EndDate"),
                    Explanation1 = row.Field<string>("Explanation1"),
                    Explanation2 = row.Field<string>("Explanation2"),
                    IsIgnoreCampaign = row.Field<bool>("IsIgnoreCampaign"),
                    IsJustAvailable = row.Field<bool>("IsJustAvailable"),
                    IsOneUsed = row.Field<bool>("IsOneUsed"),
                    IsAutoUse = row.Field<bool>("IsAutoUse"),
                    Manufacturers = row.Field<string>("Manufacturers"),
                    MinPrice = row.Field<double>("MinPrice"),
                    Price = row.Field<double>("Price"),
                    ProductGroups = row.Field<string>("ProductGroups"),
                    StartDate = row.Field<DateTime>("StartDate"),
                    Type = row.Field<int>("Type"),
                    IsCancelMainDisc = row.Field<bool>("IsCancelMainDisc"),
                    IsCancelAdditionalDisc = row.Field<bool>("IsCancelAdditionalDisc"),
                    IsCancelManuelDisc = row.Field<bool>("IsCancelManuelDisc"),
                    CalculateType = row.Field<int>("CalculateType"),
                    MinQuantity = row.Field<double>("MinQuantity"),
                    Rules = row.Field<string>("Rules"),
                    SpecialCodes = row.Field<string>("SpecialCodes"),
                    IsUsedForCustomer = row.Field<bool>("IsUsed"),
                    IsOnlySelectedItemTotal = row.Field<bool>("IsOnlySelectedItemTotal"),
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductQuantity = row.Field<int>("ProductQuantity"),
                    IsCounter = row.Field<bool>("IsCounter"),
                    UserType = row.Field<int>("UserType")
                };
                list.Add(obj);
            }
            return list;
        }


        public bool UpdateCouponPriority()
        {
            return DAL.UpdateCouponPriority(Id, Priority);
        }

        public bool Add()
        {
            return DAL.InsertCoupon(Code, Header, Price, Discount,Discount1,Discount2, Type, IsIgnoreCampaign, IsJustAvailable, StartDate, EndDate, Manufacturers, ProductGroups, MinPrice, Explanation1, Explanation2, IsOneUsed, IsAutoUse, IsCancelMainDisc, IsCancelAdditionalDisc, IsCancelManuelDisc, CalculateType, MinQuantity, Rules, SpecialCodes, IsOnlySelectedItemTotal, ProductCode, ProductQuantity,IsCounter,UserType, CreateId);
        }

        public bool Update()
        {
            return DAL.UpdateCoupon(Id, Code, Header, Price, Discount,Discount1,Discount2, Type, IsIgnoreCampaign, IsJustAvailable, StartDate, EndDate, Manufacturers, ProductGroups, MinPrice, Explanation1, Explanation2, IsOneUsed, IsAutoUse, IsCancelMainDisc, IsCancelAdditionalDisc, IsCancelManuelDisc, CalculateType, MinQuantity, Rules, SpecialCodes, IsOnlySelectedItemTotal, ProductCode, ProductQuantity,IsActive, IsCounter, UserType, EditId, Deleted);
        }
        #endregion
    }

    public partial class DataAccessLayer
    {
        public bool UpdateCouponPriority(int pId, int pPriority)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_CouponPriority", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pPriority });
        }


        public DataTable GenerateCouponCode()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetItem_CouponCode");
        }

        public DataTable GetCouponStatistics(int pCouponId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetItem_CouponStatistics", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCouponId });
        }

        public DataTable GetBasketAutoCoupon(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_BasketAutoCoupon", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }

        public DataTable GetCustomerCouponByCode(int pCustomerId, string pCode)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_CustomerCouponByCode", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pCode });
        }
        public DataTable GetCouponList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_Coupon");
        }

        public DataTable GetCouponListByCustomerId(int pCustomerId, int pType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_CouponByCustomerId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pType });
        }

        public bool UpdateCoupon(int pId, string pCode, string pHeader, double pPrice, double pDiscount,double pDiscount1,double pDiscount2, int pType, bool pIsIgnoreCampaign, bool pIsJustAvailable, DateTime pStartDate, DateTime pEndDate, string pManufacturers, string pProductGroups, double pMinPrice, string pExplanation1, string pExplanation2, bool pIsOneUsed, bool pIsAutoUse, bool pIsCancelMainDisc, bool pIsCancelAdditionalDisc, bool pIsCancelManuelDisc, int pCalculateType, double pMinQuantity, string pRules, string pSpecialCodes, bool pIsOnlySelectedItemTotal, string pProductCode, int pProductQuantity,bool pIsActive,bool pIsCounter,int pUserType, int pEditId, bool pDeleted)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Coupon", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pCode, pHeader, pPrice, pDiscount, pDiscount1, pDiscount2, pType, pIsIgnoreCampaign, pIsJustAvailable, pStartDate, pEndDate, pManufacturers, pProductGroups, pMinPrice, pExplanation1, pExplanation2, pIsOneUsed, pIsAutoUse, pIsCancelMainDisc, pIsCancelAdditionalDisc, pIsCancelManuelDisc, pCalculateType, pMinQuantity, pRules, pSpecialCodes, pIsOnlySelectedItemTotal, pProductCode, pProductQuantity,pIsActive, pIsCounter, pUserType, pEditId, pDeleted });
        }

        public bool InsertCoupon(string pCode, string pHeader, double pPrice, double pDiscount,double pDiscount1,double pDiscount2, int pType, bool pIsIgnoreCampaign,bool pIsJustAvailable, DateTime pStartDate, DateTime pEndDate, string pManufacturers, string pProductGroups, double pMinPrice, string pExplanation1, string pExplanation2, bool pIsOneUsed, bool pIsAutoUse, bool pIsCancelMainDisc, bool pIsCancelAdditionalDisc, bool pIsCancelManuelDisc, int pCalculateType, double pMinQuantity, string pRules, string pSpecialCodes, bool pIsOnlySelectedItemTotal, string pProductCode, int pProductQuantity,bool pIsCounter,int pUserType, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Coupon", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCode, pHeader, pPrice, pDiscount, pDiscount1,pDiscount2, pType, pIsIgnoreCampaign, pIsJustAvailable, pStartDate, pEndDate, pManufacturers, pProductGroups, pMinPrice, pExplanation1, pExplanation2, pIsOneUsed, pIsAutoUse, pIsCancelMainDisc, pIsCancelAdditionalDisc, pIsCancelManuelDisc, pCalculateType, pMinQuantity, pRules, pSpecialCodes, pIsOnlySelectedItemTotal, pProductCode, pProductQuantity, pIsCounter, pUserType, pCreateId });
        }
    }
}