
using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.ErpLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using B2b.Web.v4.Models.SyncLayer;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace B2b.Web.v4.Controllers
{

    [RoutePrefix("api/EryazApi")]
    public class ValuesController : ApiController
    {
        [HttpPost, Route("ExecuteReader")]
        public string ExecuteReader([FromBody] GeneralParameters parameters)
        {
            if (Authenticate(parameters))
            {
                var data = DatabaseContext.ExecuteReader(parameters.CommandType, parameters.CommandText);
                var jsonData = JsonConvert.SerializeObject(data);

                return jsonData;
            }
            return JsonConvert.SerializeObject(new DataTable());
        }

        [HttpPost, Route("ExecuteReaderParams")]
        public string ExecuteReaderParams([FromBody] GeneralParameters parameters)
        {
            if (Authenticate(parameters))
            {
                var parameterNames = parameters.ParameterNames.Aggregate("",
                    (current, a) => current + (a.ToString() + ","));
                var parameterValues = parameters.ParameterValues.Aggregate("", (current, a) => current + (a.ToString() + ","));

                var data = DatabaseContext.ExecuteReader(parameters.CommandType,
                    parameters.CommandText, parameters.ParameterNames, parameters.ParameterValues);
                var jsonData = JsonConvert.SerializeObject(data);
                parameters.ParameterNames = new object[] { parameterNames };
                parameters.ParameterValues = new object[] { parameterValues };
                return jsonData;

            }
            return JsonConvert.SerializeObject(new DataTable());
        }
        [HttpPost, Route("ExecuteNonQuery")]
        public string ExecuteNonQuery([FromBody] GeneralParameters parameters)
        {
            if (Authenticate(parameters))
            {
                var parameterNames = parameters.ParameterNames.Aggregate("",
                    (current, a) => current + (a.ToString() + ","));
                var parameterValues = parameters.ParameterValues.Aggregate("", (current, a) => current + (a.ToString() + ","));

                return JsonConvert.SerializeObject(DatabaseContext.ExecuteNonQuery(parameters.CommandType,
                    parameters.CommandText, parameters.ParameterNames
                     , parameters.ParameterValues));
            }
            return JsonConvert.SerializeObject(false);
        }

        [HttpPost]
        [Route("GetSettings")]
        public List<SyncSettings> GetSettings()
        {
            List<SyncSettings> settingslist = SyncSettings.GetActiveSyncTransferType();
            return settingslist;
            // return JsonConvert.SerializeObject(settingslist);
        }

        [HttpPost]
        [Route("SaveOrder")]
        public string SaveOrder([FromBody] object responseStr)
        {
            List<RemoteOrder> requestItem = JsonConvert.DeserializeObject<List<RemoteOrder>>(responseStr.ToString());
            if (requestItem == null || requestItem.Count == 0)
                return "Veriler çözümlenemedi.Json formatının doğruluğundan emin olunuz";

            Customer customer = Customer.GetCustomerByCode(requestItem.First().CustomerCode);
            if (customer == null || customer.Id <= 0)
                return "Cari bulunamadı. Cari kodunun doğru olduğundan emin olunuz!";
            Salesman salesman = Salesman.GetByCode(requestItem.First().SalesmanCode);

            if (salesman == null || salesman.Id <= 0)
                return "Temsilci bulunamadı. Temsilci kodunun doğru olduğundan emin olunuz!";


            List<Currency> CurrencyList = Currency.GetList();
            customer = Customer.GetById(customer.Id, customer.Users.Id);


            string returnStr = string.Empty;
            foreach (RemoteOrder item in requestItem)
            {
                Product product = Product.GetByCode(item.ProductCode, LoginType.Salesman, customer);
                if (product == null || product.Id == 0)
                    returnStr += item.ProductCode + " kodlu ürün bulunamadı.";
                else
                {
                    if (item.IsUseSpecialPrice)
                    {
                        product.Price = item.Price;
                        product.PriceCurrency = item.Currency;
                        product.PriceCurrencyRate = CurrencyList.Where(x => x.Type == item.Currency).Count() > 0 ? CurrencyList.Where(x => x.Type == item.Currency).First().Rate : 0;
                        product.CalculateDetailInformation(true, customer.CampaignStatu, item.Quantity);
                    }

                    item.Product = product;

                    // item.Product.CalculateDetailInformation(true, customer.CampaignStatu, item.Quantity);
                }

            }
            double TotalPriceCustomerCurrency = 0;
            double TotalDiscountCustomerCurrency = 0;
            double TotalCostCustomerCurrency = 0;
            double TotalVATCustomerCurrency = 0;


            foreach (RemoteOrder item in requestItem)
            {
                TotalPriceCustomerCurrency += (item.Product.PriceValue * item.Quantity) * item.Product.PriceCurrencyRate;
                TotalDiscountCustomerCurrency += ((item.Product.PriceValue * item.Quantity) - (item.Product.Cost * item.Quantity)) * item.Product.PriceCurrencyRate;
                TotalCostCustomerCurrency += (item.Product.Cost * item.Quantity) * item.Product.PriceCurrencyRate;
                TotalVATCustomerCurrency += ((item.Product.Cost * item.Quantity) * (item.Product.VatRate / 100)) * item.Product.PriceCurrencyRate;
            }



            OrderHeader header = new OrderHeader()
            {
                CustomerId = customer.Id,
                UserId = customer.Users.Id,
                SalesmanId = salesman.Id,
                GeneralTotal = TotalPriceCustomerCurrency,
                Discount = TotalDiscountCustomerCurrency,
                NetTotal = TotalCostCustomerCurrency,
                Vat = TotalVATCustomerCurrency,
                Total = (TotalCostCustomerCurrency + TotalVATCustomerCurrency),
                Notes = "",
                SalesmanNotes = "Api dinamik sipairş",
                SendingTypeId = 1,
                NumberOfProduct = requestItem.Count,
                PaymentId = string.Empty,
                Status = OrderStatus.UnKnown,
                Currency = customer.CurrencyType,
                TotalAvailable = 0,

            };

            int orderId = header.Save();

            if (orderId > 0)
            {
                foreach (RemoteOrder item in requestItem)
                {
                    OrderDetail detailItem = new OrderDetail();
                    {
                        bool isCampaign = (customer.CampaignStatu && customer.Users.AuthorityUser._CampaignStatu && item.Product.Campaign.Type > 0 && item.Quantity >= item.Product.Campaign.MinOrder) ? true : false;
                        detailItem.OrderId = orderId;
                        detailItem.ProductId = item.Product.Id;
                        detailItem.ProductCode = item.Product.Code;
                        detailItem.ListPrice = item.Product.PriceList.Value;
                        detailItem.ListPriceCurrency = item.Product.PriceList.Currency;
                        detailItem.ListPriceCurrencyRate = item.Product.PriceCurrencyRate;
                        detailItem.Currency = customer.CurrencyType;
                        detailItem.CurrencyRate = CurrencyList.First(x => x.Type == customer.CurrencyType).Rate;
                        detailItem.CurrencyLocal = "TL";
                        detailItem.Disc1 = item.Product.Rule.Disc1;
                        detailItem.Disc2 = item.Product.Rule.Disc2;
                        detailItem.Disc3 = item.Product.Rule.Disc3;
                        detailItem.Disc4 = item.Product.Rule.Disc4;
                        detailItem.DiscSpecial = 0;
                        detailItem.DueDay = item.Product.Rule.DueDay;
                        detailItem.IsCampaign = isCampaign;
                        detailItem.CampaignId = isCampaign ? item.Product.Campaign.Id : -1;
                        detailItem.CampaignCode = isCampaign ? item.Product.Campaign.Code : string.Empty;
                        detailItem.DiscCampaign = isCampaign ? item.Product.Campaign.Discount : 0;
                        detailItem.Price = (isCampaign && (item.Product.Campaign.Type == CampaignType.NetPrice || item.Product.Campaign.Type == CampaignType.GradualNetPrice)) ? item.Product.CampaignPriceCustomer.ValueFinal : item.Product.PriceListCustomer.ValueFinal;
                        detailItem.NetPrice = (item.Product.PriceNetCustomer.ValueFinal);
                        detailItem.Amount = (item.Product.PriceValue * item.Quantity * item.Product.PriceCurrencyRate);
                        detailItem.NetAmount = (item.Product.PriceNetCustomer.ValueFinal * item.Quantity);
                        detailItem.VatAmount = ((item.Product.Cost * item.Quantity) * (item.Product.VatRate / 100) * item.Product.PriceCurrencyRate);
                        detailItem.Quantity = item.Quantity;
                        detailItem.CouponId = item.Product.CouponItem.UsedDiscountTl > 0 ? item.Product.CouponItem.Id : -1;
                        detailItem.CouponTotal = item.Product.CouponItem.UsedDiscountTl > 0 ? item.Product.CouponItem.UsedDiscountTl : 0;
                        detailItem.DiscCoupon = item.Product.CouponItem.UsedDiscountTl > 0 ? item.Product.CouponItem.Discount : 0;
                        detailItem.DiscCoupon1 = item.Product.CouponItem.UsedDiscountTl > 0 ? item.Product.CouponItem.Discount1 : 0;
                        detailItem.DiscCoupon2 = item.Product.CouponItem.UsedDiscountTl > 0 ? item.Product.CouponItem.Discount2 : 0;
                        detailItem.CustomerId = customer.Id;
                        detailItem.ItemExplanation = string.Empty;

                    }

                    bool result = detailItem.Add();
                    if (!result)
                    {
                        returnStr += item.ProductCode + "kodlu ürünün kalem satırı oluşturulamadı";

                    }
                }

            }
            else
                return "Sipariş oluşturma hata aldı.";
            RemoteOrderResponse responseResult = new RemoteOrderResponse();

            if (returnStr == "")
            {
                responseResult.Result = true;
                header.Status = OrderStatus.OnHold;
                returnStr = "Sipairş No :" + orderId.ToString();
                header.UpdateOrderHeaderStatu();
            }


            responseResult.OrderId = header.Id;
            responseResult.Explanation = returnStr;

            

            return JsonConvert.SerializeObject(responseResult);

        }

        [HttpPost]
        [Route("SaveErpData")]
        public string SaveErpData([FromBody] object responseStr)
        {
            string ipValue = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ipValue))
                ipValue = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            var result = JsonConvert.DeserializeObject<Tuple<DataTable, SyncSettings>>(responseStr.ToString());

            SyncSettings item = result.Item2;
            SyncSettings.UpdateLastUpdate(item.Id);
            AddLogMesssage("Veriler Seriliaze Edildi", LogMessageType.Success, item);

            SyncSettings controlSettings = SyncSettings.GetSettingItem(item.Id, item.TransferTypeId);
            if (ipValue != "::1" && ipValue == "127.0.0.1" && controlSettings.CompanySettings.ServerIp != ipValue)
            {
                AddLogMesssage("İzinsiz İp Adresinden Gelen İstek. İp:" + ipValue, LogMessageType.Error, item);
                return "";
            }


            DataTable dt = new DataTable();
            dt = result.Item1;

            List<DataColumns> listInsertColumns = DataColumns.GetColumns(item.InsertTable, GlobalSettings.ConnectionString, GlobalSettings.Database);
            List<CombineColumns> listCombineColumn = CombineColumns.GetCombineColumns(listInsertColumns, dt);
            if (listCombineColumn.First().Error)
            {
                AddLogMesssage(listCombineColumn.First().ErrorMessage, LogMessageType.Error, item);
            }
            else
            {
                List<InsertCommand> listStr = DbContext.GetInsertCommand(listCombineColumn, dt, item.InsertTable, 500);
                if (listStr.Count > 0)
                {
                    if (listStr.First().Error)
                    {
                        AddLogMesssage("İşlemde Hata Gerçekleşti" + listStr[0].Message, LogMessageType.Error, item);
                    }
                    else
                    {

                        try
                        {
                            if (item.IsFirst)
                                DbContext.TruncateTable(item.InsertTable);

                            int totalRows = item.SendedCount;
                            int transmittedRows = 0;

                            AddLogMesssage(string.Format("Aktarılan Kayıt: 0/{0}", totalRows), LogMessageType.Success, item);

                            using (MySqlConnection mcon = new MySqlConnection(GlobalSettings.ConnectionString))
                            {
                                mcon.Open();
                                foreach (var insertCommandObj in listStr)
                                {
                                    try
                                    {
                                        if (insertCommandObj.InsertCount > 0)
                                        {
                                            using (MySqlCommand ms = new MySqlCommand(insertCommandObj.Command, mcon))
                                            {
                                                ms.CommandTimeout = 0;
                                                ms.CommandType = CommandType.Text;
                                                ms.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        AddLogMesssage(string.Format("HATA: {0} - {1}", ex.Message, insertCommandObj.Command), LogMessageType.Error, item);
                                    }

                                    transmittedRows += insertCommandObj.InsertCount;
                                    AddLogMesssage(string.Format("Aktarılan Kayıt: {0}/{1}", transmittedRows, totalRows), LogMessageType.Success, item);
                                    //   responseValue.ProgressValue = Convert.ToInt32(((transmittedRows * 100) / totalRows)) >= 100 ? 100 : ((transmittedRows * 100) / totalRows);
                                }
                                mcon.Close();
                            }

                            if (item.SendedCount >= item.EndCount)
                            {
                                if (!String.IsNullOrEmpty(item.SyncProcedureName))
                                {
                                    AddLogMesssage("Procedure çalıştırılıyor", LogMessageType.Success, item);
                                    DbContext.ExecuteStoredProcedure(item.SyncProcedureName, DBType.MySql);
                                }

                                if (!String.IsNullOrEmpty(item.AfterErpProcedure))
                                {
                                    AddLogMesssage("ERP Procedure çalıştırılıyor", LogMessageType.Success, item);
                                    //ERP Procedure Çalıştır
                                }

                                AddLogMesssage(string.Format("Transfer tamamlandı. Aktarılan kayıt sayısı: {0}", totalRows), LogMessageType.Success, item);
                            }

                            //responseValue.Status = 0;

                        }
                        catch (Exception ex)
                        {
                            AddLogMesssage("İşlemde Hata Gerçekleşti =>" + ex.Message, LogMessageType.Error, item);
                        }
                    }
                }
                else
                    AddLogMesssage("Transfer başlatılamadı - Data Yok", LogMessageType.Error, item);
            }


            return "";
        }
        private void AddLogMesssage(string message, LogMessageType status, SyncSettings item)
        {
            if (DateTime.Now.Hour >= 22 && DateTime.Now.Hour < 23)
                RuningControl.SyncResponseValues.Log = new List<LogMessage>();

            RuningControl.SyncResponseValues.SettingsId = item.SettingsId;
            RuningControl.SyncResponseValues.Status = 0;
            RuningControl.SyncResponseValues.ProgressValue = 0;
            List<LogMessage> list = new List<LogMessage>();
            list.Add(new LogMessage { Message = "[" + DateTime.Now.ToString() + "] [" + item.SyncTransferType.Name + "] " + message + "</br>", Type = status });
            RuningControl.SyncResponseValues.Log.AddRange(list);

            LogSync log = new LogSync()
            {
                SettingsId = item.SettingsId,
                ProgressValue = 0,
                Message = "[" + DateTime.Now.ToString() + "] [" + item.SyncTransferType.Name + "] " + message,
                Type = 1
            };
            log.Add();
        }

        [HttpPost]
        [Route("SaveSyncLogs")]
        public string SaveSyncLogs([FromBody] SyncResponseValues item)
        {

            try
            {

                RuningControl.SyncResponseValues.SettingsId = item.SettingsId;
                RuningControl.SyncResponseValues.Status = item.Status;
                RuningControl.SyncResponseValues.ProgressValue = item.ProgressValue;

                RuningControl.SyncResponseValues.Log.AddRange(item.Log);
                for (int i = 0; i < item.Log.Count; i++)
                {

                    LogSync log = new LogSync()
                    {
                        SettingsId = item.SettingsId,
                        ProgressValue = 0,
                        Message = item.Log[i].Message,
                        Type = 0
                    };
                    log.Add();
                }

                return "success";
            }
            catch (Exception)
            {
                RuningControl.SyncResponseValues = new SyncResponseValues();
                return "error";
            }
        }



        [HttpPost]
        [Route("CheckRunning")]
        public string CheckRunning([FromBody] bool control)
        {
            RuningControl.IsRunning = control;
            RuningControl.LastCheck = DateTime.Now;
            return "";
        }
        public bool Authenticate(GeneralParameters parameters)
        {
            if (parameters.Authenticate.Username == "OpscW2" && parameters.Authenticate.Password == "fuKyfPdZW3kFk3NwZ3")
            {
                return true;
            }
            Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.None, "Authenticate Error", "Hatalı Kullanıcıdan Gelen İstek UserName:" + parameters.Authenticate.Username + " Password:" + parameters.Authenticate.Password, string.Empty);
            return false;
        }
    }
}
