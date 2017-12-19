using System;
using System.Data;
using System.Data.SqlClient;

namespace JDBHandlers
{
	/// <summary>
	/// SQL 命令處理基底類別
	/// </summary>
	public class SqlHandle : System.IDisposable
	{
		public SqlParameter[] MParam;
		private GosmioSqlDataReader _MDataReader;
		protected SqlCommand MCmd;
		private string _MConnString;
		private string _MFunctionName;
		SqlConnection _MConnection = null;
		public int MCommandTimeout;
		private Boolean _MIsSqlErrorLog;

		private int _MErrorNo;				// 錯誤代碼
		private string _MErrorMsg;			// 錯誤訊息

		/// <summary>
		/// 建構子
		/// </summary>
		/// <param name="_conn_string">db連線字串</param>
		public SqlHandle(string _conn_string, string _function_name)
		{
			_MConnString = _conn_string;
			_MFunctionName = _function_name;
			_MIsSqlErrorLog = true;
			Init();
		}

		/// <summary>
		/// 建構子
		/// </summary>
		/// <param name="_conn_string">db連線字串</param>
		public SqlHandle(string _conn_string, string _function_name, Boolean _is_sql_error_log)
		{
			_MConnString = _conn_string;
			_MFunctionName = _function_name;
			_MIsSqlErrorLog = _is_sql_error_log;
			Init();
		}

		/// <summary>
		/// 當使用 using 時在結束時會執行這一段程序
		/// </summary>
		public void Dispose()
		{
			Close();
		}

		private void Init()
		{
			MParam = null;
			_MDataReader = null;
			MCmd = null;
			_MErrorNo = 0;
			_MErrorMsg = "";
			MCommandTimeout = 0;
		}

		public void Close()
		{
			if (_MDataReader != null)
			{
				_MDataReader.Close();
			}
			if (_MConnection != null)
			{
				_MConnection.Close();
			}
		}

		#region 供外部使用的執行SQL命令系列

		/// <summary>
		/// 有回傳資料的SQL執行程序 (預設 解釋命令 = CommandType.StoredProcedure)
		/// </summary>
		/// <param name="_sp_name">欲執行的SQL SP 名稱</param>
		/// <param name="_function_name">供寫Log的上層函式名稱</param>
		/// <returns></returns>
		public GosmioSqlDataReader ExecuteReader(string _sp_name)
		{
			return ExecuteReader(CommandType.StoredProcedure, _sp_name);
		}

		/// <summary>
		/// 有回傳資料的SQL執行程序 
		/// </summary>
		/// <param name="_sp_name">欲執行的SQL SP 名稱</param>
		/// <param name="_function_name">供寫Log的上層函式名稱</param>
		/// <returns></returns>
		public GosmioSqlDataReader ExecuteReader(CommandType commandType, string _sp_name)
		{
			SetCommandInit(commandType, _sp_name);

			// 連線DB
			if (ConnectionOpen())
			{
				return null;
			}

			DateTime tTimeStart = DateTime.Now;
			try
			{
				_MDataReader = new GosmioSqlDataReader(MCmd, _MFunctionName);

				_MDataReader.mObjDtr = ExecuteReaderProcess();

				return _MDataReader;
			}
			catch (System.Exception ex)
			{
				if (_MIsSqlErrorLog)
				{
					Logger.WriteErrorLog(_MFunctionName, ex, MCmd, tTimeStart);
				}else{
					Logger.WriteErrorLog(_MFunctionName, ex, null, tTimeStart);
				}
				return null;
			}
		}

		/// <summary>
		/// 無回傳資料的SQL執行程序 (預設 解釋命令 = CommandType.StoredProcedure)
		/// </summary>
		/// <param name="commandType">指定如何解釋命令</param>
		/// <param name="commandText">SQL命令</param>
        /// <returns>-998 ConnectionOpen失敗, -997 發生例外, -1 失敗, 0=成功</returns>
		public int ExecuteNonQuery(string _sql_string)
		{
			return ExecuteNonQuery(CommandType.StoredProcedure, _sql_string);
		}

