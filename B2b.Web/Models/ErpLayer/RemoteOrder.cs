using B2b.Web.v4.Models.EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.ErpLayer
{
    public class RemoteOrder
    {
        public string SalesmanCode { get; set; }
        public string CustomerCode { get; set; }
        public string ProductCode { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public string Currency { get; set; }
        public bool IsUseSpecialPrice { get; set; }
        public Product Product { get; set; }
    }

    public class RemoteOrderResponse
    {
        public bool Result { get; set; }
        public int OrderId { get; set; }
        public string Explanation { get; set; }
    }
}