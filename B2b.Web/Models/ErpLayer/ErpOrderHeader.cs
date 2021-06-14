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
    public class ErpOrderHeader
    {
        public string DocumentNo { get; set; }
        public string CustomerCode { get; set; }
        public DateTime DocumentDate { get; set; }
        public string TeslimatAdresi { get; set; }
        public string Depokodu { get; set; }

        public static List<ErpOrderHeader> GetListForMars()
        {
            ErpFunctionDetail yearItem = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.MarsSevkEmri).Where(x => x.Settings.IsActiveCompany).First();

            GeneralParameters parametres = new GeneralParameters();
            parametres.CommandType = CommandType.StoredProcedure;
            parametres.CommandText = "Eryaz_MarsSevk_SiparisBaslik";

            DataTable dt = ErpHelper.FireServiceMethod(parametres, yearItem.Settings).ConvertResponseDataTable();
            List<ErpOrderHeader> list = dt.DataTableToList<ErpOrderHeader>();

            return list;
        }

        public bool UpdateAsGenerated()
        {
            ErpFunctionDetail yearItem = ErpFunctionDetail.GetActiveDetailList((int)ErpFunctionTypeEnum.MarsSevkEmri).Where(x => x.Settings.IsActiveCompany).First();

            GeneralParameters parametres = new GeneralParameters();
            parametres.CommandType = CommandType.StoredProcedure;
            parametres.CommandText = "Eryaz_MarsSevk_Update_SiparisBaslik_SevkGenerated";
            parametres.ParameterNames = new object[] { "pHeader" };
            parametres.ParameterValues = new object[] { DocumentNo };

            DataTable dt = ErpHelper.FireServiceMethod(parametres, yearItem.Settings).ConvertResponseDataTable();

            return true;
        }
    }
}