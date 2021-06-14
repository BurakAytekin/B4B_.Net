using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.Log;
using B2b.Web.v4.Controllers;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class CampaignController : AdminBaseController
    {
        // GET: Admin/Campaign
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ImportExcelCampaign()
        {
            return View();
        }

        public ActionResult ProductOfDay()
        {
            return View();
        }

        #region  HttpPost Methods
        [HttpPost]
        public string UploadCampaing(List<ExcelUpload> uploadList, int campaignType, DateTime startDate, DateTime endDate)
        {

            ExcelUploadResult resultList = new ExcelUploadResult();
            CampaignType selectedCampaignType = (CampaignType)campaignType;

            foreach (var uploadItem in uploadList)
            {
                Campaign campaing = new Campaign();
                campaing.Code = uploadItem.CampaignCode;
                campaing.Code2 = uploadItem.CampaignCode2;
                campaing.TotalQuantity = uploadItem.TotalQuantity;
                campaing.EndQuantity = uploadItem.EndQuantity;
                campaing.MinOrder = uploadItem.MinOrder;
                campaing.Discount = uploadItem.Discount;
                campaing.Type = selectedCampaignType;
                campaing.PriceP = uploadItem.PriceP;
                campaing.PriceV = uploadItem.PriceV;
                campaing.CurrencyP = uploadItem.CurrencyP;
                campaing.CurrencyV = uploadItem.CurrencyV;

                bool isNotGrudual = (selectedCampaignType == CampaignType.NetPrice || selectedCampaignType == CampaignType.GrossPrice || selectedCampaignType == CampaignType.Discount);

                if (isNotGrudual && resultList.SuccessList.Where(x => x.ProductCode == uploadItem.ProductCode).Count() > 0)
                {
                    campaing.Message = "Kademli Olmayan Kampalar Stok Kodu Tekrarlanamaz";
                    resultList.ErrorList.Add(campaing);
                }
                else
                {
                    var product = Product.GetByCode(uploadItem.ProductCode);
                    if (product.Id > 0)
                    {
                        campaing.Id = product.Campaign.Id;
                        campaing.ProductId = product.Id;
                        campaing.ProductCode = product.Code;
                        campaing.ProductName = product.Name;
                        campaing.StartDate = startDate;
                        campaing.FinishDate = endDate;
                        campaing.CreateId = AdminCurrentSalesman.Id;
                        campaing.IsActive = true;


                        if (selectedCampaignType == CampaignType.GradualDiscount || selectedCampaignType == CampaignType.Discount)
                        {
                            campaing.CurrencyP = product.PriceCurrency;
                            campaing.CurrencyV = product.PriceCurrency;
                        }

                        resultList.SuccessList.Add(campaing);
                    }
                    else
                    {
                        campaing.ProductCode = uploadItem.ProductCode;
                        campaing.Message = "Stok Kodu Hatalı";

                        resultList.ErrorList.Add(campaing);
                    }

                }


            }

            return JsonConvert.SerializeObject(resultList);

        }
        [HttpPost]
        public string SaveCampaignExcel(List<Campaign> campaignList)
        {
            bool result = false;


            if (campaignList != null)
            {
                foreach (var campaign in campaignList)
                {
                    campaign.PriceV = campaign.PriceP;
                    campaign.CurrencyV = campaign.CurrencyP;

                    if (campaign.Id == 0)
                    {

                        campaign.CreateId = AdminCurrentSalesman.Id;
                        int id = campaign.Add();
                        result = id > 0;
                    }
                    else
                    {
                        campaign.EditId = AdminCurrentSalesman.Id;
                        result = campaign.Update();
                    }
                }
            }


            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);

        }
        [HttpPost]
        public string GetCampaignHeader(int type, string t9Text)
        {
            List<Campaign> list = Campaign.GetCampaigHeaderByType(type, t9Text);
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string GetProductOfDayList(string t9Text)
        {
            List<ProductOfDayCs> list = ProductOfDayCs.GetproductOfDay(t9Text);
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string DeleteProductOfDays(string DeleteIds)
        {
            bool result = false;

            DeleteIds = DeleteIds.Substring(0, DeleteIds.Length - 1);
            result = ProductOfDayCs.Delete(DeleteIds, AdminCurrentSalesman.Id);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }

        [HttpPost]
        public string SaveProductOfDay(ProductOfDayCs campaign, string imageBaseIcon)
        {

            bool result = false;
            if (!campaign.IsUseProductPicture)
            {
                string imgTypeIcon = GetFileType(imageBaseIcon);

                string fileIconName = Guid.NewGuid().ToString();
                byte[] fileIconData = Parse(imageBaseIcon);

                string fullFtpFileIconPath = GlobalSettings.FtpServerUploadAddress + GlobalSettings.GeneralPath + fileIconName + "." + imgTypeIcon;

                result = FtpHelper.UploadRemoteServer(fileIconData, fullFtpFileIconPath);

                campaign.PicturePath = GlobalSettings.GeneralPath + fileIconName + "." + imgTypeIcon;

            }
            if (campaign.Id == 0)
            {
                campaign.CreateId = AdminCurrentSalesman.Id;
                result = campaign.Add();
            }
            else
            {
                campaign.EditId = AdminCurrentSalesman.Id;
                result = campaign.Update();
            }



            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);

        }



        [HttpPost]
        public JsonResult GetCampaignPromotion(string code)
        {

            string[] promArray = code.Split(',');

            List<PromotionList> promList = new List<PromotionList>();
            string promotionCodes = "";
            foreach (var item in promArray)
            {
                promList.Add(new PromotionList { Code = item.Substring(0, item.IndexOf(':')) + "'", Quantity = double.Parse(item.Substring(item.IndexOf(':') + 1, (item.IndexOf(';') - 1 - item.IndexOf(':')))), MinOrder = double.Parse(item.Substring(item.IndexOf(';') + 1, item.Length - item.IndexOf(';') - 2)) });
                promotionCodes += (item.Substring(0, item.IndexOf(':')) + "',");
            }
            promotionCodes = promotionCodes.Substring(0, promotionCodes.Length - 1);

            List<Product> list = Campaign.GetCampaigPromotionProduct(promotionCodes);

            foreach (var item in list)
            {
                item.TotalQuantity = promList.Where(x => x.Code.Replace("'", "") == item.Code).First().Quantity;
                item.TotalQuantityOnWay = promList.Where(x => x.Code.Replace("'", "") == item.Code).First().MinOrder;
            }

            return Json(list);
        }
        [HttpPost]
        public JsonResult GetCampaigDetail(int id)
        {
            List<Campaign> list = Campaign.GetCampaigDetail(id);
            return Json(list);
        }
        [HttpPost]
        public string DeleteCampaigns(string DeleteIds)
        {
            bool result = false;

            DeleteIds = DeleteIds.Substring(0, DeleteIds.Length - 1);

            result = Campaign.Delete(DeleteIds, AdminCurrentSalesman.Id);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string DeleteCampaignDetail(int id)
        {
            bool result = false;
            result = Campaign.DeleteleDetail(id, -1, AdminCurrentSalesman.Id);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public string SaveCampaign(Campaign campaign, List<Campaign> gradualItemList)
        {

            bool result = false;
            List<Campaign> campaingList = new List<Campaign>();
            if (campaign.Id == 0)
            {
                campaign.CreateId = AdminCurrentSalesman.Id;
                int headerId = campaign.Add();
                result = headerId > 0;


                if (campaign.Type == CampaignType.GradualDiscount || campaign.Type == CampaignType.GradualNetPrice)
                {
                    campaingList = gradualItemList as List<Campaign>;
                    Campaign firstCampaignItem = campaingList[0];
                    foreach (var item in campaingList)
                    {
                        item.HeaderId = headerId;
                        item.Type = campaign.Type;
                        item.StartDate = campaign.StartDate;
                        item.FinishDate = campaign.FinishDate;

                        item.AddDetail();
                        if (item.MinOrder > firstCampaignItem.MinOrder)
                            firstCampaignItem = item;

                    }
                    campaign.Id = headerId;
                    campaign.EditId = AdminCurrentSalesman.Id;
                    campaign.PriceP = firstCampaignItem.PriceP;
                    campaign.PriceV = firstCampaignItem.PriceV;
                    campaign.CurrencyP = firstCampaignItem.CurrencyP;
                    campaign.CurrencyV = firstCampaignItem.CurrencyV;
                    campaign.Discount = firstCampaignItem.Discount;
                    campaign.MinOrder = firstCampaignItem.MinOrder;
                    campaign.Update();
                }

            }
            else
            {
                campaign.EditId = AdminCurrentSalesman.Id;
                result = campaign.Update();
                if (campaign.Type == CampaignType.GradualDiscount || campaign.Type == CampaignType.GradualNetPrice)
                {
                    Campaign.DeleteleDetail(-1, campaign.Id, AdminCurrentSalesman.Id);
                    campaingList = gradualItemList as List<Campaign>;
                    Campaign firstCampaignItem = campaingList[0];
                    foreach (var item in campaingList)
                    {
                        item.HeaderId = campaign.Id;
                        item.Type = campaign.Type;
                        item.StartDate = campaign.StartDate;
                        item.FinishDate = campaign.FinishDate;

                        item.AddDetail();
                        if (item.MinOrder > firstCampaignItem.MinOrder)
                            firstCampaignItem = item;

                    }

                    campaign.EditId = AdminCurrentSalesman.Id;
                    campaign.PriceP = firstCampaignItem.PriceP;
                    campaign.PriceV = firstCampaignItem.PriceV;
                    campaign.CurrencyP = firstCampaignItem.CurrencyP;
                    campaign.CurrencyV = firstCampaignItem.CurrencyV;
                    campaign.Discount = firstCampaignItem.Discount;
                    campaign.MinOrder = firstCampaignItem.MinOrder;
                    campaign.Update();

                }


            }
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);

        }
        #endregion
        public class ExcelUpload
        {
            public string ProductCode { get; set; }
            public double PriceP { get; set; }
            public double PriceV { get; set; }
            public string CurrencyP { get; set; }
            public string CurrencyV { get; set; }
            public int MinOrder { get; set; }
            public string CampaignCode { get; set; }
            public string CampaignCode2 { get; set; }
            public double TotalQuantity { get; set; }
            public double EndQuantity { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime FinishDate { get; set; }
            public double Discount { get; set; }
            public bool DiscountPassive { get; set; }
        }

        public class ExcelUploadResult
        {
            public ExcelUploadResult()
            {
                SuccessList = new List<Campaign>();
                ErrorList = new List<Campaign>();
            }
            public List<Campaign> SuccessList { get; set; }
            public List<Campaign> ErrorList { get; set; }
        }


    }
}