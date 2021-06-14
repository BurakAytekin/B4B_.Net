using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class WarehouseQuantity : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Warehouse Warehouse { get; set; }
        public double Quantity { get; set; }
        public double QuantityOnWay { get; set; }
        public int QuantityType { get; set; }
        #endregion

        public static List<WarehouseQuantity> GetList(int productId, int warehouseId)
        {
            List<WarehouseQuantity> list = new List<WarehouseQuantity>();
            DataTable dt = DAL.GetWarehouseQuantityList(productId, warehouseId);
            foreach (DataRow row in dt.Rows)
            {
                WarehouseQuantity obj = new WarehouseQuantity()
                {
                    Id = row.Field<int>("Id"),
                    ProductId = row.Field<int>("ProductId"),
                    Warehouse = new Warehouse()
                    {
                        Id = row.Field<int>("WarehouseId"),
                        Code = row.Field<string>("WarehouseCode"),
                        Name = row.Field<string>("WarehouseName"),
                    },
                    Quantity = row.Field<double>("Quantity"),
                    QuantityOnWay = row.Field<double>("QuantityOnWay"),
                };
                list.Add(obj);
            }
            return list;
        }
        public static List<WarehouseQuantity> GetListProductQuantity(int productId)
        {
            List<WarehouseQuantity> list = new List<WarehouseQuantity>();
            DataTable dt = DAL.GetWarehouseProductId(productId);
            foreach (DataRow row in dt.Rows)
            {
                WarehouseQuantity obj = new WarehouseQuantity()
                {
                    Id = row.Field<int>("Id"),
                    Warehouse = new Warehouse()
                    {
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        Priority = row.Field<int>("Priority"),
                    },
                    Quantity = row.Field<double>("Quantity"),
                    QuantityOnWay = row.Field<double>("QuantityOnWay"),
                    QuantityType = row.Field<int>("QuantityType")
                };
                list.Add(obj);
            }
            return list;
        }
        public static List<WarehouseQuantity> GetListProductQuantityByCustomerId(int productId, int pCustomerId)
        {
            List<WarehouseQuantity> list = new List<WarehouseQuantity>();
            DataTable dt = DAL.GetWarehouseProductIdAndCustomerId(productId, pCustomerId);
            foreach (DataRow row in dt.Rows)
            {
                WarehouseQuantity obj = new WarehouseQuantity()
                {
                    
                    Warehouse = new Warehouse()
                    {
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                      
                    },
                    Quantity = Convert.ToDouble(row["Quantity"]),
                    QuantityOnWay = Convert.ToDouble(row["QuantityOnWay"]),
                    QuantityType = Convert.ToInt32(row["QuantityType"])
                };
                list.Add(obj);
            }
            return list;
        }
        public static bool UpdateQuantity(int pId, double pQuantityOnWay, double pQuantity,int pQuantityType)
        {
            return DAL.UpdateWareHouseQuantity(pId, pQuantityOnWay, pQuantity, pQuantityType);
        }
    }


    public partial class DataAccessLayer
    {
        public DataTable GetWarehouseQuantityList(int pProductId, int pWarehouseId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_WarehouseQuantity", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId, pWarehouseId });
        }
        public DataTable GetWarehouseProductId(int pId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_ProductWarehouse", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }
        public DataTable GetWarehouseProductIdAndCustomerId(int pProductId, int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_WarehouseQuantityByCustomerId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId, pCustomerId });
        }
        public bool UpdateWareHouseQuantity(int pId, double pQuantityOnWay, double pQuantity,int pQuantityType)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_WarehouseQuantity", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pQuantityOnWay, pQuantity , pQuantityType });
        }

    }
}