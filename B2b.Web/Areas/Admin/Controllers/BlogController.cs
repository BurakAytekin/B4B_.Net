using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
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
    public class BlogController : AdminBaseController
    {
        // GET: Admin/Blog
        public ActionResult Index()
        {
            return View();
        }

        #region HttpPost Methods
        [HttpPost]
        public string GetBlogList()
        {
            List<Blog> list = Blog.GetBlogListByAll();
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string GetBlogCommentList(int id)
        {
            List<BlogComment> list = BlogComment.GetBlogCommentListById(id);
            return JsonConvert.SerializeObject(list);
        }


        [HttpPost]
        public JsonResult DeleteBlog(int id)
        {
             bool result = false;
            Blog item = new Blog()
            {
                EditId = AdminCurrentSalesman.Id,
                Id = id,
            };
            result = item.Delete();


            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        [HttpPost]
        public JsonResult DeleteBlogComment(int id)
        {
            bool result = false;
            BlogComment item = new BlogComment()
            {
                EditId = AdminCurrentSalesman.Id,
                Id = id,
            };
            result = item.Delete();


            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        [HttpPost]
        public JsonResult ApproveBlogComment(int id)
        {
             bool result = false;
            BlogComment item = new BlogComment()
            {
                EditId = AdminCurrentSalesman.Id,
                Id = id,
                IsApproval = true,
            };
            result = item.Update();


            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        [HttpPost]
        public JsonResult AddBlogComment(int blogId, string content)
        {
            bool result = false;
            BlogComment item = new BlogComment()
            {
                CreateId = AdminCurrentSalesman.Id,
                IsApproval = true,
                Content = content,
                CustomerId = -1,
                SalesmanId = AdminCurrentSalesman.Id,
                UserId = -1,
                Header = string.Empty,
                BlogId = blogId
            };
            result = item.Add();


            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

        [HttpPost]
        public JsonResult SaveBlog(int edit, int id, string category, string header, string videoUrl, bool isShowCommentUser, bool approvalComment, int type, string content, string imageBase, string shortContent, bool isLockComment)//
        {
            bool result = false;
            string path = string.Empty;
            bool pictureUpload = false;
            if (type == 0 && !String.IsNullOrEmpty(imageBase))
            {

                string filename = Guid.NewGuid().ToString();
                string imgType = string.Empty;

                try
                {
                    imgType = GetFileType(imageBase);

                    string fullFtpFilePath = GlobalSettings.FtpServerUploadAddress + GlobalSettings.GeneralPath + filename + "." + imgType;
                    byte[] fileData = Parse(imageBase);
                    pictureUpload = FtpHelper.UploadRemoteServer(fileData, fullFtpFilePath);

                }
                catch (Exception ex)
                {
                    pictureUpload = false;
                }

                path =  GlobalSettings.GeneralPath + filename + "." + imgType;

            }
            else
            {
                path = videoUrl;
                pictureUpload = true;
            }

            if (pictureUpload)
            {
                Blog item = new Blog()
                {
                    Id = edit == 0 ? 0 : id,
                    Category = category,
                    SalesmanId = AdminCurrentSalesman.Id,
                    Header = header,
                    Path = path,
                    Type = type,
                    IsShowCommentUser = isShowCommentUser,
                    ApprovalComment = approvalComment,
                    Content = content,
                    CreateId = AdminCurrentSalesman.Id,
                    EditId = AdminCurrentSalesman.Id,
                    ShortContent = shortContent,
                    IsLockComment = isLockComment
                };
                result = edit == 0 ? item.Add() : item.Update();
            }


            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        #endregion


    }
}