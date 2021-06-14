using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class OrderForPayment
    {
        #region Payment

        public OrderHeader OrderHeader { get; set; }
        public List<Basket> BasketList { get; set; }
        public List<OrderHeaderPayment> OrderHeaderPaymentList { get; set; }
        #endregion

        public OrderForPayment(OrderHeader header, List<Basket> senderList, List<OrderHeaderPayment> orderHeaderPayments)
        {
            OrderHeader = header;
            OrderHeaderPaymentList = orderHeaderPayments;
            BasketList = senderList;
        }
    }
}