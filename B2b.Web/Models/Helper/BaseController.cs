using System.Collections.Generic;
using System.Web.Mvc;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Areas.Admin.Models;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Data;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;
using B2b.Web.v4.Models.Log;

namespace B2b.Web.v4.Models.Helper
{


    public class BaseController : Controller
    {
        #region Sessions
        protected LoginType CurrentLoginType
        {
            get { return Session["LoginType"] == null ? LoginType.None : (LoginType)Session["LoginType"]; }
            set { Session["LoginType"] = value; }
        }

        protected Customer CurrentCustomer
        {
            get { return (Customer)Session["Customer"]; }
            set { Session["Customer"] = value; }
        }

        protected Salesman CurrentSalesman
        {
            get { return (Salesman)Session["Salesman"]; }
            set { Session["Salesman"] = value; }
        }

        protected List<SalesmanOfCustomer> CurrentSalesmanOfCustomer
        {
            get { return (List<SalesmanOfCustomer>)Session["SalesmanOfCustomer"]; }
            set { Session["SalesmanOfCustomer"] = value; }
        }

        protected CompanyInformation CompanyInformationItem
        {
            get { return (CompanyInformation)Session["CompanyInformationItem"]; }
            set { Session["CompanyInformationItem"] = value; }
        }
        protected B2bRule B2BRuleItem
        {
            get { return (B2bRule)Session["B2BRuleItem"]; }
            set { Session["B2BRuleItem"] = value; }
        }

        protected FooterInformation FooterInformationItem
        {
            get { return (FooterInformation)Session["FooterInformationItem"]; }
            set { Session["FooterInformationItem"] = value; }
        }

        protected List<Currency> CurrencyList
        {
            get { return (List<Currency>)Session["CurrencyList"]; }
            set { Session["CurrencyList"] = value; }
        }


        public int CurrentSalesmanId { get { return (CurrentLoginType == LoginType.Salesman ? CurrentSalesman.Id : -1); } }
        public int CurrentEditId { get { return (CurrentLoginType == LoginType.Salesman ? CurrentSalesman.Id : CurrentCustomer.Users.Id); } }
        #endregion

        #region Methods

        public string MD5Sifreleme(string Text)
        {
            MD5CryptoServiceProvider MD5Code = new MD5CryptoServiceProvider();
            byte[] byteDizisi = Encoding.UTF8.GetBytes(Text);
            byteDizisi = MD5Code.ComputeHash(byteDizisi);
            StringBuilder sb = new StringBuilder();
            foreach (byte ba in byteDizisi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }
            return sb.ToString();
        }


        public string GetUserIpAddress()
        {
            string ip = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
                ip = HttpContext.Request.ServerVariables["REMOTE_ADDR"];
            return ip;
        }

