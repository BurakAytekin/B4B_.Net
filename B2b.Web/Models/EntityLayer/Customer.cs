using System;
using System.Collections.Generic;
using B2b.Web.v4.Models.Helper;
using System.Data;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;


namespace B2b.Web.v4.Models.EntityLayer
{

    public class CustomerSmall
    {
        public int Id { get; set; }
        public int EntegreId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string Tel1 { get; set; }
        public string StartScreen { get; set; }
        public bool IsStartScreen { get; set; }
        public string CurrencyType { get; set; }

        public Customer ConvertToCustomer()
        {
            Customer item = new Customer()
            {
                Id = Id,
                EntegreId = EntegreId,
                Code = Code,
                Name = Name,
                Address = Address,
                City = City,
                Town = Town,
                Tel1 = Tel1,
                StartScreen = StartScreen,
                IsStartScreen = IsStartScreen,
                CurrencyType = CurrencyType,
            };
            return item;
        }
    }

    [Serializable]
    public class Customer : DataAccess
    {
        #region Properties
        public Users Users { get; set; }
        public AuthorityCustomer AuthorityCustomer { get; set; }
        private Salesman _salesman;
        public Salesman Salesman
        {
            get { return _salesman ?? (_salesman = Salesman.GetSalesmanByCustomerId(Id)); }
            set { _salesman = value; }
        }
        public int Id { get; set; }
        public string StartScreen { get; set; }
        public bool IsStartScreen { get; set; }
        public int EntegreId { get; set; }
        public int SalesmanIdInt { get; set; }
        public int SalesmanId { get; set; }
        public int ShipmentId { get; set; }
        public string Code { get; set; }
        public string B2bCode { get; set; }
        public string Name { get; set; }
        public string AddressFull { get { return Address + " " + Town + "/" + City; } }
        public string Address { get; set; }
        public string City { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        public string Mail { get; set; }
        public bool CampaignStatu { get; set; }
        public bool Status { get; set; }
        public PaymentType PaymentType { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Fax1 { get; set; }
        public string Gsm1 { get; set; }
        public string Gsm2 { get; set; }
        public string Notes { get; set; }
        public string RuleCode { get; set; }
        public string Password { get; set; }
        public int NumberOfUser { get; set; }
        public int NumberOfUserAndroid { get; set; }
        public int NumberOfUserIos { get; set; }
        public bool IsEnteringInformationPermitted { get; set; }
        public double RiskLimit { get; set; }
        public double CreditLimit { get; set; }
        public bool SpecialInstallment { get; set; }
        public bool PaymentOnOrder { get; set; }
        public bool IsCurrentAccountStatu { get; set; }
        public string CurrentAccountPassword { get; set; }
        public string RegionCode { get; set; }
        public string CurrencyType { get; set; }
        public int TerminalNo { get; set; }
        public DateTime LoginTime { get; set; }
        public string LoginIp { get; set; }
        public double VatRate { get; set; }
        public bool IsVatRateActive { get; set; }
        public string CampaignCode { get; set; }
        public bool IsCampaignCodeActive { get; set; }
        public int IsConfirmKvkk { get; set; }

        #endregion

        #region Constructor
        public Customer()
        {
            TerminalNo = -1;
        }

        #endregion

        #region MD5 Şifreleme

        private static string MD5Sifreleme(string Text)
        {
            MD5CryptoServiceProvider MD5Code = new MD5CryptoServiceProvider();
            byte[] byteDizisi = Encoding.UTF8.GetBytes(Text);
            byteDizisi = MD5Code.ComputeHash(byteDizisi);
            StringBuilder sb = new StringBuilder();
            foreach (byte ba in byteDizisi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }
            return sb.ToString();
        }

        #endregion

        #region Methods

        public static Customer GetById(int id, int userId)
        {
            Customer item = new Customer();
            DataTable dt = DAL.GetCustomerById(id, userId);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                item.Id = row.Field<int>("Id");
                item.StartScreen = row.Field<string>("StartScreen");
                item.IsStartScreen = row.Field<bool>("IsStartScreen");
                item.EntegreId = row.Field<int>("EntegreId");
                item.B2bCode = row.Field<string>("B2bCode");
                item.Code = row.Field<string>("Code");
                item.Name = row.Field<string>("Name");
                item.Address = row.Field<string>("Address");
                item.City = row.Field<string>("City");
                item.Town = row.Field<string>("Town");
                item.Tel1 = row.Field<string>("Tel1");
                item.RuleCode = row.Field<string>("RuleCode");
                item.PaymentType = (PaymentType)(row.Field<int>("PaymentType"));
                item.NumberOfUser = row.Field<int>("NumberOfUser");
                item.NumberOfUserIos = row.Field<int>("NumberOfUserIos");
                item.NumberOfUserAndroid = row.Field<int>("NumberOfUserAndroid");
                item.IsEnteringInformationPermitted = row.Field<bool>("EnteringInformation");
                item.SpecialInstallment = row.Field<bool>("SpecialInstallment");
                item.TaxOffice = row.Field<string>("TaxOffice");
                item.TaxNumber = row.Field<string>("TaxNumber");
                item.PaymentOnOrder = row.Field<bool>("PaymentOnOrder");
                item.ShipmentId = row.Field<int>("ShipmentId");
                item.VatRate = row.Field<double>("VatRate");
                item.IsVatRateActive = row.Field<bool>("IsVatRateActive");
                item.Mail = row.Field<string>("Mail");
                item.IsCurrentAccountStatu = row.Field<bool>("IsCurrentAccountStatu");
                item.CurrentAccountPassword = MD5Sifreleme(row.Field<string>("CurrentAccountPassword"));
                item.CurrencyType = row.Field<string>("CurrencyType");
                item.CampaignStatu = row.Field<bool>("CampaignStatu");
                item.IsCampaignCodeActive = row.Field<bool>("IsCampaignCodeActive");
                item.CampaignCode = row.Field<string>("CampaignCode");
                item.IsConfirmKvkk = row.Field<int>("IsConfirmKvkk");
                item.RiskLimit = row.Field<double>("RiskLimit");
                item.RegionCode = row.Field<string>("RegionCode");

                item.AuthorityCustomer = new AuthorityCustomer()
                {
                    CustomerId = row.Field<int>("Id"),
                    _EnteringInformation = row.Field<bool>("EnteringInformation"),
                    _CheckBasket = row.Field<bool>("CheckBasket"),
                    _ProductRestoration = row.Field<bool>("ProductRestoration"),
                    _ShowQuantity = row.Field<bool>("ShowQuantity"),
                    _WebLogin = row.Field<bool>("WebLogin")

                };

                item.Users = new Users()
                {
                    Id = row.Field<int>("UserId"),
                    Code = row.Field<string>("UserCode"),
                    Name = row.Field<string>("UserName"),
                    Tel = row.Field<string>("Usertel"),
                    Gsm = row.Field<string>("UserGsm"),
                    Mail = row.Field<string>("UserMail"),
                    CustomerId = row.Field<int>("CustomerId"),
                    RuleCode = row.Field<string>("UserRuleCode"),
                    Latitude = row.Field<string>("Latitude"),
                    Longitude = row.Field<string>("Longitude"),
                    Password = row.Field<string>("Password"),
                    Rate = row.Field<double>("Rate"),
                    Avatar = row.Field<string>("Avatar"),
                    AuthorityUser = new AuthorityUser()
                    {
                        UserId = row.Field<int>("UserId"),
                        _CampaignStatu = row.Field<bool>("UserCampaignStatu"),
                        _ProductRestoration = row.Field<bool>("UserProductRestoration"),
                        _LockUser = row.Field<bool>("LockUser"),
                        _RuleActive = row.Field<bool>("RuleActive"),
                        _ShowQuantity = row.Field<bool>("UserShowQuantity")
                    }

                };
            }

            return item;
        }
        public static Customer GetCustomerByCode(string code)
        {
            Customer item = new Customer();
            DataTable dt = DAL.GetCustomerByCode(code);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                item.Id = row.Field<int>("Id");
                item.EntegreId = row.Field<int>("EntegreId");
                item.B2bCode = row.Field<string>("B2bCode");
                item.Code = row.Field<string>("Code");
                item.Name = row.Field<string>("Name");
                item.Address = row.Field<string>("Address");
                item.City = row.Field<string>("City");
                item.Town = row.Field<string>("Town");
                item.Tel1 = row.Field<string>("Tel1");
                item.RuleCode = row.Field<string>("RuleCode");
                item.PaymentType = (PaymentType)(row.Field<int>("PaymentType"));
                item.NumberOfUser = row.Field<int>("NumberOfUser");
                item.NumberOfUserIos = row.Field<int>("NumberOfUserIos");
                item.NumberOfUserAndroid = row.Field<int>("NumberOfUserAndroid");
                item.SpecialInstallment = row.Field<bool>("SpecialInstallment");
                item.TaxOffice = row.Field<string>("TaxOffice");
                item.TaxNumber = row.Field<string>("TaxNumber");
                item.PaymentOnOrder = row.Field<bool>("PaymentOnOrder");
                item.ShipmentId = row.Field<int>("ShipmentId");
                item.IsCurrentAccountStatu = row.Field<bool>("IsCurrentAccountStatu");
                item.CurrentAccountPassword = MD5Sifreleme(row.Field<string>("CurrentAccountPassword"));
                item.CurrencyType = row.Field<string>("CurrencyType");
                item.CampaignStatu = row.Field<bool>("CampaignStatu");
                item.IsVatRateActive = row.Field<bool>("IsVatRateActive");
                item.Mail = row.Field<string>("Mail");
                item.VatRate = row.Field<double>("VatRate");
                item.IsCampaignCodeActive = row.Field<bool>("IsCampaignCodeActive");
                item.CampaignCode = row.Field<string>("CampaignCode");
                item.IsConfirmKvkk = row.Field<int>("IsConfirmKvkk");
                item.Users = new Users
                {
                    Id = row.Field<int>("UserId")
                };
            }

            return item;
        }
        public static List<CustomerSmall> GetListLimited(string codeOrName, int basket, string city, string town, bool salesmanStatus, int salesmanId, int pStart, int pList)
        {
            List<CustomerSmall> list = new List<CustomerSmall>();
            DataTable dt = DAL.GetCustomerListLimited(codeOrName, basket, city, town, salesmanStatus, salesmanId, pStart, pList);

            foreach (DataRow row in dt.Rows)
            {
                CustomerSmall item = new CustomerSmall()
                {
                    Id = row.Field<int>("Id"),
                    EntegreId = row.Field<int>("EntegreId"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    Address = row.Field<string>("Address"),
                    City = row.Field<string>("City"),
                    Town = row.Field<string>("Town"),
                    Tel1 = row.Field<string>("Tel1"),
                };
                list.Add(item);
            }

            return list;
        }

        public static List<Customer> GetCustomerListBySearch(string code, string name, string ruleCode)
        {
            List<Customer> customerList = new List<Customer>();
            DataTable dt = DAL.GetCustomerListBySearch(code, name, ruleCode);

            foreach (DataRow row in dt.Rows)
            {
                Customer customer = new Customer()
                {
                    Id = row.Field<int>("Id"),
                    EntegreId = row.Field<int>("EntegreId"),
                    Code = row.Field<string>("Code"),
                    B2bCode = row.Field<string>("B2bCode"),
                    Name = row.Field<string>("Name"),
                    Address = row.Field<string>("Address"),
                    Town = row.Field<string>("Town"),
                    City = row.Field<string>("City"),
                    TaxOffice = row.Field<string>("TaxOffice"),
                    TaxNumber = row.Field<string>("TaxNumber"),
                    Mail = row.Field<string>("Mail"),
                    Status = Convert.ToBoolean(row["Status"]),
                    Deleted = row.Field<bool>("Deleted"),
                    Tel1 = row.Field<string>("Tel1"),
                    Tel2 = row.Field<string>("Tel2"),
                    Fax1 = row.Field<string>("Fax1"),
                    Gsm1 = row.Field<string>("Gsm1"),
                    Gsm2 = row.Field<string>("Gsm2"),
                    Notes = row.Field<string>("Notes"),
                    RuleCode = row.Field<string>("RuleCode"),
                    RiskLimit = row.Field<double>("RiskLimit"),
                    CreditLimit = row.Field<double>("CreditLimit"),
                    Password = row.Field<string>("Password"),
                    SpecialInstallment = row.Field<bool>("SpecialInstallment"),
                    IsVatRateActive = row.Field<bool>("IsVatRateActive"),
                    VatRate = row.Field<double>("VatRate"),
                    NumberOfUser = row.Field<int>("NumberOfUser"),
                    NumberOfUserAndroid = row.Field<int>("NumberOfUserAndroid"),
                    NumberOfUserIos = row.Field<int>("NumberOfUserIos"),
                    CurrentAccountPassword = row.Field<string>("CurrentAccountPassword"),
                    IsCurrentAccountStatu = row.Field<bool>("IsCurrentAccountStatu"),
                    CampaignStatu = row.Field<bool>("CampaignStatu"),
                    PaymentOnOrder = row.Field<bool>("PaymentOnOrder"),
                    CurrencyType = row.Field<string>("CurrencyType"),
                    IsCampaignCodeActive = row.Field<bool>("IsCampaignCodeActive"),
                    CampaignCode = row.Field<string>("CampaignCode"),
                    IsConfirmKvkk = row.Field<int>("IsConfirmKvkk"),
                    IsStartScreen = row.Field<bool>("IsStartScreen"),
                    StartScreen = row.Field<string>("StartScreen")
                };

                customerList.Add(customer);
            }


            return customerList;
        }
        public static List<Customer> GetListCustomerBySalesmanId(int pSalesmanId)
        {
            DataTable dt = new DataTable();
            List<Customer> list = new List<Customer>();

            dt = DAL.GetListCustomerBySalesmanId(pSalesmanId);

            foreach (DataRow row in dt.Rows)
            {
                Customer c = new Customer()
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    City = row.Field<string>("City"),
                    Address = row.Field<string>("Address"),
                    Tel1 = row.Field<string>("Tel1"),
                    Mail = row.Field<string>("Mail")
                };
                list.Add(c);
            }

            return list;
        }
        public static List<Customer> GetListCustomersBySalesmanId(int pSalesmanId)
        {
            DataTable dt = new DataTable();
            List<Customer> list = new List<Customer>();

            dt = DAL.GetListCustomersBySalesmanId(pSalesmanId);

            foreach (DataRow row in dt.Rows)
            {
                Customer c = new Customer()
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    City = row.Field<string>("City"),
                    Address = row.Field<string>("Address"),
                    Tel1 = row.Field<string>("Tel1"),
                    Mail = row.Field<string>("Mail")
                };
                list.Add(c);
            }

