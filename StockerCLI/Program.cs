using StockerCore.DL;
using StockerCore.SAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockerCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var http = new StockHTTP();
            Console.WriteLine("Downloading HTML Code:");
            List<Stock> stocks = new List<Stock>();
            Task.Run(() =>
            {
                stocks = GetStocks().Result;
            }).Wait();

            Console.WriteLine(string.Format("Found the following {0} Stocks:", stocks.Count));
            foreach (var s in stocks)
            {
                Console.WriteLine(s);
            }
            Console.ReadLine();
        }

        static async Task<List<Stock>> GetStocks()
        {
            var http = new StockHTTP();
            var stocks = await http.DownloadStocksFromYQL();

            return stocks;
        }

    }
}
