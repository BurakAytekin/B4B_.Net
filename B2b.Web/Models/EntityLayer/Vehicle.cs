using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Vehicle:DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int GroupId { get; set; }
        public int OldGroupId { get; set; }
        public int VehicleId { get; set; }
        #endregion

        #region Methods
        public bool Delete()
        {
            return DAL.DeleteVehicle(Id,EditId);
        }
        public bool Insert()
        {
            return DAL.InsertVehicle(ProductId, GroupId, OldGroupId, VehicleId,CreateId);
        }
        #endregion
    }
    public  partial class DataAccessLayer
    {
        public bool DeleteVehicle(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Delete_Vehicle_ById", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }
        public bool InsertVehicle(int pProductId, int pGroupId, int pOldGroupId, int pVehicleId,int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Vehicle", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId, pGroupId, pOldGroupId, pVehicleId, pCreateId });
        }
    }
}