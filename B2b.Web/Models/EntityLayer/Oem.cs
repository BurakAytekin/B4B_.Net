using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;


namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class Oem : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int GroupId { get; set; }
        public int GroupIdOld { get; set; }
        public int AddId { get; set; }
        public int AddType { get; set; }
        public string Brand { get; set; }
        public int Type { get; set; }
        public string OemNo { get; set; }
        #endregion

        #region Methods

        public static List<Oem> GetListByGroupId(int groupId, OemType type)
        {
            List<Oem> oemList = new List<Oem>();
            DataTable dt = DAL.GetOemListByGroupId(groupId, (int)type);

            foreach (DataRow row in dt.Rows)
            {
                Oem oem = new Oem()
                {
                    Id = row.Field<int>("Id"),
                    ProductId = row.Field<int>("ProductId"),
                    Brand = row.Field<string>("Brand"),
                    OemNo = row.Field<string>("OemNo"),
                    Type = (int)type

                };
                oemList.Add(oem);
            }

            return oemList;
        }
        public static List<Oem> GetVehicleBrandList()
        {
            List<Oem> oemList = new List<Oem>();
            DataTable dt = DAL.GetOemVehicleBrandList();

            foreach (DataRow row in dt.Rows)
            {
                Oem oem = new Oem()
                {
                    Brand = row.Field<string>("Brand")

                };
                oemList.Add(oem);
            }

            return oemList;
        }


        public static bool Insert(Product product, Oem oem, OemType type, int addId, int addType)
        {
            return DAL.InsertOemOrRival(product.Id, product.GroupId, product.OldGruopId, addType, oem.Brand, oem.OemNo, (int)type, product.Code, addId);
        }
        public static bool Update(int pId, string pBrand, string pOemNo, int pEditId)
        {
            return DAL.UpdateOemOrRival(pId, pBrand, pOemNo, pEditId);
        }
        
        public bool Delete()
        {
            return DAL.DeleteOemOrRival(Id, EditId);
        }
        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetOemListByGroupId(int pGroupId, int pType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_OemNoByGroupId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pGroupId, pType });
        }
        public DataTable GetOemVehicleBrandList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_OemBrandList", MethodBase.GetCurrentMethod().GetParameters(), new object[] { });
        }
        public bool InsertOemOrRival(int pProductId, int pGroupId, int pGroupOldId, int pAddType, string pBrand, string pOemNo, int pType, string pProductCode, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_OemOrRivalCode", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId, pGroupId, pGroupOldId, pAddType, pBrand, pOemNo, pType, pProductCode, pCreateId });
        }
        public bool UpdateOemOrRival(int pId, string pBrand, string pOemNo, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_OemOrRival", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pBrand, pOemNo, pEditId });
        }
        
        public bool DeleteOemOrRival(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_OemOrRival", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }
    }
    public enum OemType
    {
        OemNo = 0,
        RivalCode = 1,
    }

}