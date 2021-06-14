using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.Log;

namespace B2b.Web.v4.Controllers
{
    public class ComparisonController : BaseController
    {
        // GET: Comparison
        public ActionResult Index()
        {
             List<ProductComparison> list = ProductComparison.GetProductComparisonList(ComparisonOrFollowType.Comparison, CurrentLoginType, CurrentCustomer, (CurrentLoginType == LoginType.Salesman ? CurrentSalesman.Id : -1));
            ViewBag.ComparisonList = list;
            return View();
        }
        public PartialViewResult ComparisonProducts(int productId)
        {
            if (productId != -1)
            {
                ProductComparison.ComparisonInsert(productId, CurrentCustomer.Id, CurrentCustomer.Users.Id,
             CurrentLoginType == LoginType.Salesman ? CurrentSalesman.Id : -1, (int)ComparisonOrFollowType.Comparison,
             CurrentLoginType == LoginType.Salesman ? CurrentSalesman.Id : CurrentCustomer.Id);
            }

            List<ProductComparison> list = ProductComparison.GetProductComparisonPreviewList(ComparisonOrFollowType.Comparison, CurrentLoginType, CurrentCustomer, (CurrentLoginType == LoginType.Salesman ? CurrentSalesman.Id : -1));
            ViewBag.ComparisonList = list;
            return PartialView("ComparisonProducts", ViewBag);
        }


    }
}