            return list;
        }

        public static List<Customer> GetListNotConnectedCustomerBySalesmanId(int salesmanId)
        {
            DataTable dt = new DataTable();
            List<Customer> list = new List<Customer>();

            dt = DAL.GetListNotConnectedCustomerBySalesmanId(salesmanId);

            foreach (DataRow row in dt.Rows)
            {
                Customer c = new Customer()
                {
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    RuleCode = row.Field<string>("RuleCode")
                };
                list.Add(c);
            }

            return list;
        }



        public static List<CustomerSmall> GetCityList()
        {
            List<CustomerSmall> list = new List<CustomerSmall>();
            DataTable dt = DAL.GetCustomerCityList();

            foreach (DataRow row in dt.Rows)
            {
                CustomerSmall obj = new CustomerSmall()
                {
                    City = row.Field<string>("City")
                };
                list.Add(obj);
            }

            return list;
        }
        public static List<CustomerSmall> GetTownList(string city)
        {
            List<CustomerSmall> list = new List<CustomerSmall>();
            DataTable dt = DAL.GetCustomerTownList(city);

            foreach (DataRow row in dt.Rows)
            {
                CustomerSmall obj = new CustomerSmall()
                {
                    Town = row.Field<string>("Town")
                };
                list.Add(obj);
            }

            return list;
        }

