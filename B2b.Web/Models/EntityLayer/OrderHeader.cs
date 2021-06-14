using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;


namespace B2b.Web.v4.Models.EntityLayer
{
    public class OrderHeaderSmall : DataAccess
    {
        public int Id { get; set; }
        public string EntegreNo { get; set; }
        public string SenderType { get; set; }
        public string SenderName { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public string Notes { get; set; }
        public PaymentType PaymentType { get; set; }
        public string ShipmentName { get; set; }
        public int SendingTypeId { get; set; }
        public double Total { get; set; }
        public double TotalLocal { get; set; }
        public double Discount { get; set; }
        public double DiscountLocal { get; set; }
        public double Vat { get; set; }
        public double VatLocal { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusStr
        {
            get
            {
                switch (Status)
                {
                    case OrderStatus.OnHold:
                        return "Beklemede";
                    case OrderStatus.Confirmed:
                        return "Onaylandı";
                    case OrderStatus.Deleted:
                        return "Silindi";
                    case OrderStatus.OnHoldInPool:
                        return "Havuzda Bekleyen";
                    case OrderStatus.CombinedInPool:
                        return "Havuzda Birleştirildi";
                    case OrderStatus.UnKnown:
                        return "Teslim Edilemedi";
                    case OrderStatus.AutomaticTransfer:
                        return "Aktarım Bekliyor";
                    case OrderStatus.TransferError:
                        return "Aktarım Hatası";
                    case OrderStatus.RegionalDirector: //89
                        return "Bölge Yöneticisi Onayı Bekleniyor";
                    case OrderStatus.CentralApproval: //88
                        return "Merkez Onayı Bekleniyor";
                    case OrderStatus.FinanceApproval:
                        return "Muhasebe Onayı Bekleniyor";
                    default:
                        return string.Empty;
                }
            }
        }
        public ShippingStatu ShippingStatu { get; set; }
        public string CauseOfDeletion { get; set; }
        public int NumberOfProduct { get; set; }
        public string ConfirmSalesmanName { get; set; }
        public string Currency { get; set; }
        public string CurrencyAll { get; set; }
        public bool PrintStatu { get; set; }
        public CustomerSmall Customer { get; set; }
        public double GeneralTotal { get; set; }
        public double GeneralTotalLocal { get; set; }
        public string GeneralTotalStr { get { return new Price(GeneralTotal, Currency, 1).ToString(); } }
        public string DiscountStr { get { return new Price(Discount, Currency, 1).ToString(); } }
        public double NetTotal { get; set; }
        public double NetTotalLocal { get; set; }
        public string NetTotalStr { get { return new Price(NetTotal, Currency, 1).ToString(); } }
        public string VatStr { get { return new Price(Vat, Currency, 1).ToString(); } }
        public string TotalStr { get { return new Price(Total, Currency, 1).ToString(); } }
        public string ErrorMessage { get; set; }
        public double TotalAvailable { get; set; }

        public string ShipmentCity { get; set; }
        public string ShipmentTown { get; set; }

        public OrderHeader ConvertToOrder()
        {
            OrderHeader item = new OrderHeader()
            {
                Id = Id,
                EntegreNo = EntegreNo,
                SenderType = SenderType,
                SenderName = SenderName,
                ConfirmDate = ConfirmDate,
                Notes = Notes,
                PaymentType = PaymentType,
                ShipmentName = ShipmentName,
                Total = Total,
                TotalLocal = TotalLocal,
                Discount = Discount,
                DiscountLocal = DiscountLocal,
                Vat = Vat,
                VatLocal = VatLocal,
                Status = Status,
                CauseOfDeletion = CauseOfDeletion,
                NumberOfProduct = NumberOfProduct,
                ConfirmSalesmanName = ConfirmSalesmanName,
                Currency = Currency,
                CurrencyAll = CurrencyAll,
                PrintStatu = PrintStatu,
                Customer = Customer.ConvertToCustomer(),
                GeneralTotal = GeneralTotal,
                GeneralTotalLocal = GeneralTotalLocal,
                NetTotal = NetTotal,
                NetTotalLocal = NetTotalLocal,
                ErrorMessage = ErrorMessage,
                TotalAvailable = TotalAvailable
            };

            return item;
        }
    }


    [Serializable]
    public class OrderHeader : DataAccess
    {
        public OrderHeader()
        {
            PaymentId = string.Empty;
            Notes = string.Empty;
            ShipmentName = string.Empty;
            SalesmanNotes = string.Empty;
            PaymentNotes = string.Empty;
        }

        #region Properties
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int SalesmanId { get; set; }
        public int ConfirmSalesmanId { get; set; }
        public int SendingTypeId { get; set; }
        public int OldOrderId { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public double GeneralTotal { get; set; }
        public string GeneralTotalStr { get { return new Price(GeneralTotal, Currency, 1).ToString(); } }
        public double Discount { get; set; }
        public string DiscountStr { get { return new Price(Discount, Currency, 1).ToString(); } }
        public double NetTotal { get; set; }
        public string NetTotalStr { get { return new Price(NetTotal, Currency, 1).ToString(); } }
        public double Vat { get; set; }
        public string VatStr { get { return new Price(Vat, Currency, 1).ToString(); } }
        public double Total { get; set; }
        public string TotalStr { get { return new Price(Total, Currency, 1).ToString(); } }
        public string Currency { get; set; }
        public double CurrencyRate { get; set; }
        public double GeneralTotalLocal { get; set; }
        public double DiscountLocal { get; set; }
        public double NetTotalLocal { get; set; }
        public double VatLocal { get; set; }
        public double TotalLocal { get; set; }
        public string CurrencyLocal { get; set; }
        public string EntegreNo { get; set; }
        public OrderStatus Status { get; set; }
        public ShippingStatu ShippingStatu { get; set; }
        public string SenderType { get; set; }
        public string ConfirmSalesmanName { get; set; }
        public bool PrintStatu { get; set; }
        public double TotalAvailable { get; set; }
        public string StatusStr
        {
            get
            {
                switch (Status)
                {
                    case OrderStatus.OnHold:
                        return "Beklemede";
                    case OrderStatus.Confirmed:
                        return "Onaylandı";
                    case OrderStatus.Deleted:
                        return "Silindi";
                    case OrderStatus.OnHoldInPool:
                        return "Havuzda Bekleyen";
                    case OrderStatus.CombinedInPool:
                        return "Havuzda Birleştirildi";
                    case OrderStatus.UnKnown:
                        return "Teslim Edilemedi";
                    case OrderStatus.AutomaticTransfer:
                        return "Aktarım Bekliyor";
                    case OrderStatus.TransferError:
                        return "Aktarım Hatası";
                    case OrderStatus.RegionalDirector: //89
                        return "Bölge Yöneticisi Onayı Bekleniyor";
                    case OrderStatus.CentralApproval: //88
                        return "Merkez Onayı Bekleniyor";
                    case OrderStatus.FinanceApproval:
                        return "Muhasebe Onayı Bekleniyor";
                    default:
                        return string.Empty;
                }
            }
        }

        public string ShippingStatuStr
        {
            get
            {
                if (Status != OrderStatus.Confirmed)
                    return string.Empty;

                switch (ShippingStatu)
                {
                    case ShippingStatu.Shipped:
                        return "Sevk Edildi";
                    case ShippingStatu.Waiting:
                        return "Sevk Bekliyor";
                    default:
                        return string.Empty;
                }
            }
        }

        public string Notes { get; set; }
        public string SalesmanNotes { get; set; }
        public string PaymentNotes { get; set; }
        public bool FreeShipping { get; set; }
        public string ShipmentInfo { get; set; }
        public PaymentType PaymentType { get; set; }
        public int NumberOfProduct { get; set; }
        public string CauseOfDeletion { get; set; }
        public string T9 { get; set; }
        public string PaymentId { get; set; }
        public int ProgramType { get; set; }
        public Customer Customer { get; set; }
        public string ShipmentName { get; set; }
        public string CurrencyAll { get; set; }
        public string ErrorMessage { get; set; }

        public string ShipmentPerson { get; set; }
        public string ShipmentAddress { get; set; }
        public string ShipmentTel { get; set; }
        public string ShipmentCity { get; set; }
        public string ShipmentTown { get; set; }

        #endregion

        #region orderListPropertiesExtra
        private String senderName;
        public String SenderName
        {
            get { return senderName; }
            set { senderName = value; }
        }

        #endregion


        #region Methods

        public int Save()
        {
            DataTable dt = DAL.InsertOrder(CustomerId, UserId, SalesmanId, SendingTypeId, GeneralTotal, Discount, NetTotal, Vat, Total, Currency, (int)Status, Notes, 1, (int)PaymentType, NumberOfProduct, PaymentId, SalesmanNotes, TotalAvailable, ShipmentPerson, ShipmentAddress, ShipmentTel, ShipmentCity, ShipmentTown, PaymentNotes, FreeShipping, ShipmentInfo);
            return Id = Convert.ToInt32(dt.Rows[0][0]);
        }
        public bool Update()
        {
            return DAL.UpdateOrderHeader(GeneralTotal, Discount, NetTotal, Vat, Total, GeneralTotalLocal, DiscountLocal,
                NetTotalLocal, VatLocal, TotalLocal, Notes, Id, CurrencyRate);
        }
        public bool UpdateOrderHeaderStatu()
        {
            return DAL.UpdateOrderHeaderStatu(Id, (int)Status, Notes, EditId, SalesmanNotes);
        }
        public static List<OrderHeaderSmall> GetOrderList(DateTime startDate, DateTime finishDate, string t9, int status)
        {
            List<OrderHeaderSmall> list = new List<OrderHeaderSmall>();
            DataTable dt = DAL.GetOrderList(startDate, finishDate, t9, status);

            foreach (DataRow row in dt.Rows)
            {
                OrderHeaderSmall order = new OrderHeaderSmall();
                {
                    order.Id = row.Field<int>("Id");
                    order.EntegreNo = row.Field<string>("EntegreNo");
                    order.SenderType = row.Field<string>("SenderType");
                    order.SenderName = row.Field<string>("SenderName");
                    order.CreateDate = row.Field<DateTime>("CreateDate");
                    order.ConfirmDate = row.Field<DateTime?>("ConfirmDate");
                    order.Notes = row.Field<string>("Notes");
                    order.PaymentType = row.Field<PaymentType>("PaymentType");
                    order.ShipmentName = row.Field<string>("ShipmentMethod");
                    order.GeneralTotal = row.Field<double>("GeneralTotal");
                    order.Discount = row.Field<double>("Discount");
                    order.NetTotal = row.Field<double>("NetTotal");
                    order.Vat = row.Field<double>("Vat");
                    order.Total = row.Field<double>("Total");

                    order.GeneralTotalLocal = row.Field<double>("GeneralTotalLocal");
                    order.DiscountLocal = row.Field<double>("DiscountLocal");
                    order.NetTotalLocal = row.Field<double>("NetTotalLocal");
                    order.VatLocal = row.Field<double>("VatLocal");
                    order.TotalLocal = row.Field<double>("TotalLocal");

                    order.Status = (OrderStatus)row.Field<int>("Status");
                    order.CauseOfDeletion = row.Field<string>("CauseOfDeletion");
                    order.NumberOfProduct = row.Field<int>("NumberOfProduct");
                    order.ConfirmSalesmanName = row.Field<string>("ConfirmSalesmanName");
                    order.Currency = row.Field<string>("Currency");
                    order.CurrencyAll = row.Field<string>("CurrencyAll").Replace(",", "</br>");
                    order.PrintStatu = row.Field<bool>("PrintStatu");
                    order.ErrorMessage = row.Field<string>("ErrorMessage");
                    order.ShipmentCity = row.Field<string>("ShipmentCity");
                    order.ShipmentTown = row.Field<string>("ShipmentTown");

                    order.Customer = new CustomerSmall()
                    {
                        Id = row.Field<int>("CustomerId"),
                        Code = row.Field<string>("CustomerCode"),
                        Name = row.Field<string>("CustomerName"),
                        City = row.Field<string>("City")
                    };
                }

                list.Add(order);
            }
            return list;
        }

        public static List<OrderHeaderSmall> GetOrderHeaderListBySalesman(Salesman salesman, DateTime startDate, DateTime endDate, string searchText)
        {
            List<OrderHeaderSmall> list = new List<OrderHeaderSmall>();
            string[] t9Array = GenerateT9Search(searchText);
            DataTable dt = DAL.GetOrderHeaderListBySalesman(salesman.Id, startDate, endDate, t9Array[0], t9Array[1], t9Array[2], t9Array[3], t9Array[4]);
            foreach (DataRow row in dt.Rows)
            {
                OrderHeaderSmall obj = new OrderHeaderSmall()
                {
                    Id = row.Field<int>("Id"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Status = (OrderStatus)row.Field<int>("Status"),
                    ConfirmDate = row["ConfirmDate"] as DateTime?,
                    Notes = row.Field<string>("Notes"),
                    SenderName = row.Field<string>("SenderName"),
                    SendingTypeId = row.Field<int>("SendingTypeId"),
                    ShipmentName = row.Field<string>("ShipmentName"),
                    CurrencyAll = row.Field<string>("CurrencyAll").Replace(",", "</br>"),
                    Total = row.Field<double>("Total"),
                    Vat = row.Field<double>("Vat"),
                    Discount = row.Field<double>("Discount"),
                    NetTotal = row.Field<double>("NetTotal"),
                    GeneralTotal = row.Field<double>("GeneralTotal"),
                    Currency = row.Field<string>("Currency")

                };
                list.Add(obj);
            }
            return list;
        }

        public bool Delete()
        {
            return DAL.DeleteOrderHeader(Id, (int)Status, EditId);
        }
        public static OrderHeader GetOrderById(int Id)
        {
            DataTable dt = DAL.GetOrderById(Id);
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                OrderHeader orderHeader = new OrderHeader();
                {
                    orderHeader.Id = row.Field<int>("Id");
                    orderHeader.EntegreNo = row.Field<string>("EntegreNo");
                    orderHeader.SenderType = row.Field<string>("SenderType");
                    orderHeader.SenderName = row.Field<string>("SenderName");
                    orderHeader.CreateDate = row.Field<DateTime>("CreateDate");
                    orderHeader.ConfirmDate = row.Field<DateTime?>("ConfirmDate");
                    orderHeader.Notes = row.Field<string>("Notes");
                    orderHeader.SalesmanNotes = row.Field<string>("SalesmanNotes");
                    orderHeader.PaymentNotes = row.Field<string>("PaymentNotes");
                    orderHeader.FreeShipping = row.Field<bool>("FreeShipping");
                    orderHeader.PaymentType = row.Field<PaymentType>("PaymentType");
                    orderHeader.ShipmentName = row.Field<string>("ShipmentMethod");
                    orderHeader.ShipmentInfo = row.Field<string>("ShipmentInfo");

                    orderHeader.Total = row.Field<double>("Total");
                    orderHeader.Discount = row.Field<double>("Discount");
                    orderHeader.Vat = row.Field<double>("Vat");
                    orderHeader.GeneralTotal = row.Field<double>("GeneralTotal");
                    orderHeader.NetTotal = row.Field<double>("NetTotal");
                    orderHeader.Status = (OrderStatus)row.Field<int>("Status");
                    orderHeader.ConfirmSalesmanName = row.Field<string>("ConfirmSalesmanName");
                    orderHeader.Currency = row.Field<string>("Currency");
                    orderHeader.CurrencyAll = row.Field<string>("CurrencyAll").Replace(",", "</br>");
                    orderHeader.CauseOfDeletion = row.Field<string>("CauseOfDeletion");
                    orderHeader.NumberOfProduct = row.Field<int>("NumberOfProduct");

                    orderHeader.GeneralTotalLocal = row.Field<double>("GeneralTotalLocal");
                    orderHeader.DiscountLocal = row.Field<double>("DiscountLocal");
                    orderHeader.NetTotalLocal = row.Field<double>("NetTotalLocal");
                    orderHeader.VatLocal = row.Field<double>("VatLocal");
                    orderHeader.TotalLocal = row.Field<double>("TotalLocal");
                    orderHeader.ErrorMessage = row.Field<string>("ErrorMessage");

                    orderHeader.Customer = new Customer()
                    {
                        Id = row.Field<int>("CustomerId"),
                        Code = row.Field<string>("CustomerCode"),
                        Name = row.Field<string>("CustomerName"),
                        Tel1 = row.Field<string>("Tel1"),
                        Fax1 = row.Field<string>("Fax1"),
                        City = row.Field<string>("City"),
                        Town = row.Field<string>("Town"),
                        Address = row.Field<string>("Address"),
                        Salesman = new Salesman()
                        {
                            Id = Convert.ToInt32(row["SalesmanId"]),
                            Code = row.Field<string>("SalesmanCode"),
                            Name = row.Field<string>("SalesmanName")
                        }
                    };
                }
                return orderHeader;
            }
            return null;
        }
        public static List<OrderHeader> GetOrderHeaderList(LoginType loginType, Customer customer, DateTime startDate, DateTime endDate, string searchText)
        {
            List<OrderHeader> list = new List<OrderHeader>();
            string[] t9Array = GenerateT9Search(searchText);
            DataTable dt = DAL.GetOrderHeaderList((int)loginType, customer.Id, customer.Users.Id, startDate, endDate, t9Array[0], t9Array[1], t9Array[2], t9Array[3], t9Array[4]);
            foreach (DataRow row in dt.Rows)
            {
                OrderHeader obj = new OrderHeader()
                {
                    Id = row.Field<int>("Id"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Status = (OrderStatus)row.Field<int>("Status"),
                    ShippingStatu = (ShippingStatu)row.Field<int>("ShippingStatu"),
                    ConfirmDate = row["ConfirmDate"] as DateTime?,
                    Notes = row.Field<string>("Notes"),
                    SenderName = row.Field<string>("SenderName"),
                    SendingTypeId = row.Field<int>("SendingTypeId"),
                    ShipmentName = row.Field<string>("ShipmentName"),
                    CurrencyAll = row.Field<string>("CurrencyAll").Replace(",", "</br>"),
                    Total = row.Field<double>("Total"),
                    Vat = row.Field<double>("Vat"),
                    Discount = row.Field<double>("Discount"),
                    NetTotal = row.Field<double>("NetTotal"),
                    GeneralTotal = row.Field<double>("GeneralTotal"),
                    Currency = row.Field<string>("Currency"),

                    ShipmentPerson = row.Field<string>("ShipmentPerson"),
                    ShipmentAddress = row.Field<string>("ShipmentAddress"),
                    ShipmentTel = row.Field<string>("ShipmentTel"),
                    ShipmentCity = row.Field<string>("ShipmentCity"),
                    ShipmentTown = row.Field<string>("ShipmentTown"),

                };
                list.Add(obj);
            }
            return list;
        }

        public static List<OrderHeader> GetDashhBoardOrder(LoginType loginType, Customer customer)
        {
            List<OrderHeader> list = new List<OrderHeader>();
            DataTable dt = DAL.GetDashhBoardOrder((int)loginType, customer.Id, customer.Users.Id);
            foreach (DataRow row in dt.Rows)
            {
                OrderHeader obj = new OrderHeader()
                {
                    Id = row.Field<int>("Id"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Status = (OrderStatus)row.Field<int>("Status"),
                    ConfirmDate = row["ConfirmDate"] as DateTime?,
                    Notes = row.Field<string>("Notes"),
                    SenderName = row.Field<string>("SenderName"),
                    SendingTypeId = row.Field<int>("SendingTypeId"),
                    ShipmentName = row.Field<string>("ShipmentName"),
                    CurrencyAll = row.Field<string>("CurrencyAll").Replace(",", "</br>"),
                    Total = row.Field<double>("Total"),
                    Vat = row.Field<double>("Vat"),
                    Discount = row.Field<double>("Discount"),
                    NetTotal = row.Field<double>("NetTotal"),
                    GeneralTotal = row.Field<double>("GeneralTotal"),
                    Currency = row.Field<string>("Currency")

                };
                list.Add(obj);
            }
            return list;
        }

        public static bool UpdateOrderShippingStatu(string orderEntegreNo, ShippingStatu shippingStatu)
        {
            return DAL.UpdateOrderShippingStatu(orderEntegreNo, (int)shippingStatu);
        }

        #endregion

    }
    public enum OrderStatus
    {
        OnHold = 0,
        Confirmed = 1,
        Deleted = 2,
        OnHoldInPool = 3,
        CombinedInPool = 4,
        AutomaticTransfer = 98,
        UnKnown = 99,
        TransferError = 90,
        RegionalDirector = 89,
        CentralApproval = 88,
        FinanceApproval = 87,
        OnWay = 86
    }

    public enum ShippingStatu
    {
        Waiting = 0,
        Shipped = 1
    }


    public partial class DataAccessLayer
    {
        public bool DeleteOrderHeader(int pOrderId, int pStatus, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_DeleteOrReturn_Order_Header", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pOrderId, pStatus, pEditId });
        }
        public DataTable GetDashhBoardOrder(int pLoginType, int pCustomerId, int pUserId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_DashboardOrderHeader", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pLoginType, pCustomerId, pUserId });
        }

        public DataTable GetOrderHeaderListBySalesman(int pSalesmanId, DateTime pStartdate, DateTime pEndDate, string pT9Text1, string pT9Text2, string pT9Text3, string pT9Text4, string pT9Text5)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Report_GetList_OrderHeader", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSalesmanId, pStartdate, pEndDate, pT9Text1, pT9Text2, pT9Text3, pT9Text4, pT9Text5 });
        }

        public DataTable GetOrderHeaderList(int pLoginType, int pCustomerId, int pUserId, DateTime pStartdate, DateTime pEndDate, string pT9Text1, string pT9Text2, string pT9Text3, string pT9Text4, string pT9Text5)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_OrderHeader", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pLoginType, pCustomerId, pUserId, pStartdate, pEndDate, pT9Text1, pT9Text2, pT9Text3, pT9Text4, pT9Text5 });
        }
        public DataTable GetOrderList(DateTime pStartDate, DateTime pFinishDate, string pT9, int pStatus)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_Order", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pStartDate, pFinishDate, pT9, pStatus });
        }
        public DataTable GetOrderById(int pId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_Order_ById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }
        public DataTable InsertOrder(int pCustomerId, int pUserId, int pSalesmanId, int pSendingTypeId, double pGeneralTotal, double pDiscount, double pNetTotal, double pVat, double pTotal, string pCurrency, int pStatus, string pNotes, int pProgramType, int pPaymentType, int pNumberOfProduct, string pPaymentId, string pSalesmanNotes, double pTotalAvailable, string pShipmentPerson, string pShipmentAddress, string pShipmentTel, string pShipmentCity, string pShipmentTown, string pPaymentNotes, bool pFreeShipping, string pShipmentInfo)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Insert_OrderHeader_4", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId, pSalesmanId, pSendingTypeId, pGeneralTotal, pDiscount, pNetTotal, pVat, pTotal, pCurrency, pStatus, pNotes, pProgramType, pPaymentType, pNumberOfProduct, pPaymentId, pSalesmanNotes, pTotalAvailable, pShipmentPerson, pShipmentAddress, pShipmentTel, pShipmentCity, pShipmentTown, pPaymentNotes, pFreeShipping, pShipmentInfo });
        }
        public bool UpdateOrderHeader(double pGeneralTotal, double pDiscount, double pNetTotal, double pVat, double pTotal, double pGeneralTotalLocal, double pDiscountLocal, double pNetTotalLocal, double pVatLocal, double pTotalLocal, string pNotes, int pOrderId, double pCurrencyRate)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_Order_Header_1", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pGeneralTotal, pDiscount, pNetTotal, pVat, pTotal, pGeneralTotalLocal, pDiscountLocal, pNetTotalLocal, pVatLocal, pTotalLocal, pNotes, pOrderId, pCurrencyRate });
        }
        public bool UpdateOrderHeaderStatu(int pOrderId, int pStatus, string pNote, int pEditId,string pSalesmanNotes)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Order_Header_Statu_1", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pOrderId, pStatus, pNote, pEditId,pSalesmanNotes });
        }

        public bool UpdateOrderShippingStatu(string pEntegreNo, int pStatus)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Order_Header_ShippingStatu", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pEntegreNo, pStatus, });
        }

    }

}