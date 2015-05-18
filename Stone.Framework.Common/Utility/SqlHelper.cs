using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace Stone.Framework.Common.Utility
{
    public sealed class SqlHelper
    {
        #region 私有的变量和构造函数

        private SqlHelper()
        {
        }

        /// <summary>
        ///     方法被用来创建SqlParameters的数组一个SqlCommand
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandParameters"></param>
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null)
                throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (var p in commandParameters)
                {
                    if (p != null)
                    {
                        if (p.Direction == ParameterDirection.InputOutput || (p.Direction == ParameterDirection.Input) && (p.Value != null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        /// <summary>
        ///     这种方法分配的DataRow列值SqlParameters数组
        /// </summary>
        /// <param name="commandParameters">Array of SqlParameters to be assigned values</param>
        /// <param name="dataRow">The dataRow used to hold the stored procedure's parameter values</param>
        private static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
        {
            if ((commandParameters == null) || dataRow == null)
            {
                return;
            }

            var i = 0;
            foreach (var commandParameter in commandParameters)
            {
                if (commandParameter.ParameterName == null || commandParameter.ParameterName.Length <= 1)
                {
                    throw new Exception(
                        string.Format(
                            "Please provide a vaild parameter name on the parameter #{0},the ParameterName property has the following value:{1}", i,
                            commandParameter.ParameterName));
                }

                if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                {
                    commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                }
                i++;
            }
        }

        /// <summary>
        ///     This method assigns an array of values to an array of SqlParameters
        /// </summary>
        /// <param name="commandParameters">Array of SqlParameters to be assigned values</param>
        /// <param name="parameterValues">Array of objects holding the values to be assigned</param>
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            if ((commandParameters == null) || (parameterValues == null))
            {
                return;
            }

            if (commandParameters.Length != parameterValues.Length)
            {
                throw new ArgumentException("Parameter count does not match Paramater Value count.");
            }

            for (int i = 0, j = commandParameters.Length; i < j; i++)
            {
                if (parameterValues[i] is IDbDataParameter)
                {
                    var paramInstance = (IDbDataParameter)parameterValues[i];

                    if (paramInstance.Value == null)
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = paramInstance.Value;
                    }
                }
                else if (parameterValues[i] == null)
                {
                    commandParameters[i].Value = DBNull.Value;
                }
                else
                {
                    commandParameters[i].Value = parameterValues[i];
                }
            }
        }

        /// <summary>
        ///     配置 SqlConnection，SqlCommand，transaction，parameters
        ///     to the provided command
        /// </summary>
        /// <param name="command">The SqlCommand to be prepared</param>
        /// <param name="connection">A valid SqlConnection, on which to execute this command</param>
        /// <param name="transaction">A valid SqlTransaction, or 'null'</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">
        ///     An array of SqlParameters to be associated with the command or 'null' if no parameters
        ///     are required
        /// </param>
        /// <param name="mustCloseConnection"><c>true</c> if the connection was opened by the method, otherwose is false.</param>
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText,
            SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");

            if (connection.State == ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            command.Connection = connection;

            command.CommandText = commandText;

            if (transaction != null)
            {
                if (transaction.Connection == null)
                {
                    throw new ArgumentNullException("The transaction was rollbacked or commited,please provide an open transaction", "transaction");
                    command.Transaction = transaction;
                }
            }

            command.CommandType = commandType;

            if (commandParameters != null)
                AttachParameters(command, commandParameters);
        }

        #endregion 私有的变量和构造函数

        #region ExecuteNonQuery

        /// <summary>
        ///     增，删，改方法 （没有参数）
        ///     the connection string
        /// </summary>
        /// <remarks>
        ///     e.g.:
        ///     int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connectionString, commandType, commandText, null);
        }

        /// <summary>
        ///     Execute a SqlCommand (that returns no resultset) against the database specified in the connection string
        ///     using the provided parameters
        /// </summary>
        /// <remarks>
        ///     e.g.:
        ///     int result = ExecuteNonQuery(connString, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid",
        ///     24));
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
        }

        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (string.IsNullOrEmpty("spName"))
                throw new ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        /// <summary>
        /// Execute a SqlCommand (that returns no resultset and takes no parameters) against the provided SqlConnection. 
        /// </summary>
        /// <remarks>
        /// e.g.:  
        ///  int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders");
        /// </remarks>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
        }

        /// <summary>
        ///     Execute a SqlCommand (that returns no resultset) against the specified SqlConnection
        ///     using the provided parameters.
        /// </summary>
        /// <remarks>
        ///     e.g.:
        ///     int result = ExecuteNonQuery(conn, CommandType.StoredProcedure, "PublishOrders", new SqlParameter("@prodid", 24));
        /// </remarks>
        /// <param name="connection">A valid SqlConnection</param>
        /// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters used to execute the command</param>
        /// <returns>An int representing the number of rows affected by the command</returns>
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            var cmd = new SqlCommand();
            var mustCloseConnection = false;
            PrepareCommand(cmd, connection, null, commandType, commandText, commandParameters, out mustCloseConnection);
            var retVal = cmd.ExecuteNonQuery();

            cmd.Parameters.Clear();

            if (mustCloseConnection)
            {
                connection.Close();
            }
            return retVal;
        }

        public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(spName))
                throw new ArgumentNullException("spName");
            if ((parameterValues != null) && parameterValues.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
            }
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteNonQuery(transaction, commandType, commandText);
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited,please provide an open transaction", "transaction");

            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);
            int retVal = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return retVal;
        }

        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited,please provide an open transaction", "transaction");
            if (string.IsNullOrEmpty(spName))
                throw new ArgumentNullException("spName");
            if (parameterValues != null && parameterValues.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteNonQuery(transaction.Connection, CommandType.StoredProcedure, spName);
            }

        }


        #endregion

        #region ExecuteDataSet

        public static DataSet ExecuteDataSet(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataSet(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        public static DataSet ExecuteDataSet(string connectionString, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                return ExecuteDataSet(connection, commandType, commandText, commandParameters);
            }
        }

        public static DataSet ExecuteDataSet(string connectionString, string spName, params object[] parameterValues)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (string.IsNullOrEmpty(spName))
                throw new ArgumentNullException("spName");

            if (parameterValues != null && parameterValues.Length > 0)
            {
                SqlParameter[] commandparameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandparameters, parameterValues);

                return ExecuteDataSet(connectionString, CommandType.StoredProcedure, spName, commandparameters);
            }
            else
            {
                return ExecuteDataSet(connectionString, CommandType.StoredProcedure, spName);
            }
        }

        public static DataSet ExecuteDataSet(SqlConnection connection, CommandType commandType, string commandText)
        {
            return ExecuteDataSet(connection, commandType, commandText, (SqlParameter[])null);
        }

        public static DataSet ExecuteDataSet(SqlConnection connection, CommandType commandType, string commandText, SqlParameter[] commandParameters)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();

                if (mustCloseConnection)
                    connection.Close();
                return ds;
            }
        }

        public static DataSet ExecuteDataSet(SqlConnection connection, string spName, params object[] parameterValues)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(spName))
                throw new ArgumentNullException("spName");

            if (parameterValues != null && parameterValues.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connection, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteDataSet(connection, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataSet(connection, CommandType.StoredProcedure, spName);
            }
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");

            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataSet ds = new DataSet();
                da.Fill(ds);
                cmd.Parameters.Clear();
                return ds;
            }
        }

        public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");
            if (transaction != null && transaction.Connection == null)
                throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            if (string.IsNullOrEmpty(spName))
                throw new ArgumentNullException("spName");
            if ((parameterValues != null) && (parameterValues.Length > 0))
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                AssignParameterValues(commandParameters, parameterValues);
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
            }
        }

        #endregion

        #region ExecuteReader

        private enum SqlConnectionOwnership
        {
            /// <summary>Connection is owned and managed by SqlHelper</summary>
            Internal,
            /// <summary>Connection is owned and managed by the caller</summary>
            External
        }

        public static SqlDataReader ExecuteDataReader(string connectionString, CommandType commandType, string commandText)
        {
            return ExecuteDataReader(connectionString, commandType, commandText, (SqlParameter[])null);
        }

        public static SqlDataReader ExecuteDataReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                return ExecuteDataReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static SqlDataReader ExecuteDataReader(string connectionString, SqlTransaction transaction, CommandType commandType, string commandText,
            SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");

            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();

                return ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
            }
            catch
            {
                if (connection != null)
                    connection.Close();
                throw;
            }
        }

        private static SqlDataReader ExecuteDataReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText,
            SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            bool mustCloseConnection = false;

            SqlCommand cmd = new SqlCommand();
            try
            {
                PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, out mustCloseConnection);

                SqlDataReader dataReader;

                if (connectionOwnership == SqlConnectionOwnership.External)
                {
                    dataReader = cmd.ExecuteReader();
                }
                else
                {
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }

                bool canClear = true;

                foreach (SqlParameter commandParameter in cmd.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                    {
                        canClear = false;
                    }
                }

                if (canClear)
                {
                    cmd.Parameters.Clear();
                }

                return dataReader;
            }
            catch
            {

                if (mustCloseConnection)
                    connection.Close();
                throw;
            }
        }

        public static SqlDataReader ExecuteDataReader(string connectionString, string spName, params object[] parameterValues)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (string.IsNullOrEmpty("spName"))
                throw new ArgumentNullException("spName");
            if (parameterValues != null && parameterValues.Length > 0)
            {
                SqlParameter[] commandParameters = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);

                AssignParameterValues(commandParameters, parameterValues);

                return ExecuteDataReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
            }
            else
            {
                return ExecuteDataReader(connectionString, CommandType.StoredProcedure, spName);
            }
        }


        #endregion
    }

    /// <summary>
    ///     SqlHelperParameterCache提供的功能，以利用过程参数的静态缓存，并且发现为在运行时存储的过程参数的能力。
    /// </summary>
    public sealed class SqlHelperParameterCache
    {
        #region 私有的方法，变量和构造函数

        private SqlHelperParameterCache()
        {
        }

        private static readonly Hashtable paramCache = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="spName"></param>
        /// <param name="includeReturnValueParameter">是否不包括其返回值参数</param>
        /// <returns></returns>
        private static SqlParameter[] DiscoverSqlParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(spName))
                throw new ArgumentNullException("spName");

            var cmd = new SqlCommand(spName, connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            SqlCommandBuilder.DeriveParameters(cmd);
            connection.Close();

            if (!includeReturnValueParameter)
            {
                cmd.Parameters.RemoveAt(0);
            }

            var discoveredParameters = new SqlParameter[cmd.Parameters.Count];
            cmd.Parameters.CopyTo(discoveredParameters, 0);
            foreach (var discoverParameter in discoveredParameters)
            {
                discoverParameter.Value = DBNull.Value;
            }
            return discoveredParameters;
        }

        /// <summary>
        ///     缓存的SqlParameter数组的深层副本
        /// </summary>
        /// <param name="originalParameters"></param>
        /// <returns></returns>
        private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            var clonedParameters = new SqlParameter[originalParameters.Length];

            for (int i = 0, j = originalParameters.Length; i < j; i++)
            {
                clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
            }

            return clonedParameters;
        }

        #endregion 私有的方法，变量和构造函数

        #region 缓存参数

        /// <summary>
        ///     Add parameter array to the cache
        /// </summary>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="commandText">The stored procedure name or T-SQL command</param>
        /// <param name="commandParameters">An array of SqlParamters to be cached</param>
        public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");

            var hashKey = connectionString + ":" + commandText;

            paramCache[hashKey] = commandParameters;
        }

        /// <summary>
        ///     Get parameter array to the cache
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static SqlParameter[] GetCacheParameterSet(string connectionString, string commandText)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            if (string.IsNullOrEmpty(commandText))
                throw new ArgumentNullException("commandText");

            var hashKey = connectionString + ":" + commandText;

            var cacheParameters = paramCache[hashKey] as SqlParameter[];

            if (cacheParameters == null)
            {
                return null;
            }
            return CloneParameters(cacheParameters);
        }

        #endregion 缓存参数

        #region 获取参数

        /// <summary>
        ///     Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        ///     This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of SqlParameters</returns>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            return GetSpParameterSet(connectionString, spName, false);
        }

        /// <summary>
        ///     Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        ///     This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connectionString">A valid connection string for a SqlConnection</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">
        ///     A bool value indicating whether the return value parameter should be included
        ///     in the results
        /// </param>
        /// <returns>An array of SqlParameters</returns>
        private static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString");
            using (var connection = new SqlConnection(connectionString))
            {
                return GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        ///     Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        ///     This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <returns>An array of SqlParameters</returns>
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
        {
            return GetSpParameterSet(connection, spName, false);
        }

        /// <summary>
        ///     Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <remarks>
        ///     This method will query the database for this information, and then store it in a cache for future requests.
        /// </remarks>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">
        ///     A bool value indicating whether the return value parameter should be included
        ///     in the results
        /// </param>
        /// <returns>An array of SqlParameters</returns>
        internal static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            using (var clonedConnction = (SqlConnection)((ICloneable)connection).Clone())
            {
                return GetSpParameterSetInternal(clonedConnction, spName, includeReturnValueParameter);
            }
        }

        /// <summary>
        ///     Retrieves the set of SqlParameters appropriate for the stored procedure
        /// </summary>
        /// <param name="connection">A valid SqlConnection object</param>
        /// <param name="spName">The name of the stored procedure</param>
        /// <param name="includeReturnValueParameter">
        ///     A bool value indicating whether the return value parameter should be included
        ///     in the results
        /// </param>
        private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            if (connection == null)
                throw new ArgumentNullException("connection");
            if (string.IsNullOrEmpty(spName))
                throw new ArgumentNullException("spName");

            var hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter ? ":inculde ReturnValue Parameter" : "");

            SqlParameter[] cacheParameters;
            cacheParameters = paramCache[hashKey] as SqlParameter[];

            if (cacheParameters != null)
            {
                var spParameters = DiscoverSqlParameterSet(connection, spName, includeReturnValueParameter);
                paramCache[hashKey] = spParameters;
                cacheParameters = spParameters;
            }
            return cacheParameters;
        }

        #endregion 获取参数
    }
}