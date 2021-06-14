using System;
using System.Data;
using System.Reflection;

namespace B2b.Web.v4.Models.EntityLayer
{
    [Serializable]
    public class Price
    {
        #region Properties
        public double Value { get; set; }
        public string Currency { get; set; }
        public double Rate { get; set; }
        public double ValueFinal { get { return Value * Rate; } }
        public string CurrencyHtml
        {
            get
            {
                string retval = Currency == null ? "TL" : Currency.ToString();
                switch (retval)
                {
                    case "TL":
                        retval = "<i class=\"fa fa-try\" aria-hidden=\"true\"></i>";
                        break;
                    case "USD":
                        retval = "<i class=\"fa fa-usd\" aria-hidden=\"true\"></i>";
                        break;
                    case "EUR":
                        retval = "<i class=\"fa fa-eur\" aria-hidden=\"true\"></i>";
                        break;
                    case "GBP":
                        retval = "<i class=\"fa fa-gbp\" aria-hidden=\"true\"></i>";
                        break;
                    case "CNY":
                    case "JPY":
                    case "RMB":
                    case "YEN":
                        retval = "<i class=\"fa fa-jpy\" aria-hidden=\"true\"></i>";
                        break;
                    case "RUB":
                    case "RUBLE":
                        retval = "<i class=\"fa fa-rub\" aria-hidden=\"true\"></i>";
                        break;
                    default:
                        break;
                }
                return retval;
            }
        }

        public bool IsSembolAtFront
        {
            get
            {
                string retval = Currency == null ? "TL" : Currency.ToString();
                switch (retval)
                {
                    case "USD":
                    case "EUR":
                    case "GBP":
                        return true;

                    default:
                        return false;
                }
            }
        }


        #endregion

        public Price()
        {
            Value = 0;
            Currency = "TL";
            Rate = 1;
        }

        public Price(double value, string currency, double rate = 1)
        {
            Value = value;
            Currency = currency;
            Rate = rate;
        }

        public override string ToString()
        {
            if (Value == 0)
                return "-";
            string format = IsSembolAtFront ? "{1}&nbsp;{0}" : "{0}&nbsp;{1}";
            return string.Format(format, (Value * Rate).ToString("N2"), CurrencyHtml);
        }

    }

}