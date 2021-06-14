

using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class CustomerInstallment : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public int BankId { get; set; }
        public int Installment { get; set; }
        public int CustomerId { get; set; }
        public double SpecialAmount { get; set; }
        public string BankName { get; set; }
        #endregion

        #region Methods
        public static List<CustomerInstallment> GetCustomerInstallmentsByCustomerId(int pCustomerId)
        {
            List<CustomerInstallment> list = new List<CustomerInstallment>();
            DataTable dt = DAL.GetCustomerInstallmentsBtCustomerId(pCustomerId);

            foreach (DataRow row in dt.Rows)
            {
                CustomerInstallment item = new CustomerInstallment()
                {
                    Id = row.Field<int>("Id"),
                    BankId = row.Field<int>("BankId"),
                    Installment = row.Field<int>("Installment"),
                    CustomerId = row.Field<int>("CustomerId"),
                    SpecialAmount = row.Field<double>("SpecialAmount"),
                    BankName = row.Field<string>("BankName")

                };
                list.Add(item);
            }

            return list;
        }
        #endregion
    }
    public partial class DataAccessLayer
    {
        public DataTable GetCustomerInstallmentsBtCustomerId(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_CustomerInstallmentsByCustomerId", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
            
        }
    }
}