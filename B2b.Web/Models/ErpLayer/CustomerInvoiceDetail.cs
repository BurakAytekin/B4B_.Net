using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.ErpLayer
{
    public class CustomerInvoiceDetail
    {
        #region Properties

        public int InvoiceId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
        public double Price { get; set; }
        public string PriceStr { get; set; }
        public double Discount1 { get; set; }
        public double Discount2 { get; set; }
        public double Discount3 { get; set; }
        public double Discount4 { get; set; }
        public double Total { get; set; }
        public string TotalStr { get; set; }
        public string SubTotalStr { get; set; }
        public string DiscTotalStr { get; set; }
        public string VatTotalStr { get; set; }
        public string GeneralTotalStr { get; set; }
        public double Vat { get; set; }

        #endregion
    }
}