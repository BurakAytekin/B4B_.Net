using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using System.Text;
using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.ErpLayer;
using System.Data;
using Newtonsoft.Json;

namespace B2b.Web.v4.Controllers
{
    public class SearchController : BaseController
    {
        List<ErpFunctionDetail> ProductOrderYearList
        {
            get { return (List<ErpFunctionDetail>)Session["ProductOrderYearList"]; }
            set { Session["ProductOrderYearList"] = value; }
        }

        public int logId
        {
            get { return Session["logId"] != null ? (int)Session["logId"] : -1; }
            set { Session["logId"] = value; }
        }
        private string allStr = "Seçiniz";
        // GET: Search
        public ActionResult Index()
        {
            logId = -1;
            ViewBag.SearchAnnouncementsPicture = Announcements.GetAllByType(AnnouncementsType.SearchPageBanner);
            return View();
        }

        #region HttpPostMethods

        [HttpPost]
        public string GetCookieValue()
        {
            return JsonConvert.SerializeObject(GlobalSettings.CookieName);
        }

        [HttpPost]
        public JsonResult GetProductOrderYear()
        {
            ProductOrderYearList = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.ProductOrder);

            return Json(ProductOrderYearList);
        }


        [HttpPost]
        public JsonResult GetCookiieValue()
        {
            return Json(GlobalSettings.CookieName + "SearchWords");
        }

        [HttpPost]
        public JsonResult GetProductOrderList(string productCode, int id)
        {

            ErpFunctionDetail yearItem = ProductOrderYearList.Where(x => x.Id == id).First();


            GeneralParameters parametres = new GeneralParameters();
            parametres.CommandType = yearItem.FunctionType == 0 ? CommandType.StoredProcedure : CommandType.Text;
            string[] parameterNames = new string[2];
            string[] parameterValues = new string[2];

            parameterNames[0] = "pCustomerCode";
            parameterNames[1] = "pProductCode";

            parameterValues[0] = CurrentCustomer.Code;
            parameterValues[1] = productCode;

            parametres.ParameterNames = parameterNames;
            parametres.ParameterValues = parameterValues;
            parametres.CommandText = yearItem.FunctionName;

            DataTable dt = ErpHelper.FireServiceMethod(parametres, yearItem.Settings).ConvertResponseDataTable();
            List<ProductOrder> list = new List<ProductOrder>();
            list = dt.DataTableToList<ProductOrder>();


            return Json(list);
        }

