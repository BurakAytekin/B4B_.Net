using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class CouponCustomers : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public string RuleCode { get; set; }

        public string Header { get; set; }
        public int CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public bool IsUsed { get; set; }
        public bool IsActive { get; set; }

        #endregion

        #region Methods



        public static List<CouponCustomers> GetCouponCustomersList(int pCouponId, int pType)
        {
            List<CouponCustomers> list = new List<CouponCustomers>();
            DataTable dt = DAL.GetCouponCustomersList(pCouponId, pType);

            foreach (DataRow row in dt.Rows)
            {
                CouponCustomers obj = new CouponCustomers()
                {
                    Id = Convert.ToInt32(row["Id"]),
                    CouponId = Convert.ToInt32(row["CouponId"]),
                    CouponCode = row.Field<string>("CouponCode"),
                    RuleCode = row.Field<string>("RuleCode"),
                    Header = row.Field<string>("Header"),
                    CustomerId = Convert.ToInt32(row["CustomerId"]),
                    CustomerCode = row.Field<string>("CustomerCode"),
                    CustomerName = row.Field<string>("CustomerName"),
                    IsUsed = Convert.ToBoolean(row["IsUsed"]),
                    IsActive = Convert.ToBoolean(row["IsActive"])
                };
                list.Add(obj);
            }
            return list;
        }

        public bool SetAllCustomers()
        {
            return DAL.SetAllCustomers(CouponId, CreateId);
        }

        public bool Add()
        {
            return DAL.InsertCouponCustomers(CouponId, CustomerId, CreateId);
        }

        public bool Update()
        {
            return DAL.UpdateCouponCustomers(Id, IsActive, Deleted, EditId);
        }

        #endregion 

    }
    public partial class DataAccessLayer
    {
        public bool UpdateCouponCustomers(int pId, bool pIsActive, bool pDeleted, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_CouponCustomers", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pIsActive, pDeleted, pEditId });
        }

        public bool InsertCouponCustomers(int pCouponId, int pCustomerId, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_CouponCustomers", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCouponId, pCustomerId, pCreateId });
        }

        public DataTable GetCouponCustomersList(int pCouponId, int pType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_CouponCustomers", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCouponId, pType });
        }


        public bool SetAllCustomers(int pId, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_SetCouponAllCustomers", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pCreateId });
        }
    }
}