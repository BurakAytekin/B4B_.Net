using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace B2b.Web.v4.Models.Helper
{
    public static class FtpHelper
    {
        public static bool UploadRemoteServer(byte[] file, string address)
        {
            bool result = false;

            FtpWebRequest request = (FtpWebRequest)FtpWebRequest.Create(address);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.Credentials = new NetworkCredential(GlobalSettings.FtpUserName, GlobalSettings.FtpPassword);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;


            Stream reqStream = request.GetRequestStream();
            reqStream.Write(file, 0, file.Length);
            reqStream.Close();

            result = true;


            return result;
        }

        public static byte[] DownloadRemoteServer(string address)
        {
            using (WebClient request = new WebClient())
            {
                request.Credentials = new NetworkCredential(GlobalSettings.FtpUserName, GlobalSettings.FtpPassword);
                byte[] fileData = request.DownloadData(address);

                return fileData;
            }
        }

        public static List<string> ListDirectory(string address)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(address);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(GlobalSettings.FtpUserName, GlobalSettings.FtpPassword);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            List<string> list = new List<string>();

            using (var response = (FtpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream, true))
                    {
                        while (!reader.EndOfStream)
                        {
                            list.Add(reader.ReadLine());
                        }
                    }
                }
            }

            return list;

        }

        public static bool RelocateFile(string oldAddress, string newAddress)
        {
            bool result = false;

            //perform rename
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(oldAddress);
            request.Method = WebRequestMethods.Ftp.Rename;
            request.RenameTo = Uri.UnescapeDataString(newAddress);
            request.Credentials = new NetworkCredential(GlobalSettings.FtpUserName, GlobalSettings.FtpPassword);
            request.UsePassive = true;
            request.UseBinary = true;
            request.KeepAlive = false;

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            if (response.StatusCode == FtpStatusCode.CommandOK ||
                response.StatusCode == FtpStatusCode.FileActionOK)
                result = true;

            return result;
        }
    }
}