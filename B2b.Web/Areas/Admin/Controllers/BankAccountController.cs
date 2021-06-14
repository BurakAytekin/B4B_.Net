using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.Log;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class BankAccountController : AdminBaseController
    {
        // GET: Admin/BankAccount
        public ActionResult Index()
        {
              return View();
        }

        #region HttpPost Methods
        [HttpPost]
        public JsonResult GetBankAccountList()
        {
            List<BankAccount> list = BankAccount.GetBankAccountList();
            return Json(list);
        }

        [HttpPost]
        public JsonResult GetBankList()
        {
            List<BankAccount> list = BankAccount.GetBankList();
            return Json(list);
        }

        [HttpPost]
        public JsonResult UpdateCurrency(int id, string bankName, string branch, string accountNumber, string iban, int bankId)
        {
             bool result = false;
            if (id == 0)
            {
                BankAccount item = new BankAccount()
                {
                    BankName = bankName,
                    Branch = branch,
                    AccountNumber = accountNumber,
                    BankId = bankId,
                    CreateId = AdminCurrentSalesman.Id,
                    Iban = iban

                };
                result = item.Add();
            }
            else
            {
                BankAccount item = new BankAccount()
                {
                    Id = id,
                    EditId = AdminCurrentSalesman.Id,
                    BankName = bankName,
                    Branch = branch,
                    AccountNumber = accountNumber,
                    BankId = bankId,
                    Iban = iban
                };
                result = item.Update();
            }
            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        [HttpPost]
        public JsonResult DeleteBankAccount(int id)
        {
              bool result = false;
            BankAccount item = new BankAccount()
            {
                Id = id,
                EditId = AdminCurrentSalesman.Id
            };
            result = item.Delete();


            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        #endregion
    }
}