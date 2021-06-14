using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class ReportController : AdminBaseController
    {
        // GET: Admin/Report
        public ActionResult Index()
        {
            return View();
        }

        #region Seri Takip
        public ActionResult Seritakip()
        {
            return View();
        }

        [HttpPost]
        public string GetMarsSeriList()
        {
            List<Mars_SerialNumber> list = Mars_SerialNumber.GetList();
            return JsonConvert.SerializeObject(list);
        }

        public ActionResult ExportExcelMarsSeriList()
        {
            List<Mars_SerialNumber> list = Mars_SerialNumber.GetList();

            byte[] fileContents;

            using (var pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("seri_numaralar");


                int col = 1;
                int row = 1;

                ws.Cells[row, col++].Value = "Stok Kodu";
                ws.Cells[row, col++].Value = "Seri No";
                ws.Cells[row, col++].Value = "Tarih";
                ws.Cells[row, col++].Value = "İrsaliye No";
                ws.Cells[row, col++].Value = "Müşteri";
                ws.Cells[row, col].Value = "İşlendi mi?";


                ws.View.FreezePanes(2, 1); //Dondur
                ws.Cells[1, 1, 1, col].Style.Font.Bold = true;
                ws.Cells[1, 1, 1, col].Style.Font.Size = 14;



                foreach (var item in list)
                {
                    row++;
                    col = 1;

                    ws.Cells[row, col++].Value = item.StokKodu;
                    ws.Cells[row, col++].Value = item.SeriNo;
                    ws.Cells[row, col].Style.Numberformat.Format = "dd.mm.yyyy";
                    ws.Cells[row, col++].Value = item.Tarih;
                    ws.Cells[row, col++].Value = item.EvrakNo;
                    ws.Cells[row, col++].Value = item.HesapKodu;
                    ws.Cells[row, col].Value = item.IsProcessed;
                }

                ws.Cells[1, 1, row, col].Style.WrapText = false;
                ws.Cells[1, 1, row, col].AutoFitColumns(3);
                
                fileContents = pck.GetAsByteArray();
            }

            string fName = "SERİ LİSTESİ " + DateTime.Now.ToString("yyyy-MM-dd ");
            return File(fileContents, "application/vnd.ms-excel", fName + ".xlsx");
        }

        #endregion
    }
}