using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class T9 : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Key { get; set; }
        public string Data { get; set; }
        public bool Type { get; set; }
        #endregion

        #region Methods
        public bool Insert()
        {
            return DAL.InsertT9(Key, Data,Type,CreateId);
        }

        public bool Save()
        {
            return DAL.UpdateT9(Id, Key, Data);
        }

        public bool Delete()
        {
            return DAL.DeleteT9(Id);
        }

        public static List<T9> GetT9List(string key)
        {
            List<T9> list = new List<T9>();
            DataTable dt = DAL.GetT9List(key);

            foreach (DataRow row in dt.Rows)
            {
                T9 t9 = new T9()
                {
                    Id = row.Field<int>("Id"),
                    Key = row.Field<string>("Key"),
                    Data = row.Field<string>("Data")
                };
                list.Add(t9);
            }

            return list;
        }
        #endregion
    }
    public partial class DataAccessLayer
    {
        public bool InsertT9(string pKey, string pData,bool pType, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_T9", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pKey, pData , pType, pCreateId });

        }
        public bool UpdateT9(int pId, string pKey, string pData)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_T9", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pKey, pData });

        }
        public bool DeleteT9(int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_T9", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });

        }
        public DataTable GetT9List(string pKey)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_T9", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pKey });

        }
    }
}