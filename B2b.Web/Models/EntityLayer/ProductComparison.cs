using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class ProductComparison : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public int UserId { get; set; }

        public int SalesmanId { get; set; }
        public int Type { get; set; }
        public new bool Deleted { get; set; }
        public int Count { get; set; }

        #endregion

        #region Methods


        public static List<ProductComparison> GetProductComparisonList(ComparisonOrFollowType type,LoginType loginType,Customer customer ,int pSalesmanId)
        {
            DataTable dt = DAL.GetProductComparisonList((int)type,customer.Users.Id, pSalesmanId);
            List<ProductComparison> list = new List<ProductComparison>();

            foreach (DataRow row in dt.Rows)
            {
                ProductComparison item = new ProductComparison();
                {
                    item.Id = row.Field<int>("Id");
                    item.ProductId = row.Field<int>("ProductId");
                    item.CreateDate = row.Field<DateTime>("CreateDate");
                    item.Product = Product.GetById(row.Field<int>("ProductId"), loginType, customer);
                    item.CustomerId = row.Field<int>("CustomerId");
                };
                list.Add(item);
            }

            return list;
        }

        public static List<ProductComparison> GetProductComparisonPreviewList(ComparisonOrFollowType type, LoginType loginType, Customer customer, int pSalesmanId)
        {
          List<ProductComparison> list = new List<ProductComparison>();

            DataTable dt = DAL.GetProductComparisonOrFollowByUserId((int)type, customer.Users.Id);
         
            foreach (DataRow row in dt.Rows)
            {

                ProductComparison item = new ProductComparison();
                {
                    item.Id = row.Field<int>("ComparisonId");
                    item.Product=new Product
                    {
                        Id = row.Field<int>("Id"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        Manufacturer=row.Field<string>("Manufacturer"),
                        ManufacturerCode = row.Field<string>("ManufacturerCode"),
                        PicturePath = row.Field<string>("PicturePath") == string.Empty ? "../Content/images/nophoto.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath")

                };
                   item.CreateDate = row.Field<DateTime>("CreateDate");

                };
                list.Add(item);
            }

            return list;

     
        }
        public static ProductComparison GetProductComparisonCount( int pUserId)
        {
            DataTable dt = DAL.GetProductComparisonCount(pUserId);
           ProductComparison item = new ProductComparison();

            foreach (DataRow row in dt.Rows)
            {
                item.Count =Convert.ToInt32(row["Count"]);
            }


            return item;
        }

        public static bool ComparisonInsert(int productId, int customerId, int userId, int salesmanId, int type, int createId)
        {
            return DAL.InsertProductComparison(productId, customerId, userId, salesmanId, type, createId);
        }
        public static bool DeleteProductComparison(int pId, int pEditId)
        {
            return DAL.DeleteProductComparison(pId, pEditId);
        }

        #endregion


    }
    public enum ComparisonOrFollowType
    {
        Follow = 0,
        Comparison = 1

    }

    public partial class DataAccessLayer
    {
        public bool InsertProductComparison(int pProductId, int pCustomerId, int pUserId, int pSalesmanId, int pType, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_ProductComparison", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId, pCustomerId, pUserId, pSalesmanId, pType, pCreateId });
        }
        public bool DeleteProductComparison(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Delete_ProductComparison", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }

        public DataTable GetProductComparisonList(int pType ,int pUserId,int pSalesmanId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_ProductComparisonList", MethodBase.GetCurrentMethod().GetParameters(), new object[] {pType, pUserId,pSalesmanId });
        }
        public DataTable GetProductComparisonCount(int pUserId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_ProductComparisonAvaibleStatus", MethodBase.GetCurrentMethod().GetParameters(), new object[] {  pUserId});
        }
        public DataTable GetProductComparisonOrFollowByUserId(int pType, int pUserId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetListProduct_ComparisonByType", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pType, pUserId });
        }
    }


}