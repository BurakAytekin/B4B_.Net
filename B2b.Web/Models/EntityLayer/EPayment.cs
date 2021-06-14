using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;


namespace B2b.Web.v4.Models.EntityLayer
{
    public class EPayment : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CardNumber { get; set; }
        public string CardCvv { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string CardType { get; set; }
        public string Amount { get; set; }
        public string Installment { get; set; }
        public DateTime? ProcessingDate { get; set; }
        public int CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string NameSurname { get; set; }
        public string BankName { get; set; }
        public string ErrMsg { get; set; }
        public string IpAddress { get; set; }
        public string ProcReturnCode { get; set; }
        public string TransId { get; set; }
        public string Oid { get; set; }
        public string GroupId { get; set; }
        public string Appr { get; set; }
        public string Extra { get; set; }
        public string Result { get; set; }
        public string AuthCode { get; set; }
        public double Total { get; set; }
        public double TotalStr { get; set; }
        public string Discount { get; set; }
        public string BankId { get; set; }
        public string UseEPaymentType { get; set; }
        public string PaymentId { get; set; }
        public string HashParams { get; set; }
        public string HashParamsVal { get; set; }
        public string Hash { get; set; }
        public string HostRefNum { get; set; }
        public string ResponseFromServer { get; set; }
        public string OrderId { get; set; }
        public string Sipbil { get; set; }
        public string Sesbil { get; set; }
        public string UyeRef { get; set; }
        public string VbRef { get; set; }
        public string CampaignUrl { get; set; }
        public Customer Customer { get; set; }
        public DateTime RecordDate { get; set; }
        public string ExpendableBonus { get; set; }
        public int Count { get; set; }
        public string Note { get; set; }
        public string PhoneNumber { get; set; }
        public bool _3DSecure { get; set; }
        public bool UseBonus { get; set; }
        public string CustomerTotalBonus { get; set; }
        public int EpaymentStatus { get; set; }
        #endregion

        #region Methods
        public static DataTable Insert(string pCardNumber, string pNameSurname, string pExpMonth, string pExpYear, string pCvc, string pAmount, string pInstallment, int pCustomerId, string pCustomerCode, string pCustomerName, string pBankName, double pTotal, string pRate, int pBankId, string pPaymentId, int pSystemType, string pPhoneNumber, string pNote, string pCardType, int pSalesmanId, string pExtraInstallment, string pCustomerTotalBonus, string pExpendableBonus, bool pUseBonus = false, bool p3DSecure = true, int pGroupId = -1)
        {
            return DAL.InsertPayment(pCardNumber, pNameSurname, pExpMonth, pExpYear, pCvc, pAmount, pInstallment, pCustomerId, pCustomerCode, pCustomerName, pBankName, pTotal, pRate, pBankId, pPaymentId, pSystemType, pPhoneNumber, pNote, pCardType, pSalesmanId, pExtraInstallment, pCustomerTotalBonus, pExpendableBonus, pUseBonus, p3DSecure, pGroupId);
        }
        public static void Update(int pId, string pAuthCode, string pProcReturnCode, string pErrMsg, string pIpAddress, string pOrderId, string pOid, string pGroupId, string pTransId, string pExtra, string pUseEPaymentType, DateTime pProcessingDate, int pUseEpaymentId, string pCampaignUrl = "")
        {
            DAL.UpdatePayment(pId, pAuthCode, pProcReturnCode, pErrMsg, pIpAddress, pOrderId, pOid, pGroupId, pTransId, pExtra, pUseEPaymentType, pProcessingDate, pUseEpaymentId, pCampaignUrl);
        }

        public static EPayment GetListByEpaymentById(int pId)
        {
            EPayment epaymentList = new EPayment();
            DataTable dt = DAL.GetListByEpaymentById(pId);

            foreach (DataRow row in dt.Rows)
            {
                EPayment ePayments = new EPayment()
                {
                    Id = row.Field<int>("Id"),
                    CardNumber = row.Field<string>("CardNumber"),
                    NameSurname = row.Field<string>("NameSurname"),
                    BankName = row.Field<string>("BankName"),
                    RecordDate = row.Field<DateTime>("RecordDate"),
                    Total = Convert.ToDouble(row["Total"]),
                    Amount = row.Field<string>("Amount"),
                    Installment = row.Field<string>("Installment"),
                    Result = row.Field<string>("Result"),
                    AuthCode = row.Field<string>("AuthCode"),
                    UseEPaymentType = row.Field<string>("UseEPaymentType"),
                    PaymentId = row.Field<string>("PaymentId"),
                    ProcReturnCode = row.Field<string>("ProcReturnCode"),
                    ExpendableBonus = row.Field<string>("ExpendableBonus"),
                    Customer = new Customer()
                    {
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        Tel1 = row.Field<string>("Tel1"),
                        Mail = row.Field<string>("Mail")
                    }

                };
                epaymentList = (ePayments);
            }

            return epaymentList;
        }


        public static List<EPayment> GetListByCustomerId(int pCustomerId)
        {
            List<EPayment> epaymentList = new List<EPayment>();
            DataTable dt = DAL.GetEPaymentListByCustomerId(pCustomerId);

            foreach (DataRow row in dt.Rows)
            {
                EPayment ePayments = new EPayment()
                {
                    Id = row.Field<int>("Id"),
                    CardNumber = row.Field<string>("CardNumber"),
                    NameSurname = row.Field<string>("NameSurname"),
                    BankName = row.Field<string>("BankName"),
                    ProcessingDate = row.Field<DateTime?>("ProcessingDate"),
                    Total = Convert.ToDouble(row["Total"]),
                    Amount = row.Field<string>("Amount"),
                    Installment = row.Field<string>("Installment"),
                    Result = row.Field<string>("Result"),
                    AuthCode = row.Field<string>("AuthCode"),
                    UseEPaymentType = row.Field<string>("UseEPaymentType"),
                    PaymentId = row.Field<string>("PaymentId"),
                    ProcReturnCode = row.Field<string>("ProcReturnCode"),
                    ExpendableBonus = row.Field<string>("ExpendableBonus"),
                    Note = row.Field<string>("Note"),
                    Customer = new Customer()
                    {
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        Tel1 = row.Field<string>("Tel1")
                    }

                };
                epaymentList.Add(ePayments);
            }

            return epaymentList;
        }

        public static List<EPayment> GetListEpayment(DateTime startDate, DateTime endDate, string t9Text, int paymentStatu)
        {
            List<EPayment> epaymentList = new List<EPayment>();
            DataTable dt = DAL.GetListEpayment(startDate, endDate, t9Text, paymentStatu);

            foreach (DataRow row in dt.Rows)
            {
                EPayment ePayments = new EPayment()
                {
                    Id = row.Field<int>("Id"),
                    CardNumber = row.Field<string>("CardNumber"),
                    NameSurname = row.Field<string>("NameSurname"),
                    BankName = row.Field<string>("BankName"),
                    ProcessingDate = row.Field<DateTime?>("ProcessingDate"),
                    ProcReturnCode = row.Field<string>("ProcReturnCode"),
                    Amount = row.Field<string>("Amount"),
                    Total = Convert.ToDouble(row["Total"]),
                    Installment = row.Field<string>("Installment"),
                    Result = row.Field<string>("Result"),
                    AuthCode = row.Field<string>("AuthCode"),
                    ErrMsg = row.Field<string>("ErrMsg"),
                    IpAddress = row.Field<string>("IpAddress"),
                    TransId = row.Field<string>("TransId"),
                    Oid = row.Field<string>("Oid"),
                    Appr = row.Field<string>("Appr"),
                    Extra = row.Field<string>("Extra"),
                    OrderId = row.Field<string>("OrderId"),
                    PaymentId = row.Field<string>("PaymentId"),
                    UseEPaymentType = row.Field<string>("UseEPaymentType"),
                    Note = row.Field<string>("Note"),
                    PhoneNumber = row.Field<string>("PhoneNumber"),
                    _3DSecure = row.Field<bool>("_3DSecure"),
                    UseBonus = row.Field<bool>("UseBonus"),
                    CustomerTotalBonus = row.Field<string>("CustomerTotalBonus"),
                    ExpendableBonus = row.Field<string>("ExpendableBonus"),
                    EpaymentStatus = row.Field<int>("EpaymentStatus"),
                    Customer = new Customer()
                    {
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        Tel1 = row.Field<string>("Tel1")
                    }

                };
                epaymentList.Add(ePayments);
            }

            return epaymentList;
        }

        public static bool UpdatePaymentStatus(int pId, int pStatus)
        {
            return DAL.UpdatePaymentStatus(pId, pStatus);
        }
        public static EPayment GetItemPayment(int pInsertId)
        {
            EPayment epayment = new EPayment();
            DataTable dt = DAL.GetItemEPaymentByInsertId(pInsertId);

            foreach (DataRow row in dt.Rows)
            {
                epayment = new EPayment()
                {
                    Id = row.Field<int>("Id"),
                    CardNumber = row.Field<string>("CardNumber"),
                    NameSurname = row.Field<string>("NameSurname"),
                    BankName = row.Field<string>("BankName"),
                    ProcessingDate = row.Field<DateTime?>("ProcessingDate"),
                    Amount = row.Field<string>("Amount"),
                    Installment = row.Field<string>("Installment"),
                    Result = row.Field<string>("Result"),
                    AuthCode = row.Field<string>("AuthCode"),
                    UseEPaymentType = row.Field<string>("UseEPaymentType"),
                    PaymentId = row.Field<string>("PaymentId"),
                    ProcReturnCode = row.Field<string>("ProcReturnCode"),
                    ExpendableBonus = row.Field<string>("ExpendableBonus"),
                    PhoneNumber = row.Field<string>("PhoneNUmber"),
                    CampaignUrl = row.Field<string>("CampaignUrl"),
                    Customer = new Customer()
                    {
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),

                    }

                };

            }

            return epayment;
        }

