using System;

using B2b.Web.v4.Models.Helper;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Session : DataAccess
    {
        public Guid Id { get; set; }
        public LoginType Type { get; set; }
        public int LoginId { get; set; }
        public int UserId { get; set; }
        public int TerminalNo { get; set; }

        public void GetItem(Guid id)
        {
            Id = id;
            DataTable dt = DAL.GetSession(Id);

            LoginId = -1;
            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];
                Type = (LoginType)row.Field<int>("Type");
                LoginId = row.Field<int>("LoginId");
                UserId = row.Field<int>("UserId");
                TerminalNo = row.Field<int>("TerminalNo");
            }
        }
        public static void DeleteSessionWindows(Guid pId)
        {
            DAL.DeleteSession(pId);
        }

        public void CreateAndInsert()
        {
            DataTable dt = DAL.InsertSession((int)Type, LoginId, UserId, TerminalNo);

            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];
                Id = row.Field<Guid>(0);
            }
        }

        public void Delete()
        {
            DAL.DeleteSession(Id);
        }
        public static DataTable CreateAndInsert(int type, int loginId, int licenceId, int userId)
        {
            return DAL.InsertSession(type, loginId, licenceId, userId);
        }
    }

    public partial class DataAccessLayer
    {
        public DataTable GetSession(Guid pId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetItem_Session", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }

        public bool DeleteSession(Guid pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Delete_Session", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pId });
        }

        public DataTable InsertSession(int pType, int pLoginId, int pUserId, int pTerminalNo)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Insert_Session", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pType, pLoginId, pUserId, pTerminalNo });
        }
    }


}