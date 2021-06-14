using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class SalesmanOfCustomer : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int SalesmanId { get; set; }
        public Salesman Salesman { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public new bool Deleted { get; set; }



        #endregion

        #region Methods
        
        public static List<SalesmanOfCustomer> GetSalesmanOfCustomer(int pCustomerId)
        {
            DataTable dt = DAL.GetSalesmanOfCustomer(pCustomerId);
            List<SalesmanOfCustomer> list = new List<SalesmanOfCustomer>();

            foreach (DataRow row in dt.Rows)
            {
                SalesmanOfCustomer item = new SalesmanOfCustomer();
                {
                    item.Id = row.Field<int>("Id");
                    item.SalesmanId = row.Field<int>("SalesmanId");
                    item.Salesman = new Salesman()
                    {
                        Id = row.Field<int>("SalesmanId"),
                        Code = row.Field<string>("Code"),
                        Name = row.Field<string>("Name"),
                        Tel1 = row.Field<string>("Tel1"),
                        Tel2 = row.Field<string>("Tel2"),
                        Email = row.Field<string>("Email"),
                        PicturePath = row.Field<string>("PicturePath") == string.Empty || row.Field<string>("PicturePath") == null ? "../Content/Admin/images/noavatar.png" : GlobalSettings.FtpServerAddressFull + row.Field<string>("PicturePath"),

                    };
                };
                list.Add(item);
            }

            return list;
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetSalesmanOfCustomer(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_SalesmanOfCustomerList", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }
    }


}