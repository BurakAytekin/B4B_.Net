using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class ReturnForm : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int UserId { get; set; }
        public string FormNo { get; set; }
        public int CustomerId { get; set; }
        public int SalesmanId { get; set; }
        public int AddType { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductManu { get; set; }
        public string ProductManuCode { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public double Quantity { get; set; }
        public string ReturnReason { get; set; }
        public string Explanation { get; set; }
        public double Price { get; set; }
        public string CurrencyType { get; set; }
        public string ServiceName { get; set; }
        public string ServiceAddress { get; set; }
        public string ServicePhone { get; set; }
        public string ServiceAuthorizedName { get; set; }
        public string VehicleNameSurname { get; set; }
        public string VehicleAddress { get; set; }
        public string VehiclePhone { get; set; }
        public string VehiclePlateNo { get; set; }
        public string VehicleType { get; set; }
        public string VehicleFuelType { get; set; }
        public string VehicleTransmissionType { get; set; }
        public string VehicleBrand { get; set; }
        public string VehicleModel { get; set; }
        public string VehicleYear { get; set; }
        public string VehicleEngineSize { get; set; }
        public string VehicleEngineNo { get; set; }
        public string VehicleChassisNo { get; set; }
        public DateTime? PartsInstallationDate { get; set; }
        public string PartsInstallationKM { get; set; }
        public DateTime? PartsFaultDate { get; set; }
        public string PartsFaultKM { get; set; }
        public string CustomerComplaint { get; set; }
        public string ChangingParts { get; set; }
        public string ServiceReport { get; set; }
        public int MailStatu { get; set; }
        public ReturnFormStatus Status { get; set; }

        public string StatusStr
        {
            get
            {

                switch (Status)
                {
                    case ReturnFormStatus.Accepted:
                        return "Kabul Edildi";
                    case ReturnFormStatus.OnHold:
                        return "Beklemede";
                    case ReturnFormStatus.Rejected:
                        return "Red Edildi";
                    case ReturnFormStatus.Inspecting:
                        return "İnceleniyor";
                    default:
                        return "";

                };

            }
        }

        public string TypeStr
        {
            get
            {

                switch (Type)
                {
                    case 0:
                        return "Sağlam";
                    case 1:
                        return "Hasarlı";
                    case 3:
                        return "Arızalı";
                    case 4:
                        return "Fiyat Farkı";
                    default:
                        return "";
                };

            }
        }

        public int Type { get; set; }
        public int Confrim { get; set; }
        public string FilePath { get; set; }
        public string Cargo { get; set; }
        public string PriceStr { get { return (Price.ToString() + " " + CurrencyType); } }
        public CustomerSmall Customer { get; set; }
        public string CustomerCode { get { return (Customer.Code); } }
        public string CustomerName { get { return (Customer.Name); } }
        public string Manufacturer { get; set; }
        #endregion
        public ReturnForm()
        {
            Id = -1;
            FormNo = string.Empty;
            CustomerId = -1;
            SalesmanId = -1;
            AddType = 0;
            ProductCode = string.Empty;
            ProductName = string.Empty;
            ProductManu = string.Empty;
            ProductManuCode = string.Empty;
            InvoiceNumber = string.Empty;
            InvoiceDate = DateTime.MinValue;
            Quantity = -1;
            ReturnReason = string.Empty;
            Explanation = string.Empty;
            Price = -1;
            CurrencyType = string.Empty;
            ServiceName = string.Empty;
            ServiceAddress = string.Empty;
            ServicePhone = string.Empty;
            ServiceAuthorizedName = string.Empty;
            VehicleNameSurname = string.Empty;
            VehicleAddress = string.Empty;
            VehiclePhone = string.Empty;
            VehiclePlateNo = string.Empty;
            VehicleType = string.Empty;
            VehicleFuelType = string.Empty;
            VehicleTransmissionType = string.Empty;
            VehicleBrand = string.Empty;
            VehicleModel = string.Empty;
            VehicleYear = string.Empty;
            VehicleEngineSize = string.Empty;
            VehicleEngineNo = string.Empty;
            VehicleChassisNo = string.Empty;
            PartsInstallationDate = DateTime.MinValue;
            PartsInstallationKM = string.Empty;
            PartsFaultDate = DateTime.MinValue;
            PartsFaultKM = string.Empty;
            CustomerComplaint = string.Empty;
            ChangingParts = string.Empty;
            ServiceReport = string.Empty;
            Customer = new CustomerSmall();
        }
        #region Methods


        public static List<ReturnForm> GetList(int customerId)
        {
            List<ReturnForm> list = new List<ReturnForm>();
            DataTable dt = DAL.GetReturnProductList(customerId);

            foreach (DataRow row in dt.Rows)
            {
                ReturnForm obj = new ReturnForm()
                {
                    Id = row.Field<int>("Id"),
                    Type = row.Field<int>("Type"),
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductName = row.Field<string>("ProductName"),
                    InvoiceDate = row.Field<DateTime>("InvoiceDate"),
                    InvoiceNumber = row.Field<string>("InvoiceNumber"),
                    Price = row.Field<double>("Price"),
                    CurrencyType = row.Field<string>("CurrencyType"),
                    Quantity = row.Field<double>("Quantity"),
                    ReturnReason = row.Field<string>("ReturnReason"),
                    Explanation = row.Field<string>("Explanation"),
                    Status = (ReturnFormStatus)row.Field<int>("Status"),
                    CreateDate = row.Field<DateTime>("CreateDate")
                };
                list.Add(obj);
            }
            return list;
        }


        public static List<ReturnForm> GetListForAdmin(DateTime pStartDate, DateTime pEndDate, string pText, int pStatus)
        {
            List<ReturnForm> list = new List<ReturnForm>();
            DataTable dt = DAL.GetReturnProductListForAdmin(pStartDate, pEndDate, pText, pStatus);

            foreach (DataRow row in dt.Rows)
            {
                ReturnForm obj = new ReturnForm()
                {
                    Id = row.Field<int>("Id"),
                    Type = row.Field<int>("Type"),
                    ProductCode = row.Field<string>("ProductCode"),
                    ProductName = row.Field<string>("ProductName"),
                    Manufacturer = row.Field<string>("Manufacturer"),
                    ProductManuCode = row.Field<string>("ManufacturerCode"),
                    InvoiceDate = row.Field<DateTime>("InvoiceDate"),
                    InvoiceNumber = row.Field<string>("InvoiceNumber"),
                    Price = row.Field<double>("Price"),
                    CurrencyType = row.Field<string>("CurrencyType"),
                    Quantity = row.Field<double>("Quantity"),
                    ReturnReason = row.Field<string>("ReturnReason"),
                    Explanation = row.Field<string>("Explanation"),
                    Status = (ReturnFormStatus)row.Field<int>("Status"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Customer = new CustomerSmall
                    {
                        Id = row.Field<int>("CustomerId"),
                        Code = row.Field<string>("CustomerCode"),
                        Name = row.Field<string>("CustomerName")
                    }

                };
                list.Add(obj);
            }
            return list;
        }


        public bool Insert()
        {
            return DAL.InsertReturnForm(UserId, CustomerId, SalesmanId, AddType, ProductCode, InvoiceNumber, InvoiceDate, Quantity, ReturnReason, Explanation, Price, CurrencyType, Type, CreateId);
        }

        public bool Update()
        {
            return DAL.UpdateReturnForm(Id, (int)Status, Deleted);
        }

        #endregion

    }
    public enum ReturnFormStatus
    {
        OnHold = 0,
        Inspecting = 1,
        Accepted = 2,
        Rejected = 3
    }
    public partial class DataAccessLayer
    {
        public DataTable GetReturnProductList(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ReturnProductByCustomerId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }

        public DataTable GetReturnProductListForAdmin(DateTime pStartDate, DateTime pEndDate, string pText, int pStatus)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_ReturnProductBySearch", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pStartDate, pEndDate, pText, pStatus });
        }

        public bool InsertReturnForm(int pUserId, int pCustomerId, int pSalesmanId, int pAddType, string pProductCode, string pInvoicenumber, DateTime pInvoiceDate, double pQuantity, string pReturnReason, string pExplanation, double pPrice, string pCurrencyType, int pType, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_ReturnProduct", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pUserId, pCustomerId, pSalesmanId, pAddType, pProductCode, pInvoicenumber, pInvoiceDate, pQuantity, pReturnReason, pExplanation, pPrice, pCurrencyType, pType, pCreateId });
        }
        public bool UpdateReturnForm(int pId, int pStatus, bool pDeleted)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_ReturnProduct", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pStatus, pDeleted });
        }

    }
}