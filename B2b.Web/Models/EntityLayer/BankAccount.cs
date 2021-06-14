

using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class BankAccount : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string AccountNumber { get; set; }
        public string Iban { get; set; }
        public int BankId { get; set; }
        #endregion

        #region Methods
        public static List<BankAccount> GetBankAccountList()
        {
            List<BankAccount> list = new List<BankAccount>();
            DataTable dt = DAL.GetBankAccountList();

            foreach (DataRow row in dt.Rows)
            {
                BankAccount item = new BankAccount()
                {
                    Id = row.Field<int>("Id"),
                    BankName = row.Field<string>("BankName"),
                    Branch = row.Field<string>("Branch"),
                    AccountNumber = row.Field<string>("AccountNumber"),
                    Iban = row.Field<string>("Iban"),
                    BankId = row.Field<int>("BankId")
                };
                list.Add(item);
            }
            return list;
        }

        public static List<BankAccount> GetBankList()
        {
            List<BankAccount> list = new List<BankAccount>();
            DataTable dt = DAL.GetBankList();

            foreach (DataRow row in dt.Rows)
            {
                BankAccount item = new BankAccount()
                {
                    Id = row.Field<int>("Id"),
                    BankName = row.Field<string>("BankName"),
                };
                list.Add(item);
            }
            return list;
        }


        public bool Delete()
        {
            return DAL.DeleteBankAccount(Id,EditId);
        }

        public bool Update()
        {
            return DAL.UpdateBankAccount(Id, BankName, Branch, AccountNumber, Iban, BankId, EditId);
        }

        public bool Add()
        {
            return DAL.InsertBankAccount(BankName, Branch, AccountNumber, Iban, BankId, CreateId);
        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public bool DeleteBankAccount(int pId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_BankAccount", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pEditId });
        }

        public bool InsertBankAccount(string pBankName, string pBranch, string pAccountNumber, string pIban, int pBankId, int pCreateId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_BankAccount", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pBankName, pBranch, pAccountNumber, pIban, pBankId, pCreateId });
        }

        public bool UpdateBankAccount(int pId, string pBankName, string pBranch, string pAccountNumber, string pIban, int pBankId, int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Update_BankAccount", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pBankName, pBranch, pAccountNumber, pIban, pBankId, pEditId });
        }


        public DataTable GetBankAccountList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_BankAccountBy");
        }

        public DataTable GetBankList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Bank");
        }

    }
}