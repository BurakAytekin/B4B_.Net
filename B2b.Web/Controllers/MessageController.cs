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

namespace B2b.Web.v4.Controllers
{
    public class MessageController : BaseController
    {
        List<Messages> MessageList
        {
            get { return (List<Messages>)Session["MessageList"]; }
            set { Session["MessageList"] = value; }
        }

        // GET: Message
        public ActionResult Index()
        {
           ViewBag.CurrentCustomer = CurrentCustomer;
            ViewBag.CurrentCustomerJquery = JsonConvert.SerializeObject(CurrentCustomer);
            ViewBag.CurrentSalesmanOfCustomerJquery = JsonConvert.SerializeObject(CurrentSalesmanOfCustomer);
            ViewBag.CompanyInformationItemJquery = JsonConvert.SerializeObject(CompanyInformationItem);
            return View();
        }

        #region HttpPost Methods

        [HttpPost]
        public JsonResult SendMessage(string header, string content)
        {
            string salesmanIdStr = string.Empty;
            if (CurrentSalesmanOfCustomer.Count > 0)
            {
                salesmanIdStr = CurrentSalesmanOfCustomer.Aggregate(salesmanIdStr, (current, t) => current + ("<" + t.SalesmanId + ">"));
            }
            else
                salesmanIdStr = "<-1>";

            Messages item = new Messages()
            {
                CustomerId = CurrentCustomer.Id,
                UserId = CurrentCustomer.Users.Id,
                SenderId = CurrentEditId,
                Type = MessageSentertype.Customer,
                AddType = CurrentLoginType,
                Header = header,
                Content = content,
                SalesmanId = salesmanIdStr,

            };
            bool result = item.Add();

            MessageBox message = result ? new MessageBox(MessageBoxType.Success, "Kaydınız Alınmıştır") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return Json(message);
        }


        [HttpPost]
        public JsonResult CheckMessageItem(int id, bool value)
        {
            Messages item = MessageList.First(x => x.Id == id);
            item.Checked = value;

            MessageBox message = new MessageBox(MessageBoxType.Success, "Kaydınız Alınmıştır");
            return Json(message);
        }

        [HttpPost]
        public string CheckMessageAll(bool value)
        {
            foreach (Messages item in MessageList)
            {
                item.Checked = value;
            }


            return JsonConvert.SerializeObject(MessageList);
        }


        [HttpPost]
        public string GetMessageList(int messagebox, string searchText)
        {
               MessageList = Messages.GetMessageList(CurrentCustomer.Id, CurrentCustomer.Users.Id, (int)MessageSentertype.Customer, (int)CurrentLoginType, -1, (MessageBoxStatu)messagebox, searchText);

            return JsonConvert.SerializeObject(MessageList);
        } 
        #endregion

    }
}