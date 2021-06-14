using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;

namespace B2b.Web.v4.Controllers
{
    public class AccountController : BaseController
    {
        // GET: Account
        public ActionResult Index()
        {

            ViewBag.CurrentCustomer = CurrentCustomer;
            ViewBag.CurrentCustomerJquery = JsonConvert.SerializeObject(CurrentCustomer);
            ViewBag.AvatarList = GetFileNames(Server.MapPath("/Content/images/Avatar"), "*.PNG");
            return View();
        }

        private string[] GetFileNames(string path, string filter)
        {
            string[] files = Directory.GetFiles(path, filter);
            for (int i = 0; i < files.Length; i++)
                files[i] = Path.GetFileName(files[i]);
            return files;
        }
        #region   HttpPost Methods

        [HttpPost]
        public string GetCouponList(int type)
        {

            List<CouponCs> list = new List<CouponCs>();
            list = CouponCs.GetCouponListByCustomerId(CurrentCustomer.Id, type);
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public JsonResult UpdateCustomerLocation(string latitude, string longitude)
        {
            Customer item = CurrentCustomer;
            item.Users.Latitude = latitude;
            item.Users.Longitude = longitude;
            item.Users.Update();

            CurrentCustomer.Users.Latitude = latitude;
            CurrentCustomer.Users.Longitude = longitude;

            MessageBox message = new MessageBox(MessageBoxType.Success, "Lokasyonunuz Alınmıştır");

            return Json(message);
        }

        [HttpPost]
        public JsonResult UpdatePassword(string oldPassword, string newPassword, string newPasswordRepead)
        {
            MessageBox message;
            if (oldPassword != CurrentCustomer.Users.Password)
            {
                message = new MessageBox(MessageBoxType.Error, "Lütfen Eski Şifrenizi Kontrol Ediniz");
            }
            else if (newPassword != newPasswordRepead)
            {
                message = new MessageBox(MessageBoxType.Error, "Yeni Şifreleriniz Uyuşmamaktadır");
            }
            else
            {
                CurrentCustomer.Users.Password = newPassword;
                CurrentCustomer.UpdateUser();

                message = new MessageBox(MessageBoxType.Success, "Şifreniz güncellenmiştir.");
            }


            return Json(message);
        }

        [HttpPost]
        public JsonResult UpdateCustomerRate(double rate)
        {
            CurrentCustomer.Users.Rate = rate;
            CurrentCustomer.UpdateUser();
            MessageBox message = new MessageBox(MessageBoxType.Success, "Bindirim Oranı Güncellenmiştir.");
            return Json(message);
        }

        [HttpPost]
        public JsonResult SetAvatarName(string avatarName)
        {

            CurrentCustomer.Users.Avatar = avatarName;
            CurrentCustomer.UpdateUser();


            MessageBox message = new MessageBox(MessageBoxType.Success, "Avatar Güncellenmiştir.");
            return Json(message);
        }
        [HttpPost]
        public string GetSearchList(DateTime dateStart, DateTime dateEnd)
        {
            List<LogSearch> list = LogSearch.GetList(CurrentCustomer.Id, CurrentCustomer.Users.Id, (CurrentLoginType == LoginType.Customer ? -1 : CurrentSalesman.Id), dateStart.Date, dateEnd.Date.AddDays(1).AddMinutes(-1), 5000);
            return JsonConvert.SerializeObject(list);
        }
        [HttpPost]
        public string GetSearchDetailList(int id)
        {
            List<LogSearch> list = LogSearch.GetListDetail(id);
            return JsonConvert.SerializeObject(list);
        }
        #endregion
    }
}