using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;


namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class VehicleBrandModel : DataAccess
    {
        #region Properties
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
        public string EngineCode { get; set; }
        public int Hp { get; set; }
        public int Kw { get; set; }
        public long RecordCount { get; set; }
        public int VehicleId { get; set; }
        #endregion

        #region Methods
        public static List<VehicleBrandModel> GetVehicleBrandList()
        {
            DataTable dt = new DataTable();
            List<VehicleBrandModel> list = new List<VehicleBrandModel>();
            dt = DAL.GetVehicleBrandList();
            foreach (DataRow row in dt.Rows)
            {
                VehicleBrandModel vehicleBrandModel = new VehicleBrandModel()
                {
                    Brand = row.Field<string>("Brand"),
                    RecordCount = row.Field<long>("reccount")
                };
                list.Add(vehicleBrandModel);
            }
            return list;
        }
        public static List<VehicleBrandModel> GetVehicleBrandModelList(string brandName)
        {
            DataTable dt = new DataTable();
            List<VehicleBrandModel> list = new List<VehicleBrandModel>();

            dt = DAL.GetVehicleBrandModelList(brandName);

            foreach (DataRow row in dt.Rows)
            {
                VehicleBrandModel vehicleBrandModel = new VehicleBrandModel()
                {
                    Brand = brandName,
                    Model = row.Field<string>("Model")
                };
                list.Add(vehicleBrandModel);
            }

            return list;
        }
        public static List<VehicleBrandModel> GetVehicleTypeList(string brand, string model)
        {
            DataTable dt = new DataTable();
            List<VehicleBrandModel> list = new List<VehicleBrandModel>();

            dt = DAL.GetVehicleTypeList(brand, model);

            foreach (DataRow row in dt.Rows)
            {
                VehicleBrandModel vehicleBrandModel = new VehicleBrandModel()
                {
                    VehicleId =  row.Field<int>("VehicleId"),
                    Type = row.Field<string>("Type"),
                    Date = row.Field<string>("Date"),
                    EngineCode = row.Field<string>("EngineCode"),
                    Hp = row.Field<int>("Hp"),
                    Kw = row.Field<int>("Kw"),
                };
                list.Add(vehicleBrandModel);
            }

            return list;
        }

        public static List<VehicleBrandModel> GetVehicleBrandModelTypeByGroupId(int groupId)
        {
            DataTable dt = new DataTable();
            List<VehicleBrandModel> list = new List<VehicleBrandModel>();

            dt = DAL.GetVehicleMarkModelListByGroupId(groupId);

            foreach (DataRow row in dt.Rows)
            {
                VehicleBrandModel item = new VehicleBrandModel()
                {
                    VehicleId = row.Field<int>("VehicleId"),
                    Brand = row.Field<string>("Brand"),
                    Model = row.Field<string>("Model"),
                    Type = row.Field<string>("Type"),
                    Date = row.Field<string>("Date"),
                    EngineCode = row.Field<string>("EngineCode"),
                    Kw = row.Field<int>("Kw"),
                    Hp = row.Field<int>("Hp")
                };
                list.Add(item);
            }

            return list;
        }


        #endregion
    }
    public  partial class  DataAccessLayer
    {
        public DataTable GetVehicleMarkModelListByGroupId(int pGroupId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_VehicleBrandModelByGroupId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pGroupId });
        }
        public DataTable GetVehicleTypeList(string pBrand, string pModel)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_VehicleType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pBrand, pModel });
        }

        public DataTable GetVehicleBrandList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Vehicle_Brand", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }
        public DataTable GetVehicleBrandModelList(string pBrand)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Vehicle_Brand_Model", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pBrand });
        }



    }
}