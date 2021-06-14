using B2b.Web.v4.Areas.Admin.Models;
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
    public class SyncSettings : DataAccess
    {
        public SyncSettings()
        {
            TransferTypeId = 0;
            SettingsId = 0;
            Minute = 10;
            StartHour = 7;
            StartMinute = 30;
            EndHour = 21;
            EndMinute = 30;
            IsActive = true;
            ExportViewType = 0;
            BeforeErpProcedureType = 0;
            AfterErpProcedureType = 0;
            EndCount = 0;
            SendedCount = 0;
        }

        #region Properties

        public int Id { get; set; }
        public int TransferTypeId { get; set; }
        public int SettingsId { get; set; }
        public int Minute { get; set; }
        public int StartHour { get; set; }
        public int StartMinute { get; set; }
        public int EndHour { get; set; }
        public int EndMinute { get; set; }
        public string ExportView { get; set; }
        public string InsertTable { get; set; }
        public string SyncProcedureName { get; set; }
        public string BeforeErpProcedure { get; set; }
        public string AfterErpProcedure { get; set; }
        public string Solr { get; set; }
        public bool IsOneToday { get; set; }
        public bool IsActive { get; set; }
        public bool IsFirst { get; set; }
        public int EndCount { get; set; }
        public int SendedCount { get; set; }
        public string Notes { get; set; }
        public int ExportViewType { get; set; }
        public int BeforeErpProcedureType { get; set; }
        public int AfterErpProcedureType { get; set; }
        public Salesman CreateSalesman { get; set; }
        public Salesman EditSalesman { get; set; }
        public SyncTransferType SyncTransferType { get; set; }
        public Settings CompanySettings { get; set; }
        public DateTime LastStartTime { get; set; }
        //public string StartTime
        //{
        //    get
        //    {
        //        DateTime startControl = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, StartHour, StartMinute, 0);
        //        DateTime finishControl = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, EndHour, EndMinute, 0);

        //        if (startControl <= DateTime.Now && finishControl >= DateTime.Now)
        //        {
        //            DateTime lastStartTime = LastStartTime.AddMinutes(Minute);
        //            DateTime lastDate = lastStartTime < DateTime.Now ? DateTime.Now : lastStartTime;

        //            TimeSpan span = (lastDate -  DateTime.Now);
        //            return (span.Hours < 10 ? ("0"+ span.Hours.ToString()) : span.Hours.ToString()) + ":" + (span.Minutes < 10 ? ("0" + span.Minutes.ToString()) : span.Minutes.ToString()) + ":" + (span.Seconds < 10 ? ("0" + span.Seconds.ToString()) : span.Seconds.ToString());
        //        }
        //        else
        //            return "00:00:00";

        //    }
        //}

        #endregion

        #region Methods

        public static List<SyncSettings> GetActiveSyncTransferType()
        {
            List<SyncSettings> list = new List<SyncSettings>();
            DataTable dt = DAL.GetActiveSyncTransferType();

            foreach (DataRow row in dt.Rows)
            {
                SyncSettings item = new SyncSettings()
                {
                    Id = row.Field<int>("Id"),
                    AfterErpProcedure = row.Field<string>("AfterErpProcedure"),
                    AfterErpProcedureType = row.Field<int>("AfterErpProcedureType"),
                    BeforeErpProcedure = row.Field<string>("BeforeErpProcedure"),
                    BeforeErpProcedureType = row.Field<int>("BeforeErpProcedureType"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    EditDate = row["EditDate"] as DateTime?,
                    CreateId = row.Field<int>("CreateId"),
                    LastStartTime = row.Field<DateTime>("LastStartTime"),
                    CreateSalesman = new Salesman()
                    {
                        Id = row.Field<int>("CreateId"),
                        Code = row.Field<string>("CreateCode"),
                        Name = row.Field<string>("CreateName")
                    },
                    EditId = row.Field<int>("EditId"),
                    EditSalesman = new Salesman()
                    {
                        Id = row.Field<int>("EditId"),
                        Code = row.Field<string>("EditCode"),
                        Name = row.Field<string>("EditName")
                    },
                    EndHour = row.Field<int>("EndHour"),
                    EndMinute = row.Field<int>("EndMinute"),
                    ExportView = row.Field<string>("ExportView"),
                    ExportViewType = row.Field<int>("ExportViewType"),
                    InsertTable = row.Field<string>("InsertTable"),
                    IsActive = row.Field<bool>("IsActive"),
                    IsOneToday = row.Field<bool>("IsOneToday"),
                    Minute = row.Field<int>("Minute"),
                    Notes = row.Field<string>("Notes"),
                    SettingsId = row.Field<int>("SettingsId"),
                    Solr = row.Field<string>("Solr"),
                    StartHour = row.Field<int>("StartHour"),
                    StartMinute = row.Field<int>("StartMinute"),
                    SyncProcedureName = row.Field<string>("SyncProcedureName"),
                    TransferTypeId = row.Field<int>("TransferTypeId"),
                    SyncTransferType = new SyncTransferType()
                    {
                        Id = row.Field<int>("TransferTypeId"),
                        Name = row.Field<string>("TransferTypeName")
                    },
                    CompanySettings = new Settings()
                    {
                        Id = row.Field<int>("SettingsId"),
                        CompanyName = row.Field<string>("CompanyName"),
                        Company = row.Field<int>("Company"),
                        ServerName = row.Field<string>("ServerName"),
                        Database = row.Field<string>("Database"),
                        DbPassword = row.Field<string>("DbPassword"),
                        DbUser = row.Field<string>("DbUser"),
                        Donem = row.Field<int>("Donem"),
                        ErpName = row.Field<string>("ErpName"),
                        ErpPassword = row.Field<string>("ErpPassword"),
                        ErpUserName = row.Field<string>("ErpUserName"),
                        DatabaseType = row.Field<int>("DatabaseType"),
                        ServiceUserName = row.Field<string>("ServiceUserName"),
                        ServicePassword = row.Field<string>("ServicePassword"),
                        ServiceAddress = row.Field<string>("ServiceAddress"),
                        ServiceAddressLocal = row.Field<string>("ServiceAddressLocal"),
                        Port = Convert.ToUInt32(row["Port"]),
                        ServerIp = row.Field<string>("ServerIp"),
                        IsLocalCompany = row.Field<bool>("IsLocalCompany")

                    }
                };
                list.Add(item);
            }

            return list;
        }

        public int Add()
        {
            DataTable dt = DAL.InsertSyncSettings(TransferTypeId, SettingsId, Minute, StartHour, StartMinute, EndHour, EndMinute, ExportView, InsertTable, SyncProcedureName, BeforeErpProcedure, AfterErpProcedure, Solr, IsOneToday, IsActive, Notes, ExportViewType, BeforeErpProcedureType, AfterErpProcedureType, CreateId);

            return Convert.ToInt32(dt.Rows[0][0]);
        }

        public bool Update()
        {
            return DAL.UpdateSyncSettings(Id, TransferTypeId, SettingsId, Minute, StartHour, StartMinute, EndHour, EndMinute, ExportView, InsertTable, SyncProcedureName, BeforeErpProcedure, AfterErpProcedure, Solr, IsOneToday, IsActive, Notes, ExportViewType, BeforeErpProcedureType, AfterErpProcedureType,  EditId);
        }

        public static void UpdateLastUpdate(int id)
        {
            DAL.UpdateLastUpdate(id);
        }

        public static SyncSettings GetSettingItem(int pId, int pTransferTypeId)
        {
            SyncSettings list = new SyncSettings();
            DataTable dt = DAL.GetSettingItem(pId, pTransferTypeId);

            foreach (DataRow row in dt.Rows)
            {
                SyncSettings item = new SyncSettings()
                {
                    Id = row.Field<int>("Id"),
                    AfterErpProcedure = row.Field<string>("AfterErpProcedure"),
                    AfterErpProcedureType = row.Field<int>("AfterErpProcedureType"),
                    BeforeErpProcedure = row.Field<string>("BeforeErpProcedure"),
                    BeforeErpProcedureType = row.Field<int>("BeforeErpProcedureType"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    EditDate = row["EditDate"] as DateTime?,
                    CreateId = row.Field<int>("CreateId"),
                    CreateSalesman = new Salesman()
                    {
                        Id = row.Field<int>("CreateId"),
                        Code = row.Field<string>("CreateCode"),
                        Name = row.Field<string>("CreateName")
                    },
                    EditId = row.Field<int>("EditId"),
                    EditSalesman = new Salesman()
                    {
                        Id = row.Field<int>("EditId"),
                        Code = row.Field<string>("EditCode"),
                        Name = row.Field<string>("EditName")
                    },
                    EndHour = row.Field<int>("EndHour"),
                    EndMinute = row.Field<int>("EndMinute"),
                    ExportView = row.Field<string>("ExportView"),
                    ExportViewType = row.Field<int>("ExportViewType"),
                    InsertTable = row.Field<string>("InsertTable"),
                    IsActive = row.Field<bool>("IsActive"),
                    IsOneToday = row.Field<bool>("IsOneToday"),
                    Minute = row.Field<int>("Minute"),
                    Notes = row.Field<string>("Notes"),
                    SettingsId = row.Field<int>("SettingsId"),
                    Solr = row.Field<string>("Solr"),
                    StartHour = row.Field<int>("StartHour"),
                    StartMinute = row.Field<int>("StartMinute"),
                    SyncProcedureName = row.Field<string>("SyncProcedureName"),
                    TransferTypeId = row.Field<int>("TransferTypeId"),
                    SyncTransferType = new SyncTransferType()
                    {
                        Id = row.Field<int>("TransferTypeId"),
                        Name = row.Field<string>("TransferTypeName")
                    },
                    CompanySettings = new Settings()
                    {
                        Id = row.Field<int>("SettingsId"),
                        CompanyName = row.Field<string>("CompanyName"),
                        Company = row.Field<int>("Company"),
                        ServerName = row.Field<string>("ServerName"),
                        Database = Token.Decrypt(row.Field<string>("Database"), GlobalSettings.EncryptKey),
                        DbPassword = Token.Decrypt(row.Field<string>("DbPassword"), GlobalSettings.EncryptKey),
                        DbUser = Token.Decrypt(row.Field<string>("DbUser"), GlobalSettings.EncryptKey),
                        Donem = row.Field<int>("Donem"),
                        ErpName = row.Field<string>("ErpName"),
                        ErpPassword = Token.Decrypt(row.Field<string>("ErpPassword"), GlobalSettings.EncryptKey),
                        ErpUserName = Token.Decrypt(row.Field<string>("ErpUserName"), GlobalSettings.EncryptKey),
                        DatabaseType = row.Field<int>("DatabaseType"),
                        ServiceUserName = Token.Decrypt(row.Field<string>("ServiceUserName"), GlobalSettings.EncryptKey),
                        ServicePassword = Token.Decrypt(row.Field<string>("ServicePassword"), GlobalSettings.EncryptKey),
                        ServiceAddress = Token.Decrypt(row.Field<string>("ServiceAddress"), GlobalSettings.EncryptKey),
                        ServiceAddressLocal = Token.Decrypt(row.Field<string>("ServiceAddressLocal"), GlobalSettings.EncryptKey),
                        Port = Convert.ToUInt32(row["Port"]),
                        ServerIp = row.Field<string>("ServerIp")

                    }
                };
                list = (item);
            }

            return list;
        }

        #endregion

    }

    public static class RuningControl
    {
        public static bool IsRunning { get; set; }
        public static DateTime LastCheck { get; set; }
        public static SyncResponseValues SyncResponseValues = new SyncResponseValues();

    }

    public partial class DataAccessLayer
    {
        public DataTable GetActiveSyncTransferType()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_ActiveSyncTransferType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

        public DataTable GetSettingItem(int pId, int pTransferTypeId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetItem_SyncSettings", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pTransferTypeId });
        }

        public bool UpdateLastUpdate(int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_SyncSettingsLastUpdate", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }

        public bool UpdateSyncSettings(int pId, int pTransferTypeId, int pSettingsId, int pMinute, int pStartHour, int pStartMinute, int pEndHour, int pEndMinute, string pExportView, string pInsertTable, string pSyncProcedureName, string pBeforeErpProcedure, string pAfterErpProcedure, string pSolr, bool pIsOneToday, bool pIsActive, string pNotes, int pExportViewType, int pBeforeErpProcedureType, int pAfterErpProcedureType, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_SyncSettings", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pTransferTypeId, pSettingsId, pMinute, pStartHour, pStartMinute, pEndHour, pEndMinute, pExportView, pInsertTable, pSyncProcedureName, pBeforeErpProcedure, pAfterErpProcedure, pSolr, pIsOneToday, pIsActive, pNotes, pExportViewType, pBeforeErpProcedureType, pAfterErpProcedureType, pEditId });
        }

        public DataTable InsertSyncSettings(int pTransferTypeId, int pSettingsId, int pMinute, int pStartHour, int pStartMinute, int pEndHour, int pEndMinute, string pExportView, string pInsertTable, string pSyncProcedureName, string pBeforeErpProcedure, string pAfterErpProcedure, string pSolr, bool pIsOneToday, bool pIsActive, string pNotes, int pExportViewType, int pBeforeErpProcedureType, int pAfterErpProcedureType, int pCreateId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_Insert_SyncSettings", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pTransferTypeId, pSettingsId, pMinute, pStartHour, pStartMinute, pEndHour, pEndMinute, pExportView, pInsertTable, pSyncProcedureName, pBeforeErpProcedure, pAfterErpProcedure, pSolr, pIsOneToday, pIsActive, pNotes, pExportViewType, pBeforeErpProcedureType, pAfterErpProcedureType, pCreateId });
        }
    }
}