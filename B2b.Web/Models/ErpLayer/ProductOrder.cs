using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.ErpLayer
{
    public class ProductOrder
    {
        #region Properties

        public DateTime Date { get; set; }
        public string DocumentNo { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
        public double Total { get; set; }
        public double Discount1 { get; set; }
        public double Discount2 { get; set; }
        public double Discount3 { get; set; }
        public double Discount4 { get; set; }
        public string Manufacturer { get; set; }

        #endregion

    }
}