using FinanceDataMining;
using System;

namespace TestingConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            SettingService testReadingConfig = new SettingService();
            testReadingConfig.LoadConfig();
        }
    }
}
