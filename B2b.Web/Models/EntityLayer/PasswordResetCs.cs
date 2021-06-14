using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class PasswordResetCs : DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Guid { get; set; }
        public PasswordResetType Type { get; set; }
        public int Status { get; set; }
        public string Password { get; set; }
        public string PasswordRepeat { get; set; }

        #endregion

        #region Methods

        public bool Add()
        {
            return DAL.AddPasswordReset(PersonId, Guid, (int)Type);
        }

        public bool Update()
        {
            return DAL.UpdatePasswordReset(Id, Status);
        }

        public static PasswordResetCs CheckPasswordResetGuid(string guid)
        {
            PasswordResetCs list = new PasswordResetCs();
            DataTable dt = DAL.CheckPasswordResetGuid(guid);

            foreach (DataRow row in dt.Rows)
            {
                PasswordResetCs obj = new PasswordResetCs()
                {
                    Id = row.Field<int>("Id"),
                    PersonId = row.Field<int>("PersonId"),
                    Type = (PasswordResetType)row.Field<int>("Type"),
                    Status = row.Field<int>("PersonId"),
                    Guid = guid,
                    CreateDate = row.Field<DateTime>("CreateDate")

                };
                list = obj;
            }
            return list;
        }

        #endregion

    }
    public enum PasswordResetType
    {
        User = 0,
        FinancePassword = 1,
        Salesman = 2
    }

    public partial class DataAccessLayer
    {
        public DataTable CheckPasswordResetGuid(string pGuid)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_PasswordResetItem", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pGuid });
        }

        public bool AddPasswordReset(int pPersonId, string pGuid, int pType)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Insert_PasswordReset", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pPersonId, pGuid, pType });

        }

        public bool UpdatePasswordReset(int pId, int pStatus)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Password_PasswordReset", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId, pStatus });

        }

    }

}