using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class AnnouncementsController : AdminBaseController
    {
        // GET: Admin/Announcements
        public ActionResult Index()
        {
               return View();
        }


        #region HttpPost Methods

        [HttpPost]
        public string GetAnnouncementsHeader(int announcementsType, int listType)
        {
            try
            {

                var list = Announcements.GetAnnouncementHeaderList(announcementsType, listType);
                var json = JsonConvert.SerializeObject(list);
                return json;
            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.Admin, MethodBase.GetCurrentMethod().Name, ex, GetUserIpAddress(), -1, -1, AdminCurrentSalesman.Id);

                throw;
            }

        }

        [HttpPost]
        public string DeleteAnnouncements(string DeleteIds)
        {
           
            bool result = false;

            DeleteIds = DeleteIds.Substring(0, DeleteIds.Length - 1);

            result = Announcements.Delete(DeleteIds, AdminCurrentSalesman.Id);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "Silme İşlemi Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);
        }

        [HttpPost]
        public string UpdateAnnouncements(Announcements announcement)
        {
           

            announcement.EditId = AdminCurrentSalesman.Id;
            announcement.Query = String.IsNullOrEmpty(announcement.Query) ? "#" : announcement.Query;
            announcement.PriceStr = String.IsNullOrEmpty(announcement.PriceStr) ? "#" : announcement.PriceStr;
            if (announcement.AnnouncementsType == 3 || announcement.AnnouncementsType == 1 || announcement.AnnouncementsType == 11 || announcement.AnnouncementsType == 10 || announcement.AnnouncementsType == 9 || announcement.AnnouncementsType == 12)
            {
                if (announcement.ImageBase != null)
                {
                    string imgTypeIcon = GetFileType(announcement.ImageBase);

                    string fileIconName = Guid.NewGuid().ToString();
                    byte[] fileIconData = Parse(announcement.ImageBase);

                    string fullFtpFileIconPath = GlobalSettings.FtpServerUploadAddress + GlobalSettings.GeneralPath + fileIconName + "." + imgTypeIcon;

                    bool resultUp = FtpHelper.UploadRemoteServer(fileIconData, fullFtpFileIconPath);

                    announcement.PicturePath =  GlobalSettings.GeneralPath + fileIconName + "." + imgTypeIcon;
                }
            }


            bool result = Announcements.UpdateAnnouncements(announcement);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);

        }

        [HttpPost]
        public string SaveAnnouncements(Announcements announcement)
        {
            announcement.CreateId = AdminCurrentSalesman.Id;
            announcement.Query = String.IsNullOrEmpty(announcement.Query) ? "#" : announcement.Query;
            announcement.PriceStr = String.IsNullOrEmpty(announcement.PriceStr) ? "#" : announcement.PriceStr;
            if (announcement.AnnouncementsType == 3 || announcement.AnnouncementsType == 1 || announcement.AnnouncementsType == 11 || announcement.AnnouncementsType == 10 || announcement.AnnouncementsType == 9 || announcement.AnnouncementsType == 12)
            {
                if (announcement.ImageBase != null)
                {
                    string imgTypeIcon = GetFileType(announcement.ImageBase);

                    string fileIconName = Guid.NewGuid().ToString();
                    byte[] fileIconData = Parse(announcement.ImageBase);

                    string fullFtpFileIconPath = GlobalSettings.FtpServerUploadAddress  + GlobalSettings.GeneralPath + fileIconName + "." + imgTypeIcon;

                    bool resultUp = FtpHelper.UploadRemoteServer(fileIconData, fullFtpFileIconPath);

                    announcement.PicturePath =  GlobalSettings.GeneralPath + fileIconName + "." + imgTypeIcon;
                }
            }

            bool result = announcement.Add();
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);

        }

        [HttpPost]
        public string SetPriority(List<TmpPriority> id)
        {
            string listStr = string.Empty;
            foreach (var item in id)
            {
                if (item.id > 0)
                {
                    if (!string.IsNullOrEmpty(listStr))
                        listStr += ",";
                    listStr += "" + item.id + "";
                }
            }
            bool result = Announcements.ChangePriority(listStr,AdminCurrentSalesman.Id);
            MessageBox messageBox = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Tamamlandı") : new MessageBox(MessageBoxType.Error, "İşlem Sırasında Hata Oluştu.");
            return JsonConvert.SerializeObject(messageBox);

        } 
        #endregion
        public class TmpPriority
        {
            public int id { get; set; }
        }
    }
}