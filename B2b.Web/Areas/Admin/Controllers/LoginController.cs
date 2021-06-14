using System;
using System.Web.Mvc;
using System.Web.Security;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Log;
using System.Collections.Generic;
using B2b.Web.v4.Areas.Admin.Models.Security;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class LoginController : Controller// AdminBaseController
    {
        public string ip
        {
            get
            {

                string ipValue = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (string.IsNullOrEmpty(ipValue))
                    ipValue = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
                return ipValue;

            }
        }
        // GET: Admin/Login
        public ActionResult Index()
        {
            if (Session["CompanyInformationItem"] == null || Session["CompanyInformationItem"].ToString() == string.Empty)
            {
                CompanyInformation CompanyInformationItem = CompanyInformation.GetByStatus(0);
                Session["CompanyInformationItem"] = CompanyInformationItem ?? new CompanyInformation();
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
                Logon logon = new Logon();
                logon.UserCode = model.UserCode;
                logon.Password = model.Password;
                Salesman checkSalesman = logon.AdminSalesmanAuthenticationCheck();

                if (checkSalesman != null && checkSalesman.Id > 0)
                {
                    bool authenticated = true;

                    if (checkSalesman.IsAuthenticator)
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

                    Salesman salesman = logon.AdminSalesmanLogin();
                    if (salesman != null && salesman.Id > 0 && authenticated)
                    {

                        Logger.LogTransaction(ClientType.Admin, LogTransactionSource.Login, ProcessLogin.Success.ToString(), "", ip, -1, -1, -1, salesman.Id, -1);

                        Session["AdminSalesman"] = salesman;
                        List<AuthorityGroup> authorityList = AuthorityGroup.GetSalesmanAuthority(salesman.Id);
                        Session["AuthoritySalesman"] = authorityList;
                        //Log Atılacak
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        string failMessage = "Kullanıcı Kodu :" + logon.UserCode + "    Şifre:" + logon.Password;
                        Logger.LogTransaction(ClientType.Admin, LogTransactionSource.Login, ProcessLogin.Fail.ToString(), failMessage, ip, -1, -1, -1, salesman == null ? -1 : salesman.Id, -1);

                        ModelState.AddModelError("", "Kullanıcı Adı ve / veya Şifre Hatalı");
                        //Log Atılacak
                        TempData["Alert"] = "true";
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
            else
            {
                TempData["Alert"] = "true";
            }
            return View();

        }


        public ActionResult UnAuthorized()
        {
            Salesman salesman = Session["AdminSalesman"] as Salesman;
            if (salesman == null)
                return RedirectToAction("Logout", "Login");
            string ip = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Request.ServerVariables["REMOTE_ADDR"];

            Logger.LogTransaction(ClientType.Admin, LogTransactionSource.UnAuthorized, "Yetkisiz Erişim", salesman.Code + " kondlu kullanıcı yetkisiz erişim denemesinde bulunmuştur", ip, -1, -1, -1, salesman.Id, 0, 0);

            Session["AdminSalesman"] = salesman;
            ViewBag.AdminCurrentSalesman = salesman;
            return View();
        }

        public ActionResult Locked()
        {
            Salesman salesman = Session["AdminSalesman"] as Salesman;
            if(salesman == null)
                return RedirectToAction("Logout", "Login");

            salesman.Locked = true;
            Session["AdminSalesman"] = salesman;
            ViewBag.AdminCurrentSalesman = salesman;
            return View();
        }

        [HttpPost]
        public ActionResult Locked(string txtPassword)
        {
            Salesman salesman = Session["AdminSalesman"] as Salesman;
            if (salesman.Password == txtPassword)
            {
                salesman.TryCount = 0;
                salesman.Locked = false;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                salesman.Locked = true;
                salesman.TryCount = salesman.TryCount + 1;
                if(salesman.TryCount == 3)
                    return RedirectToAction("Logout", "Login");

            }
           
            Session["AdminSalesman"] = salesman;
            ViewBag.AdminCurrentSalesman = salesman;
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session["AdminSalesman"] = null;
            //Session.Abandon();
            //Session.Clear();
            return RedirectToAction("Index", "Login");
        }
    }
}