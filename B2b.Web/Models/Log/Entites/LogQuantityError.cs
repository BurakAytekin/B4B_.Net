using B2b.Web.v4.Models.EntityLayer;
using System;


namespace B2b.Web.v4.Models.Log
{
    public class LogQuantityError:DataAccess
    {
        #region Constructors
        public LogQuantityError()
        {
            Client = ClientType.B2BWeb;
        }
        #endregion

        #region Parametres
        public int Id { get; set; }
        public ClientType Client { get; set; }
        public int CustomerId { get; set; }
        public int SalesmanId { get; set; }
        public double QuantityRequested { get; set; }
        public double QuantityAvaible { get; set; }
        public string Selection { get; set; }
        public DateTime Date { get; set; }
        #endregion
    }
}