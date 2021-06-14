using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.ErpLayer
{
    public class CustomerInvoice
    {
        public CustomerInvoice()
        {

        }
        #region Properties

        public int Id { get; set; }
        public string CustomerCode { get; set; }
        public DateTime Date { get; set; }
        public DateTime DueDate { get; set; }
        public string TransactionType { get; set; }
        public string Explanation { get; set; }
        public double Debt { get; set; }
        public string DebtStr { get; set; }
        public double Credit { get; set; }
        public string CreditStr { get; set; }
        public string DocumentNo { get; set; }
        public string Ba { get; set; }
        public string CheckNo { get; set; }
        public string ReturnProduct { get; set; }
        public double Expense { get; set; }
        public double Balance { get; set; }
        public string BalanceStr { get; set; }
        public string Currency { get; set; }
        public string Href { get; set; }
        public string RemainingStr { get; set; }
        public double Remaining { get; set; }
        public int Day { get; set; }
        public double RemainingTotal { get; set; }
        public string RemainingTotalStr { get; set; }
        public double Closed { get; set; }
        public string ClosedStr { get; set; }
        public int Detail { get; set; }

        #endregion


    }
}