using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using B2b.Web.v4.Models.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.ErpLayer
{
    public class ErpOrderDetail
    {
        public string ProductCode { get; set; }
        public double Quantity { get; set; }

        public static List<ErpOrderDetail> GetListForMars(string header)
        {
            ErpFunctionDetail yearItem = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.MarsSevkEmri).Where(x => x.Settings.IsActiveCompany).First();

            GeneralParameters parametres = new GeneralParameters();
            parametres.CommandType = CommandType.StoredProcedure;
            parametres.CommandText = "Eryaz_MarsSevk_SiparisDetay";
            parametres.ParameterNames = new object[] { "pHeader" };
            parametres.ParameterValues = new object[] { header };

            DataTable dt = ErpHelper.FireServiceMethod(parametres, yearItem.Settings).ConvertResponseDataTable();
            List<ErpOrderDetail> list = dt.DataTableToList<ErpOrderDetail>();

            return list;
        }
    }
}