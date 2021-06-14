using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class LogContract : DataAccess
    {
        #region Properties

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public int SalesmanId { get; set; }
        public string Contract { get; set; }
        public ContractType Type { get; set; }

        #endregion

        #region Methods

        public bool Add()
        {
            return DAL.InsertLogContact(CustomerId, UserId, SalesmanId, Contract, (int)Type);
        }


        public static LogContract CheckKvkkContract(int pCustomerId)
        {
            LogContract list = new LogContract();
            DataTable dt = DAL.CheckKvkkContract(pCustomerId);

            foreach (DataRow row in dt.Rows)
            {
                LogContract obj = new LogContract()
                {
                    Id = row.Field<int>("Id"),
                    CreateDate = row.Field<DateTime>("CreateDate"),
                };
                list = obj;
            }
            return list;
        }

        #endregion

    }
    public enum ContractType
    {
        KvkkContract = 0,
    }


    public partial class DataAccessLayer
    {
        public DataTable CheckKvkkContract(int pCustomerId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_KvkkContract", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId });
        }


        public bool InsertLogContact(int pCustomerId, int pUserId, int pSalesmanId, string pContract, int pType)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_LogContract", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pCustomerId, pUserId, pSalesmanId, pContract, pType });
        }
    }

}