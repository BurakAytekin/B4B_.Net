using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class ProjectColorController : AdminBaseController
    {
        // GET: Admin/ProjectColor
        public ActionResult Index()
        {
          

            // Open the stream and read it back.
            //using (StreamReader sr = System.IO.File.OpenText(Server.MapPath(fileName)))
            //{
            //    string s = "ramazan";
            //    while ((s = sr.ReadLine()) != null)
            //    {
            //        Console.WriteLine(s);
            //    }
            //}


            return View();
        }

        [HttpPost]
        public JsonResult GetColorList()
        {
            List<B2bColor> list = B2bColor.GetList();
            return Json(list);
        }

        [HttpPost]
        public JsonResult AddOrUpdate(B2bColor selectedColor)
        {
            bool result = false;
            if (selectedColor != null)
            {
                if (selectedColor.Id < 1)
                {
                    selectedColor.CreateId = AdminCurrentSalesman.Id;
                    result = selectedColor.Add();
                }
                else
                {
                    selectedColor.EditId = AdminCurrentSalesman.Id;
                    result = selectedColor.Update();
                }
            }
            if (selectedColor.IsActive)
            {
                string fileName = "~/Files/Color2.less";

                if (System.IO.File.Exists(Server.MapPath(fileName)))
                {
                    System.IO.File.Delete(Server.MapPath(fileName));
                }

                // Create a new file @primary-color: #013d83;  @primarytwo-color: #e20a17;
                using (FileStream fs = System.IO.File.Create(Server.MapPath(fileName)))
                {
                    // Add some text to file
                    Byte[] title = new UTF8Encoding(true).GetBytes("@primary-color:" + selectedColor.Color1+";");
                    fs.Write(title, 0, title.Length);
                    byte[] author = new UTF8Encoding(true).GetBytes("@primarytwo-color:" + selectedColor.Color2 + "; @primary-menu-color:" + selectedColor.Color3 + ";");
                    fs.Write(author, 0, author.Length);
                }
            }

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }


        [HttpPost]
        public JsonResult DeleteColor(B2bColor selectedColor)
        {
            bool result = false;

            result = selectedColor.Update();

            var message = result ? new MessageBox(MessageBoxType.Success, "İşleminiz Gerçekleştirilmiştir .") : new MessageBox(MessageBoxType.Error, "İşleminizde Hata Gerçekleşmiştir.");
            return Json(message);
        }

    }
}