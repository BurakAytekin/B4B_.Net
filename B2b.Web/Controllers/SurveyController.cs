using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Controllers
{
    public class SurveyController : BaseController
    {
        // GET: Survey
        public ActionResult Index()
        {
            List<SurveyCs> list = SurveyCs.GetList();
            return View(list);
        }

        public ActionResult Detail(int Id)
        {
            SurveyCs survey = SurveyCs.GetItemById(Id);
            return View(survey);
        }
        [HttpPost]
        public ActionResult Detail(SurveyCs survey)
        {
            if (ModelState.IsValid)
            {
                bool result = false;

                foreach (var item in survey.Questions)
                {
                    result = SurveyCs.SaveAnswer(survey.Id, item.Id, CurrentCustomer.Id, item.Answer);
                }

                if (result)
                    ViewBag.Result = "Success";
                else
                    ViewBag.Result = "Error";
            }
            else
            {
                foreach (var item in survey.Questions)
                {
                    item.Options = SurveyQuestionOptions.GetById(item.Id);
                }
                ViewBag.Result = "Error";
            }
            return View(survey);
        }
    }
}