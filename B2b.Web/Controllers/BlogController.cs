using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;

namespace B2b.Web.v4.Controllers
{
    public class BlogController : BaseController
    {
        List<Blog> BlogList
        {
            get { return (List<Blog>)Session["BlogList"]; }
            set { Session["BlogList"] = value; }
        }

        Blog SelectedBlog
        {
            get { return (Blog)Session["SelectedBlog"]; }
            set { Session["SelectedBlog"] = value; }
        }

        // GET: Blog
        public ActionResult Index()
        {
            List<Blog> list = Blog.GetBlogCategoryList();
            list.Insert(0, new Blog() { Category = "TÜMÜ" });
            ViewBag.BlogCategory = list;

            return View();
        }


        public ActionResult BlogDetail()
        {
               ViewBag.CurrentCustomer = CurrentCustomer;
            if (Request.QueryString["detail"] == null)
                return null;

            try
            {
                int id = Convert.ToInt32(Request.QueryString["detail"]);
                SelectedBlog = BlogList.First(x => x.Id == id);
                ViewBag.BlogItem = SelectedBlog;
            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.Admin, "BlogDetail", ex, GetUserIpAddress(), CurrentCustomer.Id, CurrentCustomer.Users.Id, -1);
                return null;
            }

            return View();
        }
        #region   HttpPost Methods

        [HttpPost]
        public string GetBlogList(int dataCount, string category)
        {
               List<Blog> list = Blog.GetBlogList(category, dataCount, 10);

            if (dataCount == 0)
                BlogList = list;
            else
                BlogList.AddRange(list);

            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public string GetBlogCommentList()
        {
              List<BlogComment> list = BlogComment.GetBlogCommentList(CurrentCustomer.Id, CurrentCustomer.Users.Id, SelectedBlog.Id);
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public JsonResult AddComment(string header, string comment)
        {    BlogComment item = new BlogComment()
            {
                BlogId = SelectedBlog.Id,
                CustomerId = CurrentCustomer.Id,
                UserId = CurrentCustomer.Users.Id,
                SalesmanId = CurrentSalesmanId,
                Header = header,
                Content = comment,
                IsApproval = SelectedBlog.ApprovalComment
            };
            bool result = item.Add();


            string message = "";
            message = result ? "{\"statu\":\"success\",\"message\":\"Yorum eklenmiştir\"}" : "{\"statu\":\"error\",\"message\":\"Yorum eklemenizde bir hata gerçekleşmiştir\"}";

            return Json(message);
        }

        [HttpPost]
        public JsonResult DeleteBlogComment(int id)
        {
             BlogComment item = new BlogComment()
            {
                Id = id
            };
            item.EditId = CurrentEditId;
            bool result = item.Delete();


            string message = "";
            message = result ? "{\"statu\":\"success\",\"message\":\"Yorum silinmiştir\"}" : "{\"statu\":\"error\",\"message\":\"Yorum silmenizde bir hata gerçekleşmiştir\"}";

            return Json(message);
        }
        #endregion

    }
}