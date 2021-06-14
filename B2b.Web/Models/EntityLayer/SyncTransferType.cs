using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class SyncTransferType : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Name { get; set; }

        #endregion

        #region Methods

        public static List<SyncTransferType> GetSyncTransferType()
        {
            List<SyncTransferType> list = new List<SyncTransferType>();
            DataTable dt = DAL.GetSyncTransferType();

            foreach (DataRow row in dt.Rows)
            {
                SyncTransferType item = new SyncTransferType()
                {
                    Id = row.Field<int>("Id"),
                   Name = row.Field<string>("Name")
                };
                list.Add(item);
            }

            return list;
        }

        public bool Add()
        {
            return DAL.InsertSyncTransferType(Name, CreateId);
        }

        public bool Delete()
        {
            return DAL.DeleteSyncTransferType(Id,EditId);
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetSyncTransferType()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_SyncTransferType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

        public bool InsertSyncTransferType(string pName, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_SyncTransferType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pName, pCreateId });
        }

        public bool DeleteSyncTransferType(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_SyncTransferType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }
    }
}