        public DataTable Add()
        {
            return DAL.InsertCustomer(Code, "ERYAZ", B2bCode, Name, Address, Town, City, TaxOffice, TaxNumber, Mail, (int)PaymentType, Tel1, Tel2, Fax1, Gsm1, Gsm2, Notes, RuleCode, NumberOfUser, NumberOfUserAndroid, NumberOfUserIos, RiskLimit, CreditLimit, SpecialInstallment, PaymentOnOrder, IsCurrentAccountStatu, CurrentAccountPassword, CreateId, CampaignStatu, CurrencyType, Salesman.Id, IsVatRateActive, VatRate, IsCampaignCodeActive, CampaignCode);
        }

        public bool Update()
        {
            return DAL.UpdateCustomer(Code, B2bCode, Name, Address, Town, City, TaxOffice, TaxNumber, Mail, (int)PaymentType, Tel1, Tel2, Fax1, Gsm1, Gsm2, Notes, RuleCode, NumberOfUser, NumberOfUserAndroid, NumberOfUserIos, RiskLimit, CreditLimit, SpecialInstallment, PaymentOnOrder, IsCurrentAccountStatu, CurrentAccountPassword, EditId, CampaignStatu, CurrencyType, Salesman.Id, Id, IsVatRateActive, VatRate, IsCampaignCodeActive, CampaignCode, IsConfirmKvkk, IsStartScreen, StartScreen);
        }

