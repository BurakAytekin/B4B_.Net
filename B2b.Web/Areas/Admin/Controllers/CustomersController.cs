using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System.Net.Mail;
using B2b.Web.v4.Areas.Admin.Models.Security;
using System.Net;
using System.IO;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class CustomersController : AdminBaseController
    {

        Customer customer = new Customer();

        // GET: Admin/Customers
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult CustomersSelect()
        {
            return PartialView();
        }
        public PartialViewResult UserDetail()
        {
            return PartialView();
        }

        public PartialViewResult RuleSelect()
        {
            return PartialView();
        }
        #region   HttpPost Methods
        [HttpPost]
        public string GetCustomerRuleList()
        {
            List<Rule> rule = Rule.GetRuleCustomerList();
            return JsonConvert.SerializeObject(rule);
        }

        [HttpPost]
        public string GetCustomerLicence(int customerId)
        {
            List<Licence> list = Licence.GetLicenceByUserId(customerId, 0);
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string AddCustomer(Customer customer)
        {
             customer.CreateId = AdminCurrentSalesman.Id;
            if (customer.Salesman == null)
                customer.Salesman = new Salesman();
            var id = Convert.ToInt32(customer.Add().Rows[0][0]);
            return JsonConvert.SerializeObject(id);
        }
        [HttpPost]
        public string UpdateCustomer(Customer customer)
        {
             customer.EditId = AdminCurrentSalesman.Id;
            if (customer.Salesman == null)
                customer.Salesman = new Salesman();
            var status = customer.Update() ? 1 : -1;
            return JsonConvert.SerializeObject(status);
        }
        [HttpPost]
        public string DeleteCustomer(Customer customer)
        {
            customer.EditId = AdminCurrentSalesman.Id;

            bool result = customer.Delete();
            var messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }

        [HttpPost]
        public string GetCustomerSearch(string code, string name, string ruleCode)
        {
            code = code ?? string.Empty;
            name = name ?? string.Empty;
            ruleCode = ruleCode ?? string.Empty;
            List<Customer> customer = Customer.GetCustomerListBySearch(code, name, ruleCode);
            return JsonConvert.SerializeObject(customer);
        }

        [HttpPost]
        public string GetCustomerInstallment(int customerId)
        {
            List<CustomerInstallment> customerInstallmentList = CustomerInstallment.GetCustomerInstallmentsByCustomerId(customerId);
            return JsonConvert.SerializeObject(customerInstallmentList);
        }


        [HttpPost]
        public string GetCustomerUserList(int customerId)
        {
            List<Users> list = Users.GetUserListByCustomerId(customerId);
            return JsonConvert.SerializeObject(list);
        }
        [HttpPost]
        public string GetCustomerLoginLog(int customerId, int userId)
        {
            List<LogTransaction> list = LogTransaction.GetListCustomerLogin(customerId, userId);
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string GetAuthorityCustomer(int id)
        {
            Aut_CustomerTuple item = AuthorityCustomer.GetAuthorityCustomer(id);
            return JsonConvert.SerializeObject(item);
        }

        [HttpPost]
        public string GetAuthorityUser(int id)
        {
            Aut_UserTuple item = AuthorityUser.GetAuthorityUser(id);
            return JsonConvert.SerializeObject(item);
        }

        [HttpPost]
        public string GetWarehouseList(int customerId)
        {
            List<Warehouse> warehouse = Warehouse.GetList();
            List<Warehouse> customer_warehouse = Warehouse.GetListCustomerWarehouse(customerId);
            foreach (var customerItem in customer_warehouse)
            {
                foreach (var item in warehouse.Where(item => customerItem.Id == item.Id))
                {
                    item.Checked = false;
                    break;
                }
            }

            return JsonConvert.SerializeObject(warehouse);


        }

        [HttpPost]
        public string InsertCustomerWarehouse(Warehouse warehouseItem, int customerId)
        {
           MessageBox messageBox;
            if (warehouseItem.Checked)
            {
                bool result = Warehouse.DeleteCustomerAuthorityWarehouse(customerId, warehouseItem.Id);
                messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            }
            else
            {
                bool result = Warehouse.InsertCustomerAuthorityWarehouse(customerId, warehouseItem.Id, AdminCurrentSalesman.Id);
                messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");

            }

            return JsonConvert.SerializeObject(messageBox);
        }



        [HttpPost]
        public string UpdateUser(Users selectedUser)
        {
            bool result = false;

            if (selectedUser.Id == 0)
            {
                selectedUser.CreateId = AdminCurrentSalesman.Id;
                result = selectedUser.Add();
            }
            else
            {

                result = selectedUser.Update();
            }

            var messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");

            return JsonConvert.SerializeObject(messageBox);
        }


        [HttpPost]
        public string UpdateAuthorityCustomer(int id, bool updateValue, string field)
        {
            bool result = false;

            AuthorityCustomer item = new AuthorityCustomer();
            item.Id = id;
            item.UpdateValue = updateValue;
            item.Field = field;
            result = item.Update();

            var messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");


            return JsonConvert.SerializeObject(messageBox);
        }



        [HttpPost]
        public string UpdateAuthorityUser(int id, bool updateValue, string field)
        {
            bool result = false;

            AuthorityUser item = new AuthorityUser();
            item.Id = id;
            item.UpdateValue = updateValue;
            item.Field = field;
            result = item.Update();

            var messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");


            return JsonConvert.SerializeObject(messageBox);
        }

        [HttpPost]
        public string GetActiveManufacturerList(int id)
        {
            var manufacturerListActive = Manufacturer.GetManifacturerList();
            var manufacturerListPassive = Manufacturer.GetManifacturerPassiveListByCustomer(id);
            foreach (Manufacturer passive in manufacturerListPassive)
            {
                manufacturerListActive.Remove(manufacturerListActive.FirstOrDefault(m => m.Name == passive.Name));
            }
            MyTuple manufacturerList = new MyTuple(manufacturerListPassive, manufacturerListActive);
            return JsonConvert.SerializeObject(manufacturerList);
        }
        [HttpPost]
        public string InsertPassiveManufacturerList(int id, List<Manufacturer> manufacturer)
        {
              try
            {
                foreach (var item in manufacturer)
                {
                    Manufacturer.InsertManufacturerPassive(id, item.Name, AdminCurrentSalesman.Id);

                }

                MessageBox messageBox = new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı");
                return JsonConvert.SerializeObject(messageBox);
            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.Admin, "InsertPassiveManufacturerList", ex, GetUserIpAddress(), -1, -1, AdminCurrentSalesman.Id);

                MessageBox messageBox = new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
                return JsonConvert.SerializeObject(messageBox);
            }
        }
        [HttpPost]
        public string DeletePassiveManufacturerList(int id, List<Manufacturer> manufacturer)
        {
             try
            {
                foreach (var item in manufacturer)
                {
                    Manufacturer.DeleteManufacturerPassive(id, item.Name, AdminCurrentSalesman.Id);

                }

                MessageBox messageBox = new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı");
                return JsonConvert.SerializeObject(messageBox);
            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.Admin, "DeletePassiveManufacturerList", ex, GetUserIpAddress(), -1, -1, AdminCurrentSalesman.Id);

                MessageBox messageBox = new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
                return JsonConvert.SerializeObject(messageBox);

            }
        }


        [HttpPost]
        public string DeleteCustomerUser(int id)
        {

            bool result = customer.DeleteCustomerUser(id,AdminCurrentSalesman.Id);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "Kullanıcı Silindi") : new MessageBox(MessageBoxType.Error, "Silme İşlemi Sırasında Hata Oluştu");
            return JsonConvert.SerializeObject(messageBox);
        }

        [HttpPost]
        public string GetSalesmanAuthenticatorImage(int id)
        {
            Users user = Users.GetById(id);

            if (!String.IsNullOrEmpty(user.AuthenticatorGuid))
            {
                TwoFactorAuthenticator tfA = new TwoFactorAuthenticator();
                var setupCode = tfA.GenerateSetupCode("Eryaz Software(" + user.Code + "@" + GlobalSettings.CompanyName + ")", user.AuthenticatorGuid, 300, 400);


                WebClient wc = new WebClient();
                MemoryStream ms = new MemoryStream(wc.DownloadData(setupCode.QrCodeSetupImageUrl));
                var base64 = Convert.ToBase64String(ms.ToArray());
                string base64Array = String.Format("data:image/jpeg;base64,{0}", base64);


                MyUserTuple list = new MyUserTuple(user, base64Array);
                return JsonConvert.SerializeObject(list);
            }
            else
            {
                MyUserTuple list = new MyUserTuple(user, string.Empty);
                return JsonConvert.SerializeObject(list);
            }
        }

        [HttpPost]
        public string CreateAuthenticator(Users user, int type)
        {
            user.EditId = AdminCurrentSalesman.Id;
            user.AuthenticatorGuid = Guid.NewGuid().ToString().Replace("-", "");

            bool result = user.UpdateAuthenticatorValue();
            MessageBox messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);


        }


        #endregion

        #region RuleAdditional Codes

        [HttpPost]
        public string AddRuleAdditional(int customerId, string manufacturer, int productId, string productGroup1, string productGroup2,
            string productGroup3, double disc1, double disc2, double disc3, double disc4, bool isMainDiscountPassive, int dueDay, double rate,int salesPrice)
        {
           RuleAdditional ruleAdditional = new RuleAdditional()
            {
                CustomerId = customerId,
                CreateId = AdminCurrentSalesman.Id,
                Manufacturer = manufacturer,
                ProductId = productId,
                ProductGroup1 = productGroup1,
                ProductGroup2 = productGroup2,
                ProductGroup3 = productGroup3,
                Disc1 = disc1,
                Disc2 = disc2,
                Disc3 = disc3,
                Disc4 = disc4,
                DueDay = dueDay,
                Rate = rate,
                MainDiscountPassive = isMainDiscountPassive,
                PriceNumber = salesPrice
            };
            bool result = ruleAdditional.Insert();
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return JsonConvert.SerializeObject(messageBox);
        }

        [HttpPost]
        public string GetCustomerRuleAdditionalList(int customerId)
        {
            return JsonConvert.SerializeObject(RuleAdditional.GetList(customerId));
        }

        [HttpPost]
        public string UpdateRuleAdditional(int id, string name, string value)
        {
         
            bool result = RuleAdditional.UpdateRuleAdditional(name, Convert.ToDouble(value),id);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return JsonConvert.SerializeObject(messageBox);

        }

        [HttpPost]
        public string DeleteRuleAdditional(int id)
        {
            RuleAdditional ruleAdditional = new RuleAdditional()
            {
                Id = id,
                EditId = AdminCurrentSalesman.Id
            };

            bool result = ruleAdditional.Delete();
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu");
            return JsonConvert.SerializeObject(messageBox);
        }
        [HttpPost]
        public JsonResult GetSearchList(DateTime dateStart, DateTime dateEnd, int customerId, int userId)
        {
            List<LogSearch> list = LogSearch.GetList(customerId, userId, -1, dateStart.Date, dateEnd.Date.AddDays(1).AddMinutes(-1), 5000);
            return Json(list);
        }
        [HttpPost]
        public JsonResult GetSearchDetailList(int id)
        {
            List<LogSearch> list = LogSearch.GetListDetail(id);
            return Json(list);
        }

        [HttpPost]
        public string SendPasswordResetMailForCustomer(Customer customer, string email)
        {
             MessageBox messageBox;


            bool result = false;
            string guid = Guid.NewGuid().ToString();

            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.Body = "Merhaba,</br> Yönetici tarafından finans şifre sıfırlama isteği gönderilmiştir. Sıfırlama işlemi için lütfen bağlantıya tıklayınız. </br>" + GlobalSettings.B2bAddress + "Login/PasswordReset?parameter=" + guid;
            mail.IsBodyHtml = true;
            mail.Subject = "Şifre Resetleme";



            Tuple<bool, string> resultEmial = EmailHelper.Send(mail);

            if (resultEmial.Item1)
            {
                PasswordResetCs item = new PasswordResetCs()
                {
                    PersonId = customer.Id,
                    Type = PasswordResetType.FinancePassword,
                    Guid = guid
                };
                result = item.Add();
            }

            messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu." + resultEmial.Item2);

            return JsonConvert.SerializeObject(messageBox);
        }


        [HttpPost]
        public string SendPasswordResetMail(Users user, string email)
        {
            MessageBox messageBox;


            bool result = false;
            string guid = Guid.NewGuid().ToString();

            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.Body = "Merhaba,</br> Yönetici tarafından şifre sıfırlama isteği gönderilmiştir. Sıfırlama işlemi için lütfen bağlantıya tıklayınız. </br>" + GlobalSettings.B2bAddress + "Login/PasswordReset?parameter=" + guid;
            mail.IsBodyHtml = true;
            mail.Subject = "Şifre Resetleme";


            Tuple<bool, string> resultEmial = EmailHelper.Send(mail);

            if (resultEmial.Item1)
            {
                PasswordResetCs item = new PasswordResetCs()
                {
                    PersonId = user.Id,
                    Type = PasswordResetType.User,
                    Guid = guid
                };
                result = item.Add();
            }

            messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu." + resultEmial.Item2);

            return JsonConvert.SerializeObject(messageBox);
        }


        #endregion

        
    }

    public class MyUserTuple : Tuple<Users, string>
    {
        public MyUserTuple(Users mList1, string mList2) : base(mList1, mList2) { }
        public Users User => Item1;
        public string QrCode => Item2;
    }
    public class MyTuple : Tuple<List<Manufacturer>, List<Manufacturer>>
    {
        public MyTuple(List<Manufacturer> mList1, List<Manufacturer> mList2) : base(mList1, mList2) { }
        public List<Manufacturer> PassiveManufacturer => Item1;
        public List<Manufacturer> ActiveManufacturer => Item2;
    }

}
