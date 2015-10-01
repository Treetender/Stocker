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
            //List<Stock> stocks = new List<Stock>();
            //Task.Run(() =>
            //{
            //    stocks = GetStocks().Result;
            //}).Wait();

            //Console.WriteLine(string.Format("Found the following {0} Stocks:", stocks.Count));
            //foreach (var s in stocks)
            //{
            //    Console.WriteLine(s);
            //}

            List<string> companies = new List<string>();
            Task.Run(() =>
            {
                companies = GetCompaniesAsync().Result;
            }).Wait();

            Console.WriteLine("The following companies were found:");
            companies.ForEach(c => Console.WriteLine(c));

            Console.WriteLine("Found " + companies.Count + " Companies in the NASDAQ");
            Console.ReadLine();
        }

        static async Task<List<string>> GetCompaniesAsync()
        {
            var http = new StockHTTP();
            return await http.DownloadCompaniesFromCSVAsync();
        }


        static async Task<List<Stock>> GetStocks()
        {
            var http = new StockHTTP();
            var stocks = await http.DownloadStocksFromYQL(100.00M);

            return stocks;
        }

    }
}
