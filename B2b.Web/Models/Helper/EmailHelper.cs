using Eryaz.Services.Data;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class EmailHelper : DataAccess
    {
        public static Tuple<bool, string> Send(MailMessage mail, int type = -1)
        {
            EmailSettings es = EmailSettings.GetByType(type);

            switch (es.Type)
            {
                case 0: return SendEmailEryaz(mail, es);
                case 1: return SendEmail(mail, es);
                default: return new Tuple<bool, string>(false, "Hata oluştu");
            }
        }
        private static Tuple<bool, string> SendEmailEryaz(MailMessage mail, EmailSettings es)
        {
            //return new Tuple<bool, string>(false, "");

            try
            {
                EmailRequest emailRequest = new EmailRequest();
                emailRequest.Company = es.UserName;
                emailRequest.ApiKey = es.Password;
                emailRequest.To = string.Join(";", mail.To.Select(p => p.Address));
                emailRequest.Cc = string.Join(";", mail.CC.Select(p => p.Address));
                emailRequest.Bcc = string.Join(";", mail.Bcc.Select(p => p.Address));
                emailRequest.ReplyTo = string.Join(";", mail.ReplyToList.Select(p => p.Address));
                emailRequest.Subject = mail.Subject;
                emailRequest.Body = mail.Body;
                emailRequest.SepereratedTo = false;

                if (mail.Attachments.Count > 0)
                {
                    emailRequest.AttachmentList = new List<EmailAttachment>();
                    foreach (Attachment item in mail.Attachments)
                    {
                        emailRequest.AttachmentList.Add(new EmailAttachment() { Name = item.Name, ContentType = item.ContentType.MediaType, Data = ReadFully(item.ContentStream) });
                        //emailRequest.AttachmentList.Add(Functions.GenerateEmailAttachment(@"C:\Users\makcura\Desktop\WebServiceKurulumu.pdf"));
                    }

                }
                var client = new RestClient("https://services.eryaz.net/api/email/send");
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", JsonConvert.SerializeObject(emailRequest), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                if (response.StatusCode == HttpStatusCode.OK)
                    return new Tuple<bool, string>(true, response.ErrorMessage);
                return new Tuple<bool, string>(false, response.ErrorMessage);

            }
            catch (Exception ex)
            {
                return new Tuple<bool, string>(false, ex.Message);
            }
        }
        private static Tuple<bool, string> SendEmail(MailMessage mail, EmailSettings es)
        {
            //return new Tuple<bool, string>(false, "");
            bool retValue = true;
            string retMessage = string.Empty;

            using (SmtpClient client = new SmtpClient())
            {
                client.Port = es.HostPort ?? 0;
                //client.Timeout = 3000; çalışmadı o yüzden aşağıdaki method yazıldı.
                client.Host = es.Host;
                client.EnableSsl = es.UseSsl;
                mail.From = new MailAddress(es.FromAddress);
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(es.UserName, es.Password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;


                string logMailTo = string.Empty;
                string logMailCc = string.Empty;
                string logMailBcc = string.Empty;
                foreach (var item in mail.To)
                    logMailTo += item + ";";
                foreach (var item in mail.CC)
                    logMailCc += item + ";";
                foreach (var item in mail.Bcc)
                    logMailBcc += item + ";";


                try
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IAsyncResult result = socket.BeginConnect(client.Host, client.Port, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(2000, true);
                    if (!success)
                    {
                        socket.Close();
                        throw new ApplicationException("Host ve Port bilgilerini kontrol ediniz...");
                    }

                    client.Send(mail);
                    //Logger.LogEmail(clientType, customerId, salesmanId, userId, (mail.Attachments.Count > 0 ? true : false), logMailTo, logMailCc, logMailBcc, mail.Subject, mail.Body, fromMail, "Başarılı", "", dealerId);

                    retValue = true;
                    retMessage = string.Empty;
                }
                catch (Exception ex)
                {
                    //Logger.LogEmail(clientType, customerId, salesmanId, userId, (mail.Attachments.Count > 0 ? true : false), logMailTo, logMailCc, logMailBcc, mail.Subject, mail.Body, ex.ToString(), "Başarısız", "", dealerId);
                    retValue = false;
                    retMessage = ex.Message.Contains("AUTH") ? "Kullanıcı Adı ve Şifrenizi kontrol ediniz" : (ex.Message == "Posta gönderme hatası." ? "Ssl ayarlarını kontrol ediniz..." : ex.Message);
                }
            }

            foreach (Attachment attachment in mail.Attachments)
            {
                attachment.Dispose();
            }
            mail.Attachments.Dispose();
            mail = null;

            return new Tuple<bool, string>(retValue, retMessage);
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

    }
}
