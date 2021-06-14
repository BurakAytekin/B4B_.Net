using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace B2b.Web.v4.Models.Helper
{
    public static class HelperFunctions
    {

        //public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        //{
        //    try
        //    {
        //        List<T> list = new List<T>();

        //        foreach (var row in table.AsEnumerable())
        //        {
        //            T obj = new T();

        //            foreach (var prop in obj.GetType().GetProperties())
        //            {
        //                try
        //                {
        //                    PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
        //                    propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
        //                }
        //                catch
        //                {
        //                    continue;
        //                }
        //            }

        //            list.Add(obj);
        //        }

        //        return list;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
        public static List<T> DataTableToList<T>(this DataTable datatable) where T : class, new()
        {

            List<T> Temp = new List<T>();
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn DataColumn in datatable.Columns)
                    columnsNames.Add(DataColumn.ColumnName);
                Temp = datatable.AsEnumerable().ToList().ConvertAll<T>(row => getObject<T>(row, columnsNames));
                return Temp;
            }
            catch
            {
                return null;
            }
        }

        public static T getObject<T>(DataRow row, List<string> columnsName) where T : new()
        {
            T obj = new T();
            try
            {
                string columnname = "";
                string value = "";
                PropertyInfo[] Properties;
                Properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in Properties)
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                            }
                            else
                            {
                                value = row[columnname].ToString().Replace("%", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                            }
                        }
                    }
                }
                return obj;
            }
            catch
            {
                return obj;
            }
        }




        public static T DataTableToItem<T>(this DataTable table) where T : class, new()
        {
            try
            {
                T list = new T();

                foreach (var row in table.AsEnumerable())
                {
                    T obj = new T();

                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        try
                        {
                            PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                            propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    list = obj;
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        public static string CurrencyHtml(this string retval)
        {
            switch (retval)
            {
                case "TL":
                    retval = "&nbsp;<i class=\"fa fa-try\" aria-hidden=\"true\"></i>";
                    break;
                case "USD":
                    retval = "&nbsp;<i class=\"fa fa-usd\" aria-hidden=\"true\"></i>";
                    break;
                case "EUR":
                    retval = "&nbsp;<i class=\"fa fa-eur\" aria-hidden=\"true\"></i>";
                    break;
                case "GBP":
                    retval = "&nbsp;<i class=\"fa fa-gbp\" aria-hidden=\"true\"></i>";
                    break;
                case "CNY":
                case "JPY":
                case "RMB":
                case "YEN":
                    retval = "&nbsp;<i class=\"fa fa-jpy\" aria-hidden=\"true\"></i>";
                    break;
                case "RUB":
                case "RUBLE":
                    retval = "&nbsp;<i class=\"fa fa-rub\" aria-hidden=\"true\"></i>";
                    break;
                default:
                    break;
            }
            return retval;
        }

        public static DataTable ConvertResponseDataTable(this string response)
        {
            try
            {
                return JsonConvert.DeserializeObject<Tuple<bool, DataTable, string>>(response).Item2;
            }
            catch (Exception)
            {
                return new DataTable();
            }
        }

        public static bool ConvertResponseBoolen(this string response)
        {
            try
            {
                return JsonConvert.DeserializeObject<Tuple<bool, DataTable, string>>(response).Item1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GenerateCommaListString(this string[] list)
        {
            StringBuilder listStr = new StringBuilder();

            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(item) && item != "Seçiniz")
                {
                    if (listStr.Length > 0)
                        listStr.Append(",");
                    listStr.Append("'" + item + "'");
                }
            }

            return listStr.ToString();
        }
    }
}