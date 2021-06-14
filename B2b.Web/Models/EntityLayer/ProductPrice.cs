using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class ProductPrice : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int ProductId { get; set; }
        public int PriceNumber { get; set; }
        public int Type { get; set; }
        public string TypeStr { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public int Priority { get; set; }
        public double CurrencyRate { get; set; }


        #endregion

        #region Methods


        public static List<ProductPrice> GetListProductPrice(int prodcutId)
        {
            List<ProductPrice> priceList = new List<ProductPrice>();
            DataTable dt = DAL.GetProductPrice(prodcutId);
            foreach (DataRow row in dt.Rows)
            {
                ProductPrice price = new ProductPrice
                {
                    Id = row.Field<int>("Id"),
                    ProductId = row.Field<int>("ProductId"),
                    PriceNumber = row.Field<int>("PriceNumber"),
                    Type = row.Field<int>("Type"),
                    TypeStr = row.Field<string>("TypeStr"),
                    Price = row.Field<double>("Price"),
                    Currency = row.Field<string>("Currency"),
                 //   CurrencyRate = Convert.ToDouble(row["CurrencyRate"])
                };
                priceList.Add(price);
            }


            return priceList;
        }
        public static bool Insert(int pProductId, int pPriceNumber, string pProductCode, int pType, double pPrice, string pCurrency, int pCreateId)
        {
            return DAL.InsertProductPrice(pProductId, pPriceNumber, pProductCode, pType, pPrice, pCurrency, pCreateId);
        }
        public static bool Update(int pId, int pPriceNumber, int pType, double pPrice, string pCurrency, int pEditId)
        {
            return DAL.UpdateProductPrice(pId, pPriceNumber, pType, pPrice, pCurrency, pEditId);
        }
        public static bool Delete(int pId, int pEditId)
        {
            return DAL.DeleteProductPrice(pId, pEditId);
        }


        #endregion


    }


    public partial class DataAccessLayer
    {
        public DataTable GetProductPrice(int pId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_ProductPrice", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }
        public bool InsertProductPrice(int pProductId, int pPriceNumber, string pProductCode, int pType, double pPrice, string pCurrency, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Product_Price", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pProductId, pPriceNumber, pProductCode, pType, pPrice, pCurrency, pCreateId });
        }
        public bool UpdateProductPrice(int pId, int pPriceNumber, int pType, double pPrice, string pCurrency, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_Product_Price", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pPriceNumber, pType, pPrice, pCurrency, pEditId });
        }
        public bool DeleteProductPrice(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_Product_Price", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }
    }
}