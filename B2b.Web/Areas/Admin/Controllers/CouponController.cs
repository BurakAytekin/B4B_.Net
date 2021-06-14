using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.Log;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class CouponController : AdminBaseController
    {
        // GET: Admin/Coupon
        public ActionResult Index()
        {
            return View();
        }

        #region   HttpPost Methods
        [HttpPost]
        public string UpdateCouponPriority(int _Id, int _Priority)
        {
            CouponCs coupon = new CouponCs { Id = _Id, Priority = _Priority };
            bool result = coupon.UpdateCouponPriority();
            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return JsonConvert.SerializeObject(message);
        }

        [HttpPost]
        public string GenerateCouponCode()
        {
            CouponCs list = CouponCs.GenerateCouponCode();
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public JsonResult GetCouponStatistics(int couponId)
        {
            CouponCs list = CouponCs.GetCouponStatistics(couponId);
            return Json(list);
        }

        [HttpPost]
        public string GetCouponList()
        {
            List<CouponCs> list = CouponCs.GetCouponList();
            Session["CouponList"] = list;
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string GetProductsRuleList()
        {
            List<Rule> list = Rule.GetRuleProductList();
            list.Insert(0, new Rule() { Product = "Hepsi" });
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string GetProductsSpecialCodeList()
        {
            List<Product> list = Product.GetProductSpecialCode();
            list.Insert(0, new Product() { SpecialCode1 = "Hepsi" });

            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string GetCouponCustomersList(int couponId, int type)
        {
            List<CouponCustomers> list = CouponCustomers.GetCouponCustomersList(couponId, type);
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public JsonResult SetAllCustomers(int couponId)
        {
             bool result = false;

            CouponCustomers item = new CouponCustomers()
            {
                CouponId = couponId,
                CreateId = AdminCurrentSalesman.Id
            };
            result = item.SetAllCustomers();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        [HttpPost]
        public JsonResult ChangeActivity(int couponId, bool active)
        {
            bool result = false;

            List<CouponCs> list = Session["CouponList"] as List<CouponCs>;
            CouponCs coupon = list.Where(x => x.Id == couponId).First();
            coupon.IsActive = active;
            result = coupon.Update();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        [HttpPost]
        public JsonResult SaveCouponCustomers(CouponCustomers couponCustomers)
        {
            bool result = false;
            if (couponCustomers.Id == 0)
            {
                couponCustomers.CreateId = AdminCurrentSalesman.Id;
                result = couponCustomers.Add();
            }
            else
            {
                couponCustomers.EditId = AdminCurrentSalesman.Id;
                result = couponCustomers.Update();
            }

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        [HttpPost]
        public JsonResult SaveCoupon(CouponCs coupon)
        {
           bool result = false;

            coupon.Discount = coupon.Type == 0 ? coupon.Discount : 0;
            coupon.Discount1 = coupon.Type == 0 ? coupon.Discount1 : 0;
            coupon.Discount2 = coupon.Type == 0 ? coupon.Discount2 : 0;
            coupon.Price = coupon.Type == 1 ? coupon.Price : 0;
            coupon.ProductCode = coupon.Type == 2 ? coupon.ProductCode : string.Empty;
            coupon.MinPrice = coupon.CalculateType == 0 ? coupon.MinPrice : 0;
            coupon.MinQuantity = coupon.CalculateType == 1 ? coupon.MinQuantity : 0;

            if (coupon.Id == 0)
            {
                coupon.CreateId = AdminCurrentSalesman.Id;
                result = coupon.Add();
            }
            else
            {
                coupon.EditId = AdminCurrentSalesman.Id;
                result = coupon.Update();
            }

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }
        #endregion
    }
}