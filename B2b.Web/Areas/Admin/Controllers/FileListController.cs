using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class FileListController : AdminBaseController
    {
        List<Files> FileList
        {
            get { return (List<Files>)Session["FileList"]; }
            set { Session["FileList"] = value; }
        }

        // GET: Admin/FileList
        public ActionResult Index()
        {
        
            return View();
        }

        #region   HttpPost Methods
        [HttpPost]
        public string GetFileList()
        {
            FileList = Files.GetFileList();
            return JsonConvert.SerializeObject(FileList);

        }


        [HttpPost]
        public string DeleteFile(int id)
        {
             bool result = false;
            Files item = new Files()
            {
                Id = id,
                EditId = AdminCurrentSalesman.Id
            };
            result = item.Delete();


            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return JsonConvert.SerializeObject(message);
        }


        [HttpPost]
        public string FileSelectAll(bool checkValue)
        {

            foreach (Files item in FileList)
            {
                item.Checked = checkValue;
            };

            return JsonConvert.SerializeObject(string.Empty);

        }

        [HttpPost]
        public string SaveFile(string filePathSelected, string imageBaseIcon, string title, string name, int restriction)
        {
            bool result = false;


            if (filePathSelected != null)
            {
                var outputStream = new MemoryStream();
                using (ZipFile zipFile = new ZipFile())
                {
                    #region Helper
                    //Add Root Directory Name "Files" or Any string name
                    //zipFile.AddDirectoryByName("File");

                    //Get all filepath from folder
                    //String[] files = Directory.GetFiles(Server.MapPath("../../Content/images/404.jpg"));
                    //foreach (string file in files)
                    //{
                    //    string filePath = file;

                    //    //Adding files from filepath into Zip
                    //    zipFile.AddFile(filePath, "Files");"../../Content/images/404.jpg"
                    //} 
                    #endregion

                    string fileIconPath = string.Empty;



                    byte[] fileData = Parse(filePathSelected);

                    string fileType = GetFileType(filePathSelected);

                    fileType = fileType == "" ? "Bilinmeyen" : fileType;
                    bool remote = false;
                    if (imageBaseIcon != null)
                    {
                        remote = true;
                        string imgTypeIcon = GetFileType(imageBaseIcon);

                        string fileIconName = Guid.NewGuid().ToString();
                        byte[] fileIconData = Parse(imageBaseIcon);

                        string fullFtpFileIconPath = GlobalSettings.FtpServerUploadAddress  + GlobalSettings.GeneralPath + fileIconName + "." + imgTypeIcon;

                        result = FtpHelper.UploadRemoteServer(fileIconData, fullFtpFileIconPath);

                        imageBaseIcon =  GlobalSettings.GeneralPath + fileIconName + "." + imgTypeIcon;
                    }
                    else
                    {
                        remote = false;
                        String[] files = Directory.GetFiles(Server.MapPath("../../Content/images/fileIcons/"));

                        int count = files.Count(x => x.Contains(fileType + ".png"));

                        if (count > 0)
                            imageBaseIcon = "Content/images/fileIcons/" + fileType + ".png";
                        else
                            imageBaseIcon = "Content/images/fileIcons/noPath.png";
                    }

                    string filename = Guid.NewGuid().ToString();
                    MemoryStream stream = new MemoryStream(fileData);
                    zipFile.AddEntry(filename + "." + fileType, stream);
                    Response.ClearContent();
                    Response.ClearHeaders();
                    Response.AppendHeader("content-disposition", "attachment; filename=" + filename + ".zip");

                    //Save the zip content in output stream
                    zipFile.Save(outputStream);

                    string fullFtpFilePath = GlobalSettings.FtpServerUploadAddress  + GlobalSettings.GeneralPath + filename + ".zip";

                    result = FtpHelper.UploadRemoteServer(outputStream.ToArray(), fullFtpFilePath);

                    outputStream.Position = 0;

                    string filePath =  GlobalSettings.GeneralPath + filename + ".zip";

                    Files fileItem = new Files()
                    {
                        Title = title,
                        Name = name,
                        Path = filePath,
                        PicturePath = imageBaseIcon,
                        FileType = fileType,
                        Remote = remote,
                        Restriction = restriction,
                        CreateId = AdminCurrentSalesman.Id

                    };
                    result = fileItem.Add();
                }

            }

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return JsonConvert.SerializeObject(message);

        }

        #endregion
    }
}