namespace MyDBClientTestLibrary
{
    using System;
    using System.Reflection;
    using JDBHandlers;

    public class DBSP
    {
        private static DBSP _Instance = null;

        string _DBConnectStr = "";

        /// <summary>
        /// Instance
        /// </summary>
        public static DBSP Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new DBSP();
                }

                return _Instance;
            }
        }

        public void Init(string connectStr)
        {
            _DBConnectStr = connectStr;
            Console.WriteLine("DBConnectStr='{0}'", _DBConnectStr);
        }

        public int TestDB()
        {
            int resultCode = 1;

            try
            {
                using (SqlHandle sqlHandle = new SqlHandle(_DBConnectStr, MethodBase.GetCurrentMethod().Name))
                {
                    GosmioSqlDataReader dataReturned = sqlHandle.ExecuteReader("NSP_GameServer_TransferCntLimit_List");

                    while (dataReturned.Read())
                    {
                        int RuleID = dataReturned.GetDataForInt("RuleID");
                        Console.WriteLine("RuleID:{0}.", RuleID);
                        resultCode = 1;
                    }
                }

                return resultCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} 發生例外:{1}.", MethodBase.GetCurrentMethod().Name, ex.Message);
                return 0;
            }
        }
    }
}
