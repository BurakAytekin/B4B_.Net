using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.ErpLayer;
using B2b.Web.v4.Models.Helper;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace B2b.Web.v4.Areas.Admin.Models
{
    public class JobMarsEntegrationTo : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            // Natra verinin çekilmesi
            List<ErpOrderHeader> ohList = ErpOrderHeader.GetListForMars();


            foreach (ErpOrderHeader order in ohList)
            {
                // Mars xml dosyası oluşturma

                List<ErpOrderDetail> odList = ErpOrderDetail.GetListForMars(order.DocumentNo); // natradan sipariş detayı okunacak !!!!

                if (odList.Count > 0)
                {
                    string entegreNo = string.Format("HNT-{0}", order.DocumentNo);

                    ORDERPACKET marsPacket = new ORDERPACKET();
                    MASTER marsMaster = new MASTER();
                    marsMaster.ORDERNO = entegreNo;
                    marsMaster.DATE = order.DocumentDate.ToString("dd/MM/yy").Replace(".", "/");
                    marsMaster.TIME = order.DocumentDate.ToString("HH:mm");
                    marsMaster.DELIVERY_DATE = marsMaster.DATE;
                    marsMaster.DELIVERY_TIME = marsMaster.TIME;
                    marsMaster.EXPLANATION = string.Empty; // order.Notes.Length > 50 ? order.Notes.Substring(0, 50) : order.Notes; !!!!
                    marsMaster.FIRMID = order.CustomerCode == "TEST" ? "HNT-120.01.00.001" : string.Format("HNT{0}", order.CustomerCode);

                    if (!string.IsNullOrEmpty(order.TeslimatAdresi))
                    {
                        if (order.TeslimatAdresi.Contains("#"))
                        {
                            List<string> add = order.TeslimatAdresi.Split('#').ToList();
                            int partedCount = add.Count;
                            if (partedCount < 3)
                            {
                                add.Add("");
                                partedCount = add.Count;
                                if (partedCount < 3)
                                {
                                    add.Add("");
                                    partedCount = add.Count;
                                    if (partedCount < 3)
                                    {
                                        add.Add("");
                                        partedCount = add.Count;
                                    }
                                }
                            }

                            marsMaster.RETAIL_CUSTOMER_NAME = add[0].Length > 30 ? add[0].Substring(0, 30) : add[0];
                            marsMaster.RETAIL_CUSTOMER_ADDRESS1 = add[1].Length > 0 ? (add[1].Length > 30 ? add[1].Substring(0, 30) : add[1].Substring(0)) : string.Empty;
                            marsMaster.RETAIL_CUSTOMER_ADDRESS2 = add[1].Length > 30 ? (add[1].Length > 60 ? add[1].Substring(30, 30) : add[1].Substring(30)) : string.Empty;
                            marsMaster.RETAIL_CUSTOMER_ADDRESS3 = add[1].Length > 60 ? (add[1].Length > 90 ? add[1].Substring(60, 30) : add[1].Substring(60)) : string.Empty;
                            marsMaster.RETAIL_CUSTOMER_PHONE = add[2].Length > 30 ? add[2].Substring(0, 30) : add[2];

                        }
                        else
                        {
                            marsMaster.RETAIL_CUSTOMER_NAME = order.TeslimatAdresi.Length > 0 ? (order.TeslimatAdresi.Length > 30 ? order.TeslimatAdresi.Substring(0, 30) : order.TeslimatAdresi.Substring(0)) : string.Empty;
                            marsMaster.RETAIL_CUSTOMER_ADDRESS1 = order.TeslimatAdresi.Length > 30 ? (order.TeslimatAdresi.Length > 60 ? order.TeslimatAdresi.Substring(30, 30) : order.TeslimatAdresi.Substring(30)) : string.Empty;
                            marsMaster.RETAIL_CUSTOMER_ADDRESS2 = order.TeslimatAdresi.Length > 60 ? (order.TeslimatAdresi.Length > 90 ? order.TeslimatAdresi.Substring(60, 30) : order.TeslimatAdresi.Substring(60)) : string.Empty;
                            marsMaster.RETAIL_CUSTOMER_ADDRESS3 = order.TeslimatAdresi.Length > 90 ? (order.TeslimatAdresi.Length > 120 ? order.TeslimatAdresi.Substring(90, 30) : order.TeslimatAdresi.Substring(90)) : string.Empty;
                            marsMaster.RETAIL_CUSTOMER_PHONE = order.TeslimatAdresi.Length > 120 ? (order.TeslimatAdresi.Length > 150 ? order.TeslimatAdresi.Substring(120, 30) : order.TeslimatAdresi.Substring(120)) : string.Empty;
                        }
                    }

                    List<DETAIL> marsDetailList = new List<DETAIL>();
                    int i = 1;
                    foreach (var od in odList)
                    {
                        DETAIL marsDetail = new DETAIL();
                        marsDetail.ORDER_DATE = marsMaster.DATE;
                        marsDetail.ORDER_NO = entegreNo;
                        marsDetail.LINE_NO = i.ToString();
                        marsDetail.MATERIAL_CODE = od.ProductCode;
                        marsDetail.ORDER_QUANTITY = od.Quantity.ToString();

                        marsDetail.WAREHOUSE = order.Depokodu == "30" ? "K2" : "K1";

                        marsDetailList.Add(marsDetail);

                        i++;
                    }
                    marsMaster.DETAILS.DETAIL = marsDetailList;

                    marsPacket.MASTERS.MASTER.Add(marsMaster);

                    // nesnenin ftp ye yazılması
                    {

                        string xmlstr = ExtensionMethods.Serialize<ORDERPACKET>(marsPacket);

                        string fileName = string.Format("SIPSendOrder({0}).XML", entegreNo);
                        string fullFtpFilePathTmp = GlobalSettings.FtpServerUploadAddress + "MarsLogistics/IN/Archive/" + fileName;
                        string fullFtpFilePath = GlobalSettings.FtpServerUploadAddress + "MarsLogistics/IN/" + fileName;

                        if (GlobalSettings.MarsEntegration)
                        {
                            FtpHelper.UploadRemoteServer(Encoding.UTF8.GetBytes(xmlstr), fullFtpFilePathTmp);
                            FtpHelper.UploadRemoteServer(Encoding.UTF8.GetBytes(xmlstr), fullFtpFilePath);
                        }
                    }



                    if (GlobalSettings.MarsEntegration)
                    {
                        // Natra sipariş güncellemesi 
                        order.UpdateAsGenerated();

                        // loglamalar yapılacak
                        Logger.LogGeneral(v4.Models.Log.LogGeneralErrorType.Information, v4.Models.Log.ClientType.B2BWeb, "timer_mars_to", entegreNo, string.Empty);
                    }
                }
            }

            return null;
        }
    }
}