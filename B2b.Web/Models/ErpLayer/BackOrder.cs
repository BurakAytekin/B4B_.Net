using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.ErpLayer
{
    public class BackOrder
    {
        #region Properties

        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double QuantityTotal { get; set; }
        public double QuantitySent { get; set; }
        public double QuantityRemaining { get; set; }
        public double Quantity { get; set; }
        public string AvailabilityCss { get; set; }
        public string AvailabilityText { get; set; }

        #endregion

    }
}