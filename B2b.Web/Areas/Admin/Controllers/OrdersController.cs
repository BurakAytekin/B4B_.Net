using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;
using SelectPdf;
using System.Linq;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Text;
using System.Net.Mail;
using B2b.Web.v4.Models.ErpLayer;
using System.Data;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class OrdersController : AdminBaseController
    {
        protected OrderHeader Order
        {
            get { return (OrderHeader)Session["OrderHeader"]; }
            set { Session["OrderHeader"] = value; }
        }

        // GET: Orders
        public ActionResult Index()
        {
            ViewBag.AuthoritySendPool = AdminCurrentSalesman.AuthoritySalesman.SendPool;
            ViewBag.AuthorityAdmin = AdminCurrentSalesman.AuthoritySalesman.CustomerType;
            return View();
        }

        public ActionResult OrderDetail(int id = -1)
        {
            if (id < 1)
                return View();
            Order = OrderHeader.GetOrderById(id);
            var customer = Customer.GetCustomerByCode(Order.Customer.Code);
            Order.Customer = customer;
            GetActualCurrencies();

            // test amaçlı GenerateMarsEntegrationFile(Order);

            return View();
        }


        #region   HttpPost Methods

        [HttpPost]
        public JsonResult SavePdf(OrderHeader header, List<OrderDetail> detail)//
        {
            string contentHtml = "";
            contentHtml = System.IO.File.ReadAllText(Server.MapPath("/files/mailtemplate/order.html"));

            contentHtml = contentHtml.Replace("{CustomerCode}", header.Customer.Code).Replace("{CustomerName}", header.Customer.Name).Replace("{CustomerCity}", header.Customer.City).Replace("{CustomerTown}", header.Customer.Town).Replace("{CustomerAddress}", header.Customer.Address).Replace("{OrderNo}", header.Id.ToString()).Replace("{OrderDate}", header.CreateDate.ToString()).Replace("{Salesman}", header.Customer.Salesman.Name).Replace("{CustomerTel}", header.Customer.Tel1).Replace("{CustomerFax}", header.Customer.Fax1).Replace("{SenderName}", header.SenderName).Replace("{SenderType}", header.SenderType).Replace("{Date}", DateTime.Now.ToString()).Replace("{Total}", header.TotalStr).Replace("{NetTotal}", header.NetTotalStr).Replace("{VatTotal}", header.VatStr).Replace("{GeneralTotal}", header.GeneralTotalStr).Replace("{OrderNote}", header.Notes + " " + header.SalesmanNotes);

            string repeatItem = contentHtml.Split('|')[1];

            string repeatItemDetail = string.Empty;

            foreach (OrderDetail item in detail)
            {
                repeatItemDetail += repeatItem.Replace("{No}", item.Id.ToString()).Replace("{ProductCode}", item.ProductCode).Replace("{ProductName}", item.ProductName).Replace("{Manufacturer}", item.Manufacturer).Replace("{ShelfAddress}", item.ShelfAddress).Replace("{OrderQuantity}", item.Quantity.ToString()).Replace("{Quantity}", item.TotalQuantity.ToString()).Replace("{Discount}", item.DiscountStr).Replace("{NetPrice}", item.NetPriceStr).Replace("{ItemTotal}", item.NetAmountWithVatStr);
            }

            contentHtml = contentHtml.Replace(repeatItem, repeatItemDetail).Replace("|", "");

            HtmlToPdf converter = new HtmlToPdf();



            converter.Options.MarginLeft = 40;
            converter.Options.MarginRight = 40;
            converter.Options.MarginTop = 10;
            converter.Options.MarginBottom = 10;
            // create a new pdf document converting an url
            PdfDocument doc = converter.ConvertHtmlString(contentHtml, string.Empty);




            string path = "Files/Order/" + Guid.NewGuid() + ".pdf";

            if (System.IO.File.Exists(Server.MapPath(path)))
            {
                System.IO.File.Delete(Server.MapPath(path));
            }

            doc.Save(Server.MapPath("~/" + path));
            // save pdf document
            //byte[] pdf = doc.Save();

            // close pdf document
            doc.Close();


            // return resulted pdf document
            //FileResult fileResult = new FileContentResult(pdf, "application/pdf");
            //fileResult.FileDownloadName = "Document.pdf";


            return Json(GlobalSettings.B2bAddress + path);
            //return Json("http://localhost:35067/" + path);

        }


        [HttpPost]
        public string GetListOrderHeader(OrderSearchCriteria orderSearchCriteria)
        {
            orderSearchCriteria.EndDate = orderSearchCriteria.EndDate.Date.Add(new TimeSpan(23, 59, 59));
            return JsonConvert.SerializeObject(OrderHeader.GetOrderList(orderSearchCriteria.StartDate, orderSearchCriteria.EndDate, orderSearchCriteria.T9Text, orderSearchCriteria.OrderStatu));
        }

        [HttpPost]
        public string ChangeOrderStatus(OrderHeader order, int status)
        {
            order.Status = (OrderStatus)status;

            order.EditId = AdminCurrentSalesman.Id;
            order.SalesmanNotes = !string.IsNullOrEmpty(order.SalesmanNotes) ? order.SalesmanNotes : "";
            bool result = order.UpdateOrderHeaderStatu();
            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir.") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");

            if (status == 87 || status == 0)
            {
                string mailAddress = status == 87 ? "muhasebe@hantech.com.tr" : "siparis@hantech-turkey.com.tr";
                MailMessage mail = new MailMessage();
                mail.Subject = $"Onayınızı Bekleyen Siparişiniz Var, {order.Id}";
                mail.Body = $"{order.Customer.Code} - {order.Customer.Name} Kodlu Müşterinin {order.Id} Numaralı Siparişi Onayınızı Beklemektedir. <br><br> https://b4b.hantech-turkey.com.tr/Admin/Orders/OrderDetail/{order.Id}";
                mail.IsBodyHtml = true;
                mail.To.Add(mailAddress);
                //mail.To.Add("emre.kesler@eryaz.net");
                EmailHelper.Send(mail);
            }

            //if (status == (int)OrderStatus.AutomaticTransfer)
            //{
            //    // Mars Lojistik için sevk emirinin oluşturulması 
            //    GenerateMarsEntegrationFile(order);
            //}


            return JsonConvert.SerializeObject(message);
        }

        //[HttpPost]
        //public string MakeMarsEntegrationFile(OrderHeader order)
        //{
        //    return GenerateMarsEntegrationFile(order);
        //}

        //public string GenerateMarsEntegrationFile(OrderHeader order)
        //{
        //    // nesne oluşturma
        //    List<OrderDetail> odList = B2b.Web.v4.Models.EntityLayer.OrderDetail.GetOrderDetail(order.Id);
        //    if (odList.Any(p => p.ShippedQty > 0))
        //    {

        //        string entegreNo = string.Format("HNT-{0}", order.Id.ToString("000000"));

        //        ORDERPACKET marsPacket = new ORDERPACKET();
        //        MASTER marsMaster = new MASTER();
        //        marsMaster.ORDERNO = entegreNo;
        //        marsMaster.DATE = order.CreateDate.ToString("dd/MM/yy").Replace(".", "/"); //DateTime.Now.ToString("dd/MM/YY").Replace(".", "/");
        //        marsMaster.TIME = order.CreateDate.ToString("HH:mm"); //DateTime.Now.ToString("HH:mm");
        //        marsMaster.DELIVERY_DATE = marsMaster.DATE;
        //        marsMaster.DELIVERY_TIME = marsMaster.TIME;
        //        marsMaster.EXPLANATION = string.Empty; // order.Notes.Length > 50 ? order.Notes.Substring(0, 50) : order.Notes;
        //        marsMaster.FIRMID = order.Customer.Code == "TEST" ? "HNT-120.01.00.001" : string.Format("HNT-{0}", order.Customer.Code);

        //        List<DETAIL> marsDetailList = new List<DETAIL>();
        //        int i = 1;
        //        foreach (var od in odList.Where(p => p.ShippedQty > 0))
        //        {
        //            DETAIL marsDetail = new DETAIL();
        //            marsDetail.ORDER_DATE = marsMaster.DATE;
        //            marsDetail.ORDER_NO = entegreNo;
        //            marsDetail.LINE_NO = i.ToString();
        //            marsDetail.MATERIAL_CODE = od.ProductCode;
        //            marsDetail.ORDER_QUANTITY = od.ShippedQty.ToString();

        //            marsDetailList.Add(marsDetail);

        //            i++;
        //        }
        //        marsMaster.DETAILS.DETAIL = marsDetailList;

        //        marsPacket.MASTERS.MASTER.Add(marsMaster);

        //        // nesnenin ftp ye yazılması
        //        {

        //            string xmlstr = ExtensionMethods.Serialize<ORDERPACKET>(marsPacket);

        //            string fileName = string.Format("SIPSendOrder({0}).XML", entegreNo);
        //            string fullFtpFilePathTmp = GlobalSettings.FtpServerUploadAddress + "MarsLogistics/IN/Archive/" + fileName;
        //            string fullFtpFilePath = GlobalSettings.FtpServerUploadAddress + "MarsLogistics/IN/" + fileName;

        //            UploadRemoteServer(Encoding.UTF8.GetBytes(xmlstr), fullFtpFilePathTmp);
        //            UploadRemoteServer(Encoding.UTF8.GetBytes(xmlstr), fullFtpFilePath);

        //        }
        //    }

        //    return string.Empty;
        //}

        /// <summary>
        ///     A class only to override encoding with UTF8.
        /// </summary>
        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        [HttpPost]
        public JsonResult GetListOrderPayment(int orderId)
        {
            return Json(OrderHeaderPayment.GetListByOrderId(orderId));
        }
        [HttpPost]
        public string GetListOrderDetail(int id)
        {
            List<OrderDetail> odList = B2b.Web.v4.Models.EntityLayer.OrderDetail.GetOrderDetail(id);

            OrderHeader orderHeader = OrderHeader.GetOrderById(id);
            if (orderHeader.Status == OrderStatus.OnHold && !odList.Any(p => p.ShippedQty != 0))
            {
                // sevk miktarları otomatik hesaplama işlemi

                foreach (var item in odList)
                {
                    item.ShippedQty = 0;
                    if (item.TotalQuantity > 0)
                    {
                        if (item.TotalQuantity > item.Quantity)
                        {
                            item.ShippedQty = item.Quantity;
                        }
                        else
                        {
                            item.ShippedQty = item.TotalQuantity;
                        }
                    }
                }

            }

            return JsonConvert.SerializeObject(odList);
        }

        [HttpPost]
        public string GetOrderHeader(int id)
        {
            return JsonConvert.SerializeObject(B2b.Web.v4.Models.EntityLayer.OrderHeader.GetOrderById(id));
        }
        [HttpPost]
        public string DeleteOrReturnOrder(OrderHeader order, int status)
        {

            OrderHeader header = new OrderHeader();
            header.Id = order.Id;
            header.Status = (OrderStatus)status;
            header.EditId = AdminCurrentSalesman.Id;
            bool result = header.Delete();

            if (status == 98)
            {
                try
                {
                    Notifications.AddNotification("Sİpariş", order.Id + " nolu siparişiniz onaylanmıştır", order.Customer.Id, -1, false, false, -1);
                }
                catch (Exception)
                {

                    throw;
                }

            }

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return JsonConvert.SerializeObject(message);
        }
        [HttpPost]
        public string UpdateOrderStatu(OrderHeader order, int statu, string note)
        {

            OrderHeader header = new OrderHeader();
            header.Id = order.Id;
            header.Status = (OrderStatus)statu;
            header.EditId = AdminCurrentSalesman.Id;
            if (!string.IsNullOrEmpty(note))
            { header.Notes = note; }
            else { header.Notes = order.Notes; }




            bool result = header.UpdateOrderHeaderStatu();

            if (statu == 98)
                Notifications.AddNotification("Sİpariş", order.Id + " nolu siparişiniz onaylanmıştır", order.Customer.Id, -1, false, false, -1);




            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return JsonConvert.SerializeObject(message);
        }


        [HttpPost]
        public string ConcatOrders()
        {
            bool result = false;
            List<OrderHeaderSmall> headerList = OrderHeader.GetOrderList(DateTime.Now, DateTime.Now, string.Empty, (int)OrderStatus.OnHoldInPool);
            var results = headerList.GroupBy(p => p.Customer.Id, (key, g) => new { CustomerId = key });
            for (int i = 0; i < results.ToList().Count; i++)
            {
                //results.ToList()[0].CustomerId
                if (headerList.Where(x => x.Customer.Id == results.ToList()[i].CustomerId).Count() > 1)
                {
                    List<OrderHeaderSmall> orderList = headerList.Where(x => x.Customer.Id == results.ToList()[i].CustomerId).ToList();
                    OrderHeader firstOrder = orderList.First().ConvertToOrder();


                    for (int j = 1; j < orderList.Count(); j++)
                    {
                        OrderHeader order = orderList[j].ConvertToOrder();
                        firstOrder.GeneralTotal += order.GeneralTotal;
                        firstOrder.Discount += order.Discount;
                        firstOrder.NetTotal += order.NetTotal;
                        firstOrder.Vat += order.Vat;
                        firstOrder.Total += order.Total;
                        firstOrder.GeneralTotalLocal += order.GeneralTotalLocal;
                        firstOrder.DiscountLocal += order.DiscountLocal;
                        firstOrder.NetTotalLocal += order.NetTotalLocal;
                        firstOrder.VatLocal += order.VatLocal;
                        firstOrder.TotalLocal += order.TotalLocal;
                        firstOrder.Notes += "==>" + order.Notes;
                        bool res = B2b.Web.v4.Models.EntityLayer.OrderDetail.ChangeOrderId(order.Id, firstOrder.Id);
                        order.Status = OrderStatus.CombinedInPool;
                        order.UpdateOrderHeaderStatu();
                    }
                    result = firstOrder.Update();
                }

            }

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return JsonConvert.SerializeObject(message);
        }

        [HttpPost]
        public string GetListOrderDetailHeader(int id)
        {
            Order = OrderHeader.GetOrderById(id);
            var customer = Customer.GetCustomerByCode(Order.Customer.Code);
            Order.Customer = customer;
            return JsonConvert.SerializeObject(Order);
        }
        [HttpPost]
        public string UpdateOrderDetail(OrderDetail orderRow, OrderDetail orderOrijinalRow)
        {
            OrderHeader header = new OrderHeader();
            header = Order;
            B2b.Web.v4.Models.EntityLayer.OrderDetail.Calculate(header, orderRow);

            MessageBox messageBox = new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı");
            return JsonConvert.SerializeObject(messageBox);

        }

        [HttpPost]
        public string DeleteOrderDetail(int detailId, OrderDetail orderRow)
        {

            OrderDetail od = new OrderDetail();
            od.Id = detailId;
            od.Delete();

            OrderHeader header = new OrderHeader();
            header = Order;

            B2b.Web.v4.Models.EntityLayer.OrderDetail.Calculate(header, orderRow, true);

            MessageBox messageBox = new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı");
            return JsonConvert.SerializeObject(messageBox);

        }

        [HttpPost]
        public string GetCustomerBalanceInfo(string customerCode)
        {
            ErpFunctionDetail yearItem = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.Finance).Where(x => x.Settings.IsActiveCompany).FirstOrDefault();

            GeneralParameters parametres = new GeneralParameters();
            parametres.CommandType = yearItem.FunctionType == 0 ? CommandType.StoredProcedure : CommandType.Text;
            parametres.ParameterNames = (new string[1] { "pCustomerCode" });
            parametres.ParameterValues = (new string[1] { customerCode });
            parametres.CommandText = yearItem.FunctionName;

            DataTable dt = ErpHelper.FireServiceMethod(parametres, yearItem.Settings).ConvertResponseDataTable();
            List<CustomerInvoice> list = new List<CustomerInvoice>();
            list = dt.DataTableToList<CustomerInvoice>();

            if (list.Count > 0)
            {
                double balance = 0;

                foreach (CustomerInvoice item in list)
                {
                    balance += item.Debt;
                    balance -= item.Credit;
                    item.Balance = balance;
                    item.DebtStr = item.Debt.ToString("N2");
                    item.CreditStr = item.Credit.ToString("N2");
                    item.BalanceStr = item.Balance.ToString("N2");
                }


                string debt = list.Sum(x => x.Debt).ToString("N2");
                string credit = list.Sum(x => x.Credit).ToString("N2");
                string balanceStr = list.Last().Balance.ToString("N2");

                List<CustomerInvoice> listCheckAndContract = list.Where(x => x.DueDate > DateTime.Now && (x.TransactionType == "M. Çek Girişi" || x.TransactionType == "M. Senet Girişi")).ToList();


                return JsonConvert.SerializeObject(new { Debt = debt, Credit = credit, Balance = balanceStr, CheckAndContracts = listCheckAndContract });

            }
            else
                return JsonConvert.SerializeObject(new { Debt = "", Credit = "", Balance = "", CheckAndContracts = new List<CustomerInvoice>() });


        }
        #endregion

        public class OrderSearchCriteria
        {
            public string T9Text { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int OrderStatu { get; set; }
        }
    }
}