using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using B2b.Web.v4.Models.Helper;


namespace B2b.Web.v4.Models.EntityLayer
{
    public class Salesman : DataAccess
    {
        #region Properties
        public AuthoritySalesman AuthoritySalesman { get; set; }
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Tel1 { get; set; }
        public string Tel2 { get; set; }
        public string Email { get; set; }
        public string EmailShort { get { return (Email != null && Email.Length > 23 ? Email.Substring(0, 23) : Email); } }
        public string PicturePath { get; set; }
        public string Password { get; set; }
        public string Prefix { get; set; }
        public string Message { get; set; }
        public int NumberOfUserB2b { get; set; }
        public int NumberOfUserAndroid { get; set; }
        public int NumberOfUserIos { get; set; }
        public int IntervalTime { get; set; }
        public int NumberOfUser { get; set; }
        public bool CustomerType { get; set; }
        public int NumberOfUserModerator { get; set; }
        public string AvatarPath { get { return (String.IsNullOrEmpty(Avatar) ? "/Content/images/avatar/noavatar.png" : "/Content/images/avatar/" + Avatar); } }
        public string Avatar { get; set; }
        // Login işlemi için 
        public string LoginIp { get; set; }
        public DateTime LoginTime { get; set; }
        public int TerminalNo { get; set; }
        public bool Locked { get; set; }
        public int TryCount { get; set; }
        public int TryCountStr { get { return (3 - TryCount); } }
        public bool IsAutoLock { get; set; }
        public bool IsSystemUser { get; set; }
        public bool IsAuthenticator { get; set; }
        public bool IsB2bAuthenticator { get; set; }
        public string AuthenticatorGuid { get; set; }
        #endregion

        #region Constructor
        public Salesman()
        {
            Id = -1;
            Code = string.Empty;
            Name = string.Empty;
            Tel1 = string.Empty;
            Tel2 = string.Empty;
            Email = string.Empty;
            TerminalNo = -1;
        }
        #endregion

        #region Methods

        public static Salesman GetById(int pId)
        {
            Salesman item = new Salesman();
            DataTable dt = DAL.GetSalesmanById(pId);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                item.Id = row.Field<int>("Id");
                item.Code = row.Field<string>("Code");
                item.Name = row.Field<string>("Name");
                item.Tel1 = row.Field<string>("Tel1");
                item.Tel2 = row.Field<string>("Tel2");
                item.Email = row.Field<string>("Email");
                item.Password = row.Field<string>("Password");
                item.NumberOfUserB2b = row.Field<int>("NumberOfUserB2b");
                item.NumberOfUserAndroid = row.Field<int>("NumberOfUserAndroid");
                item.NumberOfUserModerator = row.Field<int>("NumberOfUserModerator");
                item.NumberOfUserIos = row.Field<int>("NumberOfUserIos");
                item.IsAutoLock = row.Field<bool>("IsAutoLock");
                item.IntervalTime = row.Field<int>("ModeratorIntervalTime");
                item.IsAuthenticator = row.Field<bool>("IsAuthenticator");
                item.IsB2bAuthenticator = row.Field<bool>("IsB2bAuthenticator");
                item.IsSystemUser = row.Field<bool>("IsSystemUser");
                item.AuthenticatorGuid = row.Field<string>("AuthenticatorGuid");
                item.PicturePath = row.Field<string>("PicturePath") == string.Empty || row.Field<string>("PicturePath") == null ? "../Content/Admin/images/noavatar.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath");
                item.AuthoritySalesman = new AuthoritySalesman()
                {
                    SalesmanId = row.Field<int>("Id"),
                    CampaignStatu = row.Field<bool>("CampaignStatu"),
                    ProductRestoration = row.Field<bool>("ProductRestoration"),
                    LockSalesman = row.Field<bool>("LockSalesman"),
                    Collecting = row.Field<bool>("Collecting"),
                    EnteringInformation = row.Field<bool>("EnteringInformation"),
                    CheckBasket = row.Field<bool>("CheckBasket"),
                    CustomerType = row.Field<bool>("CustomerType"),
                    HidePassword = row.Field<bool>("HidePassword"),
                    WebLogin = row.Field<bool>("WebLogin"),
                    IsSpecDiscount = row.Field<bool>("IsSpecDiscount"),
                    SendPool = row.Field<bool>("SendPool"),
                };
            }

