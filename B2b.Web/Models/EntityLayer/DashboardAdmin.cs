using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class DashboardAdmin : DataAccess
    {
        #region Properties

        public string Manufacturer { get; set; }
        public double Total { get; set; }

        public int WaitingOrder { get; set; }
        public int OrderCount { get; set; }
        public double OrderTotal { get; set; }
        public double PaymentTotal { get; set; }
        public int Month { get; set; }
        public int Type { get; set; }

        #endregion

        #region Methods

        public static List<DashboardAdmin> GetManufacturerReport(int pType)
        {
            List<DashboardAdmin> list = new List<DashboardAdmin>();
            DataTable dt = DAL.GetManufacturerReport(pType);

            foreach (DataRow row in dt.Rows)
            {
                DashboardAdmin obj = new DashboardAdmin()
                {
                    Manufacturer = row.Field<string>("Manufacturer"),
                    Total = Convert.ToDouble(row["Total"])
                };
                list.Add(obj);
            }
            return list;
        }

        public static List<DashboardAdmin> GetOrderPaymentCross()
        {
            List<DashboardAdmin> list = new List<DashboardAdmin>();
            DataTable dt = DAL.GetOrderPaymentCross();

            foreach (DataRow row in dt.Rows)
            {
                DashboardAdmin obj = new DashboardAdmin()
                {
                    Month = Convert.ToInt32(row["Month"]),
                    Type = Convert.ToInt32(row["Type"]),
                    Total = Convert.ToDouble(row["Total"])
                };
                list.Add(obj);
            }
            return list;
        }

        public static DashboardAdmin GetHeaderInformation()
        {
            DashboardAdmin item = new DashboardAdmin();
            DataTable dt = DAL.GetHeaderInformation();

            foreach (DataRow row in dt.Rows)
            {
                DashboardAdmin obj = new DashboardAdmin()
                {
                    WaitingOrder = Convert.ToInt32(row["WaitingOrder"]),
                    OrderCount = Convert.ToInt32(row["OrderCount"]),
                    OrderTotal = Convert.ToDouble(row["OrderTotal"]),
                    PaymentTotal = Convert.ToDouble(row["PaymentTotal"])
                };
                item = obj;
            }
            return item;
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetManufacturerReport(int pType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Report_Admin_DashboardManufacturer", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pType });
        }

        public DataTable GetOrderPaymentCross()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Report_Admin_DashboardOrderPaymentCross");
        }

        public DataTable GetHeaderInformation()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Report_Admin_DashboardHeader");
        }

    }
}