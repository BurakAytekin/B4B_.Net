using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.Log;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class FilesUploadController : AdminBaseController
    {
        // GET: Admin/FilesUpload
        public ActionResult Index()
        {
             return View();
        }

        public ActionResult Index2()
        {
            Response.ContentType = "multipart/form-data";
            ReturnValues files = new ReturnValues();
            int productId = Convert.ToInt32(Request.Params["productId"]);
            string productCode = Request.Params["productCode"];
            string watermark = Request.Params["watermark"];
            foreach (string upload in Request.Files)
            {
                int lastPictureId = Picture.GetPictureMaxId(productId) + 1;
                string filename = Encrypt(productCode + "_" + productId + "_" + lastPictureId, GlobalSettings.EncryptPassword);
                                                                                                                            
                try
                {
                    var httpPostedFileBase = Request.Files[upload];
                    if (httpPostedFileBase == null) continue;

                    string[] array = httpPostedFileBase.FileName.Split('.');
                    string imgType = array[array.Length - 1];
                    string fullFtpFilePath = GlobalSettings.FtpServerUploadAddress+ "Pictures/" + filename + "." + imgType;
                    int fileLength = httpPostedFileBase.ContentLength;
                    byte[] fileData = new byte[fileLength];
                    var postedFileBase = Request.Files[0];
                    if (postedFileBase != null)
                        using (var binaryReader = new BinaryReader(postedFileBase.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(postedFileBase.ContentLength);
                        }

                    string fileExt = Path.GetExtension(httpPostedFileBase.FileName);
                    if (fileExt.ToLower() == ".png" || fileExt.ToLower() == ".jpg" || fileExt.ToLower() == ".jpeg")
                    {
                        Bitmap bmp;
                        using (var ms = new MemoryStream(fileData))
                        {
                            bmp = new Bitmap(ms);
                        }
                        if (!string.IsNullOrEmpty(watermark) && watermark != "undefined")
                            fileData = PictureWatermark(bmp, watermark);

                        bool result = FtpHelper.UploadRemoteServer(fileData, fullFtpFilePath);
                        if (result)
                        {

                            Picture item = new Picture()
                            {
                                Path = "Pictures/" + filename + "." + imgType,
                                PictureId = lastPictureId,
                                ProductId = productId,
                                ProductCode = productCode,
                                CreateId = AdminCurrentSalesman.Id
                            };

                            if (item.Add())
                            {
                                ResponseMsg ResponseMsg = new ResponseMsg
                                {
                                    status = "success",
                                    name = httpPostedFileBase.FileName,
                                    success = true
                                };
                                return Json(ResponseMsg);

                            }
                            return Json(CreateMmessage(Request, upload, true));
                        }
                        return Json(CreateMmessage(Request, upload, true));
                    }
                    return Json(CreateMmessage(Request, upload, true));
                }
                catch (Exception ex)
                {
                    Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.Admin, "FileUploadContreller", ex, GetUserIpAddress(), -1, -1, AdminCurrentSalesman.Id);

                    return Json(CreateMmessage(Request, upload, true));
                }

            }
            return Json(files);
        }

        public ActionResult ImportPicture()
        {
            Logger.LogNavigation(-1, -1, AdminCurrentSalesman.Id,
                  GetControllerName() + MethodBase.GetCurrentMethod().Name, ClientType.Admin, GetUserIpAddress());
            Response.ContentType = "multipart/form-data";
            ReturnValues files = new ReturnValues();



            string watermark = Request.Params["watermark"];
            foreach (string upload in Request.Files)
            {
                var httpPostedFileBase = Request.Files[upload];
                if (httpPostedFileBase == null) continue;
                string[] array = httpPostedFileBase.FileName.Split('.');
                string imgType = array[array.Length - 1];

                string productCode = string.Empty;

                for (int i = 0; i < array.Length - 1; i++)
                {
                    productCode += (array[i].Substring(array[i].Length - 2, 2).Contains("_") ? array[i].Substring(0, array[i].Length - 2) : array[i]) + ".";
                }
                productCode = productCode.Substring(0, productCode.Length - 1);

                Product product = Product.GetByCode(productCode);

                if (product == null || product.Id == 0)
                    return Json(CreateMmessage(Request, upload, true));

                int productId = product.Id;// Convert.ToInt32(Request.Params["productId"]);


                int lastPictureId = Picture.GetPictureMaxId(productId) + 1;
                string filename = Encrypt(productCode + "_" + productId + "_" + lastPictureId, GlobalSettings.EncryptPassword);

                try
                {



                    string fullFtpFilePath = GlobalSettings.FtpServerUploadAddress  + "Pictures/" + filename + "." + imgType;
                    int fileLength = httpPostedFileBase.ContentLength;
                    byte[] fileData = new byte[fileLength];
                    var postedFileBase = Request.Files[0];
                    if (postedFileBase != null)
                        using (var binaryReader = new BinaryReader(postedFileBase.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(postedFileBase.ContentLength);
                        }

                    string fileExt = Path.GetExtension(httpPostedFileBase.FileName);
                    if (fileExt.ToLower() == ".png" || fileExt.ToLower() == ".jpg" || fileExt.ToLower() == ".jpeg")
                    {
                        Bitmap bmp;
                        using (var ms = new MemoryStream(fileData))
                        {
                            bmp = new Bitmap(ms);
                        }
                        if (!string.IsNullOrEmpty(watermark) && watermark != "undefined")
                            fileData = PictureWatermark(bmp, watermark);
                        bool result = FtpHelper.UploadRemoteServer(fileData, fullFtpFilePath);
                        if (result)
                        {

                            Picture item = new Picture()
                            {
                                Path = "Pictures/" + filename + "." + imgType,
                                PictureId = lastPictureId,
                                ProductId = productId,
                                ProductCode = productCode,
                                CreateId = AdminCurrentSalesman.Id
                            };

                            if (item.Add())
                            {
                                ResponseMsg ResponseMsg = new ResponseMsg
                                {
                                    status = "success",
                                    name = httpPostedFileBase.FileName,
                                    success = true
                                };
                                return Json(ResponseMsg);

                            }
                            return Json(CreateMmessage(Request, upload, true));
                        }
                        return Json(CreateMmessage(Request, upload, true));
                    }
                    return Json(CreateMmessage(Request, upload, true));
                }
                catch (Exception ex)
                {
                    Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.Admin, "FileUploadContreller", ex, GetUserIpAddress(), -1, -1, AdminCurrentSalesman.Id);

                    return Json(CreateMmessage(Request, upload, true));
                }

            }
            return Json(files);
        }


        public JsonResult CreateMmessage(HttpRequestBase Request, string upload, bool IsError)
        {

            ResponseMsg ResponseMsg = new ResponseMsg
            {
                status = IsError ? "error" : "success",
                name = Request.Files[upload].FileName,
                success = !IsError
            };
            return Json(ResponseMsg);
        }
        public class ResponseMsg
        {
            public string status;
            public string name;
            public bool success { get; set; }
        }

        public static string Decrypt(string EncryptedText, string Key)
        {
            EncryptedText = EncryptedText.Replace("^", "+");
            string initVector = "tu89geji340t89u2";

            int keysize = 256;
            int modAl = EncryptedText.Length % 4;
            if (modAl > 0)
            {
                EncryptedText += new string('=', 4 - modAl);
            }

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] DeEncryptedText = Convert.FromBase64String(EncryptedText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(Key, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(DeEncryptedText);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[DeEncryptedText.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        public static string Encrypt(string Text, string Key)
        {
            string initVector = "tu89geji340t89u2";

            int keysize = 256;

            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(Text);
            PasswordDeriveBytes password = new PasswordDeriveBytes(Key, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] Encrypted = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string result = Convert.ToBase64String(Encrypted);
            if (result.Contains(" ") || result.Contains("/") || result.Contains("*") || result.Contains("<") || result.Contains(">"))
                return Encrypt(Text, Key+"eryaz");
            else
                return result.Replace("+", "^");
        }
        public static byte[] PictureWatermark(Bitmap bmpFile, string watermarkText)
        {
            using (Bitmap bmp = bmpFile)
            {
                using (Graphics grp = Graphics.FromImage(bmp))
                {
                    //Set the Color of the Watermark text.
                    Brush brush = new SolidBrush(System.Drawing.Color.FromArgb(100, 255, 222, 173));
                    Brush brushS = new SolidBrush(System.Drawing.Color.FromArgb(80, 0, 0, 0));

                    float fontSize;

                    if (bmp.Width > bmp.Height)
                        fontSize = ((bmp.Height / 100) * 20);
                    else
                        fontSize = ((bmp.Width / 100) * 20);
                    
                    Font font = new System.Drawing.Font("Calibri", fontSize, System.Drawing.FontStyle.Bold, GraphicsUnit.Pixel);
                    SizeF textSize = grp.VisibleClipBounds.Size;

                    grp.TranslateTransform(textSize.Width / 2, textSize.Height / 2);
                    grp.RotateTransform(-45);

                    textSize = grp.MeasureString(watermarkText, font);

                    var v = (((textSize.Width / 2) / 100) * 2);
                    var z = (((textSize.Height / 2) / 100) * 2);

                    grp.DrawString(watermarkText, font, brushS, -((textSize.Width / 2) - v), -((textSize.Height / 2) - z));
                    grp.DrawString(watermarkText, font, brush, -(textSize.Width / 2), -(textSize.Height / 2));

                    grp.ResetTransform();
                    grp.Save();
                    brush.Dispose();
                    grp.Dispose();

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        bmp.Save(memoryStream, ImageFormat.Png);
                        memoryStream.Position = 0;
                       return ReadFully(memoryStream);
                    }
                }

            }
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

        public class ReturnValues
        {
            public string deleteType { get; set; }
            public string name { get; set; }
            public int size { get; set; }
            public string url { get; set; }
            public string thumbnailUrl { get; set; }
            public string deleteUrl { get; set; }
        }

    }
}