            return item;
        }


        public static Salesman GetByCode(string pCode)
        {
            Salesman item = new Salesman();
            DataTable dt = DAL.GetSalesmanByCode(pCode);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                item.Id = row.Field<int>("Id");
                item.Code = row.Field<string>("Code");
                item.Name = row.Field<string>("Name");
                item.Tel1 = row.Field<string>("Tel1");
                item.Tel2 = row.Field<string>("Tel2");
                item.Email = row.Field<string>("Email");
                item.Password = row.Field<string>("Password");
                item.NumberOfUserB2b = row.Field<int>("NumberOfUserB2b");
                item.NumberOfUserAndroid = row.Field<int>("NumberOfUserAndroid");
                item.NumberOfUserModerator = row.Field<int>("NumberOfUserModerator");
                item.NumberOfUserIos = row.Field<int>("NumberOfUserIos");
                item.IsAutoLock = row.Field<bool>("IsAutoLock");
                item.IntervalTime = row.Field<int>("ModeratorIntervalTime");
                item.IsAuthenticator = row.Field<bool>("IsAuthenticator");
                item.IsB2bAuthenticator = row.Field<bool>("IsB2bAuthenticator");
                item.IsSystemUser = row.Field<bool>("IsSystemUser");
                item.AuthenticatorGuid = row.Field<string>("AuthenticatorGuid");
                item.PicturePath = row.Field<string>("PicturePath") == string.Empty || row.Field<string>("PicturePath") == null ? "../Content/Admin/images/noavatar.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath");
                item.AuthoritySalesman = new AuthoritySalesman()
                {
                    SalesmanId = row.Field<int>("Id"),
                    CampaignStatu = row.Field<bool>("CampaignStatu"),
                    ProductRestoration = row.Field<bool>("ProductRestoration"),
                    LockSalesman = row.Field<bool>("LockSalesman"),
                    Collecting = row.Field<bool>("Collecting"),
                    EnteringInformation = row.Field<bool>("EnteringInformation"),
                    CheckBasket = row.Field<bool>("CheckBasket"),
                    CustomerType = row.Field<bool>("CustomerType"),
                    HidePassword = row.Field<bool>("HidePassword"),
                    WebLogin = row.Field<bool>("WebLogin"),
                    IsSpecDiscount = row.Field<bool>("IsSpecDiscount"),
                    SendPool=row.Field<bool>("SendPool"),
                };
            }

            return item;
        }
        public static List<Salesman> GetList()
        {
            List<Salesman> salesmanList = new List<Salesman>();
            DataTable dt = DAL.GetSalesmanByList();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Salesman salesman = new Salesman();
                    {
                        salesman.Id = row.Field<int>("Id");
                        salesman.Code = row.Field<string>("Code");
                        salesman.Name = row.Field<string>("Name");
                        salesman.Tel1 = row.Field<string>("Tel1");
                        salesman.Tel2 = row.Field<string>("Tel2");
                        salesman.Email = row.Field<string>("Email");
                    };
                    salesmanList.Add(salesman);
                }
            }
            return salesmanList;
        }
        public static List<Salesman> GetList(int salesmanId)
        {
            List<Salesman> salesmanList = new List<Salesman>();
            DataTable dt = DAL.GetSalesmanByList(salesmanId);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Salesman salesman = new Salesman();
                    {
                        salesman.Id = row.Field<int>("Id");
                        salesman.Code = row.Field<string>("Code");
                        salesman.Name = row.Field<string>("Name");
                        salesman.Tel1 = row.Field<string>("Tel1");
                        salesman.Tel2 = row.Field<string>("Tel2");
                        salesman.Email = row.Field<string>("Email");
                    };
                    salesmanList.Add(salesman);
                }
            }
            return salesmanList;
        }
        public static List<Salesman> GetList(string pName)
        {
            List<Salesman> salesmanList = new List<Salesman>();
            DataTable dt = DAL.GetSalesmanByList(pName);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    Salesman salesman = new Salesman();
                    {
                        salesman.Id = row.Field<int>("Id");
                        salesman.Code = row.Field<string>("Code");
                        salesman.Name = row.Field<string>("Name");
                        salesman.Tel1 = row.Field<string>("Tel1");
                        salesman.Tel2 = row.Field<string>("Tel2");
                        salesman.Email = row.Field<string>("Email");
                        salesman.Prefix = row.Field<string>("Prefix");
                        salesman.Message = row.Field<string>("Message");
                        salesman.NumberOfUser = row.Field<int>("NumberOfUserB2b");
                        salesman.NumberOfUserAndroid = row.Field<int>("NumberOfUserAndroid");
                        salesman.NumberOfUserIos = row.Field<int>("NumberOfUserIos");
                        salesman.NumberOfUserModerator = row.Field<int>("NumberOfUserModerator");
                        salesman.IntervalTime = row.Field<int>("ModeratorIntervalTime");
                        salesman.IsAutoLock = row.Field<bool>("IsAutoLock");
                        salesman.Password = row.Field<string>("Password");
                        salesman.IsAuthenticator = row.Field<bool>("IsAuthenticator");
                        salesman.IsB2bAuthenticator = row.Field<bool>("IsB2bAuthenticator");
                        salesman.AuthenticatorGuid = row.Field<string>("AuthenticatorGuid");
                        salesman.PicturePath = row.Field<string>("PicturePath") == string.Empty || row.Field<string>("PicturePath") == null ? "../Content/images/nophoto.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath");

                    };
                    salesmanList.Add(salesman);
                }
            }
            return salesmanList;
        }
        public static Salesman GetSalesmanByCustomerId(int customerId)
        {
            DataTable dt = DAL.GetSalesmanByCustomerId(customerId);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                Salesman salesman = new Salesman()
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    Prefix = row.Field<string>("Prefix"),

                    Message = row.Field<string>("Message"),
                    NumberOfUser = row.Field<int>("NumberOfUserB2b"),
                    IntervalTime = row.Field<int>("ModeratorIntervalTime"),
                    Password = row.Field<string>("Password"),
                    Tel1 = row.Field<string>("Tel1"),
                    Tel2 = row.Field<string>("Tel2"),
                    Email = row.Field<string>("Email"),
                    NumberOfUserAndroid = row.Field<int>("NumberOfUserAndroid"),
                    NumberOfUserIos = row.Field<int>("NumberOfUserIos"),
                    NumberOfUserModerator = row.Field<int>("NumberOfUserModerator"),

                };
                return salesman;
            }
            else
            {
                return null;
            }
        }

        public bool AddOrUpdate()
        {
            #region Null Control
            Tel1 = Tel1 ?? string.Empty;
            Tel2 = Tel2 ?? String.Empty;
            Email = Email ?? String.Empty;
            Prefix = Prefix ?? String.Empty;
            Message = Message ?? String.Empty;
            #endregion

            return DAL.InsertOrUpdate(Id, Code, Name, Password, Tel1, Tel2, Email, Prefix, Message, NumberOfUser, NumberOfUserAndroid, NumberOfUserIos, NumberOfUserModerator, IsAutoLock, IntervalTime, IsAuthenticator, IsB2bAuthenticator, CreateId, EditId);
        }

        public bool UpdateAuthenticatorValue()
        {
            return DAL.UpdateAuthenticatorValue(Id, AuthenticatorGuid, EditId);
        }

        public static bool InsertOrDeleteSalesmanOfCustomer(int SalesmanId, string CustomerIds, int CreateId, int type)
        {
            return DAL.InsertOrDeleteSalesmanOfCustomer(SalesmanId, CustomerIds, CreateId, type);

        }

        public bool Delete()
        {
            return DAL.DeleteSalesman(Id, Deleted, EditId);
        }

        public static bool UpdateSalesmanPassword(int pId, string pPassword)
        {
            return DAL.UpdateSalesmanPassword(pId, pPassword);
        }


        public static bool UpdatePicturePath(string pPath, int pSalesmanId, int pEditId)
        {
            #region Null Control
            pPath = pPath ?? String.Empty;
            #endregion
            return DAL.UpdateSalesmanPicture(pSalesmanId, pPath, pEditId);
        }
        #endregion

    }

    public partial class DataAccessLayer
    {
        public bool DeleteSalesman(int pId, bool pDeleted, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_DeleteSalesman", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pDeleted, pEditId });
        }


        public bool UpdateSalesmanPassword(int pId, string pPassword)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_SalesmanPassword", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pPassword });
        }

        public DataTable GetSalesmanById(int pId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_SalesmanById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }

        public DataTable GetSalesmanByCode(string pCode)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_SalesmanByCode", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCode });
        }

        public DataTable GetSalesmanByList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Salesman", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }
        public DataTable GetSalesmanByList(int pSalesmanId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_SalesmanById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSalesmanId });
        }
        public DataTable GetSalesmanByList(string pName)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_SalesmanByName", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pName });
        }
        public DataTable GetSalesmanByCustomerId(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetItem_SalesmanByCustomerId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }


        public bool UpdateSalesmanPicture(int pId, string pPath, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_SalesmanPicutre", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pPath, pEditId });
        }

        public bool InsertOrDeleteSalesmanOfCustomer(int pSalesmanId, string pCustomerId, int pCreateId, int pType)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_InsertOrDelete_SalesmanOfCustomer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSalesmanId, pCustomerId, pCreateId, pType });
        }


        public bool InsertOrUpdate(int pId, string pCode, string pName, string pPassword, string pTel1, string pTel2, string pEmail, string pPrefix, string pMessage, int pNumberOfUser, int pNumberOfUserAndroid, int pNumberOfUserIos, int pNumberOfUserModerator, bool pIsAutoLock, int pIntervalTime, bool pIsAuthenticator, bool pIsB2bAuthenticator, int pCreateId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Or_Update_Salesman", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pCode, pName, pPassword, pTel1, pTel2, pEmail, pPrefix, pMessage, pNumberOfUser, pNumberOfUserAndroid, pNumberOfUserIos, pNumberOfUserModerator, pIsAutoLock, pIntervalTime, pIsAuthenticator, pIsB2bAuthenticator, pCreateId, pEditId });
        }

        public bool UpdateAuthenticatorValue(int pId, string pAuthenticatorGuid, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_UpdateAuthenticatorGuid", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pAuthenticatorGuid, pEditId });
        }

    }
}