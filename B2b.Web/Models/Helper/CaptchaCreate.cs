using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Models.Helper
{
    public class CaptchaCreate: ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            Bitmap bitmap = new Bitmap(context.HttpContext.Server.MapPath("~/Content/images/codeback.jpg"));
            Graphics graphics = Graphics.FromImage(bitmap);
            string code = GetCaptchaString();
            SolidBrush brush = new SolidBrush(Color.White);
            graphics.DrawString(code, new Font("Pristina", 25, FontStyle.Bold), brush, new Point(10, 16));
            HttpResponseBase response = context.HttpContext.Response;
            bitmap.Save(response.OutputStream, ImageFormat.Gif);
            response.ContentType = "image/jpeg";
            context.HttpContext.Session["captcha"] = code;

        }

        public string GetCaptchaString()
        {
            return new Random().Next(10000, 99999).ToString();
        }
    }
}