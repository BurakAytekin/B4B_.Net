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
    public class ErpFunctionDetail : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int SettingsId { get; set; }
        public Settings Settings { get; set; }
        public int TypeId { get; set; }
        public string Header { get; set; }
        public string Name { get; set; }
        public string FunctionName { get; set; }
        public int FunctionType { get; set; }
        public bool IsActive { get; set; }
        public string Notes { get; set; }
        public string FunctionDetailInvoice { get; set; }
        public string FunctionDetailChceck { get; set; }

        #endregion

        #region Methods

        public static List<ErpFunctionDetail> GetDetailList(int pTypeId)
        {
            List<ErpFunctionDetail> list = new List<ErpFunctionDetail>();
            DataTable dt = DAL.GetDetailList(pTypeId);

            foreach (DataRow row in dt.Rows)
            {
                ErpFunctionDetail item = new ErpFunctionDetail()
                {
                    Id = row.Field<int>("Id"),
                    TypeId = pTypeId,
                    SettingsId = row.Field<int>("SettingsId"),
                    Header = row.Field<string>("Header"),
                    Name = row.Field<string>("Name"),
                    FunctionName = row.Field<string>("FunctionName"),
                    FunctionType = row.Field<int>("FunctionType"),
                    IsActive = row.Field<bool>("IsActive"),
                    Notes = row.Field<string>("Notes"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                    EditDate = row["EditDate"] as DateTime?,
                    CreateName = row.Field<string>("CreateName"),
                    LastUpdateName = row.Field<string>("LastUpdateName"),
                    FunctionDetailChceck = row.Field<string>("FunctionDetailChceck"),
                    FunctionDetailInvoice = row.Field<string>("FunctionDetailInvoice")

                };
                list.Add(item);
            }

            return list;
        }

        public static List<ErpFunctionDetail> GetActiveDetailList(int pTypeId)
        {
            List<ErpFunctionDetail> list = new List<ErpFunctionDetail>();
            DataTable dt = DAL.GetActiveDetailList(pTypeId);

            foreach (DataRow row in dt.Rows)
            {
                ErpFunctionDetail item = new ErpFunctionDetail()
                {
                    Id = row.Field<int>("Id"),
                    TypeId = pTypeId,
                    SettingsId = row.Field<int>("SettingsId"),
                    Settings = new Settings()
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
                        ServiceUserName = Token.Decrypt(row.Field<string>("ServiceUserName"), GlobalSettings.EncryptKey),
                        ServicePassword = Token.Decrypt(row.Field<string>("ServicePassword"), GlobalSettings.EncryptKey),
                        ServiceAddress = Token.Decrypt(row.Field<string>("ServiceAddress"), GlobalSettings.EncryptKey),
                        ServiceAddressLocal = Token.Decrypt(row.Field<string>("ServiceAddressLocal"), GlobalSettings.EncryptKey),
                        Port = Convert.ToUInt32(row["Port"]),
                        ServerIp = row.Field<string>("ServerIp"),
                        IsLocalCompany = row.Field<bool>("IsLocalCompany"),
                        IsActiveCompany = row.Field<bool>("IsActiveCompany")
                    },
                    Header = row.Field<string>("Header"),
                    Name = row.Field<string>("Name"),
                    FunctionName = row.Field<string>("FunctionName"),
                    FunctionDetailInvoice = row.Field<string>("FunctionDetailInvoice"),
                    FunctionDetailChceck = row.Field<string>("FunctionDetailChceck"),
                    FunctionType = row.Field<int>("FunctionType"),
                    IsActive = row.Field<bool>("IsActive"),
                    Notes = row.Field<string>("Notes"),

                };
                list.Add(item);
            }

            return list;
        }

        public int Add()
        {
            DataTable dt = DAL.InsertErpFunctionDetail(SettingsId, TypeId, Header, Name, FunctionName, FunctionType, FunctionDetailInvoice, FunctionDetailChceck, IsActive, Notes, CreateId);

            return Convert.ToInt32(dt.Rows[0][0]);
        }

        public bool Update()
        {
            return DAL.UpdateErpFunctionDetail(Id, SettingsId, TypeId, Header, Name, FunctionName, FunctionType, FunctionDetailInvoice, FunctionDetailChceck, IsActive, Notes, Deleted, EditId);
        }


        #endregion

    }

    public enum ErpFunctionTypeEnum
    {
        Finance = 1,
        ProductOrder = 2,
        UnClosedInvoice = 3,
        BackOrder = 4,
        CustomerDashboard = 5,
        ReturnProduct = 6,
        ReturnProductManufacturer = 7,
        MarsSevkEmri = 8
    }

    public partial class DataAccessLayer
    {
        public bool UpdateErpFunctionDetail(int pId, int pSettingsId, int pTypeId, string pHeader, string pName, string pFunctionName, int pFunctionType, string pFunctionDetailInvoice, string pFunctionDetailChceck, bool pIsActive, string pNote, bool pDeleted, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_ErpFunctionDetail", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pSettingsId, pTypeId, pHeader, pName, pFunctionName, pFunctionType, pFunctionDetailInvoice, pFunctionDetailChceck, pIsActive, pNote, pDeleted, pEditId });
        }

        public DataTable InsertErpFunctionDetail(int pSettingsId, int pTypeId, string pHeader, string pName, string pFunctionName, int pFunctionType, string pFunctionDetailInvoice, string pFunctionDetailChceck, bool pIsActive, string pNote, int pCreateId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_Insert_ErpFunctionDetail", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSettingsId, pTypeId, pHeader, pName, pFunctionName, pFunctionType, pFunctionDetailInvoice, pFunctionDetailChceck, pIsActive, pNote, pCreateId });
        }

        public DataTable GetDetailList(int pTypeId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_ErpFunctionDetail", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pTypeId });
        }

        public DataTable GetActiveDetailList(int pTypeId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ActiveErpFunctionDetail", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pTypeId });
        }
    }
}