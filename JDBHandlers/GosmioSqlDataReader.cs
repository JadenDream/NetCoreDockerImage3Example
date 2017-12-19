using System;
using System.Data.SqlClient;

namespace JDBHandlers
{
	/// <summary>
	/// 專屬的 SqlDataReader 資料包裝類別
	/// </summary>
	public class GosmioSqlDataReader
	{
		private string mFunctionName;		// 處理的函式名稱 (方便輸出Log用)
		private SqlCommand mCmd;			// 目前執行的 SqlCommand (方便輸出Log用)
		private bool mIsError;				// 是否發生錯誤
		public SqlDataReader mObjDtr;

		public GosmioSqlDataReader(SqlCommand _cmd, string _function_name)
		{
			mObjDtr = null;
			mIsError = false;
			mFunctionName = _function_name;
			mCmd = _cmd;
		}

		public bool IsError()
		{
			return mIsError;
		}

		public void Close()
		{
			if (mObjDtr != null)
			{
				mObjDtr.Close();
			}
		}

		/// <summary>
		/// 是否有資料
		/// </summary>
		/// <returns></returns>
		public bool Read()
		{
			if (mObjDtr == null)
			{
				return false;
			}
			return mObjDtr.Read();
		}

		#region 取得資料系列

		/// <summary>
		/// 依照傳入之欄位名稱,回傳SqlDataReader資料中的對應資料行資料 (ForInt)
		/// </summary>
		/// <param name="_name">欄位名稱</param>
		/// <returns></returns>
		public int GetDataForInt(string _name)
		{
			try
			{
				return mObjDtr.GetInt32(mObjDtr.GetOrdinal(_name));
			}
			catch (System.Exception ex)
			{
				string tTitle = mFunctionName + " [" + _name + "] ";
				Logger.WriteErrorLog(tTitle, ex, mCmd);
				mIsError = true;
				return 0;
			}
		}

		/// <summary>
		/// 依照傳入之欄位名稱,回傳SqlDataReader資料中的對應資料行資料 (ForInt)
		/// </summary>
		/// <param name="_name">欄位名稱</param>
		/// <returns></returns>
		public byte GetDataForByte(string _name)
		{
			try
			{
				return mObjDtr.GetByte(mObjDtr.GetOrdinal(_name));
			}
			catch (System.Exception ex)
			{
				string tTitle = mFunctionName + " [" + _name + "] ";
				Logger.WriteErrorLog(tTitle, ex, mCmd);
				mIsError = true;
				return (byte)0;
			}
		}

		/// <summary>
		/// 依照傳入之欄位名稱,回傳SqlDataReader資料中的對應資料行資料 (For String)
		/// </summary>
		/// <param name="_name">欄位名稱</param>
		/// <returns></returns>
		public string GetDataForStr(string _name)
		{
			try
			{
				return mObjDtr.GetString(mObjDtr.GetOrdinal(_name));
			}
			catch (System.Exception ex)
			{
				string tTitle = mFunctionName + " [" + _name + "] ";
				Logger.WriteErrorLog(tTitle, ex, mCmd);
				mIsError = true;
				return "";
			}
		}

		/// <summary>
		/// 依照傳入之欄位名稱,回傳SqlDataReader資料中的對應資料行資料 (For Int64)
		/// </summary>
		/// <param name="_name">欄位名稱</param>
		/// <returns></returns>
		public long GetDataForInt64(string _name)
		{
			try
			{
				return mObjDtr.GetInt64(mObjDtr.GetOrdinal(_name));
			}
			catch (System.Exception ex)
			{
				string tTitle = mFunctionName + " [" + _name + "] ";
				Logger.WriteErrorLog(tTitle, ex, mCmd);
				mIsError = true;
				return 0;
			}
		}

        /// <summary>
        /// 依照傳入之欄位名稱,回傳SqlDataReader資料中的對應資料行資料 (For Double)
        /// </summary>
        /// <param name="_name">欄位名稱</param>
        /// <returns></returns>
        public double GetDataForDouble(string _name)
        {
            try
            {
                return mObjDtr.GetDouble(mObjDtr.GetOrdinal(_name));
            }
            catch (System.Exception ex)
            {
                string tTitle = mFunctionName + " [" + _name + "] ";
                Logger.WriteErrorLog(tTitle, ex, mCmd);
                mIsError = true;
                return 0;
            }
        }

		/// <summary>
		/// 照傳入之欄位名稱,回傳SqlDataReader資料中的對應資料行資料 (For 時間格式的字串 yyyy/MM/dd[hh:mm:ss])
		/// </summary>
		/// <param name="_name">欄位名稱</param>
		/// <returns></returns>
		public string GetDataForDateStr(string _name)
		{
			try
			{
				return Convert.ToDateTime(mObjDtr[_name]).ToString("yyyy/MM/dd[HH:mm:ss]");
			}
			catch (System.Exception ex)
			{
				string tTitle = mFunctionName + " [" + _name + "] ";
				Logger.WriteErrorLog(tTitle, ex, mCmd);
				mIsError = true;
				return "";
			}
		}

		/// <summary>
		/// 照傳入之欄位名稱,回傳SqlDataReader資料中的對應資料行資料 (For Decimal)
		/// </summary>
		/// <param name="_name">欄位名稱</param>
		/// <returns></returns>
		public Decimal GetDataForDecimal(string _name)
		{
			try
			{
				return mObjDtr.GetDecimal(mObjDtr.GetOrdinal(_name));
			}
			catch (System.Exception ex)
			{
				string tTitle = mFunctionName + " [" + _name + "] ";
				Logger.WriteErrorLog(tTitle, ex, mCmd);
				mIsError = true;
				return 0;
			}
		}

		/// <summary>
		/// 依照傳入之欄位名稱,回傳SqlDataReader資料中的對應資料行資料 (For DateTime 的字串模式)
		/// </summary>
		/// <param name="_name">欄位名稱</param>
		/// <returns></returns>
		public string GetDataForDateTimeStr(string _name)
		{
			try
			{
				return Convert.ToDateTime(mObjDtr[_name]).ToString("yyyy/MM/dd HH:mm:ss");
			}
			catch (System.Exception ex)
			{
				string tTitle = mFunctionName + " [" + _name + "] ";
				Logger.WriteErrorLog(tTitle, ex, mCmd);
				mIsError = true;
				return "";
			}
		}

		/// <summary>
		/// 依照傳入之欄位名稱,回傳SqlDataReader資料中的對應資料行資料 (For DateTime 的字串模式)
		/// </summary>
		/// <param name="_name">欄位名稱</param>
		/// <returns></returns>
		public DateTime GetDataForDateTime(string _name)
		{
			try
			{
				return Convert.ToDateTime(mObjDtr[_name]);
			}
			catch (System.Exception ex)
			{
				string tTitle = mFunctionName + " [" + _name + "] ";
				Logger.WriteErrorLog(tTitle, ex, mCmd);
				mIsError = true;
				return DateTime.Now;
			}
		}

		#endregion

	}
}
