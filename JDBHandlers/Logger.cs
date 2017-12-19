using System;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace JDBHandlers
{
	public static class Logger
	{
		#region Private Method
		/// <summary>
		/// 取得不同日誌的存放位置
		/// </summary>
		/// <param name="logType">日誌類型</param>
		/// <returns>日誌檔案路徑</returns>
		private static string GetLogFilePath(LogType logType)
		{
			switch (logType)
			{
				case LogType.Trace:
					return @"C:\DBConnLog\Trace\Trace_" + System.DateTime.Now.ToString("yyyy-MM-dd-HH") + ".txt";
				case LogType.CCWLog:
					return @"C:\DBConnLog\CCW\CCW_" + System.DateTime.Now.ToString("yyyy-MM-dd-HH") + ".txt";
				case LogType.SqlLog:
					return @"C:\DBConnLog\SQL\SqlLog_" + System.DateTime.Now.ToString("yyyy-MM-dd-HH") + ".sql";
				case LogType.WSLog:
					return @"C:\DBConnLog\WS\WSLog_" + System.DateTime.Now.ToString("yyyy-MM-dd-HH") + ".txt";
				default:
					return @"C:\DBConnLog\Error\ErrorLog_" + System.DateTime.Now.ToString("yyyy-MM-dd-HH") + ".txt";
			}
		}

		private static void WriteLog(LogType logType, string format, params object[] args)
		{
			StringBuilder objSB = new StringBuilder();
			objSB.AppendLine(string.Format("-- =========={0}==========", System.DateTime.Now.ToString()));
			objSB.AppendFormat(format, args);
			WriteLog(logType, objSB.ToString());
		}

		/// <summary>
		/// 寫入日誌
		/// </summary>
		/// <param name="logType">日誌類型</param>
		/// <param name="log">日誌內容</param>
		public static void WriteLog(LogType logType, string log)
		{
			string fullfilename = GetLogFilePath(logType);
			string dirname = Path.GetDirectoryName(fullfilename);

			lock (typeof(Logger))
			{
				if (dirname.Trim() != "" && Directory.Exists(dirname) == false)
				{
					Directory.CreateDirectory(dirname);
				}

				using (FileStream objFS = new FileStream(fullfilename, FileMode.Append, FileAccess.Write, FileShare.Read))
				{
					using (StreamWriter objSW = new StreamWriter(objFS, System.Text.Encoding.GetEncoding("Big5")))
					{
						objSW.WriteLine(log);
						objSW.Close();
					}
					objFS.Close();
				}
			}
		}

		/// <summary>
		/// 產生SQL檔案  // BTODO 需檢查是否還有存在之必要
		/// </summary>
		/// <param name="log">日誌資訊</param>
		/// <param name="sql">SQL語法</param>
		/// <param name="param">參數</param>
		private static string BuildSqlLog(string log, string sql, SqlParameter[] param)
		{
			string logHeader = string.Format("-- =========={0}::{1}==========", log, System.DateTime.Now.ToString()) + Environment.NewLine;
			logHeader += string.Format("exec {0} ", sql);

			StringBuilder objSB = new StringBuilder();

			foreach (SqlParameter p in param)
			{
				switch (p.SqlDbType)
				{
					case SqlDbType.Char:
					case SqlDbType.NChar:
					case SqlDbType.Text:
					case SqlDbType.VarChar:
					case SqlDbType.NVarChar:
					case SqlDbType.NText:
						objSB.Append(string.Format(", {0}='{1}'", p.ParameterName, p.Value.ToString()));
						break;
					case SqlDbType.DateTime:
						objSB.Append(string.Format(", {0}='{1}'", p.ParameterName, Convert.ToDateTime(p.Value).ToString("yyyy/MM/dd HH:mm:ss")));
						break;
					default:
						objSB.Append(string.Format(", {0}={1}", p.ParameterName, p.Value.ToString()));
						break;
				}

				switch (p.Direction)
				{
					case ParameterDirection.InputOutput:
					case ParameterDirection.Output:
						objSB.Append(" out");
						break;
					default:
						break;
				}
			}
			
			return logHeader + objSB.ToString(1, objSB.Length - 1);
		}

		/// <summary>
		/// 產生SQL檔案 - 多載 by brady
		/// </summary>
		/// <param name="log">日誌資訊</param>
		/// <param name="sql">SQL語法</param>
		/// <param name="param">參數</param>
		private static string BuildSqlLog(string log, string sql, SqlParameterCollection param)
		{
			string logHeader = string.Format("-- =========={0}::{1}==========", log, System.DateTime.Now.ToString()) + Environment.NewLine;
			logHeader += string.Format("exec {0} ", sql);

			StringBuilder objSB = new StringBuilder();

			foreach (SqlParameter p in param)
			{
				switch (p.SqlDbType)
				{
					case SqlDbType.Char:
					case SqlDbType.NChar:
					case SqlDbType.Text:
					case SqlDbType.VarChar:
					case SqlDbType.NVarChar:
					case SqlDbType.NText:
						objSB.Append(string.Format(", {0}='{1}'", p.ParameterName, p.Value.ToString()));
						break;
					case SqlDbType.DateTime:
						objSB.Append(string.Format(", {0}='{1}'", p.ParameterName, Convert.ToDateTime(p.Value).ToString("yyyy/MM/dd HH:mm:ss")));
						break;
					default:
						objSB.Append(string.Format(", {0}={1}", p.ParameterName, p.Value.ToString()));
						break;
				}

				switch (p.Direction)
				{
					case ParameterDirection.InputOutput:
					case ParameterDirection.Output:
						objSB.Append(" out");
						break;
					default:
						break;
				}
			}

			if (objSB.Length > 1)
			{
				return logHeader + objSB.ToString(1, objSB.Length - 1);
			}
			else
			{
				return logHeader;
			}
		}

		#endregion

		#region Public Method
		/// <summary>
		/// 記錄Error日誌
		/// </summary>
		/// <param name="log">文字</param>
		public static void WriteErrorLog(string log)
		{
			WriteErrorLog(log, null);
		}

		/// <summary>
		/// 記錄Error日誌
		/// </summary>
		/// <param name="log">Error日誌</param>
		/// <param name="ex">Exception物件</param>
		public static void WriteErrorLog(string log, Exception ex)
		{
			StringBuilder objSB = new StringBuilder();
			objSB.AppendLine(string.Format("=========={0}::{1}==========", log, System.DateTime.Now.ToString()));
			if (ex != null)
			{
				objSB.AppendLine(string.Format("Message:{0}", ex.Message));
				objSB.AppendLine(string.Format("Source:{0}", ex.Source));
				objSB.AppendLine(string.Format("StackTrace:{0}", ex.StackTrace));
			}
			WriteLog(LogType.CCWLog, objSB.ToString());
		}

		/// <summary>
		/// 記錄Error日誌，並寫出SQL檔案
		/// </summary>
		/// <param name="log">Error日誌</param>
		/// <param name="ex">Exception物件</param>
		/// <param name="sql">SQL語法</param>
		/// <param name="param">參數</param>
		public static void WriteErrorLog(string log, Exception ex, string sql, SqlParameter[] param)
		{
			WriteErrorLog(log, ex);
			string sqlLog = BuildSqlLog(log, sql, param);
			WriteLog(LogType.SqlLog, sqlLog);
		}

		/// <summary>
		/// 記錄Error日誌，並寫出SQL檔案
		/// </summary>
		/// <param name="log">Error日誌</param>
		/// <param name="ex">Exception物件</param>
		/// <param name="sql">SQL語法</param>
		/// <param name="param">參數</param>
		public static void WriteErrorLog(string log, Exception ex, string sql, SqlParameter[] param, DateTime time_start)
		{
			double tSpendTime = ((TimeSpan)(DateTime.Now - time_start)).TotalMilliseconds;
			log = log + "(花費時間:" + tSpendTime.ToString() + ')';
			WriteErrorLog(log, ex);
			string sqlLog = BuildSqlLog(log, sql, param);
			WriteLog(LogType.SqlLog, sqlLog);
		}

		/// <summary>
		/// 記錄Error日誌，並寫出SQL檔案 多載 by Brady
		/// </summary>
		/// <param name="log">Error日誌</param>
		/// <param name="ex">Exception物件</param>
		/// <param name="sql">SQL語法</param>
		/// <param name="param">參數</param>
		public static void WriteErrorLog(string log, Exception ex, SqlCommand cmd)
		{
			WriteErrorLog(log, ex);
			if (cmd != null)
			{
				string sqlLog = BuildSqlLog(log, cmd.CommandText, cmd.Parameters);
				WriteLog(LogType.SqlLog, sqlLog);
			}
		}

		/// <summary>
		/// 記錄Error日誌，並寫出SQL檔案(增加紀錄處理時間) 多載 by Brady
		/// </summary>
		/// <param name="log">Error日誌</param>
		/// <param name="ex">Exception物件</param>
		/// <param name="cmd">SqlCommand</param>
		/// <param name="DateTime">程序處理開始時間</param>
		public static void WriteErrorLog(string log, Exception ex, SqlCommand cmd, DateTime _start_time)
		{
			double tSpendTime = ((TimeSpan)(DateTime.Now - _start_time)).TotalMilliseconds;
			log = log + "(花費時間:" + tSpendTime.ToString() + ')';

			WriteErrorLog(log, ex);
			if (cmd != null)
			{
				string sqlLog = BuildSqlLog(log, cmd.CommandText, cmd.Parameters);
				WriteLog(LogType.SqlLog, sqlLog);
			}
		}
		#endregion
	}
}