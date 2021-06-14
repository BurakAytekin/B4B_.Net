using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class BatchOperation : DataAccess
    {
        public BatchOperation()
        {
            TableName = string.Empty;
            FieldName = string.Empty;
            FieldText = string.Empty;
            Explanation = string.Empty;
            FieldValue = string.Empty;
            ProcessType = string.Empty;
            IsEqual = true;
        }


        #region Properties
        public int Id { get; set; }
        public string TableName { get; set; }
        public string FieldName { get; set; }
        public string FieldText { get; set; }
        public BatchFieldType Type { get; set; }
        public string Explanation { get; set; }
        public double MinValue { get; set; }
        public bool IsUpdatable { get; set; }
        public int Priority { get; set; }
        public int PrioritySelected { get; set; }
        public string FieldValue { get; set; }
        public string Process { get; set; }
        public string ProcessType { get; set; }
        public bool IsEqual { get; set; }
        public DateTime DateStartValue { get; set; }
        public DateTime DateEndValue { get; set; }
        public string FieldSelection { get; set; }
        public string ChangeType { get; set; }
        public bool IsSave { get; set; }
        #endregion

        #region Methods

        public  static List<BatchOperation> GetList()
        {
            List<BatchOperation> list = new List<BatchOperation>();
            DataTable dt = DAL.GetBatchOperationList();

            foreach (DataRow row in dt.Rows)
            {
                BatchOperation obj = new BatchOperation()
                {
                    Id = row.Field<int>("Id"),
                    TableName = row.Field<string>("TableName"),
                    FieldName = row.Field<string>("FieldName"),
                    FieldText = row.Field<string>("FieldText"),
                    Explanation = row.Field<string>("Explanation"),
                    Type = (BatchFieldType)row.Field<int>("Type"),
                    IsUpdatable = row.Field<bool>("IsUpdatable"),
                    Priority = row.Field<int>("Priority")
                };
                list.Add(obj);
            }

            return list;

        }

        #endregion

    }

    public partial class DataAccessLayer
    {
        public DataTable GetBatchOperationList()
        {
            return DatabaseContext.ExecuteReader(CommandType.StoredProcedure, "_GetList_BatchOperation");
        }
    }

        public enum BatchFieldType
    {
        _Varchar = 0,
        _Integer = 1,
        _Double = 2,
        _DateTime= 3
    }
}