		/// <summary>
		/// 無回傳資料的SQL執行程序
		/// </summary>
		/// <param name="commandType">指定如何解釋命令</param>
		/// <param name="commandText">SQL命令</param>
        /// <returns>-998 ConnectionOpen失敗, -997 發生例外, -1 失敗, 0=成功</returns>
		public int ExecuteNonQuery(CommandType commandType, string _sql_string)
		{
			SetCommandInit(commandType, _sql_string);

			// 連線DB
			if (ConnectionOpen())
			{
				return -998;
			}

			DateTime tTimeStart = DateTime.Now;
			try
			{
				// Call the overload that takes a connection in place of the connection string
				return ExecuteNonQueryProcess();
			}
			catch (System.Exception ex)
			{
				if (_MIsSqlErrorLog)
				{
					Logger.WriteErrorLog(_MFunctionName, ex, MCmd, tTimeStart);
				}
				else
				{
					Logger.WriteErrorLog(_MFunctionName, ex, null, tTimeStart);
				}
				return -997;
			}
		}

		/// <summary>
		/// 執行查詢，並傳回查詢所傳回的結果集第一個資料列的第一個資料行。會忽略其他的資料行或資料列
		/// (預設 解釋命令 = CommandType.StoredProcedure)
		/// </summary>
		/// <param name="_sql_string">SQL命令</param>
		/// <returns></returns>
		public object ExecuteScalar(string _sql_string)
		{
			return ExecuteScalar(CommandType.StoredProcedure, _sql_string);
		}

		/// <summary>
		/// 執行查詢，並傳回查詢所傳回的結果集第一個資料列的第一個資料行。會忽略其他的資料行或資料列
		/// </summary>
		/// <param name="commandType">指定如何解釋命令</param>
		/// <param name="commandText">SQL命令</param>
		/// <returns></returns>
		public object ExecuteScalar(CommandType commandType, string _command_text)
		{
			DateTime tTimeStart = DateTime.Now;
			SetCommandInit(commandType, _command_text);

			// 連線DB
			if (ConnectionOpen())
			{
				return null;
			}
			
			try
			{
				return ExecuteScalarProcess();
			}
			catch (System.Exception ex)
			{
				if (_MIsSqlErrorLog)
				{
					Logger.WriteErrorLog(_MFunctionName, ex, MCmd, tTimeStart);
				}
				else
				{
					Logger.WriteErrorLog(_MFunctionName, ex, null, tTimeStart);
				}
				return null;
			}
		}

		#endregion

		#region 類別內部命令處理

		/// <summary>
		/// 連線DB   失敗回傳 true
		/// </summary>
		/// <returns>連線DB失敗回傳 true</returns>
		private bool ConnectionOpen()
		{
			DateTime tTimeStart = DateTime.Now;
			try
			{
				if (_MConnection == null)
				{
					_MConnection = new SqlConnection(_MConnString);
				}
				if (_MConnection.State != ConnectionState.Open)
				{
					_MConnection.Open();
				}
			}
			catch (System.Exception ex)
			{
				string tLog = _MFunctionName + "== 連線DB失敗 ==";
				if (_MIsSqlErrorLog)
				{
					Logger.WriteErrorLog(tLog, ex, MCmd, tTimeStart);
				}
				else
				{
					Logger.WriteErrorLog(tLog, ex, null, tTimeStart);
				}
				return true;
			}

			return false;
		}

		private void SetCommandInit(CommandType commandType, string _command_text)
		{
			try
			{
				// Create a command and prepare it for execution
				MCmd = new SqlCommand();

				// 如果 CommandTimeout 有設定的話則指定 Command Timeout 
				if (MCommandTimeout > 0)
				{
					MCmd.CommandTimeout = MCommandTimeout;
				}

				MCmd.CommandText = _command_text;

				// Set the command type
				MCmd.CommandType = commandType;

				// Attach the command parameters if they are provided
				if (MParam != null)
				{
					AttachParameters(MCmd, MParam);
				}
			}
			catch (System.Exception ex)
			{
				string tLog = _MFunctionName + "== SetCommandInit() Exception ==";
				Logger.WriteErrorLog(tLog, ex, null);
				return;
			}
		}

		private object ExecuteScalarProcess()
		{
			if (_MConnection == null) throw new ArgumentNullException("connection");

			PrepareCommand(MCmd, _MConnection, (SqlTransaction)null);

			// Execute the command & return the results
			object retval = MCmd.ExecuteScalar();

			// Detach the SqlParameters from the command object, so they can be used again
			MCmd.Parameters.Clear();

