using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using B2b.Web.v4.Models.Log;


 namespace B2b.Web.v4.Models.EntityLayer
{
    public class LogGeneralError : DataAccess
    {
        #region Constructors
        public LogGeneralError()
        {
            Client = ClientType.B2BWeb;
            LogType = LogGeneralErrorType.Error;
            CustomerId = -1;
            SalesmanId = -1;
            UserId = -1;
            Latitude = 0;
            Longitude = 0;
            LicenceId = -1;
        }
        #endregion

        #region Properties
        public int Id { get; set; }
        public ClientType Client { get; set; }
        public int CustomerId { get; set; }
        public int SalesmanId { get; set; }
        public string Source { get; set; }
        public string Explanation { get; set; }
        public int LicenceId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int UserId { get; set; }
        public string IpAddress { get; set; }
        public LogGeneralErrorType LogType { get; set; }
        public async Task<bool> Save()
        {
            return DAL.SaveLogGeneral(Client.ToString(), CustomerId, SalesmanId, LogType.ToString(), Source, Explanation, UserId, IpAddress, LicenceId, Latitude, Longitude);
        }

        #endregion
    }


    public partial class DataAccessLayer
    {
        public bool SaveLogGeneral(string pClientType, int pCustomerId, int pSalesmanId, string pLogType, string pSource, string pExplanation, int pUserId, string pIpAddress, int pLicenceId, double pLatitude, double pLongitude)
        {
            return DatabaseContext.ExecuteNonQuery(CommandType.StoredProcedure, "_Insert_Log_GeneralError", MethodBase.GetCurrentMethod().GetParameters(), new object[] { pClientType, pCustomerId, pSalesmanId, pLogType, pSource, pExplanation, pUserId, pIpAddress, pLicenceId, pLatitude, pLongitude });

        }
    }
}