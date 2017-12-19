using System;

namespace myApp
{
    class Program
    {
        string _ConnStr = string.Empty;

        static void Main(string[] args)
        {
            Console.WriteLine("Init Sets:");
            foreach (string arg in args)
            {
                Console.WriteLine(arg);
            }
            
            myLibrary.Class1.Instance.TestRun(args);
            Console.ReadLine();
        }

        
    }
}
