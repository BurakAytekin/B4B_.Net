using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace B2b.Web.v4.Controllers
{
    public class PartialController : BaseController
    {
        // Üreticiler

        public PartialViewResult ManufacturerView()
        {
            ViewBag.Manufacturers = Announcements.GetAllByType(AnnouncementsType.Brands);
            return PartialView("ManufacturerView", ViewBag);
        }

        //Email Kaydı
        public PartialViewResult EmailNotFoundView()
        {
            return new PartialViewResult();
        }

        //Yeni Ürünler
        public PartialViewResult NewProductView()
        {
            return PartialView("NewProductView", Product.Search(0, 10, CurrentLoginType, CurrentCustomer, -1, "*", "*", "*", "*", "*", 1, 0, 0, 0, "*", "*", "*", -1, 0, 0, 0, 0, ""));
        }

        //Taksit Bilgileri
        public PartialViewResult InstallmentInformationView()
        {
            return new PartialViewResult();
        }
        public PartialViewResult InstallmentInformationPaymentView()
        {
            return new PartialViewResult();
        }

        [HttpPost]
        public string GetInstallmentBankCardList()
        {
            List<PosInstallment> list = PosInstallment.GetListByCardType();
            return JsonConvert.SerializeObject(list);
        }
        [HttpPost]
        public string GetBankInstalmentList(string Type, int bankId,int posBankId)
        {
            List<PosInstallment> list2 = PosInstallment.GetListInstalmentByCardType(Type,bankId, posBankId);
            return JsonConvert.SerializeObject(list2);
        }

        //Banka bilgileri
        public PartialViewResult BankAccountView()
        {
            return PartialView("BankAccountView", BankAccount.GetBankAccountList());
        }

        //Kayan yazı
        public PartialViewResult ScrollingTextView()
        {
            return PartialView("ScrollingTextView", Announcements.GetAllByType(AnnouncementsType.MenuFloatingWriting));
        }


        //Price View
        #region HttpPost Methods
        [HttpPost]

        public PartialViewResult PriceView(Basket basket)
        {

            basket.Product.CalculateDetailInformation(false, (CurrentCustomer.CampaignStatu && CurrentCustomer.Users.AuthorityUser._CampaignStatu), basket.Quantity, basket.DiscSpecial, basket.IsFixedPrice, basket.FixedPrice, basket.FixedCurrency, basket.FixedCurrencyRate, CurrentCustomer.Users.Rate);
            ViewBag.PriceViewItem = basket;
            return PartialView("PriceView", ViewBag);
        }
        [HttpPost]
        public PartialViewResult ProdcutPriceView(Product product, int productId)
        {
            productId = productId == -1 ? product.Id : productId;

            // Product product = Product.GetById(productId,CurrentLoginType,CurrentCustomer);
            product = product == null ? Product.GetById(productId, CurrentLoginType, CurrentCustomer) : product;

            product.CalculateDetailInformation(false, (CurrentCustomer.CampaignStatu && CurrentCustomer.Users.AuthorityUser._CampaignStatu), -1, 0);
            ViewBag.Product = product;

            return PartialView("ProdcutPriceView", ViewBag);
        }


        [HttpPost]
        public PartialViewResult CampaignMinOrderView(Product productItem, int productId)
        {
            productItem = productItem == null ? Product.GetById(productId) : productItem;
            if (productItem.Campaign.Type > 0)
                ViewBag.CampaignMinOrderView = "En az " + productItem.Campaign.MinOrder.ToString() + " Adet alımlarda kampanya geçerlidir.";
            else
                ViewBag.CampaignMinOrderView = null;
            return PartialView("CampaignMinOrderView", ViewBag);
        }

        [HttpPost]
        public PartialViewResult WarehouseView(Product productItem, int productId)
        {

            productItem = productItem == null ? Product.GetById(productId) : productItem;
            List<WarehouseQuantity> warehouseQuantityList = WarehouseQuantity.GetListProductQuantityByCustomerId(productItem.Id,
                CurrentCustomer.Id);
            // Product productItem=Product.GetByOnlyId(productId);
            List<TempWarehouseQuantity> qunatityList = new List<TempWarehouseQuantity>();

            LoginType quantiyAuthorityLoginType;
            if (CurrentLoginType != LoginType.Salesman && CurrentCustomer.AuthorityCustomer._ShowQuantity && CurrentCustomer.Users.AuthorityUser._ShowQuantity)
                quantiyAuthorityLoginType = LoginType.Salesman;
            else if (CurrentLoginType == LoginType.Salesman)
                quantiyAuthorityLoginType = LoginType.Salesman;
            else
                quantiyAuthorityLoginType = LoginType.Customer;
            foreach (var warehouseQuantity in warehouseQuantityList)
            {

                productItem.AvailabilityStatus = productItem.CalculateAvaibilityStatus(warehouseQuantity.Quantity);
                string availabilityText, availabilityCss;
                productItem.SetAvailabilityClass(quantiyAuthorityLoginType, warehouseQuantity.Quantity, productItem.AvailabilityStatus, out availabilityCss, out availabilityText);
                TempWarehouseQuantity tmpWarehouse = new TempWarehouseQuantity(warehouseQuantity.Warehouse.Name, availabilityCss, availabilityText);
                qunatityList.Add(tmpWarehouse);

            }

            ViewBag.WareHouseProductViewList = qunatityList;
            return PartialView("WarehouseView", ViewBag);
        }

        [HttpPost]
        public PartialViewResult KitDetailView(Product productItem)
        {
            productItem.KitDetailList = Product.GetListByKitMainId(productItem.Id, CurrentCustomer.Id);
            ViewBag.KitList = productItem.KitDetailList;
            return PartialView("KitDetailView", ViewBag);
        }
        #endregion

        public class TempWarehouseQuantity
        {
            public string Name { get; set; }
            public string AvailabilityCss { get; set; }
            public string AvailabilityText { get; set; }

            public TempWarehouseQuantity(string name, string availabilityCss, string availabilityText)
            {
                Name = name;
                AvailabilityCss = availabilityCss;
                AvailabilityText = availabilityText;

            }

        }


        //Blog
        public PartialViewResult BlogLastView()
        {
            List<Blog> blogList = Blog.GetBlogList(string.Empty, 0, 5);
            ViewBag.BlogList = blogList;
            return PartialView("BlogLastView", ViewBag);
        }

        //Blog
        public PartialViewResult BlogTagView()
        {
            List<Blog> blogTagList = Blog.GetBlogCategoryList();
            blogTagList.Insert(0, new Blog() { Category = "TÜMÜ" });
            ViewBag.BlogTagList = blogTagList;
            return PartialView("BlogTagView", ViewBag);
        }

        //BlogComment
        public PartialViewResult BlogCommentView()
        {
            List<BlogComment> BlogCommentList = BlogComment.GetBlogCommentListByBanner();
            ViewBag.BlogCommentList = BlogCommentList;
            return PartialView("BlogCommentView", ViewBag);
        }



    }


}