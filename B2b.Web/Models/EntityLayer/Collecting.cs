using System;
using System.Collections.Generic;
using System.Reflection;
using  System.Data;


namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class Collecting:DataAccess
    {
        #region Constructors
        public Collecting()
        {
            BankCode = string.Empty;
            Bank = string.Empty;
            BankBranch = string.Empty;
            BankBranchCode = string.Empty;
            AccountNo = string.Empty;
            DrawerPerson = string.Empty;
            DrawerPlace = string.Empty;
            DrawerDate = DateTime.MinValue;
            DueDate = DateTime.MinValue;
            CardNumber = string.Empty;
            PlaceHolder = string.Empty;
            CardType = string.Empty;
            ValidDate = string.Empty;
            Cvc = string.Empty;
            Amount = 0;
            Currency = string.Empty;
            Installment = 0;
            PaymentDate = DateTime.MinValue;
            City = string.Empty;
            Address = string.Empty;
            Guarantor = string.Empty;
            Note = string.Empty;
            CreateDate = DateTime.MinValue;
            DocumentNo = string.Empty;
            CollectingType = -1;
            Status = 0;
            DocumentDate = DateTime.MinValue;
            Type = -1;
            ItemNo = string.Empty;
            AsilCiro = 0;
            CiroEden = string.Empty;
            ProcessDate = DateTime.MinValue;
        }

        #endregion

        #region Properties

        public string Key { get; set; }
        public int Id { get; set; }
        public int UserId { get; set; }
        public int HeaderId { get; set; }
        public string BankCode { get; set; }
        public string Bank { get; set; }
        public string BankBranch { get; set; }
        public string BankBranchCode { get; set; }
        public string AccountNo { get; set; }
        public string DrawerPerson { get; set; }
        public string DrawerPlace { get; set; }
        public DateTime? DrawerDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string CardNumber { get; set; }
        public string PlaceHolder { get; set; }
        public string CardType { get; set; }
        public string ValidDate { get; set; }
        public string Cvc { get; set; }
        public double Amount { get; set; }
        public double AmountKrs { get; set; }
        public string Currency { get; set; }
        public int Installment { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Guarantor { get; set; }
        public string Note { get; set; }
      
        public Customer Customer { get; set; }
        public Salesman Salesman { get; set; }
        public string DocumentNo { get; set; }
        public int CollectingType { get; set; }
        public int Status { get; set; }
        public DateTime? DocumentDate { get; set; }
        public int Type { get; set; }
        public string ItemNo { get; set; }
        public int AsilCiro { get; set; }
        public string CiroEden { get; set; }

        public bool HasPdf { get; set; }
        public DateTime? ProcessDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public DateTime? ModeratorConfirmDate { get; set; }
        public byte[] Pdf { get; set; }
        public string CollectingTypeText { get { switch (CollectingType) { case 0: return "Nakit"; case 1: return "Çek"; case 2: return "Senet"; case 3: return "Kredi Kartı"; case 5: return "Mail Order"; case 6: return "Havale"; case 99: return "Sanal Pos"; default: return ""; } } }
        public string StatusStr { get { if (Status == 0) return "Bekliyor"; if (Status == 1) return "Onaylandı"; if (Status == 2) return "Silindi"; if (Status == 99) return "Çekim Başarılı"; return ""; } }
        public Price PriceTotal{ get { return new Price(Amount, Currency, 1); } }
        public string PriceTotalStr {
            get { return PriceTotal.ToString(); }
        }
        #region Report
        public string EntegreNo { get; set; }
        public int MyProperty { get; set; }
        #endregion

        #endregion

        #region Const

        public const string PREVENDLINE = "PRINT 1,2";
        public const string PARSELINE = "TEXT 1,@ROW,\"2\",0,1,1,\"----------------------------------------\"";
        public const string ONAYCARILINE = "TEXT 1,@ROW,\"1\",0,1,1,\"Onaylayan Cari\"";
        public const string ONAYPLASIYERLINE = "TEXT 1,@ROW,\"1\",0,1,1,\"Onaylayan Plasiyer\"";
        public const int YCOOR = 1225;
        public const int ROWHEIGHT = 40;
        public const int MAXROWCOUNT = 34;

        public const string PATH = "CollectingPrinterFiles/";
        public const string FIRMA = "[FirmTittle]";
        public const string MAKBUZNO = "[MakbuzNo]";
        public const string TEL = "[FirmPhoneNu]";
        public const string FAX = "[FirmFax]";
        public const string ADRES = "[Address]";
        public const string EMAIL = "[FirmMail]";
        public const string KODU = "[CustCode]";
        public const string UNVANI = "[CustTittle]";

        public const string CBD_ESKIBAKIYE = "TEXT 1,@ROW,\"3\",0,1,1,\"Eski Bakiye      :[CBD_ESKIBAKIYE]\"";
        public const string CBD_TAHTOP = "TEXT 1,@ROW,\"3\",0,1,1,\"Tahsilat Toplamı :[CBD_TAHTOP]\"";
        public const string CBD_SONBAKIYE = "TEXT 1,@ROW,\"3\",0,1,1,\"Son Durum Bakiye :[CBD_SONBAKIYE]\"";

        public const string TO_NAKTOP = "[TO_NAKTOP]";
        public const string TO_CEKTOP = "[TO_CEKTOP]";
        public const string TO_SENTOP = "[TO_SENTOP]";
        public const string TO_TAHTOP = "[TO_TAHTOP]";

        public const string TD_TUR = "TEXT 1,@ROW,\"3\",0,1,1,\"Tür      :[TD_TUR]\"";
        public const string TD_BANKA = "TEXT 1,@ROW,\"3\",0,1,1,\"Banka    :[TD_BANKA]\"";
        public const string TD_EVRAKNO = "TEXT 1,@ROW,\"3\",0,1,1,\"Evrak No :[TD_EVRAKNO]\"";
        public const string TD_VADE = "TEXT 1,@ROW,\"3\",0,1,1,\"Vade     :[TD_VADE]\"";
        public const string TD_TUTAR = "TEXT 1,@ROW, \"3\",0,1,1,\"Tutar    :[TD_TUTAR]\"";

        public const string PAGEHEIGHT = "[PAGEHEIGHT]";
        public const string CARIADRES = "[Address]";
        public const string ILCE = "[Town]";
        public const string IL = "[City]";
        public const string VERGIDAIRESI = "[TaxOffice]";
        public const string VERGINO = "[TaxNumber]";
        public const string TARIH = "[Date]";
        public const string PLASIYER = "[SalesmanName]";
        #endregion
        public void Save()
        {
            DAL.InsertCollecting(HeaderId, Customer.Id, Salesman.Id, Currency, Bank, BankBranch, AccountNo, DrawerPerson, DrawerPlace, DrawerDate, DueDate, CardNumber, PlaceHolder, CardType, ValidDate, Cvc, Amount, Installment, PaymentDate, City, Address, Guarantor, Note, DocumentNo, Type, Status, DocumentDate, ItemNo, AsilCiro, CiroEden, ProcessDate, Customer.Users.Id, BankCode,CreateDate);

        }

        public static List<Collecting> GetListBySalesmanId(int salesmanId, int CustomerId, int userId)
        {
            List<Collecting> list = new List<Collecting>();
            DataTable dt = DAL.GetListCollectingBySalesmanId(salesmanId, CustomerId, userId);

            foreach (DataRow row in dt.Rows)
            {
                Collecting oem = new Collecting()
                {
                    Id = row.Field<int>("Id"),
                    BankCode = row.Field<string>("BankCode"),
                    Bank = row.Field<string>("Bank"),
                    BankBranch = row.Field<string>("BankBranch"),
                    AccountNo = row.Field<string>("AccountNo"),
                    DrawerPerson = row.Field<string>("DrawerPerson"),
                    DrawerPlace = row.Field<string>("DrawerPlace"),
                    DrawerDate = row.Field<DateTime?>("DrawerDate"),
                    DueDate = row.Field<DateTime?>("DueDate"),
                    CardNumber = row.Field<string>("CardNumber"),
                    PlaceHolder = row.Field<string>("PlaceHolder"),
                    CardType = row.Field<string>("CardType"),
                    ValidDate = row.Field<string>("ValidDate"),
                    Cvc = row.Field<string>("Cvc"),
                    Amount = row.Field<double>("Amount"),
                    Currency = row.Field<string>("Currency"),
                    Installment = row.Field<int>("Installment"),
                    PaymentDate = row.Field<DateTime?>("PaymentDate"),
                    City = row.Field<string>("City"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Address = row.Field<string>("Address"),
                    Guarantor = row.Field<string>("Guarantor"),
                    Note = row.Field<string>("Note"),
                    Customer = Customer.GetById(row.Field<int>("CustomerId"), row.Field<int>("UserId")),
                    Salesman = Salesman.GetById(row.Field<int>("SalesmanId")),
                    DocumentNo = row.Field<string>("DocumentNo"),
                    CollectingType = row.Field<int>("Type"),
                    Status = row.Field<int>("Status"),
                    DocumentDate = row.Field<DateTime?>("DocumentDate"),
                    Type = row.Field<int>("Type"),
                    ItemNo = row.Field<string>("BankBranch"),
                    AsilCiro = row.Field<int>("AsilCiro"),
                    CiroEden = row.Field<string>("CiroEden"),
                };
                list.Add(oem);
            }

            return list;
        }
        public static bool DeleteById(int pId)
        {
            return DAL.DeleteCollecting(pId);
        }
        public static void UpdateCollectingHeaderId(string ids, int headerId, string documentNo,string pnrText)
        {
            DAL.UpdateCollectingHeaderId(ids, headerId, documentNo, pnrText);
        }

        public static List<Collecting> GetListByHeaderId(int headerId)
        {
            List<Collecting> list = new List<Collecting>();
            DataTable dt = DAL.GetListCollectingByHeaderId(headerId);

            foreach (DataRow row in dt.Rows)
            {
                Collecting collecting = new Collecting()
                {
                    Id = row.Field<int>("Id"),
                    HeaderId = row.Field<int>("HeaderId"),
                    BankCode = row.Field<string>("BankCode"),
                    Bank = row.Field<string>("Bank"),
                    BankBranch = row.Field<string>("BankBranch"),
                    AccountNo = row.Field<string>("AccountNo"),
                    DrawerPerson = row.Field<string>("DrawerPerson"),
                    DrawerPlace = row.Field<string>("DrawerPlace"),
                    DrawerDate = row.Field<DateTime?>("DrawerDate"),
                    DueDate = row.Field<DateTime?>("DueDate"),
                    CardNumber = row.Field<string>("CardNumber"),
                    PlaceHolder = row.Field<string>("PlaceHolder"),
                    CardType = row.Field<string>("CardType"),
                    ValidDate = row.Field<string>("ValidDate"),
                    Cvc = row.Field<string>("Cvc"),
                    Amount = row.Field<double>("Amount"),
                    Currency = row.Field<string>("Currency"),
                    Installment = row.Field<int>("Installment"),
                    PaymentDate = row.Field<DateTime?>("PaymentDate"),
                    City = row.Field<string>("City"),
                    Address = row.Field<string>("Address"),
                    Guarantor = row.Field<string>("Guarantor"),
                    Note = row.Field<string>("Note"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Customer = Customer.GetById(row.Field<int>("CustomerId"), row.Field<int>("UserId")),
                    DocumentNo = row.Field<string>("DocumentNo"),
                    CollectingType = row.Field<int>("Type"),
                    Status = row.Field<int>("Status"),
                    DocumentDate = row.Field<DateTime?>("DocumentDate"),
                    Type = row.Field<int>("Type"),
                    ItemNo = row.Field<string>("ItemNo"),
                    AsilCiro = row.Field<int>("AsilCiro"),
                    CiroEden = row.Field<string>("CiroEden"),
                };
                list.Add(collecting);
            }

            return list;
        }
        public static List<Collecting> GetListByHeaderBySalesmanAndDate(int pSalesmanId, DateTime pStartDate, DateTime pEndDate)
        {
            List<Collecting> list = new List<Collecting>();
            DataTable dt = DAL.GetListCollecting_HeaderBySalesmanIdAndDate(pSalesmanId, pStartDate, pEndDate);

            foreach (DataRow row in dt.Rows)
            {
                Collecting collecitng = new Collecting()
                {
                    Id = row.Field<int>("Id"),
                    Customer = Customer.GetById(row.Field<int>("CustomerId"), row.Field<int>("UserId")),
                    DocumentNo = row.Field<string>("DocumentNo"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Status = row.Field<int>("Status"),
                    Amount = row.Field<double>("Amount"),
                    Currency = row.Field<string>("Currency")
                    
                };
                list.Add(collecitng);
            }

            return list;
        }
    }
    public  partial class DataAccessLayer
    {
        public bool InsertCollecting(int pHeaderId, int pCustomerId, int pSalesmanId, string pCurrency, string pBank, string pBankBranch, string pAccountNo, string pDrawerPerson, string pDrawerPlace, DateTime? pDrawerDate, DateTime? pDueDate, string pCardNumber, string pPlaceHolder, string pCardType, string pValidDate, string pCvc, double pAmount, int pInstallment, DateTime? pPaymentDate, string pCity, string pAddress, string pGuarantor, string pNote, string pDocumentNo, int pCollectingType, int pStatus, DateTime? pDocumentDate, string pItemNo, int pAsilCiro, string pCiroEden, DateTime? pProcessDate, int pUserId, string pBankCode,DateTime pCreateDate)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_Collecting", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pHeaderId, pCustomerId, pSalesmanId, pCurrency, pBank, pBankBranch, pAccountNo, pDrawerPerson, pDrawerPlace, pDrawerDate, pDueDate, pCardNumber, pPlaceHolder, pCardType, pValidDate, pCvc, pAmount, pInstallment, pPaymentDate, pCity, pAddress, pGuarantor, pNote, pDocumentNo, pCollectingType, pStatus, pDocumentDate, pItemNo, pAsilCiro, pCiroEden, pProcessDate, pUserId, pBankCode, pCreateDate });
        }
        public DataTable GetListCollectingBySalesmanId(int pSalesmanId, int pCustomerId, int pUserId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_CollectingBySalesmanId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSalesmanId, pCustomerId, pUserId });
        }
        public DataTable GetListCollectingByHeaderId(int pHeaderId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_CollectingByHeaderId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pHeaderId });
        }
        public DataTable GetListCollecting_HeaderBySalesmanIdAndDate(int pSalesmanId, DateTime pStartDate, DateTime pEndDate)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_CollectingHeaderBySalesmanIdAndDate", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSalesmanId, pStartDate, pEndDate });
        }
        public bool DeleteCollecting(int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Delete_Collecting", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }
        public bool UpdateCollectingHeaderId(string pId, int pHeaderId, string pDocumentNo,string pPnrText)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_CollectingHeaderId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId , pHeaderId, pDocumentNo, pPnrText });
        }
    }
}