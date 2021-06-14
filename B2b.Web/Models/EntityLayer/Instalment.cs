

namespace B2b.Web.v4.Models.EntityLayer
{
    public class Instalment: DataAccess
    {
        #region Properties
        public int Id { get; set; }
        public int BankId { get; set; }
        public int Installment { get; set; }
        public string ExtraInstallment { get; set; }
        public string DeferalInstallment { get; set; }
        public double CommissionRate { get; set; }
        public string InstallmentText { get; set; }
        public string InstallmentValue { get; set; }
        public int InstallmentType { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
        #endregion
    }
}