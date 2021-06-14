using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log.EPayment;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Settings : DataAccess
    {
        public Settings()
        {

        }

        #region Properties

        public int Id { get; set; }
        public string ErpName { get; set; }
        public string CompanyName { get; set; }
        public int Company { get; set; }
        public string ServerName { get; set; }
        public string Database { get; set; }
        public string ErpUserName { get; set; }
        public string ErpPassword { get; set; }
        public string DbUser { get; set; }
        public string DbPassword { get; set; }
        public int Donem { get; set; }
        public int Status { get; set; }
        public int BranchCode { get; set; }
        public bool IsActiveCompany { get; set; }
        public int DatabaseType { get; set; }
        public string ServiceUserName { get; set; }
        public string ServicePassword { get; set; }
        public uint Port { get; set; }
        public string ServerIp { get; set; }
        public string ServiceAddress { get; set; }
        public string ServiceAddressLocal { get; set; }
        public bool IsLocalCompany { get; set; }
        public bool IsLocalB2b { get; set; }

        #endregion

        #region Methods

        public static List<Settings> GetSettingsList()
        {
            List<Settings> list = new List<Settings>();
            DataTable dt = DAL.GetSettingsList();

            foreach (DataRow row in dt.Rows)
            {
                Settings item = new Settings()
                {
                    Id = row.Field<int>("Id"),
                    BranchCode = row.Field<int>("BranchCode"),
                    Company = row.Field<int>("Company"),
                    CompanyName = row.Field<string>("CompanyName"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    Database = Token.Decrypt(row.Field<string>("Database"), GlobalSettings.EncryptKey),
                    DbPassword = Token.Decrypt(row.Field<string>("DbPassword"), GlobalSettings.EncryptKey),
                    DbUser = Token.Decrypt(row.Field<string>("DbUser"), GlobalSettings.EncryptKey),
                    Donem = row.Field<int>("Donem"),
                    Deleted = row.Field<bool>("Deleted"),
                    ErpName = row.Field<string>("ErpName"),
                    ErpPassword = Token.Decrypt(row.Field<string>("ErpPassword"), GlobalSettings.EncryptKey),
                    ErpUserName = Token.Decrypt(row.Field<string>("ErpUserName"), GlobalSettings.EncryptKey),
                    IsActiveCompany = row.Field<bool>("IsActiveCompany"),
                    ServerName = row.Field<string>("ServerName"),
                    DatabaseType = row.Field<int>("DatabaseType"),
                    ServiceUserName = Token.Decrypt(row.Field<string>("ServiceUserName"), GlobalSettings.EncryptKey),
                    ServicePassword = Token.Decrypt(row.Field<string>("ServicePassword"), GlobalSettings.EncryptKey),
                    Port = Convert.ToUInt32(row["Port"]),
                    ServerIp = row.Field<string>("ServerIp"),
                    ServiceAddress = Token.Decrypt(row.Field<string>("ServiceAddress"), GlobalSettings.EncryptKey),
                    ServiceAddressLocal = Token.Decrypt(row.Field<string>("ServiceAddressLocal"), GlobalSettings.EncryptKey),
                    IsLocalCompany = row.Field<bool>("IsLocalCompany"),
                    IsLocalB2b = row.Field<bool>("IsLocalB2b")
                };
                list.Add(item);
            }

            return list;
        }

        public bool Add()
        {
            return DAL.InsertSettings(ErpName, CompanyName, Company, ServerName, Database, ErpUserName, ErpPassword, DbUser, DbPassword, Donem, BranchCode, IsActiveCompany, DatabaseType, ServiceUserName, ServicePassword,(int)Port,ServerIp, ServiceAddress,ServiceAddressLocal, IsLocalCompany, IsLocalB2b, CreateId);
        }
        public bool Update()
        {
            return DAL.UpdateSettings(Id, ErpName, CompanyName, Company, ServerName, Database, ErpUserName, ErpPassword, DbUser, DbPassword, Donem, BranchCode, IsActiveCompany, DatabaseType, ServiceUserName, ServicePassword, (int)Port, ServerIp, ServiceAddress,ServiceAddressLocal, IsLocalCompany, IsLocalB2b, EditId, Deleted);
        }

        #endregion

    }
    public partial class DataAccessLayer
    {
        public DataTable GetSettingsList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_Settings", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

        public bool InsertSettings(string pErpName, string pCompanyName, int pCompany, string pServerName, string pDatabase, string pErpUserName, string pErpPassword, string pDbUser, string pDbPassword, int pDonem, int pBranchCode, bool pIsActiveCompany, int pDatabaseType, string pServiceUserName, string pServicePassword,int pPort, string pServerIp,string pServiceAddress,string pServiceAddressLocal, bool pIsLocalCompany,bool pIsLocalB2b, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Settings", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pErpName, pCompanyName, pCompany, pServerName, pDatabase, pErpUserName, pErpPassword, pDbUser, pDbPassword, pDonem, pBranchCode, pIsActiveCompany, pDatabaseType, pServiceUserName, pServicePassword, pPort,pServerIp, pServiceAddress, pServiceAddressLocal, pIsLocalCompany, pIsLocalB2b, pCreateId });
        }

        public bool UpdateSettings(int pId, string pErpName, string pCompanyName, int pCompany, string pServerName, string pDatabase, string pErpUserName, string pErpPassword, string pDbUser, string pDbPassword, int pDonem, int pBranchCode, bool pIsActiveCompany, int pDatabaseType, string pServiceUserName, string pServicePassword, int pPort, string pServerIp, string pServiceAddress, string pServiceAddressLocal,bool pIsLocalCompany,bool pIsLocalB2b, int pEditId, bool pDeleted)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Settings", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pErpName, pCompanyName, pCompany, pServerName, pDatabase, pErpUserName, pErpPassword, pDbUser, pDbPassword, pDonem, pBranchCode, pIsActiveCompany, pDatabaseType, pServiceUserName, pServicePassword, pPort,pServerIp, pServiceAddress, pServiceAddressLocal, pIsLocalCompany, pIsLocalB2b, pEditId, pDeleted });
        }
    }
}