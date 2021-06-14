using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace B2b.Web.v4.Models.EntityLayer
{
    public class SpecialReports : DataAccess
    {
        public SpecialReports()
        {
        }
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string Parameters { get; set; }
        public string SpName { get; set; }
        #endregion
    }
}