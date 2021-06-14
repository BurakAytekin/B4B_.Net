using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using System.Web.UI;
using B2b.Web.v4.Models.Log;
using System.Net.Sockets;
using B2b.Web.v4.Models.Log.EPayment;
using System.Data;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class SyncController : AdminBaseController
    {
        // GET: Admin/Sync
        public ActionResult Index()
        {
              return View();
        }

        public ActionResult DataTransfer()
        {
                return View();
        }


        #region   HttpPost Methods
        [HttpPost]
        public JsonResult FireRefresh()
        {
            RuningControl.SyncResponseValues.Log = new List<LogMessage>();
            return Json(string.Empty);
        }


        [HttpPost]
        public JsonResult CheckTransfer()
        {
            if (RuningControl.SyncResponseValues == null || RuningControl.SyncResponseValues.Log == null)
            {
                RuningControl.SyncResponseValues = new SyncResponseValues();
                RuningControl.SyncResponseValues.Log = new List<LogMessage>();
            }

            Tuple<bool, SyncResponseValues> tuple =
            new Tuple<bool, SyncResponseValues>(RuningControl.IsRunning, RuningControl.SyncResponseValues);

            return Json(tuple);
        }

        [HttpPost]
        public JsonResult SetValues(bool type, SyncSettings item)
        {
            try
            {
                string ssss = Token.Decrypt("cJ9y8xx0U8BLfbfPqM6rN8djs6SIanUnxOVphrO8R4M=", GlobalSettings.EncryptKey);

                string param = Token.Encrypt((type ? "WebAddress-" : "StopService-") + (item.CompanySettings.IsLocalB2b ? GlobalSettings.B2bAddressLocal : GlobalSettings.B2bAddress), GlobalSettings.EncryptKey);
                Socket soket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                soket.Connect(new IPEndPoint(IPAddress.Parse(item.CompanySettings.ServerIp), 8984));
                byte[] sendData = Encoding.UTF8.GetBytes(param);
                soket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback(AsyncCallBack), null);
            }
            catch (Exception ex)
            {
                RuningControl.SyncResponseValues.SettingsId = -1;
                RuningControl.SyncResponseValues.Status = 0;
                RuningControl.SyncResponseValues.ProgressValue = 0;
                List<LogMessage> list = new List<LogMessage>();
                list.Add(new LogMessage { Message = "HATA " + ex.Message + "</br>", Type = LogMessageType.Error });
                RuningControl.SyncResponseValues.Log.AddRange(list);

                LogSync log = new LogSync()
                {
                    SettingsId = 0,
                    ProgressValue = 0,
                    Message = "HATA " + ex.Message,
                    Type = 1,
                    CreateId = AdminCurrentSalesman.Id
                };
                log.Add();

            }



            return Json(string.Empty);
        }

        [HttpPost]
        public JsonResult FireResponse(SyncSettings item)
        {
            try
            {
                string param = Token.Encrypt("Transfer-" + item.TransferTypeId, GlobalSettings.EncryptKey);
                Socket soket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                soket.Connect(new IPEndPoint(IPAddress.Parse(item.CompanySettings.ServerIp), 8984));
                byte[] sendData = Encoding.UTF8.GetBytes(param);
                soket.BeginSend(sendData, 0, sendData.Length, SocketFlags.None, new AsyncCallback(AsyncCallBack), null);
            }
            catch (Exception ex)
            {
                RuningControl.SyncResponseValues.SettingsId = -1;
                RuningControl.SyncResponseValues.Status = 0;
                RuningControl.SyncResponseValues.ProgressValue = 0;
                List<LogMessage> list = new List<LogMessage>();
                list.Add(new LogMessage { Message = "HATA " + ex.Message + "</br>", Type = LogMessageType.Error });
                RuningControl.SyncResponseValues.Log.AddRange(list);

                LogSync log = new LogSync()
                {
                    SettingsId = 0,
                    ProgressValue = 0,
                    Message = "HATA " + ex.Message,
                    Type = 1
                };
                log.Add();
            }


            return Json(string.Empty);
        }

        private void AsyncCallBack(IAsyncResult ar)
        {

        }



        [HttpPost]
        public JsonResult GetTrasnferTypeList()
        {
            List<SyncTransferType> list = SyncTransferType.GetSyncTransferType();
            return Json(list);
        }

        [HttpPost]
        public JsonResult GetActiveTrasnferTypeList()
        {
            List<SyncSettings> list = SyncSettings.GetActiveSyncTransferType();
            return Json(list);
        }


        [HttpPost]
        public JsonResult GetSettingItem(int id, int transferTypeId)
        {
            SyncSettings item = SyncSettings.GetSettingItem(id, transferTypeId);
            return Json(item);
        }

        [HttpPost]
        public JsonResult SaveTrasnferType(string name)
        {
            bool result = false;

            SyncTransferType item = new SyncTransferType()
            {
                Name = name,
                CreateId = AdminCurrentSalesman.Id
            };
            result = item.Add();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);

        }

        [HttpPost]
        public JsonResult SaveSyncSettings(SyncSettings selectedItem)
        {
            bool result = false;

            if (selectedItem.Id == 0)
            {
                selectedItem.CreateId = AdminCurrentSalesman.Id;
                selectedItem.Id = selectedItem.Add();
                result = true;
            }
            else
            {
                selectedItem.EditId = AdminCurrentSalesman.Id;
                result = selectedItem.Update();
            };

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .", selectedItem.Id) : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);

        }

        [HttpPost]
        public JsonResult DeleteTrasnferType(int id)
        {
            bool result = false;

            SyncTransferType item = new SyncTransferType()
            {
                Id = id,
                EditId = AdminCurrentSalesman.Id
            };
            result = item.Delete();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);

        }
        [HttpPost]
        public JsonResult GetTransferLog(DateTime minDate, DateTime maxDate)
        {
            List<LogSync> list = LogSync.GetTransferLog(minDate, maxDate);
            return Json(list);
        }

        #endregion

    }
}