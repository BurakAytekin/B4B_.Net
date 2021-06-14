using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.ErpLayer
{
    public class CustomerBalanceInformation
    {
        #region Properties
        public double UnClosedBalance { get; set; }
        public double Balance { get; set; }

        public string UnClosedBalanceStr { get; set; }
        public string BalanceStr { get; set; }
        #endregion

    }
}