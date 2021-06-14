using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace B2b.Web.v4.Models.EntityLayer
{
    [XmlRoot(ElementName = "REQUEST")]
    public class REQUEST
    {
        [XmlElement(ElementName = "START_DATE_TIME")]
        public string START_DATE_TIME { get; set; }
        [XmlElement(ElementName = "WAREHOUSE")]
        public string WAREHOUSE { get; set; }
        [XmlElement(ElementName = "MAXRECORDS")]
        public string MAXRECORDS { get; set; }
    }

    [XmlRoot(ElementName = "SERIAL")]
    public class SERIAL
    {
        [XmlElement(ElementName = "SERIALNO")]
        public List<string> SERIALNO { get; set; }
    }

    [XmlRoot(ElementName = "DETAIL")]
    public class WAYBILLDETAIL
    {
        [XmlElement(ElementName = "TRANSACTION_TITLE")]
        public string TRANSACTION_TITLE { get; set; }
        [XmlElement(ElementName = "TRANSACTION_DATE")]
        public string TRANSACTION_DATE { get; set; }
        [XmlElement(ElementName = "TRANSACTION_NO")]
        public string TRANSACTION_NO { get; set; }
        [XmlElement(ElementName = "TRANSACTION_LINE_NO")]
        public string TRANSACTION_LINE_NO { get; set; }
        [XmlElement(ElementName = "MATERIAL_CODE")]
        public string MATERIAL_CODE { get; set; }
        [XmlElement(ElementName = "MATERIAL_NAME")]
        public string MATERIAL_NAME { get; set; }
        [XmlElement(ElementName = "BARCODE")]
        public string BARCODE { get; set; }
        [XmlElement(ElementName = "QUANTITY")]
        public string QUANTITY { get; set; }
        [XmlElement(ElementName = "UNIT")]
        public string UNIT { get; set; }
        [XmlElement(ElementName = "ORDER_DATE")]
        public string ORDER_DATE { get; set; }
        [XmlElement(ElementName = "ORDER_LINE_NO")]
        public string ORDER_LINE_NO { get; set; }
        [XmlElement(ElementName = "LINE_NO")]
        public string LINE_NO { get; set; }
        [XmlElement(ElementName = "ORDER_NUMBER")]
        public string ORDER_NUMBER { get; set; }
        [XmlElement(ElementName = "PALLET_NUMBER")]
        public string PALLET_NUMBER { get; set; }
        [XmlElement(ElementName = "SPECIAL_CODE")]
        public string SPECIAL_CODE { get; set; }
        [XmlElement(ElementName = "BEST_BEFORE")]
        public string BEST_BEFORE { get; set; }
        [XmlElement(ElementName = "ORDER_EVRAKTIP")]
        public string ORDER_EVRAKTIP { get; set; }
        [XmlElement(ElementName = "PL3_DETAILORDERNUM")]
        public string PL3_DETAILORDERNUM { get; set; }
        [XmlElement(ElementName = "LOGISTICS_SERVICE_PROVIDER")]
        public string LOGISTICS_SERVICE_PROVIDER { get; set; }
        [XmlElement(ElementName = "SERIAL")]
        public SERIAL SERIAL { get; set; }
    }

    [XmlRoot(ElementName = "DETAILS")]
    public class WAYBILLDETAILS
    {
        [XmlElement(ElementName = "DETAIL")]
        public List<WAYBILLDETAIL> DETAIL { get; set; }
    }

    [XmlRoot(ElementName = "MASTER")]
    public class WAYBILLMASTER
    {
        [XmlElement(ElementName = "TRANSACTION_TITLE")]
        public string TRANSACTION_TITLE { get; set; }
        [XmlElement(ElementName = "TRANSACTION_DATE")]
        public string TRANSACTION_DATE { get; set; }
        [XmlElement(ElementName = "TRANSACTION_TIME")]
        public string TRANSACTION_TIME { get; set; }
        [XmlElement(ElementName = "TRANSACTION_NO")]
        public string TRANSACTION_NO { get; set; }
        [XmlElement(ElementName = "TRANSACTION_CODE")]
        public string TRANSACTION_CODE { get; set; }
        [XmlElement(ElementName = "WORKORDER_NO")]
        public string WORKORDER_NO { get; set; }
        [XmlElement(ElementName = "FIRMID")]
        public string FIRMID { get; set; }
        [XmlElement(ElementName = "WAREHOUSE")]
        public string WAREHOUSE { get; set; }
        [XmlElement(ElementName = "CONSIGMENT_NOTE_NUMBER")]
        public string CONSIGMENT_NOTE_NUMBER { get; set; }
        [XmlElement(ElementName = "CONSIGMENT_NOTE_DATE")]
        public string CONSIGMENT_NOTE_DATE { get; set; }
        [XmlElement(ElementName = "INVOICE_NUMBER")]
        public string INVOICE_NUMBER { get; set; }
        [XmlElement(ElementName = "INVOICE_DATE")]
        public string INVOICE_DATE { get; set; }
        [XmlElement(ElementName = "PLATE_NUMBER")]
        public string PLATE_NUMBER { get; set; }
        [XmlElement(ElementName = "TRANSACTION_REASON_CODE")]
        public string TRANSACTION_REASON_CODE { get; set; }
        [XmlElement(ElementName = "TRANSACTION_REASON_EXPLANATION")]
        public string TRANSACTION_REASON_EXPLANATION { get; set; }
        [XmlElement(ElementName = "INSERT_TIME")]
        public string INSERT_TIME { get; set; }
        [XmlElement(ElementName = "INSERT_USER")]
        public string INSERT_USER { get; set; }
        [XmlElement(ElementName = "CANCEL")]
        public string CANCEL { get; set; }
        [XmlElement(ElementName = "CUSTOMER_ORDER_NUMBER")]
        public string CUSTOMER_ORDER_NUMBER { get; set; }
        [XmlElement(ElementName = "SPECIAL_CODE")]
        public string SPECIAL_CODE { get; set; }
        [XmlElement(ElementName = "AUTHORIZATION_CODE")]
        public string AUTHORIZATION_CODE { get; set; }
        [XmlElement(ElementName = "DRIVERNAME")]
        public string DRIVERNAME { get; set; }
        [XmlElement(ElementName = "TRANSPORTATRANSCODE")]
        public string TRANSPORTATRANSCODE { get; set; }
        [XmlElement(ElementName = "DELIVERYDATE")]
        public string DELIVERYDATE { get; set; }
        [XmlElement(ElementName = "DELIVEREDBY")]
        public string DELIVEREDBY { get; set; }
        [XmlElement(ElementName = "DELIVEREDTO")]
        public string DELIVEREDTO { get; set; }
        [XmlElement(ElementName = "UUID")]
        public string UUID { get; set; }
        [XmlElement(ElementName = "DETAILS")]
        public WAYBILLDETAILS DETAILS { get; set; }
    }

    [XmlRoot(ElementName = "MASTERS")]
    public class WAYBILLMASTERS
    {
        [XmlElement(ElementName = "MASTER")]
        public WAYBILLMASTER MASTER { get; set; }
    }

    [XmlRoot(ElementName = "TRANSACTIONPACKET")]
    public class TRANSACTIONPACKET
    {
        [XmlElement(ElementName = "PROJECT_CODE")]
        public string PROJECT_CODE { get; set; }
        [XmlElement(ElementName = "REQUEST")]
        public REQUEST REQUEST { get; set; }
        [XmlElement(ElementName = "MASTERS")]
        public WAYBILLMASTERS MASTERS { get; set; }
    }

}