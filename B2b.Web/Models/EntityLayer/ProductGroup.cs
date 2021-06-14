
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class ProductGroup : DataAccess
    {
        #region Properties
        public string Name { get; set; }
        public long RecordCount { get; set; }
        #endregion

        #region Methods
        public static List<ProductGroup> GetProductGroup1List(int customerId=-1)
        {
            List<ProductGroup> list = new List<ProductGroup>();
            DataTable dt = DAL.GetProductGroup1List(customerId);
            foreach (DataRow row in dt.Rows)
            {
                ProductGroup obj = new ProductGroup()
                {
                    Name = row.Field<string>("ProductGroup1"),
                    RecordCount = row.Field<long>("reccount")
                };
                list.Add(obj);
            }
            return list;
        }

        public static List<ProductGroup> GetProductGroup2List(int customerId, string group1Name)
        {
            List<ProductGroup> list = new List<ProductGroup>();
            DataTable dt = DAL.GetProductGroup2List(customerId, group1Name);
            foreach (DataRow row in dt.Rows)
            {
                ProductGroup obj = new ProductGroup()
                {
                    Name = row.Field<string>("ProductGroup2"),
                    RecordCount = row.Field<long>("reccount")
                };
                list.Add(obj);
            }
            return list;
        }

        public static List<ProductGroup> GetProductGroup3List(int customerId, string group1Name, string group2Name)
        {
            List<ProductGroup> list = new List<ProductGroup>();
            DataTable dt = DAL.GetProductGroup3List(customerId, group1Name, group2Name);
            foreach (DataRow row in dt.Rows)
            {
                ProductGroup obj = new ProductGroup()
                {
                    Name = row.Field<string>("ProductGroup3"),
                    RecordCount = row.Field<long>("reccount")
                };
                list.Add(obj);
            }
            return list;
        }
        public static List<ProductGroup> GetProductGroup4List(int customerId, string group1Name, string group2Name, string group3Name)
        {
            List<ProductGroup> list = new List<ProductGroup>();
            DataTable dt = DAL.GetProductGroup4List(customerId, group1Name, group2Name, group3Name);
            foreach (DataRow row in dt.Rows)
            {
                ProductGroup obj = new ProductGroup()
                {
                    Name = row.Field<string>("ProductGroup4")
                };
                list.Add(obj);
            }
            return list;
        }


        public static List<ProductGroup> GetProductGroup1ListByManufacturer(int customerId, string manufacturer)
        {
            List<ProductGroup> list = new List<ProductGroup>();
            DataTable dt = DAL.GetProductGroup1ListByManufacturer(customerId, manufacturer);
            foreach (DataRow row in dt.Rows)
            {
                ProductGroup obj = new ProductGroup()
                {
                    Name = row.Field<string>("ProductGroup1"),
                    RecordCount = row.Field<long>("reccount")
                };
                list.Add(obj);
            }
            return list;
        }

        public static List<ProductGroup> GetProductGroup2ListByManufacturer(int customerId, string group1Name, string manufacturer)
        {
            List<ProductGroup> list = new List<ProductGroup>();
            DataTable dt = DAL.GetProductGroup2ListByManufacturer(customerId, group1Name, manufacturer);
            foreach (DataRow row in dt.Rows)
            {
                ProductGroup obj = new ProductGroup()
                {
                    Name = row.Field<string>("ProductGroup2"),
                    RecordCount = row.Field<long>("reccount")
                };
                list.Add(obj);
            }
            return list;
        }

        public static List<ProductGroup> GetProductGroup3ListByManufacturer(int customerId, string productGroup1, string productGroup2, string manufacturer)
        {
            List<ProductGroup> list = new List<ProductGroup>();
            DataTable dt = DAL.GetProductGroup3ListByManufacturer(customerId, productGroup1, productGroup2, manufacturer);
            foreach (DataRow row in dt.Rows)
            {
                ProductGroup obj = new ProductGroup()
                {
                    Name = row.Field<string>("ProductGroup3"),
                    RecordCount = row.Field<long>("reccount")
                };
                list.Add(obj);
            }
            return list;
        }
        #endregion
    }

    public partial class DataAccessLayer
    {
        public DataTable GetProductGroup1List(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ProductGroup1", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }

        public DataTable GetProductGroup2List(int pCustomerId, string pProductGroup1)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ProductGroup2", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pProductGroup1 });
        }

        public DataTable GetProductGroup3List(int pCustomerId, string pProductGroup1, string pProductGroup2)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ProductGroup3", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pProductGroup1, pProductGroup2 });
        }
        public DataTable GetProductGroup4List(int pCustomerId, string pProductGroup1, string pProductGroup2, string pProductGroup3)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ProductGroup4", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pProductGroup1, pProductGroup2, pProductGroup3 });
        }

        public DataTable GetProductGroup1ListByManufacturer(int pCustomerId, string pManufacturer)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ProductGroup1_ByManufacturer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pManufacturer });
        }

        public DataTable GetProductGroup2ListByManufacturer(int pCustomerId, string pProductGroup1, string pManufacturer)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ProductGroup2_ByManufacturer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pProductGroup1, pManufacturer });
        }


        public DataTable GetProductGroup3ListByManufacturer(int pCustomerId, string pProductGroup1, string pProductGroup2, string pManufacturer)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ProductGroup3_ByManufacturer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pProductGroup1, pProductGroup2, pManufacturer });
        }
    }
}