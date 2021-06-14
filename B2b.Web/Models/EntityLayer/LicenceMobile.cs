using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class LicenceMobile:DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public LicenceMobileSource Source { get; set; }
        public int UserId { get; set; }
        public int UserType { get; set; }
        public int TerminalNo { get; set; }
        public DateTime Date { get; set; }
        public string IMEI { get; set; }
        public string DeviceType { get; set; }


        #endregion

        #region Methods
        public static List<LicenceMobile> GetListByUser(int userType, int userId)
        {
            DataTable dt = new DataTable();
            List<LicenceMobile> list = new List<LicenceMobile>();

            dt = DAL.GetLicenceMobielListByUserId(userType, userId);

            foreach (DataRow row in dt.Rows)
            {
                LicenceMobile licence = new LicenceMobile()
                {
                    Id = row.Field<int>("Id"),
                    //Source = (LicenceMobileSource)row.Field<int>("DeviceType"),
                    TerminalNo = row.Field<int>("TerminalNo"),
                    UserId = row.Field<int>("UserId"),
                    UserType = row.Field<int>("UserType"),
                    IMEI = row.Field<string>("IMEI"),
                    Date = row.Field<DateTime>("Date"),
                    DeviceType = row.Field<string>("DeviceType"),
                };
                list.Add(licence);
            }

            return list;
        }

        public bool Delete()
        {
            return DAL.DeleteLicenceMobile(Id);
        }
        #endregion
    }
    public  partial class  DataAccessLayer
    {
        public DataTable GetLicenceMobielListByUserId(int pUserType, int pUserId)
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_Admin_GetList_LicenceMobile_ByUser", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pUserType, pUserId });
        }
        public bool DeleteLicenceMobile(int pId)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Admin_Delete_LicenceMobile", MethodBase.GetCurrentMethod().GetParameters(),
                new object[] { pId });
        }
    }
    public enum LicenceMobileSource
    {
        Android = 1,
        IOS = 2,
        WindowsPhone=3
    }
}