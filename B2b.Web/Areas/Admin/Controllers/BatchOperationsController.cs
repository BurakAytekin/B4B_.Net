using B2b.Web.v4.Areas.Admin.Models;
using B2b.Web.v4.Models.EntityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using B2b.Web.v4.Models.Log;
using Newtonsoft.Json;

namespace B2b.Web.v4.Areas.Admin.Controllers
{
    public class BatchOperationsController : AdminBaseController
    {
        // GET: Admin/BatchOperations
        public ActionResult Index()
        {
              return View();
        }

        #region HttpPost Methods
        [HttpPost]
        public string GetFieldList(string tableName, bool type)
        {
            List<BatchOperation> list = BatchOperation.GetList();
            list = list.Where(x => x.TableName.Contains(tableName)).ToList();
            if (type)
                list = list.Where(x => x.IsUpdatable == true).ToList();
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public JsonResult GenerateQuery(List<BatchOperation> fieldList, List<BatchOperation> changeFieldList, string table)
        {
             string query = "UPDATE ";
            query += table + " SET ";
            foreach (BatchOperation item in changeFieldList.OrderBy(x => x.PrioritySelected))
            {
                switch (item.ChangeType)
                {
                    case "0":
                        query += item.FieldName + " = " + GetFieldValue(item) + " ,";
                        break;
                    case "1":
                        query += item.FieldName + " = " + item.FieldSelection + " ,";
                        break;
                    case "2":
                        query += item.FieldName + " = CONCAT(" + item.FieldSelection + ",'" + item.FieldValue + "') ,";
                        break;
                    case "3":
                        query += item.FieldName + " = CONCAT('" + item.FieldValue + "'," + item.FieldSelection + ") ,";
                        break;
                }

            }

            query = query.Substring(0, query.Length - 1);

            List<string> ruleArray = new List<string>();

            if (fieldList.Count > 0)
            {
                query += " WHERE ";
                foreach (BatchOperation item in fieldList.OrderBy(x => x.PrioritySelected))
                {
                    if (item.FieldName == "(" || item.FieldName == ")")
                    {
                        ruleArray.Add(item.FieldName);
                    }

                    else
                    {
                        if (item.Process.Contains("LIKE"))
                        {
                            ruleArray.Add(item.FieldName);
                            ruleArray.Add(item.Process.Replace("{0}", item.FieldValue));
                        }
                        else if (item.Process.Contains("BETWEEN"))
                        {
                            ruleArray.Add(item.FieldName);
                            ruleArray.Add(item.Process);
                            ruleArray.Add(item.DateStartValue.ToString());
                            ruleArray.Add("AND");
                            ruleArray.Add(item.DateEndValue.ToString());
                        }
                        else
                        {
                            ruleArray.Add(item.FieldName);
                            ruleArray.Add(item.Process);
                            ruleArray.Add(GetFieldValue(item));
                        }
                    }
                    ruleArray.Add(item.ProcessType);
                }

                for (int i = 0; i < ruleArray.Count - 1; i++)
                {
                    if (i != 0 && (i + 1) < ruleArray.Count && ruleArray[i + 1] == ")")
                    {
                        ruleArray[i] = "";
                    }

                    query += " " + ruleArray[i];
                }


            }

            return Json("{\"statu\":\"success\",\"query\":\"" + query + "\"}");
        } 
        #endregion

        private string GetFieldValue(BatchOperation item)
        {
            switch (item.Type)
            {
                case BatchFieldType._Varchar:
                    return "'" + item.FieldValue + "'";

                case BatchFieldType._Integer:
                case BatchFieldType._Double:
                    return item.FieldValue;

                case BatchFieldType._DateTime:
                    return "'" + item.FieldValue + "'";

                default:
                    return "'" + item.FieldValue + "'";
            }


        }
    }
}