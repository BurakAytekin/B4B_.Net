using B2b.Web.v4.Models.Helper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace B2b.Web.v4.Models.SyncLayer
{
    public class DbContext
    {
        internal static DataTable ExecuteCommand(string command, string connectionStr, DBType dbType)
        {
            DataTable dt = new DataTable();
            if (DBType.MySql == dbType)
            {
                MySqlDataAdapter msda = new MySqlDataAdapter(command, new MySqlConnection(connectionStr));
                msda.Fill(dt);
            }
            else if (DBType.MsSql == dbType)
            {
                SqlDataAdapter sda = new SqlDataAdapter(command, new SqlConnection(connectionStr));
                sda.Fill(dt);
            }
            return dt;
        }


        internal static DataType GetDataTypeForString(string pType, DBType dbType)
        {
            DataType DataTypee = new DataType();
            if (dbType == DBType.MySql)
            {
                switch (pType)
                {
                    case "int":
                        DataTypee = DataType.INT;
                        break;

                    case "datetime":
                        DataTypee = DataType.DATETIME;
                        break;

                    case "date":
                        DataTypee = DataType.DATE;
                        break;

                    case "char":
                        DataTypee = DataType.VARCHAR;
                        break;

                    case "mediumtext":
                        DataTypee = DataType.VARCHAR;
                        break;

                    case "longtext":
                        DataTypee = DataType.VARCHAR;
                        break;

                    case "double":
                        DataTypee = DataType.DOUBLE;
                        break;

                    case "float":
                        DataTypee = DataType.DOUBLE;
                        break;

                    default:
                        DataTypee = DataType.VARCHAR;
                        break;
                }
            }
            else if (dbType == DBType.MsSql)
            {
                switch (pType)
                {
                    case "int":
                        DataTypee = DataType.INT;
                        break;

                    case "varchar":
                        DataTypee = DataType.VARCHAR;
                        break;

                    case "float":
                        DataTypee = DataType.DOUBLE;
                        break;

                    case "datetime":
                        DataTypee = DataType.DATETIME;
                        break;

                    default:
                        DataTypee = DataType.VARCHAR;
                        break;
                }
            }
            return DataTypee;
        }

        internal static List<InsertCommand> GetInsertCommand(List<CombineColumns> listCombineColumn, DataTable dt, string insertTableName, int fragmentationNumber)
        {
            StringBuilder sb = new StringBuilder();
            List<InsertCommand> list = new List<InsertCommand>();
            bool Error = false;



            if (!Error)
            {
                int partCount = dt.Rows.Count / fragmentationNumber;
                int remaining = 0;

                string command = GenerateInsertIntoPart(listCombineColumn, insertTableName);

                if (partCount > 0)
                    remaining = dt.Rows.Count - (fragmentationNumber * partCount);

                if (partCount == 0)
                {
                    InsertCommand str = new InsertCommand
                    {
                        Error = false,
                        Message = string.Empty,
                        Command = command + GenerateInsertValuesPart(dt, 0, dt.Rows.Count, listCombineColumn),
                        InsertCount = dt.Rows.Count,
                    };
                    list.Add(str);
                }
                else
                {
                    for (int i = 0; i < partCount; i++)
                    {
                        int startIndex = i * fragmentationNumber;
                        int endIndex = (i + 1) * fragmentationNumber;
                        InsertCommand str = new InsertCommand
                        {
                            Error = false,
                            Message = string.Empty,
                            Command = command + GenerateInsertValuesPart(dt, startIndex, endIndex, listCombineColumn),
                            InsertCount = fragmentationNumber,
                        };
                        list.Add(str);
                    }

                    if (remaining > 0)
                    {
                        int startIndex = partCount * fragmentationNumber;
                        int endIndex = dt.Rows.Count;
                        InsertCommand str = new InsertCommand
                        {
                            Error = false,
                            Message = string.Empty,
                            Command = command + GenerateInsertValuesPart(dt, startIndex, endIndex, listCombineColumn),
                            InsertCount = remaining,
                        };
                        list.Add(str);
                    }
                }
            }
            return list;
        }


        private static string GenerateInsertIntoPart(List<CombineColumns> listCombineColumn, string insertTableName)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            sb.Append(insertTableName);
            sb.Append(" (");
            for (int i = 0; i < listCombineColumn.Count; i++)
            {
                if (i > 0) sb.Append(",");
                DataColumns ids = listCombineColumn[i].InsertColumns;
                sb.Append(ids.InsertDataField);
            }
            sb.Append(") ");
            return sb.ToString();
        }

        private static string GenerateInsertValuesPart(DataTable dt, int startIndex, int endIndex, List<CombineColumns> listCombineColumn)
        {
            // data yoksa
            if (endIndex == 0)
                return string.Empty;

            string values = "Values ";
            StringBuilder sb = new StringBuilder();
            for (int i = startIndex; i < endIndex; i++)
            {
                sb.Append("(");
                for (int j = 0; j < listCombineColumn.Count; j++)
                {
                    DataColumns ids = listCombineColumn[j].InsertColumns;
                    DataColumns eds = listCombineColumn[j].ExportColumns;
                    string value = string.Empty;

                    switch (ids.InsertDataType)
                    {
                        case DataType.VARCHAR:
                            {
                                value = dt.Rows[i][eds.ExportDataField] is DBNull ? "-" : Replace(dt.Rows[i][eds.ExportDataField].ToString());
                                value = RemoveSpecialCharacters(value.Trim());
                                break;
                            }
                        case DataType.INT:
                            {
                                value = dt.Rows[i][eds.ExportDataField] is DBNull ? "0" : Replace(dt.Rows[i][eds.ExportDataField].ToString());
                                value = value.Trim();
                                try
                                {
                                    double test = Convert.ToDouble(value);
                                    int test1 = Convert.ToInt32(test);
                                    value = test1.ToString();
                                }
                                catch (Exception)
                                {
                                    value = "-1";
                                }
                            }
                            break;
                        case DataType.DOUBLE:
                            {
                                value = dt.Rows[i][eds.ExportDataField] is DBNull ? "0.0" : ReplaceDouble(dt.Rows[i][eds.ExportDataField].ToString());
                                value = value.Trim();
                            }
                            break;
                        case DataType.DATETIME:
                            {
                                value = dt.Rows[i][eds.ExportDataField] is DBNull ? "1990.01.01" : ReplaceDateTime(dt.Rows[i][eds.ExportDataField].ToString());
                                DateTime time = Convert.ToDateTime(value);
                                value = time.ToString("dd.MM.yyyy");
                            }
                            break;
                        case DataType.DATE:
                            {
                                value = dt.Rows[i][eds.ExportDataField] is DBNull ? "1990.01.01" : dt.Rows[i][eds.ExportDataField].ToString().Replace("AM", "");
                                DateTime time = Convert.ToDateTime(value);
                                //Value = time.ToString("dd.MM.yyyy");
                                value = time.Year + "." + time.Month + "." + time.Day;
                            }
                            break;
                        default:
                            {
                                value = dt.Rows[i][eds.ExportDataField] is DBNull ? "-" : Replace(dt.Rows[i][eds.ExportDataField].ToString());

                            }
                            break;
                    }

                    value = RemoveSpecialCharacters(value.Trim());
                    sb.Append(string.Format("'{0}'", value));

                    if (j < (listCombineColumn.Count - 1))
                    {
                        sb.Append(",");
                    }
                }

                sb.Append(")");
                if (i < endIndex - 1)
                    sb.Append(",");
            }

            values += sb.ToString();
            return values;

        }


        private static string RemoveSpecialCharacters(string str)
        {
            return str.Replace("\n", "")
                .Replace("\t", "")
                .Replace("Ý", "")
                .Replace("~", "")
                //.Replace("\"", "")
                .Replace("'", "")
                //.Replace("\\", "")
                //.Replace("/", "")
                .Replace("Þ", "")
                .Replace("Ð", "")
                .Replace("…", "")
                .Replace("Š", "");
            //.Replace("’", "");
        }

        private static string ReplaceDateTime(string Value)
        {
            string newDate = "";
            if (Value != "")
            {
                string yil = Value.Substring(6, 4);
                string ay = Value.Substring(3, 2);
                string gun = Value.Substring(0, 2);
                newDate = yil + "." + ay + "." + gun;
            }
            return newDate;
        }

        private static string ReplaceDouble(string p)
        {
            return p.Replace(",", ".");
        }

        private static string Replace(string p)
        {
            return p.Replace("'", "");
        }


        internal static void TruncateTable(string TableName)
        {
            MySqlConnection _mCon = new MySqlConnection(GlobalSettings.ConnectionString);
            MySqlCommand mc = new MySqlCommand("TRUNCATE " + TableName, _mCon);
            mc.CommandTimeout = 0;
            mc.CommandType = CommandType.Text;
            _mCon.Open();
            mc.ExecuteNonQuery();
            _mCon.Close();
        }


        internal static void ExecuteStoredProcedure(string StoreProcedureName, DBType dbType)
        {
            if (dbType == DBType.MySql)
            {
                using (MySqlConnection conn = new MySqlConnection(GlobalSettings.ConnectionString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(StoreProcedureName, conn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            else if (dbType == DBType.MsSql)
            {
                using (SqlConnection conn = new SqlConnection(GlobalSettings.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(StoreProcedureName, conn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
        }

    }
    public enum DataType
    {
        VARCHAR,
        INT,
        DOUBLE,
        DATETIME,
        DATE
    }
    public enum DBType
    {
        MySql,
        MsSql,
        Oracle
    }

}