        [HttpPost]
        public string GetManufaturerList()
        {
            List<Manufacturer> manufactuerList = Manufacturer.GetList(CurrentCustomer.Id);
            manufactuerList.Insert(0, new Manufacturer() { Name = allStr });
            return JsonConvert.SerializeObject(manufactuerList);
        }
        [HttpPost]
        public string GetProductGroup1List()
        {
            List<ProductGroup> productGroupList = ProductGroup.GetProductGroup1List(CurrentCustomer.Id);
            //productGroupList.Insert(0, new ProductGroup() { Name = allStr });
            productGroupList.RemoveAll(R => R.Name == "0");

            List<ProductGroup> productGroupListCustomOrder = new List<ProductGroup>();
            for (int i = 0; i < productGroupList.Count; i++)
            {
                if (i==0)
                {
                    try
                    {
                        productGroupListCustomOrder.Add(productGroupList.Where(x => x.Name == "DUVAR TİPİ KLİMALAR").First());

                    }
                    catch (Exception)
                    {

                    }
                }
                else if (i == 2)
                {
                    try
                    {
                        productGroupListCustomOrder.Add(productGroupList.Where(x => x.Name == "MULTİ KLİMALAR").First());

                    }
                    catch (Exception)
                    {

                    }
                }
                else if (i == 1)
                {
                    try
                    {
                        productGroupListCustomOrder.Add(productGroupList.Where(x => x.Name == "TİCARİ KLİMALAR").First());

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                else if (i == 3)
                {
                    try
                    {
                        productGroupListCustomOrder.Add(productGroupList.Where(x => x.Name == "OPSİYONEL KUMANDA").First());

                    }
                    catch (Exception)
                    {


                    }          
                        }
                else if (i == 4)
                {
                    try
                    {
                        productGroupListCustomOrder.Add(productGroupList.Where(x => x.Name == "VRF SİSTEMLER").First());

                    }
                    catch (Exception)
                    {

                    }
                }
                else if (i == 5)
                {
                    try
                    {
                        productGroupListCustomOrder.Add(productGroupList.Where(x => x.Name == "YEDEK PARÇA").First());

                    }
                    catch (Exception)
                    {

                        //throw;
                    }
                }
                //else
                //{
                //    foreach (var item in productGroupList.Where(x=>x.Name!= "SPLİT" && x.Name!= "MULTİ SPLİT" && x.Name!="TİCARİ" && x.Name!="KUMANDA" ))
                //    {
                //        productGroupListCustomOrder.Add(productGroupList.Where(x => x.Name == "KUMANDA").First());
                //    }
                //}
            }

            return JsonConvert.SerializeObject(productGroupListCustomOrder);
        }

        [HttpPost]
        public string GetProductGroup2List(string group1Name)
        {
            List<ProductGroup> productGroupList = ProductGroup.GetProductGroup2List(CurrentCustomer.Id, group1Name);
            //productGroupList.Insert(0, new ProductGroup() { Name = allStr });
            productGroupList.RemoveAll(R => R.Name == "0");

            return JsonConvert.SerializeObject(productGroupList);
        }

        [HttpPost]
        public string GetProductGroup3List(string group1Name, string group2Name)
        {
            List<ProductGroup> productGroupList = ProductGroup.GetProductGroup3List(CurrentCustomer.Id, group1Name, group2Name);
            //productGroupList.Insert(0, new ProductGroup() { Name = allStr });
            productGroupList.RemoveAll(R => R.Name == "0");

            return JsonConvert.SerializeObject(productGroupList);
        }
        [HttpPost]
        public string GetProductGroup4List(string group1Name, string group2Name, string group3Name)
        {
            List<ProductGroup> productGroupList = ProductGroup.GetProductGroup4List(CurrentCustomer.Id, group1Name, group2Name,group3Name);
            //productGroupList.Insert(0, new ProductGroup() { Name = allStr });
            productGroupList.RemoveAll(R => R.Name == "0");

            return JsonConvert.SerializeObject(productGroupList);
        }
        [HttpPost]
        public string GetVehicleBrandList()
        {
            List<VehicleBrandModel> list = VehicleBrandModel.GetVehicleBrandList();
            list.Insert(0, new VehicleBrandModel() { Brand = allStr });
            return JsonConvert.SerializeObject(list);
        }
        [HttpPost]
        public string GetVehicleModelList(string brand)
        {
            List<VehicleBrandModel> list = VehicleBrandModel.GetVehicleBrandModelList(brand);
            list.Insert(0, new VehicleBrandModel() { Model = allStr });
            return JsonConvert.SerializeObject(list);
        }
        [HttpPost]
        public string SearchProduct(int dataCount, string manufacturer, string vehicleBrand, string vehicleModel, string productGroup1, string productGroup2, string productGroup3,
         string t9Text, bool campaign, bool newArrival, bool newProduct, bool comparsionProduct, bool onQuantity, bool onWay, bool image)
        {
            var manufacturers = manufacturer != null ? manufacturer.Split((char)44).GenerateCommaListString() : "*";


            SearchCriteria sc = new SearchCriteria()
            {
                Manufacturer = manufacturers ?? "*",
                T9Text = t9Text ?? "*",
                ProductGroup1 = !string.IsNullOrEmpty(productGroup1) ? productGroup1 : "*",
                ProductGroup2 = !string.IsNullOrEmpty(productGroup2) ? productGroup2 : "*",
                ProductGroup3 = !string.IsNullOrEmpty(productGroup3) ? productGroup3 : "*",
                VehicleBrand = !string.IsNullOrEmpty(vehicleBrand) ? vehicleBrand : "*",
                VehicleBrandModel = !string.IsNullOrEmpty(vehicleModel) ? vehicleModel : "*",
                VehicleType = "*",
                Campaign = campaign,
                NewArrival = newArrival,
                NewProduct = newProduct,
                OnQuantity = onQuantity,
                OnWay = onWay,
                Picture = image,
                ComparsionProduct = comparsionProduct
            };

            string vOrderQuery = "";
            List<Product> list = Product.Search(dataCount, 24, CurrentLoginType, CurrentCustomer, CurrentSalesmanId,
               sc.Manufacturer, sc.T9Text, sc.ProductGroup1, sc.ProductGroup2, sc.ProductGroup3,
               Convert.ToInt32(sc.NewProduct), Convert.ToInt32(sc.ComparsionProduct), Convert.ToInt32(sc.Campaign), Convert.ToInt32(sc.NewArrival),
               sc.VehicleBrand, sc.VehicleBrandModel, sc.VehicleType, -1, Convert.ToInt32(sc.OnQuantity), Convert.ToInt32(sc.OnWay), Convert.ToInt32(sc.Picture), Convert.ToInt32(sc.BannerStatu), vOrderQuery);

            //LogSearch(sc, list);
            if (dataCount == 0)
                HttpContext.Session["SearchProductList"] = list;
            else
            {
                List<Product> oldList = HttpContext.Session["SearchProductList"] as List<Product>;
                oldList.AddRange(list);
                HttpContext.Session["SearchProductList"] = oldList;
            }


            #region Loglama
            if (dataCount == 0)
            {


                var searchType = list.Count > 15 ? 1 : 0;
                logId = Logger.LogSearchHeader(
                       CurrentCustomer.Id,
                       CurrentCustomer.Users.Id,
                        CurrentSalesmanId,
                       sc,
                       (list.Count > 0 ? ProcessSearch.Success : ProcessSearch.NoRecord), GetUserIpAddress(), -1, searchType);
                #region Log Detail
                //  var sqlString = "";
                StringBuilder sqlString = new StringBuilder();

                for (int i = 0; i < list.Count; i++)
                {
                    sqlString.Append("INSERT INTO log_search_detail( ");
                    sqlString.Append("HeaderId,");
                    sqlString.Append("ProductCode,");
                    sqlString.Append("Price,");
                    sqlString.Append(" Currency,");
                    sqlString.Append("TotalQuantity,");
                    sqlString.Append("GroupId,");
                    sqlString.Append("ProductId");
                    sqlString.Append(" ) ");
                    sqlString.Append("VALUES");
                    sqlString.Append(" (");
                    sqlString.Append(logId + ",");
                    sqlString.Append("'" + list[i].Code + "',");
                    sqlString.Append("'" + list[i].PriceNetCustomer.ValueFinal.ToString("N4") + "',");
                    sqlString.Append("'" + list[i].CustomerCurrency + "',");
                    sqlString.Append("" + list[i].TotalQuantity + ",");
                    sqlString.Append("" + list[i].Id + ",");
                    sqlString.Append("" + list[i].GroupId + "");
                    sqlString.Append("); ");

                    if (i == 15)
                        break;

                }
                DatabaseContext.ExecuteNonQuery(System.Data.CommandType.Text, sqlString.ToString());
                #endregion

            }
            #endregion

            return JsonConvert.SerializeObject(list);
        }
        [HttpPost]
        public string GetProductImages(int id)
        {
            List<Picture> list = Picture.GetPicturePathByProductId(id);
            if (list == null || list.Count == 0)
            {
                list = new List<Picture>();
                list.Add(new Picture() { Path = "images/nophoto.jpg" });
            }
            return JsonConvert.SerializeObject(list);
        }

        #region AddBasketMethods
        [HttpPost]
        public JsonResult GetAvailable(int id)
        {
            List<Basket> basketAvailableList = Basket.GetAvailableInBasket(CurrentCustomer, CurrentLoginType, id, CurrentCustomer.Id, (int)CurrentLoginType, CurrentCustomer.Users.Id);
            return Json(basketAvailableList.Count > 0 ? basketAvailableList[0].Quantity.ToString() : string.Empty);
        }
        [HttpPost]
        public JsonResult AddBasket(int id, double qty)
        {
            //Dil Ayarları için daha sonra düzenlenecek
            List<Basket> basketAvailableList = Basket.GetAvailableInBasket(CurrentCustomer, CurrentLoginType, id, CurrentCustomer.Id, (int)CurrentLoginType, CurrentCustomer.Users.Id);

            double cmpAvailableQuantity = 0;

            Product product = Product.GetById(id, CurrentLoginType, CurrentCustomer); //productList.Where(p => p.Id == id).First();
            if (qty < product.MinOrder)
            {

                return Json("{\"statu\":\"error\",\"message\":\"Minimum sipariş adedinden daha az sipariş veremezsiniz.\",\"cmpAvailableQuantity\":" + cmpAvailableQuantity + "}");

            }
            else
            {
                if (product.IsPackIncrease && product.MinOrder != 1)
                {
                    double k = qty / product.MinOrder;

                    if (qty % product.MinOrder == 0)
                        qty = Convert.ToInt32(k) * product.MinOrder;
                    else
                        qty = (Convert.ToInt32(Math.Floor(k)) + 1) * product.MinOrder;
                }
                if (basketAvailableList.Count > 0)
                {
                    Basket basket = basketAvailableList[0];
                    basket.Quantity = basketAvailableList[0].Quantity + qty;
                    basket.DiscSpecial = 0;
                    basket.Update();

                    if (product.Campaign.Type > 0 && basket.Quantity < product.Campaign.MinOrder)
                        cmpAvailableQuantity = product.Campaign.MinOrder - basket.Quantity;
                }
                else
                {
                    Basket basket = new Basket();
                    {
                        basket.Product = product;
                        basket.CustomerId = CurrentCustomer.Id;
                        basket.SalesmanId = CurrentSalesmanId;
                        basket.Quantity = qty;
                        basket.DiscSpecial = 0;
                        basket.RecordDate = DateTime.Now;
                        basket.AddType = (int)CurrentLoginType;
                        basket.ClientNumber = -1;
                        basket.UserId = CurrentCustomer.Users.Id;
                        basket.UserCode = CurrentCustomer.Users.Code;
                        basket.CustomerCode = CurrentCustomer.Code;
                        basket.ProductCode = product.Code;
                        basket.LogId = logId;
                    }
                    basket.Add();

                    if (product.Campaign.Type > 0 && basket.Quantity < product.Campaign.MinOrder)
                        cmpAvailableQuantity = product.Campaign.MinOrder - basket.Quantity;
                }
            }

            string message = $"Sepetinize {product.Code} ürününden {qty} {product.Unit} eklendi.";
            return Json("{\"statu\":\"success\",\"message\":\"" + message + "\",\"cmpAvailableQuantity\":" + cmpAvailableQuantity + "}");
        }
        #endregion




        [HttpPost]
        public void LogSearchOpenDetail(int id, string productCode)
        {
            Logger.LogSearchOpenDetail(CurrentCustomer.Id, CurrentCustomer.Code, CurrentCustomer.Users.Id, CurrentSalesmanId, id, productCode, logId);
        }

        [HttpPost]
        public JsonResult GetAlternativeList(int groupId)
        {
            List<Product> list = Product.Search(0, 500, CurrentLoginType, CurrentCustomer, -1, "*", "*", "*", "*", "*", 0, 0, 0, 0, "*", "*", "*", groupId, 0, 0, 0, 0, "");
            return Json(list);
        }
        [HttpPost]
        public JsonResult GetLinkedList(int productId)
        {
            List<Product> list = Product.GetProductListLinked(productId, CurrentLoginType, CurrentCustomer);
            return Json(list);
        }


        [HttpPost]
        public JsonResult GetOemRivalCodeList(int groupId, int type)
        {
            List<Oem> list = Oem.GetListByGroupId(groupId, (OemType)type);
            return Json(list);
        }

        [HttpPost]
        public JsonResult GetVehicleList(int groupId)
        {
            List<VehicleBrandModel> list = VehicleBrandModel.GetVehicleBrandModelTypeByGroupId(groupId);
            return Json(list);
        }



        [HttpPost]
        public string GetProductExplanation(int productId)
        {
            Product item = Product.GetById(productId, CurrentLoginType, CurrentCustomer);
            return JsonConvert.SerializeObject(item);
        }

        [HttpPost]
        public JsonResult AddOemBlackList(int id, int productId, string newName, string note, int type)
        {

            OemBlackList item = new OemBlackList
            {
                OemNo = newName,
                OemId = id,
                Note = note,
                ProductId = productId,
                Type = (BlackListType)type,
                AddType = (int)CurrentLoginType,
                CreateId = CurrentEditId
            };
            item.Add();

            MessageBox message = new MessageBox(MessageBoxType.Success, "Kaydınız Alınmıştır");

            return Json(message);
        }
        [HttpPost]
        public JsonResult AddOemOrRivalCode(int productId, string newData, string vehicleBrand, int type)
        {

            OemBlackList item = new OemBlackList();
            item.OemNo = newData;
            item.OemId = -1;
            item.ProductId = productId;
            item.BrandName = vehicleBrand;
            item.Note = string.Empty;
            item.Type = (BlackListType)type;
            item.AddType = (int)CurrentLoginType;
            item.CreateId = CurrentEditId;
            item.Add();

            MessageBox message = new MessageBox(MessageBoxType.Success, "Kaydınız Alınmıştır");
            return Json(message);
        }

        [HttpPost]
        public JsonResult ReturnProduct(string code)
        {
            //Ramazan
            List<ProductOrder> list = new List<ProductOrder>();
            return Json(list);
        }
        [HttpPost]
        public JsonResult FollowProductOrComparisonRemove(int Id)
        {
            bool statu = ProductComparison.DeleteProductComparison(Id, CurrentCustomer.Users.Id);
            return Json(statu);
        }

        [HttpPost]
        public string ProducrComparisonCount()
        {
            ProductComparison productComparison = ProductComparison.GetProductComparisonCount(CurrentCustomer.Users.Id);

            return JsonConvert.SerializeObject(productComparison);
        }

        [HttpPost]
        public string GetCampaignDetail(int productId)
        {
            Product item = Product.GetById(productId, CurrentLoginType, CurrentCustomer);

            return JsonConvert.SerializeObject(item);
        }


        #endregion
        #region PatialViewMethods
        public PartialViewResult ProductDetail(int productId, int groupId, string productCode, int campaignType)
        {
            ViewBag.ProductId = productId;
            ViewBag.GroupId = groupId;
            ViewBag.ProductCode = productCode;
            ViewBag.CampaignType = campaignType;
            return PartialView("ProductDetail", ViewBag);
        }
        public PartialViewResult OemOrRivalInsert(int productId)
        {
            ViewBag.ProductId = productId;

            return PartialView("OemOrRivalInsert", ViewBag);
        }
        public PartialViewResult CustomAddBasket(int productId, string productCode)
        {
            ViewBag.ProductId = productId;
            ViewBag.ProductCode = productCode;
            ViewBag.Product = Product.GetById(productId, CurrentLoginType, CurrentCustomer);
            ViewBag.PictureList = Picture.GetPicturePathByProductId(productId);
            return PartialView("CustomAddBasket", ViewBag);
        }

        public PartialViewResult FollowProducts(int productId)
        {
            ProductComparison.ComparisonInsert(productId, CurrentCustomer.Id, CurrentCustomer.Users.Id,
                CurrentLoginType == LoginType.Salesman ? CurrentSalesman.Id : -1, (int)ComparisonOrFollowType.Follow,
                CurrentLoginType == LoginType.Salesman ? CurrentSalesman.Id : CurrentCustomer.Id);
            ViewBag.Products = ProductComparison.GetProductComparisonList(ComparisonOrFollowType.Follow, CurrentLoginType, CurrentCustomer, (CurrentLoginType == LoginType.Salesman ? CurrentSalesman.Id : -1));
            return PartialView("FollowProducts", ViewBag);
        }

        #endregion
    }
}