using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;


namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class ShipmentType : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }
        #endregion

        #region Methods
        public static List<ShipmentType> GetShipmentTypeList()
        {
            List<ShipmentType> list = new List<ShipmentType>();
            DataTable dt = DAL.GetShipmentTypeList();

            foreach (DataRow row in dt.Rows)
            {
                ShipmentType item = new ShipmentType()
                {
                    Id = row.Field<int>("Id"),
                    Name = row.Field<string>("Name"),
                    Priority = row.Field<int>("Priority")
                };
                list.Add(item);
            }
            return list;
        }

        public static List<ShipmentType> GetShipmentTypeListForAdmin()
        {
            List<ShipmentType> list = new List<ShipmentType>();
            DataTable dt = DAL.GetShipmentTypeListForAdmin();

            foreach (DataRow row in dt.Rows)
            {
                ShipmentType item = new ShipmentType()
                {
                    Id = row.Field<int>("Id"),
                    Name = row.Field<string>("Name"),
                    Priority = row.Field<int>("Priority")
                };
                list.Add(item);
            }
            return list;
        }

        public bool Update()
        {
            return DAL.UpdateShipmentType(Id, Name, EditId, Priority);
        }

        public bool Delete()
        {
            return DAL.DeleteShipmentType(Id, EditId);
        }

        public bool Add()
        {
            return DAL.InsertShipmentType(Name, CreateId, Priority);
        }


        #endregion
    }

    public partial class DataAccessLayer
    {
        public bool InsertShipmentType(string pName, int pCreateId, int pPriority)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_ShipmentType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pName, pCreateId, pPriority });
        }

        public bool UpdateShipmentType(int pId, string pName, int pEditId, int pPriority)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_ShipmentType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pName, pEditId, pPriority });
        }

        public bool DeleteShipmentType(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_ShipmentType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId});
        }

        public DataTable GetShipmentTypeList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ShipmentType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

        public DataTable GetShipmentTypeListForAdmin()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_ShipmentType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }

    }
}