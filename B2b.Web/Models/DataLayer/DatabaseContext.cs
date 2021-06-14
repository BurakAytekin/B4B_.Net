using B2b.Web.v4.Models.Helper;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Reflection;
using B2b.Web.v4.Models.Log;
using System.Data.SqlClient;

namespace B2b.Web.v4.Models.EntityLayer
{
    public class DatabaseContext
    {
        public static DataTable ExecuteReader(CommandType commandType, string commandText)
        {
            try
            {
                return Eryaz.Utility.MySqlHelper.ExecuteDataTable(GlobalSettings.ConnectionString, commandType, commandText);
            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.None, commandText.Length > 50 ? commandText.Substring(0, 49) : commandText, ex, string.Empty);
                return new DataTable();
            }
        }

        public static DataTable ExecuteReader(CommandType commandType, string commandText, object[] parameterNames, object[] parameterValues)
        {
            try
            {
                return Eryaz.Utility.MySqlHelper.ExecuteDataTable(GlobalSettings.ConnectionString, commandType, commandText, GenerateParamMysql(parameterNames, parameterValues));


            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.None, commandText.Length > 50 ? commandText.Substring(0, 49) : commandText, ex, string.Empty);
                return new DataTable();
            }
        }
        public static DataTable ExecuteReader(CommandType commandType, string commandText, ParameterInfo[] parameterNames, object[] parameterValues)
        {
            try
            {
                return Eryaz.Utility.MySqlHelper.ExecuteDataTable(GlobalSettings.ConnectionString, commandType, commandText, GenerateParam(parameterNames, parameterValues));
            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.None, commandText.Length > 50 ? commandText.Substring(0, 49) : commandText, ex, string.Empty);
                return new DataTable();
            }
        }

        public static DataTable ExecuteReaderReport(CommandType commandType, string commandText, MySqlParameter[] mysqlParams)
        {
            try
            {
                return Eryaz.Utility.MySqlHelper.ExecuteDataTable(GlobalSettings.ConnectionString, commandType, commandText, mysqlParams);
            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.None, commandText.Length > 50 ? commandText.Substring(0, 49) : commandText, ex, string.Empty);
                return new DataTable();
            }
        }


        public static DataSet ExecuteReaderDs(CommandType commandType, string commandText)
        {
            try
            {
                return Eryaz.Utility.MySqlHelper.ExecuteDataset(GlobalSettings.ConnectionString, CommandType.StoredProcedure, commandText);
            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.None, commandText.Length > 50 ? commandText.Substring(0, 49) : commandText, ex, string.Empty);
                return new DataSet();
            }
        }

        public static DataSet ExecuteReaderDs(CommandType commandType, string commandText, ParameterInfo[] parameterNames, object[] parameterValues)
        {
            try
            {
                return Eryaz.Utility.MySqlHelper.ExecuteDataset(GlobalSettings.ConnectionString, CommandType.StoredProcedure, commandText, GenerateParam(parameterNames, parameterValues));
            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.None, commandText.Length > 50 ? commandText.Substring(0, 49) : commandText, ex, string.Empty);
                return new DataSet();
            }
        }

        public static bool ExecuteNonQuery(CommandType commandType, string commandText, ParameterInfo[] parameterNames, object[] parameterValues)
        {
            try
            {
                Eryaz.Utility.MySqlHelper.ExecuteNonQuery(GlobalSettings.ConnectionString, CommandType.StoredProcedure, commandText, GenerateParam(parameterNames, parameterValues));
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.None, commandText.Length > 50 ? commandText.Substring(0, 49) : commandText, ex, string.Empty);
                return false;
            }
        }
        public static bool ExecuteNonQuery(CommandType commandType, string commandText, object[] parameterNames, object[] parameterValues)
        {
            try
            {
                Eryaz.Utility.MySqlHelper.ExecuteNonQuery(GlobalSettings.ConnectionString, commandType, commandText, GenerateParamMysql(parameterNames, parameterValues));
                return true;

            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.None, commandText.Length > 50 ? commandText.Substring(0, 49) : commandText, ex, string.Empty);
                return false;
            }
        }
        public static int ExecuteScalar(CommandType commandType, string commandText, ParameterInfo[] parameterNames, object[] parameterValues)
        {
            try
            {
                return Convert.ToInt16(Eryaz.Utility.MySqlHelper.ExecuteScalar(GlobalSettings.ConnectionString, CommandType.StoredProcedure, commandText, GenerateParam(parameterNames, parameterValues)));
            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.None, commandText.Length > 50 ? commandText.Substring(0, 49) : commandText, ex, string.Empty);
                return -2;
            }
        }


        public static bool ExecuteNonQuery(CommandType commandType, string commandText)
        {
            try
            {
                Eryaz.Utility.MySqlHelper.ExecuteNonQuery(GlobalSettings.ConnectionString, commandType, commandText);
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogGeneral(LogGeneralErrorType.Error, ClientType.None, commandText.Length > 50 ? commandText.Substring(0, 49) : commandText, ex, string.Empty);
                return false;
            }
        }




        public static MySqlParameter[] GenerateParam(ParameterInfo[] parameterNames, params object[] parameterValues)
        {
            int length = parameterNames.Length;
            MySqlParameter[] sqlParams = new MySqlParameter[length];

            for (int i = 0; i < length; i++)
            {
                MySqlParameter parameter = new MySqlParameter();
                parameter.ParameterName = "@" + parameterNames[i].Name;
                parameter.Value = parameterValues[i];
                sqlParams[i] = parameter;
            }

            return sqlParams;
        }

        public static MySqlParameter[] GenerateParamMysql(object[] parameterNames, params object[] parameterValues)
        {
            int length = parameterNames.Length;
            MySqlParameter[] sqlParams = new MySqlParameter[length];

            for (int i = 0; i < length; i++)
            {
                MySqlParameter parameter = new MySqlParameter();
                parameter.ParameterName = "@" + parameterNames[i];
                parameter.Value = parameterValues[i];
                sqlParams[i] = parameter;
            }

            return sqlParams;
        }

        public static MySqlParameter[] GenerateMysqlParamByArray(string[] parameterNames, string[] types, object[] parameterValues)
        {
            if (parameterNames == null) return null;

            int length = parameterNames.Length;
            MySqlParameter[] sqlParams = new MySqlParameter[length];

            DbType dType = DbType.String;
            object val = new object();

            for (int i = 0; i < length; i++)
            {
                if (types[i].Contains("checkbox"))
                {
                    dType = DbType.Int16;
                    val = Convert.ToInt16(Convert.ToBoolean(parameterValues[i]));
                }
                else if (types[i].Contains("datetime"))
                {
                    dType = DbType.DateTime;
                    val = Convert.ToDateTime(parameterValues[i]);
                }
                else
                {
                    dType = DbType.String;
                    val = parameterValues[i];
                }
                MySqlParameter parameter = new MySqlParameter();
                parameter.ParameterName = "@" + parameterNames[i];
                parameter.DbType = dType;
                parameter.Value = val;
                sqlParams[i] = parameter;
            }

            return sqlParams;
        }


        public static SqlParameter[] GenerateMsSqlParamByArray(string[] parameterNames, string[] types, object[] parameterValues)
        {
            int length = parameterNames.Length;
            SqlParameter[] sqlParams = new SqlParameter[length];


            DbType dType = DbType.String;
            object val = new object();

            for (int i = 0; i < length; i++)
            {
                if (types[i].Contains("checkbox"))
                {
                    dType = DbType.Int16;
                    val = Convert.ToInt16(Convert.ToBoolean(parameterValues[i]));
                }
                else if (types[i].Contains("datetime"))
                {
                    dType = DbType.DateTime;
                    val = Convert.ToDateTime(parameterValues[i]);
                }
                else
                {
                    dType = DbType.String;
                    val = parameterValues[i];
                }
                SqlParameter parameter = new SqlParameter();
                parameter.ParameterName = "@" + parameterNames[i];
                parameter.DbType = dType;
                parameter.Value = val;
                sqlParams[i] = parameter;
            }

            return sqlParams;
        }
    }
}