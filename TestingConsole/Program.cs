using Data;
using Data.DataModels;
using FinanceDataMining.CryproApi;
using FinanceDataMining.Models;
using InfluxDbData;
using Microsoft.EntityFrameworkCore;
using Repository;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestingConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ProcessManager processManager = new ProcessManager();
            await processManager.DownloadAssets();
        }
    }
}
