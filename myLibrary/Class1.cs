using System;
using MyDBClientTestLibrary;

namespace myLibrary
{
    public class Class1
    {
        private static Class1 _Instance = null;

        private string _DBConnectStr = string.Empty;

        private Class1()
        {

        }

        /// <summary>
        /// Instance
        /// </summary>
        public static Class1 Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new Class1();
                }

                return _Instance;
            }
        }

        public void TestRun(string[] args)
		{
            Console.WriteLine("Run {0}.{1}."
				, System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName
				, System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (args.Length < 2)
            {
                return;
            }

            
            if ((args.Length%2)!=0)
            {
                Console.WriteLine("Setinit Error.");
                return;
            }
            int initSetCount = args.Length / 2;
            int SetIndex = 0;
            for (int i=0;i< initSetCount;i++)
            {
                this.SetInit(args[SetIndex++], args[SetIndex++]);
            }

            if (this._DBConnectStr.Length < 1)
            {
                return;
            }

            MyDBClientTestLibrary.DBSP tDB = MyDBClientTestLibrary.DBSP.Instance;
            tDB.Init(this._DBConnectStr);
            tDB.TestDB();
        }

        private void SetInit(string key, string value)
        {
            switch (key)
            {
                case "-s":
                    this._DBConnectStr = value;
                    break;
                default:
                    Console.WriteLine("Got a Error Set Key({0})..", key);
                    break;
            }
        }
    }
}
