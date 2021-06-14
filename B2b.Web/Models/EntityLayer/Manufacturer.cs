using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Manufacturer : DataAccess
    {
        #region Properties
        public string Name { get; set; }
        public long RecordCount { get; set; }
        #endregion

        #region Methods
        public static List<Manufacturer> GetList(int customerId)
        {
            List<Manufacturer> list = new List<Manufacturer>();
            DataTable dt = DAL.GetManufacturerList(customerId);
            foreach (DataRow row in dt.Rows)
            {
                Manufacturer obj = new Manufacturer()
                {
                    Name = row.Field<string>("Manufacturer"),
                    RecordCount = row.Field<long>("reccount"),
                };
                list.Add(obj);
            }
            return list;
        }
        public static List<Manufacturer> GetManifacturerList()
        {
            DataTable dt = new DataTable();
            List<Manufacturer> list = new List<Manufacturer>();

            dt = DAL.GetManufacturerList();

            foreach (DataRow row in dt.Rows)
            {
                Manufacturer manufacturer = new Manufacturer()
                {
                    Name = row.Field<string>("Manufacturer"),
                    RecordCount = row.Field<long>("reccount"),
                };
                list.Add(manufacturer);
            }

            return list;
        }

        public static List<Manufacturer> GetManifacturerPassiveListByCustomer(int customerId)
        {
            DataTable dt = new DataTable();
            List<Manufacturer> list = new List<Manufacturer>();

            dt = DAL.GetManufacturerPassiveListByCustomer(customerId);

            foreach (DataRow row in dt.Rows)
            {
                Manufacturer manufacturer = new Manufacturer()
                {
                    Name = row.Field<string>("Manufacturer")
                };
                list.Add(manufacturer);
            }

            return list;
        }
        public static bool InsertManufacturerPassive(int customerId, string manufacturer,int createId)
        {
            return DAL.InsertManufacturerPassive(customerId, manufacturer, createId);
        }
        public static bool DeleteManufacturerPassive(int customerId, string manufacturer,int editId)
        {
            return DAL.DeleteManufacturerPassive(customerId, manufacturer,editId);
        }
        #endregion
    }

    public partial class DataAccessLayer
    {
        public DataTable GetManufacturerList(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Manufacturer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }
        public DataTable GetManufacturerList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_Manufacturer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }
        public DataTable GetManufacturerPassiveListByCustomer(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_Manufacturer_Passive_ByCustomerId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }
        public bool InsertManufacturerPassive(int pCustomerId, string pManufacturer, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Manufacturer_Passive", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pManufacturer, pCreateId });
        }
        public bool DeleteManufacturerPassive(int pCustomerId, string pManufacturer, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_Manufacturer_Passive", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pManufacturer, pEditId });
        }
    }
}