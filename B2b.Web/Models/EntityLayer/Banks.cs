using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Banks : DataAccess
    {

        #region Properties

        public int Id { get; set; }
        public string BankName { get; set; }
        public string Type { get; set; }


        #endregion

    }
}