using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace B2b.Web.v4.Models.EntityLayer
{
    public class ReportOrder : DataAccess
    {
        #region Properties
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public double Amount { get; set; }
        public double NetAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string OrderStatusStr
        {
            get
            {
                switch (OrderStatus)
                {
                    case OrderStatus.OnHold:
                        return "Beklemede";
                    case OrderStatus.Confirmed:
                        return "Onaylandı";
                    case OrderStatus.Deleted:
                        return "Silindi";
                    case OrderStatus.OnHoldInPool:
                        return "Havuzda Bekleyen";
                    case OrderStatus.CombinedInPool:
                        return "Havuzda Birleştirildi";
                    default:
                        return string.Empty;
                }
            }
        }
        public string OrderNotes { get; set; }
        #endregion

    }
}