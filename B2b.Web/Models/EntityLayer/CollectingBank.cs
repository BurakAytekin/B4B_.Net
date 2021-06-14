using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;


namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class CollectingBank:DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        #endregion
        #region Methods

        public static List<CollectingBank> GetList()
        {
            List<CollectingBank> list = new List<CollectingBank>();
            DataTable dt = DAL.GetCollectingBankList();

            foreach (DataRow row in dt.Rows)
            {
                CollectingBank obj = new CollectingBank()
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    Value = row.Field<string>("Value"),

                };
                list.Add(obj);
            }

            return list;
        }
        #endregion
    }
    public  partial class DataAccessLayer
    {
        public DataTable GetCollectingBankList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_CollectingBank", MethodBase.GetCurrentMethod().GetParameters(), new object[] {  });
        }
    }
}