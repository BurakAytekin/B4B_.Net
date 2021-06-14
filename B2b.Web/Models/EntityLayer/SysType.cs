using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class SysType : DataAccess
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Title { get; set; }
        public int KeyId { get; set; }
        public string Description { get; set; }


        public static List<SysType> GetListType(int pTypeId)
        {
            List<SysType> list = new List<SysType>();
            DataTable dt = DAL.GetListType(pTypeId);

            foreach (DataRow row in dt.Rows)
            {
                SysType obj = new SysType()
                {
                    Id = row.Field<int>("Id"),
                    ParentId = row.Field<int>("ParentId"),
                    Title = row.Field<string>("Title"),
                    KeyId = row.Field<int>("KeyId"),
                    Description = row.Field<string>("Description")
                };
                list.Add(obj);
            }
            return list;
        }

    }

    public partial class DataAccessLayer
    {
        public DataTable GetListType(int pTypeId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Type", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pTypeId });
        }
       
    }
}