

using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Currency : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public string Type { get; set; }
        public double Rate { get; set; }
        public string Icon { get; set; }
        public bool CheckBist { get; set; }

        #endregion

        #region Methods
        public static List<Currency> GetList()
        {
            List<Currency> list = new List<Currency>();
            DataTable dt = DAL.GetCurrencyList();

            foreach (DataRow row in dt.Rows)
            {
                Currency obj = new Currency()
                {
                    Id = row.Field<int>("Id"),
                    Type = row.Field<string>("Type"),
                    Rate = row.Field<double>("Rate"),
                    Icon = row.Field<string>("Icon"),
                    CheckBist = row.Field<bool>("CheckBist")
                };
                list.Add(obj);
            }
            return list;
        }

        public bool Delete()
        {
            return DAL.DeleteCurrency(Id);
        }

        public bool AddOrUpdate()
        {
            return DAL.AddOrUpdate(Id, Type, Rate, Icon, CheckBist, CreateId,EditId);
        }

        #endregion
    }

    public partial class DataAccessLayer
    {
        public DataTable GetCurrencyList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_Currency");
        }

        public bool DeleteCurrency(int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_Currency", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }

        public bool AddOrUpdate(int pId, string pType, double pRate, string pIcon, bool pCheckBist, int pCreateId , int pEditId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_Or_Update_Currency", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pType, pRate, pIcon, pCheckBist, pCreateId,pEditId });
        }
    }
}