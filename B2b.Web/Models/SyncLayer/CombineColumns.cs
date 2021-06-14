using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.SyncLayer
{
    public class CombineColumns
    {
        #region Properties
        public DataColumns ExportColumns { get; set; }
        public DataColumns InsertColumns { get; set; }
        public bool Error { get; set; }
        public string ErrorMessage { get; set; }
        #endregion
        #region Methods
        internal static List<CombineColumns> GetCombineColumns(List<DataColumns> listInsertColumns, DataTable listExportColumns)
        {
            List<CombineColumns> list = new List<CombineColumns>();
            if (listInsertColumns.Count != listExportColumns.Columns.Count)
            {
                CombineColumns item = new CombineColumns()
                {
                    Error = true,
                    ErrorMessage = "Kolon sayıları tutmuyor! ---> Insert :" + listInsertColumns.Count.ToString() + " EXPORT : " + listExportColumns.Columns.Count.ToString()
                };
                list.Add(item);
            }
            else
            {
                for (int i = 0; i < listExportColumns.Columns.Count; i++)
                {
                    CombineColumns s = new CombineColumns
                    {
                        ExportColumns = new DataColumns
                        {
                            ExportDataField = listExportColumns.Columns[i].ColumnName,
                            //   ExportDataType = listExportColumns[i].ExportDataType
                        },
                        InsertColumns = new DataColumns
                        {
                            InsertDataField = listInsertColumns[i].InsertDataField,
                            InsertDataType = listInsertColumns[i].InsertDataType
                        }
                    };
                    list.Add(s);
                }

            }
            return list;
        }
        #endregion

    }
}