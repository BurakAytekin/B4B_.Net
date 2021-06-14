using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Warehouse : DataAccess
    {
        #region Contructors
        public Warehouse()
        {
            Id = -1;
            Code = string.Empty;
            Name = string.Empty;
            Priority = -1;
            Checked = true;
        }

        public Warehouse(int id, string code, string name, int priority)
        {
            Id = id;
            Code = code;
            Name = name;
            Priority = priority;
        }
        #endregion

        #region Properties
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        public bool Checked { get; set; }

        #endregion

        #region Methods
        public static List<Warehouse> GetList()
        {
            List<Warehouse> list = new List<Warehouse>();
            DataTable dt = DAL.GetWarehouseList();
            foreach (DataRow row in dt.Rows)
            {
                Warehouse obj = new Warehouse()
                {
                    Id = row.Field<int>("Id"),
                    Code = row.Field<string>("Code"),
                    Name = row.Field<string>("Name"),
                    Priority = row.Field<int>("Priority"),
                };
                list.Add(obj);
            }
            return list;
        }
        public static List<Warehouse> GetListCustomerWarehouse(int pCustomerId)
        {
            List<Warehouse> list = new List<Warehouse>();
            DataTable dt = DAL.GetCustomerWarehouseList(pCustomerId);
            foreach (DataRow row in dt.Rows)
            {
                Warehouse obj = new Warehouse()
                {
                    Id = row.Field<int>("Id"),

                };
                list.Add(obj);
            }
            return list;
        }


        public bool Add()
        {
            return DAL.InsertWarehouse(Code, Name, Priority, CreateId);
        }

        public bool Update()
        {
            return DAL.UpdateWarehouse(Id, Code, Name, Priority, EditId);
        }

        public bool Delete()
        {
            return DAL.DeleteWarehouse(Id, EditId);
        }

        public static bool InsertCustomerAuthorityWarehouse(int pCustomerId, int pWarehouseId ,int pCreateId)
        {
            return DAL.InsertCustomerAuthorityWarehouse(pCustomerId, pWarehouseId, pCreateId);
        }

        public static bool DeleteCustomerAuthorityWarehouse(int pCustomerId, int pWarehouseId)
        {
            return DAL.DeleteCustomerAuthorityWarehouse(pCustomerId, pWarehouseId);
        }

        #endregion
    }

    public partial class DataAccessLayer
    {
        public DataTable GetWarehouseList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Warehouse");
        }
        public DataTable GetCustomerWarehouseList(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Customer_Warehouse", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }

        public bool InsertWarehouse(string pCode, string pName, int pPriority, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Warehouse", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCode, pName, pPriority, pCreateId });
        }

        public bool UpdateWarehouse(int pId, string pCode, string pName, int pPriority, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Warehouse", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pCode, pName, pPriority, pEditId });
        }

        public bool DeleteWarehouse(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_Warehouse", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }
        public bool InsertCustomerAuthorityWarehouse(int pCustomerId, int pWarehouseId, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_CustomerWarehouseAuthority", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pWarehouseId, pCreateId });
        }
        public bool DeleteCustomerAuthorityWarehouse(int pCustomerId, int pWarehouseId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_CustomerWarehouse", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pWarehouseId });
        }
    }
}