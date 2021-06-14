using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class ErpFunctionType : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region Methods

        public static List<ErpFunctionType> GetList()
        {
            List<ErpFunctionType> list = new List<ErpFunctionType>();
            DataTable dt = DAL.GetServiceTypeList();

            foreach (DataRow row in dt.Rows)
            {
                ErpFunctionType obj = new ErpFunctionType()
                {
                    Id = row.Field<int>("Id"),
                    Name = row.Field<string>("Name")
                };
                list.Add(obj);
            }
            return list;
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetServiceTypeList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_ErpfunctionType");
        }
    }
}