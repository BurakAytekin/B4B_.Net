using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using B2b.Web.v4.Models.Log.Entites;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace B2b.Web.v4
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class LoginService : WebService
    {
        [WebMethod]
        public LoginServiceReturnClass Login(int type, string customerCode, string code, string password, string pBoardSN, string pBiosSN, string pHddModel, string pHddSN, string pCpuID, string pCpuName, int pSource, string pArchitecture, string pServicePack, string pComputerName, string pLoginName, string pCaption, string pIpAddress)
        {
            Logon logon = new Logon();
            logon.CustomerCode = customerCode;
            logon.UserCode = code;
            logon.Password = password;


            int logonId = -1;
            string logonCode = string.Empty;
            int userId = -1;
            string userCode = string.Empty;

            int maxUser = 0;

            if (type == 0)
            {
                Customer cust = logon.CustomerLogin();
                if (cust != null)
                {
                    logonId = cust.Id;
                    logonCode = cust.Code;
                    userId = cust.Users.Id;
                    userCode = cust.Users.Code;
                    maxUser = cust.NumberOfUser;
                }
            }
            else if (type == 1)
            {
                Salesman sals = logon.SalesmanLogin();
                if (sals != null)
                {
                    logonId = sals.Id;
                    logonCode = sals.Code;
                    maxUser = sals.NumberOfUserB2b;
                }
            }
            else if (type == 2)
            {
                Salesman salesman = logon.AdminSalesmanLogin();
                if (salesman != null && salesman.Id > 0)
                {
                    logonId = salesman.Id;
                    logonCode = salesman.Code;
                    maxUser = salesman.NumberOfUserModerator;
                }
            }
            else
                logonId = -1;

            if (logonId == -1)
            {
                LogLogin log = new LogLogin()
                {
                    Type = ((LoginType)type).ToString(),
                    B2bCode = logon.CustomerCode,
                    UserCode = logon.UserCode,
                    Pass = logon.Password
                };
                Logger.LogTransaction(ClientType.B2BWpf, LogTransactionSource.Login, ProcessLogin.Fail.ToString(), Serializer.Serialize(log).InnerXml, pIpAddress);
                return new LoginServiceReturnClass(false, Guid.Empty, "Hatalı kullanıcı adı/şifre.");
            }

            Licence licence = Licence.CheckLicence(pBoardSN, pBiosSN, pHddModel, pHddSN, pCpuID, pCpuName, pSource, pArchitecture, pServicePack, pComputerName, pLoginName, type);

            if (maxUser > 0)
            {
                if (!licence.IsAvaible)
                {
                    licence = new Licence();
                    licence.Type = (LoginType)type;
                    licence.LoginId = logonId;
                    licence.LoginCode = logonCode;
                    licence.UserId = userId;
                    licence.UserCode = userCode;

                    licence.GetTerminals();

                    if (licence.TerminalList.Count >= maxUser)
                        return new LoginServiceReturnClass(false, Guid.Empty, "Lisans hakkınız dolmuştur. Lütfen firma ile görüşünüz.");
                    else if (licence.TerminalList.Count > 0)
                    {
                        Dictionary<int, int> dict = new Dictionary<int, int>();
                        foreach (Terminal item in licence.TerminalList)
                            dict.Add(item.No, 0);

                        for (int i = 1; i <= maxUser; i++)
                        {
                            if (!dict.ContainsKey(i))
                            {
                                licence.TerminalNo = i;
                                licence.CreateAndInsert(type, GlobalSettings.CompanyName, licence.LoginId, licence.LoginCode, licence.UserId, licence.UserCode, pSource, pIpAddress, pBoardSN, pBiosSN, pHddModel, pHddSN, pCpuName, pCpuID, pLoginName, pCaption, pServicePack, pArchitecture, pComputerName, licence.TerminalNo);
                                break;
                            }
                        }
                    }
                    else
                    {
                        licence.TerminalNo = 1;
                        licence.CreateAndInsert(type, GlobalSettings.CompanyName, licence.LoginId, licence.LoginCode, licence.UserId, licence.UserCode, pSource, pIpAddress, pBoardSN, pBiosSN, pHddModel, pHddSN, pCpuName, pCpuID, pLoginName, pCaption, pServicePack, pArchitecture, pComputerName, licence.TerminalNo);
                    }
                }
                else
                {
                    if (licence.Type == (LoginType)type && licence.LoginId == logonId && licence.UserId == userId)
                    {
                        licence.TerminalNo = licence.TerminalNo;
                    }
                    else
                    {
                        Licence.InsertBlackList(type, GlobalSettings.CompanyName, licence.UserCode, logon.Code, licence.TerminalNo, pSource, pIpAddress, pBoardSN, pBiosSN, pHddModel, pHddSN, pCpuName, pCpuID, pLoginName, pCaption, pServicePack, pArchitecture, pComputerName);
                        return new LoginServiceReturnClass(false, Guid.Empty, "Lisans doğrulanamadı. (" + licence.UserCode + " ==> " + logon.Code + "-" + logon.UserCode + ")");
                    }
                }
            }
            else
            {
                return new LoginServiceReturnClass(false, Guid.Empty, "Programa giriş yetkiniz bulunmamaktadır.");
            }

            switch (type)
            {
                case 0: Logger.LogTransaction(ClientType.B2BWpf, LogTransactionSource.Login, ProcessLogin.Success.ToString(), string.Empty, pIpAddress, licence.Id, logonId, userId); break;
                case 1: Logger.LogTransaction(ClientType.B2BWpf, LogTransactionSource.Login, ProcessLogin.Success.ToString(), string.Empty, pIpAddress, licence.Id, -1, -1, logonId); break;
                case 2: Logger.LogTransaction(ClientType.B2BWpf, LogTransactionSource.Login, ProcessLogin.Success.ToString(), string.Empty, pIpAddress, licence.Id, -1, -1, logonId); break;
                default:
                    break;
            }
            //if (type == 0)
            //    Logger.LogTransaction(LogTransactionSource.Login, ProcessLogin.Success.ToString(), string.Empty, DateTime.Now, logonId);
            //else
            //    Logger.LogTransaction(LogTransactionSource.Login, ProcessLogin.Success.ToString(), string.Empty, DateTime.Now, -1, logonId);

            Session session = new Session();
            session.LoginId = licence.LoginId;
            session.UserId = licence.UserId;
            session.Type = licence.Type;
            session.TerminalNo = licence.TerminalNo;
            session.CreateAndInsert();

            return new LoginServiceReturnClass(true, session.Id, string.Empty);

        }

        [WebMethod]
        public LoginServiceReturnClass ChangePasword(Guid sessionId, string pNewPassword, string pIpAddress)
        {
            B2b.Web.v4.Models.EntityLayer.Session session = new global::B2b.Web.v4.Models.EntityLayer.Session();
            session.GetItem(sessionId);

            if (session.LoginId == -1)
            {
                Logger.LogTransaction(ClientType.B2BWpf, LogTransactionSource.PasswordChange, ProcessLogin.Fail.ToString(), "Hatalı session " + sessionId.ToString(), pIpAddress);
                return new LoginServiceReturnClass(false, Guid.Empty, "Hatalı session");
            }
            else
            {
                bool retVal = Password.ChangePasWordWindows(pNewPassword, (session.Type == LoginType.Customer ? session.UserId : session.LoginId), (int)session.Type);

                if (retVal)
                {
                    if (session.Type == LoginType.Customer)
                        Logger.LogTransaction(ClientType.B2BWpf, LogTransactionSource.PasswordChange, ProcessLogin.Success.ToString(), "Şifre Değişimi", pIpAddress, -1, session.LoginId, session.UserId);
                    else
                        Logger.LogTransaction(ClientType.B2BWpf, LogTransactionSource.PasswordChange, ProcessLogin.Success.ToString(), "Şifre Değişimi", pIpAddress, -1, -1, -1, session.LoginId);
                    return new LoginServiceReturnClass(true, sessionId, string.Empty);
                }
                else
                {
                    if (session.Type == LoginType.Customer)
                        Logger.LogTransaction(ClientType.B2BWpf, LogTransactionSource.PasswordChange, ProcessLogin.Fail.ToString(), "Şifre Değişimi", pIpAddress, -1, session.LoginId, session.UserId);
                    else
                        Logger.LogTransaction(ClientType.B2BWpf, LogTransactionSource.PasswordChange, ProcessLogin.Fail.ToString(), "Şifre Değişimi", pIpAddress, -1, -1, -1, session.LoginId);

                    return new LoginServiceReturnClass(false, sessionId, "Hata oluştu, tekrar deneyiniz.");
                }
            }
        }

    }
    [Serializable]
    public class LoginServiceReturnClass
    {
        public bool Status { get; set; }
        public Guid SessionId { get; set; }
        public string Message { get; set; }

        public LoginServiceReturnClass()
        { }

        public LoginServiceReturnClass(bool status, Guid sessionId, string message)
        {
            Status = status;
            SessionId = sessionId;
            Message = message;
        }
    }
}