        public string GetControllerName()
        {

            return this.ControllerContext.RouteData.Values["controller"] + "=>" + this.ControllerContext.RouteData.Values["action"];
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {


            base.OnActionExecuting(filterContext);
            string loginPagePath = "/Login/";
            if (CurrentLoginType != LoginType.None)
            {
                Session["SystemRoles"] = CurrentLoginType.ToString();
                if (CurrentLoginType == LoginType.Customer && this.CurrentCustomer == null)
                    ContextRedirectToAciton(filterContext, loginPagePath);
                if (CurrentLoginType == LoginType.Salesman && this.CurrentSalesman == null)
                    ContextRedirectToAciton(filterContext, loginPagePath);




                if (Session["SalesmanOfCustomer"] == null)
                {
                    if (this.CurrentCustomer != null)
                    {
                        CurrentSalesmanOfCustomer = SalesmanOfCustomer.GetSalesmanOfCustomer(CurrentCustomer.Id);
                        ViewBag.CurrentSalesmanOfCustomer = CurrentSalesmanOfCustomer;

                    }
                    else
                    {
                        ContextRedirectToAciton(filterContext, loginPagePath);
                    }

                }

                if (B2BRuleItem != null)
                {
                    if (B2BRuleItem.Maintenance)
                    {
                        filterContext.Result = new RedirectResult("/Login/Maintenance");
                        return;
                    }
                }

                if (this.CurrentCustomer != null)
                {
                    ViewBag.CurrentCustomer = this.CurrentCustomer;
                }
                if (FooterInformationItem == null)
                {
                    FooterInformationItem = FooterInformation.GetFooterItem();
                    ViewBag.FooterInformationItem = FooterInformationItem;
                }
                else
                    ViewBag.FooterInformationItem = FooterInformationItem;

                if (CurrencyList == null || CurrencyList.Count == 0)
                {
                    GetActualCurrencies();
                }

                Logger.LogNavigation(CurrentCustomer.Id, CurrentCustomer.Users.Id, CurrentSalesmanId,
          GetControllerName(), ClientType.B2BWeb, GetUserIpAddress());


            }
            else
            {
                ContextRedirectToAciton(filterContext, loginPagePath);

            }
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {

            if (filterContext.HttpContext.Session["LoginType"] != null &&
                LoginType.Salesman == ((LoginType)filterContext.HttpContext.Session["LoginType"]))
            {
                if (filterContext.HttpContext.Session["Salesman"] == null)
                {
                    filterContext.Result = new RedirectResult("/Login/Logout");
                }
            }
            else if (filterContext.HttpContext.Session["LoginType"] != null &&
                     LoginType.Customer == ((LoginType)filterContext.HttpContext.Session["LoginType"]))
            {
                if (filterContext.HttpContext.Session["Customer"] == null)
                {
                    filterContext.Result = new RedirectResult("/Login/Logout");
                }
            }
            else
            {

                filterContext.Result = new RedirectResult("/Login/Logout");
            }

        }
        protected void ContextRedirectToAciton(ActionExecutingContext filterContext, string path)
        {
            filterContext.Result = new RedirectResult(path);

        }
        protected void GetActualCurrencies()
        {
            CurrencyList = Currency.GetList();
        }



        #endregion
    }


    public class MessageBox
    {
        public string Statu { get; set; }
        public string Message { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public int ResultId { get; set; }
        public MessageBox(MessageBoxType statu, string message, int resultId = -1)
        {
            switch (statu)
            {
                case MessageBoxType.Error:
                    Statu = "error";
                    Color = "error";
                    Icon = "fa fa-ban";
                    break;
                case MessageBoxType.Info:
                    Statu = "info";
                    Color = "info";
                    Icon = "fa fa-info-circle";
                    break;
                case MessageBoxType.Success:
                    Statu = "success";
                    Color = "success";
                    Icon = "fa fa-check";
                    break;
                case MessageBoxType.Warning:
                    Statu = "warning";
                    Color = "warning";
                    Icon = "fa fa-exclamation-triangle";
                    break;
                case MessageBoxType.Payment:
                    Statu = "payment";
                    Color = "warning";
                    Icon = "fa fa-exclamation-triangle";
                    break;

            }
            Message = message;
            ResultId = resultId;

        }


    }


    #region Enums

    public enum MessageBoxType
    {
        Success,
        Error,
        Warning,
        Info,
        Payment

    }
    public enum Language
    {
        TR,
        RU,
        EN,
        AZ,
        AR,
        DE,
        FR
    }

    public enum LoginType
    {
        Customer = 0,
        Salesman = 1,
        Admin = 2,
        None = 99,
    }

    public enum SystemType
    {
        Android = 3,
        Web = 1,
        WinForm = 2
    }

    public enum AnnouncementsType
    {
        Article = 0,
        Picture = 1,
        Campaign = 2,
        VisualOpeningMessage = 3,
        GeneralOpeningMessage = 4,
        SalesmanOpeningMessage = 5,
        MenuFloatingWriting = 6,
        VirtualPosAnnouncement = 7,
        VirtualPosPicture = 8,
        Brands = 9,
        SearchPageBanner = 10,
        ShipmentType = 11,
        LoginPictures = 12
    }

    public enum PosBanks
    {
        Garanti = 1,
        Akbank = 2,
        IsBank = 3,
        Finansbank = 4,
        Halkbank = 5,
        Anadolubank = 6,
        Turkiyefinans = 7,
        Teb = 8,
        Yapikredi = 9,
        Kuveytturk = 10,
        Hsbc = 12,
        Vakifbank = 13,
        Ziraatbank = 14,
        Denizbank = 15,
        Ingbank = 16,
        Sekerbank = 17,
        Albaraka = 18,
        IsBankInnova = 19,
        ZiraatbankInnova = 20,
        QNBFinansbank = 21
    }
    public enum Bank
    {
        ISBANK = 30,
        AKBANK = 22,
        YAPIKREDI = 27,
        TEB = 32,
        FINANSBANK = 1,
        HALKBANK = 11,
        GARANTIBANK = 34,
        ZIRAATBANK = 31,
        DENIZBANK = 35,
        VAKIFBANK = 26,
        SEKERBANK = 35,
        ING = 9,
        HSBC = 8,
        TURKIYEFINANS = 23,
        BELIRSIZ = 99
    }

    public enum _3DSecureType
    {
        OnlyNonSecure = 0,
        Only3D = 1,
        NonSecureOr3D = 2,
        Customer3DandSalesmanNonsecure = 3

    }

    public enum TransactionType
    {
        Sale = 1,
        PointSearch = 2,
        PointSale = 3,
        Cancel = 4,
        Refund = 5
    }
    #endregion
}