using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Xml.Serialization;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class LogSearch : DataAccess
    {

        #region Parametres
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int SalesmanId { get; set; }
        public SearchCriteria Parameters { get; set; }
        public ProcessSearch Result { get; set; }
        public DateTime Date { get; set; }
        public string IpAddress { get; set; }
        public int LicenceId { get; set; }
        public int Type { get; set; }
        public Product Product { get; set; }
        #endregion

        public bool Insert()
        {

            DataTable dt = DAL.InserLogSearchHeader(CustomerId, UserId, SalesmanId, XmlSerialization.Serailize(Parameters), Result.ToString(), IpAddress, LicenceId, Type);
            if (dt.Rows.Count > 0)
            {
                Id = Convert.ToInt32(dt.Rows[0][0]);
                return true;
            }
            else
            {
                Id = -1;
                return false;
            }
        }

        public static List<LogSearch> GetList(int customerId, int userId, int salesmanId, DateTime startDate, DateTime endDate, int limit)
        {
            List<LogSearch> list = new List<LogSearch>();
            DataTable dt = DAL.GetListLogSearchHeader(customerId, userId, salesmanId, startDate.Date, endDate.Date.AddDays(1).AddMinutes(-1), limit);
            foreach (DataRow row in dt.Rows)
            {
                LogSearch obj = new LogSearch()
                {
                    Id = row.Field<int>("Id"),
                    CustomerId = row.Field<int>("CustomerId"),
                    UserId = row.Field<int>("UserId"),
                    SalesmanId = row.Field<int>("SalesmanId"),
                    Parameters = XmlSerialization.Deserialize<SearchCriteria>(row.Field<string>("Parameters")),
                    Result = (ProcessSearch)Enum.Parse(typeof(ProcessSearch), row.Field<string>("Result")),
                    Date = row.Field<DateTime>("Date"),
                };
                list.Add(obj);
            }

            return list;
        }

        public static List<LogSearch> GetListDetail(int headerId)
        {
            List<LogSearch> list = new List<LogSearch>();
            DataTable dt = DAL.GetListLogSearchDetail(headerId);
            foreach (DataRow row in dt.Rows)
            {
                LogSearch obj = new LogSearch()
                {
                  Product = new Product
                  {
                      Code = row.Field<string>("ProductCode"),
                      Name = row.Field<string>("Name"),
                      Manufacturer = row.Field<string>("Manufacturer"),
                      TotalQuantity = row.Field<double>("TotalQuantity"),
                      Price =Convert.ToDouble(row["Price"]),
                      CustomerCurrency = row.Field<string>("Currency")
                  }
                };
                list.Add(obj);
            }

            return list;

        } 
    }

    public enum ProcessSearch
    {
        Success,
        NoRecord,
        Fail
    }

    public partial class DataAccessLayer
    {


        public DataTable InserLogSearchHeader(int pCustomerId, int pUserId, int pSalesmanId, string pParameters, string pResult, string pIpAddress, int pLicenceId, int pType)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Insert_Log_Search", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId, pSalesmanId, pParameters, pResult, pIpAddress, pLicenceId, pType });
        }


        public DataTable GetListLogSearchHeader(int pCustomerId, int pUserId, int pSalesmanId, DateTime pStartDate, DateTime pEndDate, int pLimit)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Log_Search_Header", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId, pSalesmanId, pStartDate, pEndDate, pLimit });
        }
        public DataTable GetListLogSearchDetail(int pHeaderId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_LogSearch_Detail", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pHeaderId });
        }
    }
}