			return retval;
		}

		private int ExecuteNonQueryProcess()
		{
			if (_MConnection == null) throw new ArgumentNullException("connection");

			PrepareCommand(MCmd, _MConnection, (SqlTransaction)null);

			// Finally, execute the command
			int retval = MCmd.ExecuteNonQuery();
			if (retval == 0)
			{
				// 0表示失敗  但是在這邊的回傳值0表示成功  所以需要反轉
				retval = -1;
			}
			else
			{
				retval = 0;
			}

			// Detach the SqlParameters from the command object, so they can be used again
			MCmd.Parameters.Clear();

			return retval;
		}

		// 連線處理
		private SqlDataReader ExecuteReaderProcess()
		{
			DateTime tTimeStart = DateTime.Now;
			try
			{
				PrepareCommand(MCmd, _MConnection, (SqlTransaction)null);

				SqlDataReader dataReader = MCmd.ExecuteReader(CommandBehavior.CloseConnection);

				bool canClear = true;
				foreach (SqlParameter commandParameter in MCmd.Parameters)
				{
					if (commandParameter.Direction != ParameterDirection.Input)
						canClear = false;
				}

				if (canClear)
				{
					MCmd.Parameters.Clear();
				}

				return dataReader;
			}
			catch (System.Exception ex)
			{
				if (_MIsSqlErrorLog)
				{
					Logger.WriteErrorLog(_MFunctionName, ex, MCmd, tTimeStart);
				}
				else
				{
					Logger.WriteErrorLog(_MFunctionName, ex, null, tTimeStart);
				}
				return null;
			}
		}

		/// <summary>
		/// This method opens (if necessary) and assigns a connection, transaction, command type and parameters 
		/// to the provided command
		/// </summary>
		/// <param name="command">The SqlCommand to be prepared</param>
		/// <param name="connection">A valid SqlConnection, on which to execute this command</param>
		/// <param name="transaction">A valid SqlTransaction, or 'null'</param>
		/// <param name="commandType">The CommandType (stored procedure, text, etc.)</param>
		/// <param name="commandText">The stored procedure name or T-SQL command</param>
		/// <param name="commandParameters">An array of SqlParameters to be associated with the command or 'null' if no parameters are required</param>
		/// <param name="mustCloseConnection"><c>true</c> if the connection was opened by the method, otherwose is false.</param>
		private void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction)
		{
			command.Connection = connection;

			// If we were provided a transaction, assign it
			if (transaction != null)
			{
				if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
				command.Transaction = transaction;
			}
			return;
		}

		/// <summary>
		/// This method is used to attach array of SqlParameters to a SqlCommand.
		/// 
		/// This method will assign a value of DbNull to any parameter with a direction of
		/// InputOutput and a value of null.  
		/// 
		/// This behavior will prevent default values from being used, but
		/// this will be the less common case than an intended pure output parameter (derived as InputOutput)
		/// where the user provided no input value.
		/// </summary>
		/// <param name="command">The command to which the parameters will be added</param>
		/// <param name="commandParameters">An array of SqlParameters to be added to command</param>
		private void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
		{
			if (command == null) throw new ArgumentNullException("command");
			if (commandParameters != null)
			{
				foreach (SqlParameter p in commandParameters)
				{
					if (p != null)
					{
						// Check for derived output value with no value assigned
						if ((p.Direction == ParameterDirection.InputOutput ||
							p.Direction == ParameterDirection.Input) &&
							(p.Value == null))
						{
							p.Value = DBNull.Value;
						}
						command.Parameters.Add(p);
					}
				}
			}
		}

		#endregion

		/// <summary>
		/// 錯誤代碼  0=無錯誤
		/// </summary>
		public int ErrorNo
		{
			get
			{
				return _MErrorNo;
			}
		}

		/// <summary>
		/// 錯誤訊息紀錄
		/// </summary>
		public string ErrorMsg
		{
			get
			{
				return _MErrorMsg;
			}
		}

		/// <summary>
		/// 是否有錯誤
		/// </summary>
		/// <returns></returns>
		public bool IsError()
		{
			if ((_MErrorNo != 0))
			{
				return true;
			}

			if (_MDataReader != null)
			{
				if (_MDataReader.IsError())
				{
					return true;
				}
			}

			return false;
		}
	}
}
