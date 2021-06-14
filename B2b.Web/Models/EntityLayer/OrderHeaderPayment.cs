using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class OrderHeaderPayment : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int OrderId { get; set; }
        public int Type { get; set; }

        public string TypeStr
        {
            get
            {
                switch (Type)
                {
                    case 1:
                        return "Nakit / Havale";
                    case 2:
                        return "Çek";
                    case 3:
                        return "Senet";
                    case 4:
                        return "Dbs";
                    case 5:
                        return "Kredi Kartı";
                    default:
                        return "-";
                }
            }
        }

        public double Total { get; set; }
        public DateTime PaymentDate { get; set; }
        public string ReceiptNo { get; set; }
        public string BankName { get; set; }
        public bool IsPaymentOk { get; set; }

        public virtual Base64Input Base64 { get; set; }
        public string Image { get; set; }
        #endregion
        //Insert_OrderHeader_Payment

        #region Methods
        public void Insert()
        {
            DAL.InsertOrderHeaderPayment(OrderId, Type, Total, PaymentDate, ReceiptNo, BankName, CreateId,Image);
        }

        public static List<OrderHeaderPayment> GetListByOrderId(int pOrderId)
        {
            List<OrderHeaderPayment> list = new List<OrderHeaderPayment>();
            DataTable dt = DAL.GetListOrderHeaderPaymentByOrderId(pOrderId);
            foreach (DataRow row in dt.Rows)
            {
                OrderHeaderPayment item = new OrderHeaderPayment();
                item.Total = row.Field<double>("Total");
                item.Type = row.Field<int>("Type");
                item.BankName = row.Field<string>("BankName");
                item.ReceiptNo = row.Field<string>("ReceiptNo");
                item.PaymentDate = row.Field<DateTime>("PaymentDate");
                item.Image = row.Field<string>("Image");

                list.Add(item);
            }
            return list;
        }
        #endregion
    }
    public partial class DataAccessLayer
    {
        public bool InsertOrderHeaderPayment(int pOrderId, int pType, double pTotal, DateTime pPaymentDate, string pReceiptNo, string pBankName, int pCreateId, string pImage)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "Insert_OrderHeader_Payment", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pOrderId, pType, pTotal, pPaymentDate, pReceiptNo, pBankName, pCreateId, pImage });
        }

        public DataTable GetListOrderHeaderPaymentByOrderId(int pOrderId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "GetList_OrderHeaderPaymetByOrderId",
                MethodBase.GetCurrentMethod().GetParameters(), new object[] { pOrderId });
        }
    }
    public class Base64Input
    {

        public int filesize { get; set; }

        public string filetype { get; set; }

        public string filename { get; set; }

        public string base64 { get; set; }
        
  
    }
}