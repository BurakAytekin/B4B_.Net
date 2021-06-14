using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using B2b.Web.v4.Models.Helper;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Licence : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public LoginType Type { get; set; }
        public int LoginId { get; set; }
        public string LoginCode { get; set; }
        public int UserId { get; set; }
        public string UserCode { get; set; }
        public int TerminalNo { get; set; }
        public bool IsAvaible { get; set; }
        public List<Terminal> TerminalList { get; set; }
        public LicenceSource Source { get; set; }
        public string IpAddress { get; set; }
        public DateTime Date { get; set; }
        #endregion

        #region Methods

        public static Licence CheckLicence(string pBoardSN, string pBiosSN, string pHddModel, string pHddSN, string pCpuID, string pCpuName, int pSource, string pArchitecture, string pServicePack, string pComputerName, string pLoginName,int pType)
        {
            DataTable dt = DAL.GetLicence(pBoardSN,
                    pBiosSN,
                    pHddModel,
                    pHddSN,
                    pCpuID,
                    pCpuName,
                    pSource,
                    pType);

            Licence lic = new Licence();
            lic.IsAvaible = false;

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];

                lic.IsAvaible = true;
                lic.Id = row.Field<int>("Id");
                lic.Type = (LoginType)row.Field<int>("Type");
                lic.LoginId = row.Field<int>("LoginId");
                lic.LoginCode = row.Field<string>("LoginCode");
                lic.UserId = row.Field<int>("UserId");
                lic.UserCode = row.Field<string>("UserCode");
                lic.TerminalNo = row.Field<int>("TerminalNo");
            }

            return lic;
        }

        public void GetTerminals()
        {
            DataTable dt = new DataTable();
            TerminalList = new List<Terminal>();

            dt = DAL.GetLicenceTerminalNoList((int)Type, LoginId);

            foreach (DataRow row in dt.Rows)
            {
                Terminal t = new Terminal()
                {
                    No = row.Field<int>("TerminalNo"),
                };
                TerminalList.Add(t);
            }
        }

        public bool CreateAndInsert(int pType, string companyName, int pLoginId, string pLoginCode, int pUserId, string pUserCode, int pSource, string pIpAdress, string pBoardSN, string pBiosSN, string pHddModel, string pHddSN, string pCpuName, string pCpuId, string pLoginName, string pOsCaption, string pOsServicePack, string pOsArchitecture, string pOsComputerName, int pTerminalNo)
        {
            Id = DAL.InsertLicence(pType, pLoginId, pLoginCode, pUserId, pUserCode, (int)LicenceSource.B2BWeb, pIpAdress, pBoardSN, pBiosSN, pHddModel, pHddSN, pCpuName, pCpuId, pLoginName, pOsCaption, pOsServicePack, pOsArchitecture, pOsComputerName, pTerminalNo);
            return Id == -1 ? false : true;
        }
        public static void InsertBlackList(int pType, string companyName, string blackListUserCode, string pUserCode, int pTerminalNo, int pSource, string pIpAdress,
     string pBoardSN, string pBiosSN, string pHddModel, string pHddSN, string pCpuName, string pCpuId,
     string pLoginName, string pOsCaption, string pOsServicePack, string pOsArchitecture, string pOsComputerName)
        {
            DAL.InsertBlacklist(pType, blackListUserCode, pUserCode, pTerminalNo, (int)LicenceSource.B2BWeb,
                    pIpAdress, pBoardSN, pBiosSN, pHddModel, pHddSN, pCpuName, pCpuId, pLoginName, pOsCaption, pOsServicePack, pOsArchitecture, pOsComputerName);
        }

        public static List<Licence> GetLicenceByUserId(int userId, int pUserType)
        {
            DataTable dt = new DataTable();
            List<Licence> list = new List<Licence>();

            dt = DAL.GetLicenceListByUserId(userId, pUserType);

            foreach (DataRow row in dt.Rows)
            {
                Licence licence = new Licence()
                {
                    Id = row.Field<int>("Id"),
                    Source = (LicenceSource)row.Field<int>("Source"),
                    TerminalNo = row.Field<int>("TerminalNo"),
                    UserId = row.Field<int>("UserId"),
                    UserCode = row.Field<string>("UserCode"),
                    IpAddress = row.Field<string>("IpAdress"),
                    Date = row.Field<DateTime>("Date"),
                };
                list.Add(licence);
            }

            return list;
        }

        public bool Delete()
        {
            return DAL.DeleteLicence(Id);
        }
        #endregion
    }
    public partial class DataAccessLayer
    {
        public DataTable GetLicenceListByUserId(int pUserId, int pUserType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_LicenceByUserId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pUserId, pUserType });
        }

        public bool DeleteLicence(int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_Licence", MethodBase.GetCurrentMethod().GetParameters(),
                new object[] { pId });
        }
        public DataTable GetLicence(string pBoardSN, string pBiosSN, string pHddModel, string pHddSN, string pCpuID, string pCpuName, int pSource,int pType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_Licence_1", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pBoardSN, pBiosSN, pHddModel, pHddSN, pCpuID, pCpuName, pSource, pType });
        }

        public DataTable GetLicenceTerminalNoList(int pType, int pLoginId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_LicenceTerminalNo", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pType, pLoginId });
        }
        public int InsertLicence(int pType, int pLoginId, string pLoginCode, int pUserId, string pUserCode, int pSource, string pIpAdress, string pBoardSN, string pBiosSN, string pHddModel, string pHddSN, string pCpuName, string pCpuId, string pLoginName, string pOsCaption, string pOsServicePack, string pOsArchitecture, string pOsComputerName, int pTerminalNo)
        {
            return DatabaseContext.ExecuteScalar(CommandType.StoredProcedure, "_Insert_Licence", MethodBase.GetCurrentMethod().GetParameters(),
                 new object[] { pType, pLoginId, pLoginCode, pUserId, pUserCode, pSource, pIpAdress, pBoardSN, pBiosSN, pHddModel, pHddSN, pCpuName, pCpuId, pLoginName, pOsCaption, pOsServicePack, pOsArchitecture, pOsComputerName, pTerminalNo });
        }
        public bool InsertBlacklist(int pType, string pUserCode, string pOwnerUserCode, int pTerminalNo, int pSource, string pIpAdress,
          string pBoardSN, string pBiosSN, string pHddModel, string pHddSN, string pCpuName, string pCpuId,
          string pLoginName, string pOsCaption, string pOsServicePack, string pOsArchitecture, string pOsComputerName)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_Blacklist", MethodBase.GetCurrentMethod().GetParameters(),
                 new object[] { pType,
                pUserCode, pOwnerUserCode, pTerminalNo, pSource, pIpAdress, pBoardSN, pBiosSN, pHddModel, pHddSN, pCpuName,
                pCpuId, pLoginName, pOsCaption, pOsServicePack, pOsArchitecture, pOsComputerName });
        }
    }

    public class Terminal
    {
        public int No { get; set; }
    }

    public enum LicenceSource
    {
        //B2B = 0,
        //Moderator = 1,
        B2BWeb = 2,
        Admin = 3
    }

}