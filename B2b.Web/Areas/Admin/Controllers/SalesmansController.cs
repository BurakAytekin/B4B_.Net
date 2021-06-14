using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;
using System.Net.Mail;
using B2b.Web.v4.Areas.Admin.Models.Security;
using System.Net;
using System.IO;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class SalesmansController : AdminBaseController
    {
        // GET: Admin/Salesman
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult SalesmansSelect()
        {
            return PartialView();
        }

        #region   HttpPost Methods

        [HttpPost]
        public string SendPasswordResetMail(Salesman user, string email)
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
                    Type = PasswordResetType.Salesman,
                    Guid = guid
                };
                result = item.Add();
            }

            messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu." + resultEmial.Item2);

            return JsonConvert.SerializeObject(messageBox);
        }

        [HttpPost]
        public string GetSalesmanList(string name)
        {
            List<Salesman> salesman = Salesman.GetList(name ?? "");

            return JsonConvert.SerializeObject(salesman);
        }

        [HttpPost]
        public string GetSalesmanAuthenticatorImage(int id)
        {
            Salesman salesman = Salesman.GetById(id);

            if (!String.IsNullOrEmpty(salesman.AuthenticatorGuid))
            {
                TwoFactorAuthenticator tfA = new TwoFactorAuthenticator();
                var setupCode = tfA.GenerateSetupCode("Eryaz Software(" + salesman.Code + "@" + GlobalSettings.CompanyName + ")", salesman.AuthenticatorGuid, 300, 400);


                WebClient wc = new WebClient();
                MemoryStream ms = new MemoryStream(wc.DownloadData(setupCode.QrCodeSetupImageUrl));
                var base64 = Convert.ToBase64String(ms.ToArray());
                string base64Array = String.Format("data:image/jpeg;base64,{0}", base64);


                MySalesmanTuple list = new MySalesmanTuple(salesman, base64Array);
                return JsonConvert.SerializeObject(list);
            }
            else
            {
                MySalesmanTuple list = new MySalesmanTuple(salesman, string.Empty);
                return JsonConvert.SerializeObject(list);
            }
        }


        [HttpPost]
        public string saveSalesmanImage(int id, string imageBase)
        {
            MessageBox messageBox;
            if (id == 0)
            {
                messageBox = new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
                return JsonConvert.SerializeObject(messageBox);
            }

            bool result = false;
            if (imageBase != null)
            {
                string imgTypeIcon = GetFileType(imageBase);
                string fileIconName = Guid.NewGuid().ToString();
                byte[] fileIconData = Parse(imageBase);
                string fullFtpFileIconPath = GlobalSettings.FtpServerUploadAddress +
                                             GlobalSettings.SalesmanPath + fileIconName + "." + imgTypeIcon;
                string ftpFileIconPath = GlobalSettings.SalesmanPath + fileIconName + "." +
                                         imgTypeIcon;

                result = FtpHelper.UploadRemoteServer(fileIconData, fullFtpFileIconPath);
                if (result)
                {
                    Salesman.UpdatePicturePath(ftpFileIconPath, id, AdminCurrentSalesman.Id);
                    messageBox = result
                        ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                        : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
                    return JsonConvert.SerializeObject(messageBox + fullFtpFileIconPath);
                }
                else
                {
                    messageBox = result
                       ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                       : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
                }
            }
            else
            {
                messageBox = result
                       ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                       : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            }
            return JsonConvert.SerializeObject(messageBox);
        }

        [HttpPost]
        public string GetLicenceList(int salesmanId)
        {
            List<Licence> tmpLicence = Licence.GetLicenceByUserId(salesmanId, 1);
            MyTuple licence = new MyTuple(tmpLicence.Where(l => l.Source == LicenceSource.B2BWeb).ToList(), LicenceMobile.GetListByUser(1, salesmanId), tmpLicence.Where(l => l.Source == LicenceSource.Admin).ToList());
            return JsonConvert.SerializeObject(licence);
        }

        [HttpPost]
        public string GetAuthoritySalesman(int id)
        {
            Aut_SalesmanTuple item = AuthoritySalesman.GetAuthoritySalesman(id);
            return JsonConvert.SerializeObject(item);
        }

        [HttpPost]
        public string GetCustomerList(int id)
        {
            var customerList = Customer.GetListCustomerBySalesmanId(id);
            return JsonConvert.SerializeObject(customerList);
        }
        [HttpPost]
        public string UpdateAuthoritySalesman(int id, bool updateValue, string field)
        {
            MessageBox messageBox;
            bool result = false;

            AuthoritySalesman item = new AuthoritySalesman();
            item.Id = id;
            item.UpdateValue = updateValue;
            item.Field = field;
            result = item.Update();

            messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");


            return JsonConvert.SerializeObject(messageBox);
        }
        public class MyTuple : Tuple<List<Licence>, List<LicenceMobile>, List<Licence>>
        {
            public MyTuple(List<Licence> mList1, List<LicenceMobile> mList2, List<Licence> mList3) : base(mList1, mList2, mList3) { }
            public List<Licence> LicenceB2B => Item1;
            public List<LicenceMobile> LicenceMobile => Item2;
            public List<Licence> LicenceAdmin => Item3;
        }
        [HttpPost]
        public string SaveSalesman(Salesman salesman)
        {
            salesman.CreateId = AdminCurrentSalesman.Id;
            salesman.EditId = AdminCurrentSalesman.Id;
            bool result = salesman.AddOrUpdate();
            MessageBox messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);


        }

        [HttpPost]
        public string DeleteSalesman(Salesman salesman)
        {
            salesman.CreateId = AdminCurrentSalesman.Id;
            salesman.EditId = AdminCurrentSalesman.Id;
            salesman.Deleted = true;
            bool result = salesman.Delete();
            MessageBox messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);


        }

        [HttpPost]
        public string CreateAuthenticator(Salesman salesman, int type)
        {
            salesman.EditId = AdminCurrentSalesman.Id;
            salesman.AuthenticatorGuid = Guid.NewGuid().ToString().Replace("-", "");

            bool result = salesman.UpdateAuthenticatorValue();
            MessageBox messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);


        }

        [HttpPost]
        public string DeleteLicence(Licence licenceItem)
        {
            Licence licence = licenceItem;
            bool result = licence.Delete();
            MessageBox messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);

        }
        [HttpPost]
        public string DeleteLicenceMobile(LicenceMobile licenceItem)
        {
            LicenceMobile licence = licenceItem;
            bool result = licence.Delete();
            MessageBox messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);

        }

        [HttpPost]
        public string ConnectCustomer(int salesmanId, int[] customerId, int type = 1)
        {
            bool result = false;
            string idS = "";
            foreach (int item in customerId)
            {
                if (type == 1)
                    result = Salesman.InsertOrDeleteSalesmanOfCustomer(salesmanId, item.ToString(), AdminCurrentSalesman.Id, type);
                else
                    idS += "'" + item.ToString() + "',";
            }
            if (type == 0)
            {
                idS = idS.Substring(0, (idS.Length - 1));
                result = Salesman.InsertOrDeleteSalesmanOfCustomer(salesmanId, idS, AdminCurrentSalesman.Id, type);
            }

            MessageBox messageBox = result
                ? new MessageBox(MessageBoxType.Success, "İşlem Başarılı")
                : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }

        [HttpPost]
        public string GetListNotConnectedCustomerBySalesmanId()
        {
            var notConnectedCustomerList = Customer.GetListNotConnectedCustomerBySalesmanId(AdminCurrentSalesman.Id); ;
            return JsonConvert.SerializeObject(notConnectedCustomerList);
        }

        #endregion

    }
    public class MySalesmanTuple : Tuple<Salesman, string>
    {
        public MySalesmanTuple(Salesman mList1, string mList2) : base(mList1, mList2) { }
        public Salesman Salesman => Item1;
        public string QrCode => Item2;
    }
}