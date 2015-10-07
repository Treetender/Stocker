using StockerCore.DL;
using StockerCore.SAL;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockerCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var http = new StockHTTP();
            Console.WriteLine("Downloading HTML Code:");

            List<Stock> stocks = null;
            Task.Run(() =>
            {
                stocks = GetStocks().Result;
            }).Wait();

            stocks.ForEach(s => Console.WriteLine(s));
            Console.ReadLine();
        }

        static async Task<List<Stock>> GetStocks()
        {
            var http = new StockHTTP();
            var queries = await http.DownloadCompaniesFromCSVAsync();
            List<Stock> stocks = new List<Stock>();
            decimal i = 1.0M;
            decimal totals = queries.Count;
            foreach (var query in queries)
            {
                List<Stock> results = await http.DownloadStocksFromYQL(query);
                results.ForEach(r => stocks.Add(r));
                Console.WriteLine(string.Format("{0:p} Complete", (i / totals)));
                i += 1;
            }
            return stocks;
        }

    }
}
