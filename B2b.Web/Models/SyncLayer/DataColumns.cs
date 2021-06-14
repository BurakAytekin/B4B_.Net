using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.SyncLayer
{
    public class DataColumns
    {

        #region Properties
        public string InsertDataField { get; set; }
        public string ExportDataField { get; set; }
        public DataType InsertDataType { get; set; }
        #endregion


        #region Methods
        internal static List<DataColumns> GetColumns(string tableName, string connStr, string dbName)
        {
            try
            {
                DataTable dt = DbContext.ExecuteCommand("SELECT * FROM  information_schema.COLUMNS WHERE TABLE_NAME='" + tableName + "' AND TABLE_SCHEMA= '" + dbName + "'; ", connStr, DBType.MySql);
                List<DataColumns> list = new List<DataColumns>();
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["EXTRA"].ToString() != "auto_increment")
                    {
                        if (dr["COLUMN_COMMENT"].ToString() != "NONE")
                        {
                            DataColumns item = new DataColumns
                            {
                                InsertDataField = dr["COLUMN_NAME"].ToString(),
                                InsertDataType = DbContext.GetDataTypeForString(dr["DATA_TYPE"].ToString(), DBType.MySql)

                            };
                            list.Add(item);
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

    }

    public class InsertCommand
    {
        public string Command { get; set; }
        public bool Error { get; set; }
        public string Message { get; set; }
        public int InsertCount { get; set; }

        public InsertCommand()
        {
            Command = string.Empty;
            Error = false;
            Message = string.Empty;
            InsertCount = 0;
        }
    }
}