        public bool Delete()
        {
            return DAL.DeleteCustomer(EditId, Id);
        }

        public bool DeleteCustomerUser(int id, int EditId)
        {
            return DAL.DeleteCustomerUser(id, EditId);
        }
        public bool UpdateUser()
        {
            return DAL.UpdateUserPassword(Id, Users.Id, Users.Password, Users.Rate, Users.Avatar);
        }

        public static bool UpdateCustomerCurrentAccountPassword(int pId, string pCurrentAccountPassword)
        {
            return DAL.UpdateCustomerCurrentAccountPassword(pId, pCurrentAccountPassword);
        }

        #endregion

    }
    public enum PaymentType
    {
        Cash = 0,   // Peşin
        Due = 1     // Vadeli
    }

    public partial class DataAccessLayer
    {
        public bool UpdateCustomerCurrentAccountPassword(int pId, string pCurrentAccountPassword)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_CustomerCurrentAccountPassword", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pCurrentAccountPassword });

        }

        public bool UpdateUserPassword(int pCustomerId, int pUserId, string pPassword, double pRate, string pAvatar)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Update_Customer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId, pPassword, pRate, pAvatar });

        }
        public bool DeleteCustomer(int pEditId, int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_Customer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pEditId, pId });

        }

        public bool DeleteCustomerUser(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_Customer_User", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });

        }

        public DataTable InsertCustomer(string pCode, string pPassword, string pB2bCode, string pName, string pAddress, string pTown, string pCity, string pTaxOffice, string pTaxNumber, string pMail, int pPaymentType, string pTel1, string pTel2, string pFax1,
        string pGsm1, string pGsm2, string pNotes, string pRuleCode, int pNumberOfUser, int pNumberOfUserAndroid, int pNumberOfUserIos, double pRiskLimit, double pCreditLimit, bool pSpecialInstallment, bool pPaymentOnOrder, bool pIsCurrentAccountStatu, string pCurrentAccountPassword, int pCreateId, bool pCampaignStatu, string pCurrencyType, int pSalesmanId, bool pIsVatRateActive, double pVatRate, bool pIsCampaignCodeActive, string pCampaignCode)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_Insert_Customer",
                MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCode, pPassword, pB2bCode, pName, pAddress, pTown, pCity, pTaxOffice, pTaxNumber, pMail, pPaymentType, pTel1, pTel2, pFax1, pGsm1, pGsm2, pNotes, pRuleCode, pNumberOfUser, pNumberOfUserAndroid, pNumberOfUserIos, pRiskLimit, pCreditLimit, pSpecialInstallment, pPaymentOnOrder, pIsCurrentAccountStatu, pCurrentAccountPassword, pCreateId, pCampaignStatu, pCurrencyType, pSalesmanId, pIsVatRateActive, pVatRate, pIsCampaignCodeActive, pCampaignCode });
        }
        public bool UpdateCustomer(string pCode, string pB2bCode, string pName, string pAddress, string pTown, string pCity, string pTaxOffice, string pTaxNumber, string pMail, int pPaymentType, string pTel1, string pTel2, string pFax1,
             string pGsm1, string pGsm2, string pNotes, string pRuleCode, int pNumberOfUser, int pNumberOfUserAndroid, int pNumberOfUserIos, double pRiskLimit, double pCreditLimit, bool pSpecialInstallment, bool pPaymentOnOrder, bool pIsCurrentAccountStatu, string pCurrentAccountPassword, int pEditId, bool pCampaignStatu, string pCurrencyType, int pSalesmanId, int pCustomerId, bool pIsVatRateActive, double pVatRate, bool pIsCampaignCodeActive, string pCampaignCode, int pIsConfirmKvkk, bool pIsStartScreen, string pStartScreen)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Customer",
                MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCode, pB2bCode, pName, pAddress, pTown, pCity, pTaxOffice, pTaxNumber, pMail, pPaymentType, pTel1, pTel2, pFax1, pGsm1, pGsm2, pNotes, pRuleCode, pNumberOfUser, pNumberOfUserAndroid, pNumberOfUserIos, pRiskLimit, pCreditLimit, pSpecialInstallment, pPaymentOnOrder, pIsCurrentAccountStatu, pCurrentAccountPassword, pEditId, pCampaignStatu, pCurrencyType, pSalesmanId, pCustomerId, pIsVatRateActive, pVatRate, pIsCampaignCodeActive, pCampaignCode, pIsConfirmKvkk, pIsStartScreen, pStartScreen });
        }

        public DataTable GetCustomerById(int pCustomerId, int pUserId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_CustomerById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId });
        }
        public DataTable GetListCustomerBySalesmanId(int pSalesmanId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetItem_CustomerBySalesmanId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSalesmanId });
        }
        public DataTable GetListCustomersBySalesmanId(int pSalesmanId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetItem_CustomersBySalesmanId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSalesmanId });
        }
        public DataTable GetCustomerListLimited(string pCodeOrName, int pBasket, string pCity, string pTown, bool pSalesmanStatus, int pSalesmanID, int pStart, int pList)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_CustomerListLimited", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCodeOrName, pBasket, pCity, pTown, pSalesmanStatus, pSalesmanID, pStart, pList });
        }
        public DataTable GetCustomerListBySearch(string pCode, string pName, string pRuleCode)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_Customer_Search", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCode, pName, pRuleCode });
        }
        public DataTable GetCustomerByCode(string pCode)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_CustomerByCode", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCode });
        }
        public DataTable GetCustomerCityList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Customer_City", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }
        public DataTable GetCustomerTownList(string pCity)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Customer_Town", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCity });
        }

        public DataTable GetListNotConnectedCustomerBySalesmanId(int pSalesmanId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_NotConnectedCustomerBySalesmanId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSalesmanId });
        }
    }


}