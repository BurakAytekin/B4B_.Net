using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using B2b.Web.v4.Areas.Admin.Models.Security;

namespace B2b.Web.v4.Controllers
{
    public class LoginController : Controller
    {
        private string ip
        {
            get
            {
                string ipValue = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipValue))
                    ipValue = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                return ipValue;
            }
        }

        public ActionResult sLogin()
        {
            Session["SalesmanOfCustomer"] = null;
            Session["FooterInformationItem"] = null;
            Session["ProductOfDayList"] = null;
            Session["NotificationsList"] = null;
            Session["LoginAnnouncements"] = Announcements.GetAllByType(AnnouncementsType.LoginPictures);
            Session["B2BRuleItem"] = B2bRule.GetGeneralRuleList();
            Session["captchaControl"] = "false";

            if (Session["CompanyInformationItem"] == null || Session["CompanyInformationItem"].ToString() == string.Empty)
            {
                CompanyInformation CompanyInformationItem = CompanyInformation.GetByStatus(0);
                Session["CompanyInformationItem"] = CompanyInformationItem == null ? new CompanyInformation() : CompanyInformationItem;
            }
           
            if (Request.QueryString["Session"] != null)
            {
                Guid sId = Guid.Parse(Request.QueryString["Session"]);
                Session session = new Session();
                session.GetItem(sId);
                session.Delete(); // veri tabanındakini siler

                if (session.LoginId != -1)
                {
                    LoginType CurrentLoginType = session.Type;
                    HttpContext.Session["LoginType"] = CurrentLoginType;
                    if (session.Type == LoginType.Customer)
                    {
                        Customer customer = Customer.GetById(session.LoginId, session.UserId);
                        customer.TerminalNo = session.TerminalNo;
                        if (customer != null && customer.Id > 0)
                        {
                            customer.LoginTime = DateTime.Now;
                            customer.LoginIp = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                            if (string.IsNullOrEmpty(customer.LoginIp))
                                customer.LoginIp = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                            HttpContext.Session["Customer"] = customer;
                            AddLoginCookie(customer.Id, customer.Users.Id, CurrentLoginType);
                            AddUserNameCokkie(customer.Code, customer.Users.Code);
                            string startScreen = customer.IsStartScreen ? customer.StartScreen : (Session["B2BRuleItem"] as B2bRule).StartScreen;

                            return RedirectToAction("Index", startScreen);
                        }
                    }
                    else if (session.Type == LoginType.Salesman)
                    {
                        Salesman CurrentSalesman = Salesman.GetById(session.LoginId);
                        if (CurrentSalesman != null && CurrentSalesman.Id > 0)
                        {
                            CurrentSalesman.LoginTime = DateTime.Now;
                            CurrentSalesman.LoginIp = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                            if (string.IsNullOrEmpty(CurrentSalesman.LoginIp))
                                CurrentSalesman.LoginIp = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                            HttpContext.Session["Salesman"] = CurrentSalesman;
                            HttpContext.Session["LoginType"] = CurrentLoginType;
                            AddLoginCookie(CurrentSalesman.Id, -1, CurrentLoginType);
                            AddUserNameCokkie(CurrentSalesman.Code, "");
                            return Redirect("/Login/CustomerSelect");
                        }
                    }
                    else if (session.Type == LoginType.Admin)
                    {
                        Salesman CurrentSalesman = Salesman.GetById(session.LoginId);
                        Session["AdminSalesman"] = CurrentSalesman;
                        List<AuthorityGroup> authorityList = AuthorityGroup.GetSalesmanAuthority(CurrentSalesman.Id);
                        Session["AuthoritySalesman"] = authorityList;
                        //Log Atılacak
                        return Redirect("/Admin");
                    }
                }
            }
            return View();
        }

        [MaintenanceFilter]
        public ActionResult Index()
        {
            Session["SalesmanOfCustomer"] = null;
            Session["FooterInformationItem"] = null;
            Session["ProductOfDayList"] = null;
            Session["NotificationsList"] = null;
            Session["LoginAnnouncements"] = Announcements.GetAllByType(AnnouncementsType.LoginPictures);
            Session["B2BRuleItem"] = B2bRule.GetGeneralRuleList();
            Session["captchaControl"] = "false";

            if (Session["CompanyInformationItem"] == null || Session["CompanyInformationItem"].ToString() == string.Empty)
            {
                CompanyInformation CompanyInformationItem = CompanyInformation.GetByStatus(0);
                Session["CompanyInformationItem"] = CompanyInformationItem == null ? new CompanyInformation() : CompanyInformationItem;
            }
           
            if (Request.Cookies[GlobalSettings.CookieName] != null)
            {
                HttpCookie cookie = Request.Cookies.Get(GlobalSettings.CookieName);
                if (cookie != null)
                {
                    LoginType CurrentLoginType = (LoginType)(Enum.Parse(typeof(LoginType), cookie.Values["LoginType"]));
                    HttpContext.Session["LoginType"] = CurrentLoginType;
                    if (CurrentLoginType == LoginType.Customer)
                    {
                        int id = CheckIdHash(cookie.Values["Id"]);
                        int userId = CheckIdHash(cookie.Values["UserId"]);

                        if (id == -1)
                        {
                            return RedirectToAction("Logout");
                        }
                        Customer customer = Customer.GetById(id, userId);
                        if (customer != null && customer.Id > 0 && (Session["B2BRuleItem"] as B2bRule).CustomerWebLogin && customer.AuthorityCustomer._WebLogin)
                        {
                            Logger.LogTransaction(ClientType.B2BWeb, LogTransactionSource.Login, ProcessLogin.Success.ToString(), string.Empty, ip, -1, customer.Id, customer.Users.Id);
                            customer.LoginTime = DateTime.Now;
                            customer.LoginIp = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                            if (string.IsNullOrEmpty(customer.LoginIp))
                                customer.LoginIp = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                            HttpContext.Session["Customer"] = customer;
                            string startScreen = customer.IsStartScreen ? customer.StartScreen : (Session["B2BRuleItem"] as B2bRule).StartScreen;

                            return RedirectToAction("Index", startScreen);
                        }
                        else
                        {
                            return RedirectToAction("Logout");
                        }
                    }
                    else
                    {
                        int id = CheckIdHash(cookie.Values["Id"]);
                        if (id == -1)
                        {
                            return RedirectToAction("Logout");
                        }
                        Salesman CurrentSalesman = Salesman.GetById(id);
                        if (CurrentSalesman != null && CurrentSalesman.Id > 0 && (Session["B2BRuleItem"] as B2bRule).SalesmanWebLogin)
                        {
                            Logger.LogTransaction(ClientType.B2BWeb, LogTransactionSource.Login, ProcessLogin.Success.ToString(), string.Empty, ip, -1, -1, -1, CurrentSalesman.Id, -1);
                            CurrentSalesman.LoginTime = DateTime.Now;
                            CurrentSalesman.LoginIp = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                            if (string.IsNullOrEmpty(CurrentSalesman.LoginIp))
                                CurrentSalesman.LoginIp = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                            HttpContext.Session["Salesman"] = CurrentSalesman;
                            return RedirectToAction("CustomerSelect");
                        }
                        else
                        {
                            return RedirectToAction("Logout");
                        }
                    }
                }
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(Logon model)
        {
            if (ModelState.IsValid)
            {
                if (Session["captchaControl"].ToString() == "true" && Session["captcha"] != null &&
                    model.Captcha != Session["captcha"].ToString())
                {
                    ModelState.AddModelError("", "Doğrulama Kodunu Hatalı Girdiniz.");
                    TempData["Alert"] = "true";
                    Session["captchaControl"] = "true";
                }
                else
                {
                    LoginType CurrentLoginType = new LoginType();

                    CurrentLoginType = String.IsNullOrEmpty(model.CustomerCode)
                        ? LoginType.Salesman
                        : LoginType.Customer;

                    if (CurrentLoginType == LoginType.Customer)
                    {
                        Logon logon = new Logon();
                        logon.CustomerCode = model.CustomerCode;
                        logon.UserCode = model.UserCode;
                        logon.Password = model.Password;


                        Users checkUser = logon.UserAuthenticationCheck();

                        if (checkUser != null && checkUser.Id > 0)
                        {
                            bool authenticated = true;

                            if (checkUser.IsAuthenticator)
                            {
                                if (model.Password.Length > 6)
                                {
                                    string password = model.Password.Substring(model.Password.Length - 6, 6);
                                    logon.Password = logon.Password.Replace(password, "");
                                    TwoFactorAuthenticator tfA = new TwoFactorAuthenticator();
                                    var result = tfA.ValidateTwoFactorPIN(checkUser.AuthenticatorGuid, password);

                                    authenticated = result;
                                }
                                else
                                    authenticated = false;

                            }


                            Customer customer = logon.CustomerLogin();
                            if (customer != null && customer.Id != 0 && authenticated)
                            {
                                if (!(Session["B2BRuleItem"] as B2bRule).CustomerWebLogin || !customer.AuthorityCustomer._WebLogin)
                                {
                                    ModelState.AddModelError("", "Web Giriş Yetkilerinizi Kontrol Ediniz");
                                    TempData["Alert"] = "true";

                                }
                                else
                                {
                                    Logger.LogTransaction(ClientType.B2BWeb, LogTransactionSource.Login, ProcessLogin.Success.ToString(), string.Empty, ip, -1, customer.Id, customer.Users.Id);

                                    AddLoginCookie(customer.Id, customer.Users.Id, CurrentLoginType);
                                    AddUserNameCokkie(logon.CustomerCode, logon.UserCode);
                                    HttpContext.Session["Customer"] = customer;
                                    HttpContext.Session["LoginType"] = CurrentLoginType;
                                    string startScreen = customer.IsStartScreen ? customer.StartScreen : (Session["B2BRuleItem"] as B2bRule).StartScreen;

                                    return Redirect("/" + startScreen);
                                }

                            }
                            else
                            {
                                string failMessage = "Kullanıcı(User) Kodu :" + logon.UserCode + "    Şifre:" + logon.Password;
                                Logger.LogTransaction(ClientType.B2BWeb, LogTransactionSource.Login, ProcessLogin.Fail.ToString(), failMessage, ip);

                                ModelState.AddModelError("", "Kullanıcı Adı Veya Şifre Hatalı");
                                TempData["Alert"] = "true";
                                Session["captchaControl"] = "true";
                            }

                        }
                        else
                        {
                            string failMessage = "Kullanıcı Kodu :" + logon.UserCode + "    Şifre:" + logon.Password;
                            Logger.LogTransaction(ClientType.Admin, LogTransactionSource.Login, ProcessLogin.Fail.ToString(), failMessage, ip, -1, -1, -1, checkUser == null ? -1 : checkUser.Id, -1);

                            ModelState.AddModelError("", "Kullanıcı Adı ve / veya Şifre Hatalı");
                            //Log Atılacak
                            TempData["Alert"] = "true";
                        }

                    }
                    else
                    {
                        Logon logon = new Logon();
                        logon.UserCode = model.UserCode;
                        logon.Password = model.Password;

                        Salesman checkSalesman = logon.AdminSalesmanAuthenticationCheck();

                        if (checkSalesman != null && checkSalesman.Id > 0)
                        {
                            bool authenticated = true;

                            if (checkSalesman.IsB2bAuthenticator)
                            {
                                if (model.Password.Length > 6)
                                {
                                    string password = model.Password.Substring(model.Password.Length - 6, 6);
                                    logon.Password = logon.Password.Replace(password, "");
                                    TwoFactorAuthenticator tfA = new TwoFactorAuthenticator();
                                    var result = tfA.ValidateTwoFactorPIN(checkSalesman.AuthenticatorGuid, password);

                                    authenticated = result;
                                }
                                else
                                    authenticated = false;

                            }

                            Salesman salesman = logon.SalesmanLogin();
                            if (salesman != null && salesman.Id != 0 && authenticated)
                            {
                                if (!(Session["B2BRuleItem"] as B2bRule).SalesmanWebLogin || !salesman.AuthoritySalesman.WebLogin)
                                {
                                    ModelState.AddModelError("", "Web Giriş Yetkilerinizi Kontrol Ediniz");
                                    TempData["Alert"] = "true";

                                }
                                else
                                {
                                    Logger.LogTransaction(ClientType.B2BWeb, LogTransactionSource.Login, ProcessLogin.Success.ToString(), string.Empty, ip, -1, -1, -1, salesman.Id, -1);

                                    AddLoginCookie(salesman.Id, -1, CurrentLoginType);
                                    AddUserNameCokkie(logon.UserCode, "");
                                    HttpContext.Session["Salesman"] = salesman;
                                    HttpContext.Session["LoginType"] = CurrentLoginType;
                                    return Redirect("/Login/CustomerSelect");
                                }

                            }
                            else
                            {
                                string failMessage = "Kullanıcı Kodu :" + logon.UserCode + "    Şifre:" + logon.Password;
                                Logger.LogTransaction(ClientType.B2BWeb, LogTransactionSource.Login, ProcessLogin.Fail.ToString(), failMessage, ip);

                                ModelState.AddModelError("", "Kullanıcı Adı Veya Şifre Hatalı");
                                TempData["Alert"] = "true";
                                Session["captchaControl"] = "true";
                            }


                        }
                        else
                        {
                            string failMessage = "Kullanıcı Kodu :" + logon.UserCode + "    Şifre:" + logon.Password;
                            Logger.LogTransaction(ClientType.Admin, LogTransactionSource.Login, ProcessLogin.Fail.ToString(), failMessage, ip, -1, -1, -1, checkSalesman == null ? -1 : checkSalesman.Id, -1);

                            ModelState.AddModelError("", "Kullanıcı Adı ve / veya Şifre Hatalı");
                            //Log Atılacak
                            TempData["Alert"] = "true";
                        }

                    }

                }
            }
            else
            {
                TempData["Alert"] = "true";
                Session["captchaControl"] = "true";
            }

            return View();
        }

        #region AddCookie
        protected void AddLoginCookie(int id, int userId, LoginType CurrentLoginType)
        {
            HttpCookie eryazCookie = new HttpCookie(GlobalSettings.CookieName);
            eryazCookie.Values.Add("LoginType", ((int)CurrentLoginType).ToString());
            eryazCookie.Values.Add("Id", GenerateIdHash(id) + "-" + id.ToString());
            eryazCookie.Values.Add("UserId", GenerateIdHash(userId) + "-" + userId.ToString());
            eryazCookie.Expires = DateTime.Now.AddDays(15);
            Response.Cookies.Add(eryazCookie);
        }

        protected void AddUserNameCokkie(string customerCode, string userName)
        {
            HttpCookie eryazCookie = new HttpCookie(GlobalSettings.CookieNameUserName);
            eryazCookie.Values.Add("CustomerCode", customerCode);
            eryazCookie.Values.Add("UserName", userName);
            eryazCookie.Expires = DateTime.Now.AddDays(365);
            Response.Cookies.Add(eryazCookie);
        }
        #region Private Methods
        byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
        #endregion
        protected string GenerateIdHash(int id)
        {
            return GetHashString("ERYAZ" + id.ToString() + "eratech");
        }

        protected int CheckIdHash(string cokkieStr)
        {
            int id = -1;
            string[] arr = cokkieStr.Split('-');

            if (arr.Length == 2)
            {
                if (Int32.TryParse(arr[1], out id))
                {
                    if (arr[0] != GenerateIdHash(id))
                        id = -1;
                }
            }
            return id;
        }
        #endregion


        public ActionResult PasswordReset()
        {
            if (Request.QueryString["parameter"] == null)
                return Redirect("/Login/Logout");

            string parameter = Request.QueryString["parameter"].ToString();

            PasswordResetCs resetItem = PasswordResetCs.CheckPasswordResetGuid(parameter);
            if (resetItem == null || resetItem.Id == 0)
                return Redirect("/Login/Logout");

            ViewBag.PasswordResetItem = resetItem;


            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PasswordReset(PasswordResetCs model)
        {

            if (model.Password != model.PasswordRepeat)
            {
                ModelState.AddModelError("", "Şifreler uyuşmamaktadır");
                TempData["Alert"] = "true";
            }
            else
            {
                if (Request.QueryString["parameter"] == null)
                    return Redirect("/Login/Logout");

                string parameter = Request.QueryString["parameter"].ToString();

                PasswordResetCs resetItem = PasswordResetCs.CheckPasswordResetGuid(parameter);
                if (resetItem == null || resetItem.Id == 0)
                    return Redirect("/Login/Logout");

                switch (resetItem.Type)
                {
                    case PasswordResetType.User:
                        Users item = new Users()
                        {
                            Id = resetItem.PersonId,
                            Password = model.Password
                        };
                        item.UpdatePassword();
                        break;
                    case PasswordResetType.FinancePassword:
                        Customer.UpdateCustomerCurrentAccountPassword(resetItem.PersonId, model.Password);

                        break;
                    case PasswordResetType.Salesman:
                        Salesman.UpdateSalesmanPassword(resetItem.PersonId, model.Password);
                        break;
                    default:
                        return Redirect("/Login/Logout");

                }
                resetItem.Status = 1;
                resetItem.Update();
                return Redirect("/Login/Logout");
            }





            return View();
        }


        public ActionResult Unauthorized()
        {

            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            if (Request.Cookies[GlobalSettings.CookieName] != null)
            {
                HttpCookie myCookie = new HttpCookie(GlobalSettings.CookieName);
                myCookie.Expires = DateTime.Now.AddDays(-20);
                Response.Cookies.Add(myCookie);
            }


            return RedirectToAction("Index", "Login");

        }


        public ActionResult CustomerSelect(string CustomerId, string UserId)
        {
            Session["SalesmanOfCustomer"] = null;
            Session["HomeCampaignList"] = null;
            Session["ProductOfDayList"] = null;
            Session["NotificationsList"] = null;

            if (HttpContext.Session["Salesman"] == null)
                return Redirect("/Login/");
            if (!string.IsNullOrEmpty(CustomerId) && !string.IsNullOrEmpty(UserId))
            {
                int id = Convert.ToInt32(CustomerId);
                int userId = Convert.ToInt32(UserId);
                //Log atılacak
                Customer currentCustomer = Customer.GetById(id, userId);
                if (currentCustomer.Id != 0)
                {
                    HttpContext.Session["Customer"] = currentCustomer;
                    string startScreen = currentCustomer.IsStartScreen ? currentCustomer.StartScreen : (Session["B2BRuleItem"] as B2bRule).StartScreen;

                    return RedirectToAction("Index", startScreen);
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }

        }

        #region HttpPost Methods

        [HttpPost]
        public JsonResult GetCustomerSelectData(string codeOrName, string city, string town, bool basketType, int count)
        {
            try
            {
                if (string.IsNullOrEmpty(city) || city == "Seçiniz")
                    city = "*";
                if (string.IsNullOrEmpty(town) || town == "Seçiniz")
                    town = "*";

                Salesman salesman = Session["Salesman"] as Salesman;
                List<CustomerSmall> customerList = new List<CustomerSmall>();
                Logger.LogTransaction(ClientType.B2BWeb, LogTransactionSource.CustomerSelect, ProcessLogin.SelectCustomer.ToString(), String.Empty, ip, -1, -1, -1, salesman.Id, -1);
                customerList = Customer.GetListLimited(codeOrName ?? string.Empty, basketType ? 1 : 0, city, town, salesman.AuthoritySalesman.CustomerType, salesman.Id, count, 25);
                return Json(customerList);

            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.B2BWeb, "GetCustomerSelectData", ex, String.Empty);
                return Json(null);
            }

        }

        [HttpPost]
        public JsonResult GetCityList()
        {
            List<CustomerSmall> cityList = Customer.GetCityList();
            cityList.Insert(0, new CustomerSmall() { City = "Seçiniz" });
            return Json(cityList);
        }
        [HttpPost]
        public JsonResult GetTownList(string city)
        {
            List<CustomerSmall> townList = Customer.GetTownList(city);
            townList.Insert(0, new CustomerSmall() { Town = "Seçiniz" });
            return Json(townList);
        }

        [HttpPost]
        public JsonResult GetUserList(int id)
        {
            List<Users> list = Users.GetUserListByCustomerId(id);
            return Json(list);
        }
        #endregion
        public CaptchaCreate CaptchaCreate()
        {
            return new CaptchaCreate();
        }


        public ActionResult CustomerSelectDetail(string customerCode, int customerId)
        {

            Dashboard dashboardItem = Dashboard.GetCustomerDetailCalculate(customerId);
            Salesman salesman = Session["Salesman"] as Salesman;
            Customer customer = Customer.GetById(customerId, -1);

            if (salesman.AuthoritySalesman.HidePassword)
            {
                dashboardItem.Customer.Password = "****";
            }

            var basket = Basket.GetBasketList(customer, LoginType.Customer, -1, 0);
            basket.AddRange(Basket.GetBasketList(customer, LoginType.Salesman, -1, 1));
            dashboardItem.BasketTotal = CalculateBasketTotals(basket, customer);
            dashboardItem.Rule = Dashboard.GetCustomerDetailDiscoun(customer.Id, customer.RuleCode);
            return PartialView("CustomerSelectDetail", dashboardItem);
        }
        private double CalculateBasketTotals(List<Basket> basketList, Customer CurrentCustomer = null)
        {
            double total = 0;
            foreach (var basket in basketList)
            {
                basket.Product.CalculateDetailInformation(basket.IsCancelCampaign, (CurrentCustomer.CampaignStatu && CurrentCustomer.Users.AuthorityUser._CampaignStatu), basket.Quantity, basket.DiscSpecial, basket.IsFixedPrice, basket.FixedPrice, basket.FixedCurrency, basket.FixedCurrencyRate, 0, false);
                if (basket.Checked)
                {
                    //TotalPriceCustomerCurrency += basket.TotalPrice * basket.Product.PriceCurrencyRate;
                    //TotalDiscountCustomerCurrency += basket.TotalDiscount * basket.Product.PriceCurrencyRate;
                    //TotalCostCustomerCurrency += basket.TotalCost * basket.Product.PriceCurrencyRate;
                    //TotalVATCustomerCurrency += basket.TotalVAT * basket.Product.PriceCurrencyRate;

                    total += (basket.TotalCost * basket.Product.PriceCurrencyRate) +
                             (basket.TotalVAT * basket.Product.PriceCurrencyRate);
                }
            }


            return Convert.ToDouble(total.ToString("N4"));

        }


        public ActionResult Maintenance()
        {
            return View();
        }

    }



}