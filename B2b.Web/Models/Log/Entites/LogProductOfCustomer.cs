

using B2b.Web.v4.Models.EntityLayer;

namespace B2b.Web.v4.Models.Log
{
    public class LogProductOfDay : DataAccess
    {
        #region Properties
        public int ProductOfDayId { get; set; }
        public int CustomerId { get; set; }
        public int SalesmanId { get; set; }
        public LogProductOfDayStatus Status { get; set; }
        #endregion
    }
    public enum LogProductOfDayStatus
    {
        Canceled = 0,
        Accepted = 1,
        AutoClose = 2,
    }
}