        public static double GetSuccessPaymentTotal(Customer customer)
        {
            EPayment epaymentList = new EPayment();
            DataTable dt = DAL.GetSuccessPaymentTotal(customer.Id);



            return Convert.ToDouble(dt.Rows[0][0]);
        }

        #endregion
    }
    public partial class DataAccessLayer
    {
        public DataTable GetSuccessPaymentTotal(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Payment_GetSuccessPaymentTotal", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }

        public DataTable GetListByEpaymentById(int pId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Payment_GetListEPaymentListById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }

        public bool UpdatePaymentStatus(int pId, int pStatus,string pEntegreNo="")
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_PaymentStatus", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pStatus,pEntegreNo });
        }

        public DataTable InsertPayment(string pCardNumber, string pNameSurname, string pExpMonth, string pExpYear, string pCvc, string pAmount, string pInstallment, int pCustomerId, string pCustomerCode, string pCustomerName, string pBankName, double pTotal, string pRate, int pBankId, string pPaymentId, int pSystemType, string pPhoneNumber, string pNote, string pCardType, int pSalesmanId, string pExtraInstallment, string pCustomerTotalBonus, string pExpendableBonus, bool pUseBonus, bool p3DSecure, int pGroupId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Payment_Insert_Payment", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCardNumber, pNameSurname, pExpMonth, pExpYear, pCvc, pAmount, pInstallment, pCustomerId, pCustomerCode, pCustomerName, pBankName, pTotal, pRate, pBankId, pPaymentId, pSystemType, pPhoneNumber, pNote, pCardType, pSalesmanId, pExtraInstallment, pCustomerTotalBonus, pExpendableBonus, pUseBonus, p3DSecure, pGroupId });
        }
        public bool UpdatePayment(int pId, string pAuthCode, string pProcReturnCode, string pErrMsg, string pIpAddress, string pOrderId, string pOid, string pGroupId, string pTransId, string pExtra, string pUseEPaymentType, DateTime pProcessingDate, int pUseEpaymentId, string pCampaignUrl)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Payment_Update_Payment", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pAuthCode, pProcReturnCode, pErrMsg, pIpAddress, pOrderId, pOid, pGroupId, pTransId, pExtra, pUseEPaymentType, pProcessingDate, pUseEpaymentId, pCampaignUrl });
        }
        public DataTable GetEPaymentListByCustomerId(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Payment_GetListEPaymentListByCustomerId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }
        public DataTable GetListEpayment(DateTime pStartDate, DateTime pEndDate, string pT9Text, int pPaymentStatu)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetListEPayment", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pStartDate, pEndDate, pT9Text, pPaymentStatu });
        }
        public DataTable GetItemEPaymentByInsertId(int pInsertId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_PaymentByInsertId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pInsertId });
        }
    }
}