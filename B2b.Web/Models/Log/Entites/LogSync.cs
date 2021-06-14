
using B2b.Web.v4.Models.EntityLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class LogSync : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int SettingsId { get; set; }
        public int ProgressValue { get; set; }
        public string Message { get; set; }
        public int Type { get; set; }

        #endregion


        #region Methods

        public bool Add()
        {
            return DAL.InsertSyncLog(SettingsId, ProgressValue, Message, Type, CreateId);
        }

        public static List<LogSync> GetTransferLog(DateTime minDate, DateTime maxDate)
        {

            List<LogSync> list = new List<LogSync>();
            DataTable dt = DAL.GetTransferLog(minDate, maxDate);

            foreach (DataRow row in dt.Rows)
            {
                LogSync item = new LogSync()
                {
                    Id = row.Field<int>("Id"),
                    SettingsId = row.Field<int>("SettingsId"),
                    ProgressValue = row.Field<int>("ProgressValue"),
                    Message = row.Field<string>("Message"),
                    Type = row.Field<int>("Type"),
                    CreateDate = row.Field<DateTime>("CreateDate")
                };
                list.Add(item);
            }

            return list;
        }

        #endregion
    }

    public partial class DataAccessLayer
    {

        public DataTable GetTransferLog(DateTime pMinDAte,DateTime pMaxDate)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_SyncTransferLog", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pMinDAte, pMaxDate });
        }

        public bool InsertSyncLog(int pSettingsId,int pProgressValue, string pMessage, int pType, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_SyncLog", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pSettingsId, pProgressValue, pMessage, pType, pCreateId });
        }
    }
}