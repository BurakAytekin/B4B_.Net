using System;
using System.Threading.Tasks;
using B2b.Web.v4.Models.Log.Entites;
using B2b.Web.v4.Models.Log;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Logger
    {

        public static Task<bool> LogGeneral(LogGeneralErrorType type, ClientType clientType, string source, Exception ex, string ipAddress, int customerId = -1, int userId = -1, int salesmanId = -1, int licenceId = -1, double latitude = 0, double longitude = 0)
        {
            LogGeneralError log = new LogGeneralError()
            {
                LogType = type,
                Client = clientType,
                Explanation = (ex != null) ? ex.Message : string.Empty,
                Source = source,
                IpAddress = ipAddress,
                CustomerId = customerId,
                SalesmanId = salesmanId,
                UserId = userId,
                LicenceId = licenceId,
                Latitude = latitude,
                Longitude = longitude,

            };
            return Task.Run(() => log.Save());
        }

        public static Task<bool> LogGeneral(LogGeneralErrorType type, ClientType clientType, string source, string ex, string ipAddress, int customerId = -1, int userId = -1, int salesmanId = -1, int licenceId = -1, double latitude = 0, double longitude = 0)
        {
            LogGeneralError log = new LogGeneralError()
            {
                LogType = type,
                Client = clientType,
                Explanation = ex ?? string.Empty,
                Source = source,
                IpAddress = ipAddress,
                CustomerId = customerId,
                SalesmanId = salesmanId,
                UserId = userId,
                LicenceId = licenceId,
                Latitude = latitude,
                Longitude = longitude

            };
            return Task.Run(() => log.Save());
        }

        public static Task<bool> LogTransaction(ClientType clientType, LogTransactionSource source, string process, string message, string ipAddress, int licenceId = -1, int customerId = -1, int userId = -1, int salesmanId = -1, double latitude = 0.0, double longitude = 0.0)
        {
            LogTransaction log = new LogTransaction()
            {
                ClientType = clientType,
                CustomerId = customerId,
                SalesmanId = salesmanId,
                UserId = userId,
                Source = source,
                Process = process,
                Explanation = message ?? string.Empty,
                Latitude = latitude,
                Longitude = longitude,
                IpAddress = ipAddress,

                LicenceId = licenceId
            };
            return Task.Run(() => log.Save());
        }
        public static Task<bool> LogNavigation(int customerId, int userId, int salesmanId, string navigation, ClientType clientType, string ipAddress)
        {
            
            LogNavigation log = new LogNavigation()
            {
                CustomerId = customerId,
                UserId = userId,
                SalesmanId = salesmanId,
                Navigation = navigation,
                ClientType = clientType,
                IpAddress = ipAddress
            };
            return Task.Run(() => log.Save());
        }



        public static int LogSearchHeader(int customerId, int userId, int salesmanId, SearchCriteria paremeters, ProcessSearch result, string ipAddress, int licenceId, int pType)
        {
            LogSearch log = new LogSearch()
            {
                CustomerId = customerId,
                UserId = userId,
                SalesmanId = salesmanId,
                Parameters = paremeters,
                Result = result,
                IpAddress = ipAddress,
                LicenceId = licenceId,
                Type = pType
            };
            log.Insert();
            return log.Id;

        }
        public static void LogSearchOpenDetail(int customerId, string customerCode, int userId, int salesmanId, int productId, string productCode, int logHeaderId)
        {
            LogSearchOpenDetail log = new LogSearchOpenDetail()
            {
                CustomerId = customerId,
                CustomerCode = customerCode,
                UserId = userId,
                SalesmanId = salesmanId,
                ProductId = productId,
                ProductCode = productCode,
                LogHeaderId = logHeaderId

            };
            log.Insert();

        }

        #region PaymentLog

        public static int LogPayment(LogPaymentType type, string source, string ex, string currentpaymentid, string bankName, int customerId = -1, int salesmanId = -1, bool redirect = true)
        {
            LogPayment log = new LogPayment()
            {
                LogType = type,
                Explanation = ex,
                Source = source,
                CustomerId = customerId,
                SalesmanId = salesmanId,
                CurrentPaymentId = currentpaymentid,
                BankName = bankName
            };

            log.Save();
            return log.Id;
        }
        #endregion


    }

}