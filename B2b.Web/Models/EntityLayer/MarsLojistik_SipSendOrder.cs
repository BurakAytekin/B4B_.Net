using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace B2b.Web.v4.Models.EntityLayer
{

    [XmlRoot(ElementName = "ORDERPACKET")]
    public class ORDERPACKET
    {
        [XmlElement(ElementName = "PROJECT_CODE")]
        public string PROJECT_CODE { get; set; }
        [XmlElement(ElementName = "MASTERS")]
        public MASTERS MASTERS { get; set; }
        [XmlElement(ElementName = "RESULT")]
        public string RESULT { get; set; }
        [XmlElement(ElementName = "REASON")]
        public string REASON { get; set; }


        public ORDERPACKET()
        {
            PROJECT_CODE = "HANTECH";
            this.MASTERS = new MASTERS();
            RESULT = "Success";
            REASON = string.Empty;
        }
    }

    [XmlRoot(ElementName = "MASTERS")]
    public class MASTERS
    {
        [XmlElement(ElementName = "MASTER")]
        public List<MASTER> MASTER { get; set; }

        public MASTERS()
        {
            this.MASTER = new List<EntityLayer.MASTER>();
        }
    }

    [XmlRoot(ElementName = "MASTER")]
    public class MASTER
    {
        [XmlElement(ElementName = "ORDER_TITLE")]
        public string ORDER_TITLE { get; set; }
        [XmlElement(ElementName = "ORDERNO")]
        public string ORDERNO { get; set; }
        [XmlElement(ElementName = "TYPE")]
        public string TYPE { get; set; }
        [XmlElement(ElementName = "DATE")]
        public string DATE { get; set; }
        [XmlElement(ElementName = "TIME")]
        public string TIME { get; set; }
        [XmlElement(ElementName = "DELIVERY_DATE")]
        public string DELIVERY_DATE { get; set; }
        [XmlElement(ElementName = "DELIVERY_TIME")]
        public string DELIVERY_TIME { get; set; }
        [XmlElement(ElementName = "LAST_DELIVERY_DATE")]
        public string LAST_DELIVERY_DATE { get; set; }
        [XmlElement(ElementName = "LAST_DELIVERY_TIME")]
        public string LAST_DELIVERY_TIME { get; set; }
        [XmlElement(ElementName = "EXPLANATION")]
        public string EXPLANATION { get; set; }
        [XmlElement(ElementName = "EXPLANATION2")]
        public string EXPLANATION2 { get; set; }
        [XmlElement(ElementName = "FIRMID")]
        public string FIRMID { get; set; }
        [XmlElement(ElementName = "SPECIAL_CODE")]
        public string SPECIAL_CODE { get; set; }
        [XmlElement(ElementName = "AUTHORIZATION_CODE")]
        public string AUTHORIZATION_CODE { get; set; }
        [XmlElement(ElementName = "ACTION_CODE")]
        public string ACTION_CODE { get; set; }
        [XmlElement(ElementName = "STATUS")]
        public string STATUS { get; set; }
        [XmlElement(ElementName = "FREE_CODE1")]
        public string FREE_CODE1 { get; set; }
        [XmlElement(ElementName = "FREE_CODE2")]
        public string FREE_CODE2 { get; set; }
        [XmlElement(ElementName = "FREE_CODE3")]
        public string FREE_CODE3 { get; set; }
        [XmlElement(ElementName = "FREE_CODE4")]
        public string FREE_CODE4 { get; set; }
        [XmlElement(ElementName = "FREE_CODE5")]
        public string FREE_CODE5 { get; set; }
        [XmlElement(ElementName = "RETAIL_CUSTOMER_NAME")]
        public string RETAIL_CUSTOMER_NAME { get; set; }
        [XmlElement(ElementName = "RETAIL_CUSTOMER_ADDRESS1")]
        public string RETAIL_CUSTOMER_ADDRESS1 { get; set; }
        [XmlElement(ElementName = "RETAIL_CUSTOMER_ADDRESS2")]
        public string RETAIL_CUSTOMER_ADDRESS2 { get; set; }
        [XmlElement(ElementName = "RETAIL_CUSTOMER_ADDRESS3")]
        public string RETAIL_CUSTOMER_ADDRESS3 { get; set; }
        [XmlElement(ElementName = "RETAIL_CUSTOMER_PHONE")]
        public string RETAIL_CUSTOMER_PHONE { get; set; }
        [XmlElement(ElementName = "RETAIL_CUSTOMER_TAXOFFICE")]
        public string RETAIL_CUSTOMER_TAXOFFICE { get; set; }
        [XmlElement(ElementName = "RETAIL_CUSTOMER_TAXNUMBER")]
        public string RETAIL_CUSTOMER_TAXNUMBER { get; set; }
        [XmlElement(ElementName = "PL3_INVNUM")]
        public string PL3_INVNUM { get; set; }
        [XmlElement(ElementName = "PL3_CUSTNUM")]
        public string PL3_CUSTNUM { get; set; }
        [XmlElement(ElementName = "PL3_CUSTNAME")]
        public string PL3_CUSTNAME { get; set; }
        [XmlElement(ElementName = "PL3_CUSTTAXID")]
        public string PL3_CUSTTAXID { get; set; }
        [XmlElement(ElementName = "PL3_CUSTTAXOFFICE")]
        public string PL3_CUSTTAXOFFICE { get; set; }
        [XmlElement(ElementName = "PL3_CUSTADR1")]
        public string PL3_CUSTADR1 { get; set; }
        [XmlElement(ElementName = "PL3_CUSTADR2")]
        public string PL3_CUSTADR2 { get; set; }
        [XmlElement(ElementName = "PL3_CUSTADR3")]
        public string PL3_CUSTADR3 { get; set; }
        [XmlElement(ElementName = "INVOICECURRENCY")]
        public string INVOICECURRENCY { get; set; }
        [XmlElement(ElementName = "INVOICECURRENCYRATE")]
        public string INVOICECURRENCYRATE { get; set; }
        [XmlElement(ElementName = "INVOICEDATE")]
        public string INVOICEDATE { get; set; }
        [XmlElement(ElementName = "INVOICECURRENCYDATE")]
        public string INVOICECURRENCYDATE { get; set; }
        [XmlElement(ElementName = "INVOICECURRENCYRATECOLUMN")]
        public string INVOICECURRENCYRATECOLUMN { get; set; }
        [XmlElement(ElementName = "TOTALAMOUNTBEFORDISCOUNTI")]
        public string TOTALAMOUNTBEFORDISCOUNTI { get; set; }
        [XmlElement(ElementName = "DISCOUNT1RATIO")]
        public string DISCOUNT1RATIO { get; set; }
        [XmlElement(ElementName = "DISCOUNT2RATIO")]
        public string DISCOUNT2RATIO { get; set; }
        [XmlElement(ElementName = "DISCOUNTINAMOUNTI")]
        public string DISCOUNTINAMOUNTI { get; set; }
        [XmlElement(ElementName = "TOTALAMOUNTAFTERDISCOUNTI")]
        public string TOTALAMOUNTAFTERDISCOUNTI { get; set; }
        [XmlElement(ElementName = "SPECIALCONSUMPTIONTAXRATIOI")]
        public string SPECIALCONSUMPTIONTAXRATIOI { get; set; }
        [XmlElement(ElementName = "VATBASEI")]
        public string VATBASEI { get; set; }
        [XmlElement(ElementName = "VAT1RATIO")]
        public string VAT1RATIO { get; set; }
        [XmlElement(ElementName = "VAT1AMOUNTI")]
        public string VAT1AMOUNTI { get; set; }
        [XmlElement(ElementName = "VAT2RATIO")]
        public string VAT2RATIO { get; set; }
        [XmlElement(ElementName = "VAT2AMOUNTI")]
        public string VAT2AMOUNTI { get; set; }
        [XmlElement(ElementName = "VAT3RATIO")]
        public string VAT3RATIO { get; set; }
        [XmlElement(ElementName = "VAT3AMOUNTI")]
        public string VAT3AMOUNTI { get; set; }
        [XmlElement(ElementName = "TOTALAMOUNTI")]
        public string TOTALAMOUNTI { get; set; }
        [XmlElement(ElementName = "DETAILS")]
        public DETAILS DETAILS { get; set; }


        public MASTER()
        {
            ORDER_TITLE = "ST";
            ORDERNO = string.Empty;
            TYPE = "03";
            LAST_DELIVERY_DATE = string.Empty;
            LAST_DELIVERY_TIME = string.Empty;
            EXPLANATION = string.Empty;
            EXPLANATION2 = string.Empty;
            SPECIAL_CODE = string.Empty;
            AUTHORIZATION_CODE = string.Empty;
            ACTION_CODE = string.Empty;
            STATUS = string.Empty;
            FREE_CODE1 = string.Empty;
            FREE_CODE2 = string.Empty;
            FREE_CODE3 = string.Empty;
            FREE_CODE4 = string.Empty;
            FREE_CODE5 = string.Empty;
            RETAIL_CUSTOMER_NAME = string.Empty;
            RETAIL_CUSTOMER_ADDRESS1 = string.Empty;
            RETAIL_CUSTOMER_ADDRESS2 = string.Empty;
            RETAIL_CUSTOMER_ADDRESS3 = string.Empty;
            RETAIL_CUSTOMER_PHONE = string.Empty;
            RETAIL_CUSTOMER_TAXOFFICE = string.Empty;
            RETAIL_CUSTOMER_TAXNUMBER = string.Empty;
            PL3_INVNUM = string.Empty;
            PL3_CUSTNUM = string.Empty;
            PL3_CUSTNAME = string.Empty;
            PL3_CUSTTAXID = string.Empty;
            PL3_CUSTTAXOFFICE = string.Empty;
            PL3_CUSTADR1 = string.Empty;
            PL3_CUSTADR2 = string.Empty;
            PL3_CUSTADR3 = string.Empty;
            INVOICECURRENCY = string.Empty;
            INVOICECURRENCYRATE = string.Empty;
            INVOICEDATE = string.Empty;
            INVOICECURRENCYDATE = string.Empty;
            INVOICECURRENCYRATECOLUMN = string.Empty;
            TOTALAMOUNTBEFORDISCOUNTI = string.Empty;
            DISCOUNT1RATIO = string.Empty;
            DISCOUNT2RATIO = string.Empty;
            DISCOUNTINAMOUNTI = string.Empty;
            TOTALAMOUNTAFTERDISCOUNTI = string.Empty;
            SPECIALCONSUMPTIONTAXRATIOI = string.Empty;
            VATBASEI = string.Empty;
            VAT1RATIO = string.Empty;
            VAT1AMOUNTI = string.Empty;
            VAT2RATIO = string.Empty;
            VAT2AMOUNTI = string.Empty;
            VAT3RATIO = string.Empty;
            VAT3AMOUNTI = string.Empty;
            TOTALAMOUNTI = string.Empty;

            this.DETAILS = new DETAILS();
        }
    }


    [XmlRoot(ElementName = "DETAILS")]
    public class DETAILS
    {
        [XmlElement(ElementName = "DETAIL")]
        public List<DETAIL> DETAIL { get; set; }

        public DETAILS()
        {
            this.DETAIL = new List<EntityLayer.DETAIL>();
        }
    }

    [XmlRoot(ElementName = "DETAIL")]
    public class DETAIL
    {
        [XmlElement(ElementName = "ORDER_TITLE")]
        public string ORDER_TITLE { get; set; }
        [XmlElement(ElementName = "ORDER_DATE")]
        public string ORDER_DATE { get; set; }
        [XmlElement(ElementName = "ORDER_NO")]
        public string ORDER_NO { get; set; }
        [XmlElement(ElementName = "LINE_NO")]
        public string LINE_NO { get; set; }
        [XmlElement(ElementName = "MATERIAL_CODE")]
        public string MATERIAL_CODE { get; set; }
        [XmlElement(ElementName = "ORDER_QUANTITY")]
        public string ORDER_QUANTITY { get; set; }
        [XmlElement(ElementName = "UNIT")]
        public string UNIT { get; set; }
        [XmlElement(ElementName = "WAREHOUSE")]
        public string WAREHOUSE { get; set; }
        [XmlElement(ElementName = "UNITPRICE")]
        public string UNITPRICE { get; set; }
        [XmlElement(ElementName = "UNITPRICECURRENCY")]
        public string UNITPRICECURRENCY { get; set; }
        [XmlElement(ElementName = "LINECURRENCYDATE")]
        public string LINECURRENCYDATE { get; set; }
        [XmlElement(ElementName = "LINECURRENCYRATECOLUMN")]
        public string LINECURRENCYRATECOLUMN { get; set; }
        [XmlElement(ElementName = "LINECURRENCYRATE")]
        public string LINECURRENCYRATE { get; set; }
        [XmlElement(ElementName = "LINEDISCOUNTRATIO")]
        public string LINEDISCOUNTRATIO { get; set; }
        [XmlElement(ElementName = "SPECIALCONSUMPTIONTAXRATIO")]
        public string SPECIALCONSUMPTIONTAXRATIO { get; set; }
        [XmlElement(ElementName = "VATRATIO")]
        public string VATRATIO { get; set; }
        [XmlElement(ElementName = "VATINCLUDE")]
        public string VATINCLUDE { get; set; }
        [XmlElement(ElementName = "LINETOTALAMOUNT")]
        public string LINETOTALAMOUNT { get; set; }
        [XmlElement(ElementName = "LINETOTALSPECCONSUMPTIONTAXRATIO")]
        public string LINETOTALSPECCONSUMPTIONTAXRATIO { get; set; }
        [XmlElement(ElementName = "LINEVATBASE")]
        public string LINEVATBASE { get; set; }
        [XmlElement(ElementName = "LINEVATAMOUT")]
        public string LINEVATAMOUT { get; set; }
        [XmlElement(ElementName = "LINETOTALAMOUNTINLINECURRENCY")]
        public string LINETOTALAMOUNTINLINECURRENCY { get; set; }
        [XmlElement(ElementName = "LINETOTALAMOUNTININVOICECURRENCY")]
        public string LINETOTALAMOUNTININVOICECURRENCY { get; set; }
        [XmlElement(ElementName = "LINETOTALSPCLCONSTAXRATIOINVOICE")]
        public string LINETOTALSPCLCONSTAXRATIOINVOICE { get; set; }
        [XmlElement(ElementName = "LINEVATBASEININVOICECURRENCY")]
        public string LINEVATBASEININVOICECURRENCY { get; set; }
        [XmlElement(ElementName = "LINEVATAMOUTININVOICECURRENCY")]
        public string LINEVATAMOUTININVOICECURRENCY { get; set; }
        [XmlElement(ElementName = "LINEVATINCLUDETOTALAMOUNTINVOICE")]
        public string LINEVATINCLUDETOTALAMOUNTINVOICE { get; set; }


        public DETAIL()
        {
            ORDER_TITLE = "SD";
            UNIT = "AD";
            WAREHOUSE = "K1";

            UNITPRICE = string.Empty;
            UNITPRICECURRENCY = string.Empty;
            LINECURRENCYDATE = string.Empty;
            LINECURRENCYRATECOLUMN = string.Empty;
            LINECURRENCYRATE = string.Empty;
            LINEDISCOUNTRATIO = string.Empty;
            SPECIALCONSUMPTIONTAXRATIO = string.Empty;
            VATRATIO = string.Empty;
            VATINCLUDE = string.Empty;
            LINETOTALAMOUNT = string.Empty;
            LINETOTALSPECCONSUMPTIONTAXRATIO = string.Empty;
            LINEVATBASE = string.Empty;
            LINEVATAMOUT = string.Empty;
            LINETOTALAMOUNTINLINECURRENCY = string.Empty;
            LINETOTALAMOUNTININVOICECURRENCY = string.Empty;
            LINETOTALSPCLCONSTAXRATIOINVOICE = string.Empty;
            LINEVATBASEININVOICECURRENCY = string.Empty;
            LINEVATAMOUTININVOICECURRENCY = string.Empty;
            LINEVATINCLUDETOTALAMOUNTINVOICE = string.Empty;
            
        }
    }
    
}