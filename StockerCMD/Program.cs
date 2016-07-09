using StockerCore.DL;
using StockerCore.SAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockerCMD
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Downloading HTML Code:");

            List<Stock> stocks = null;
            try
            {
                Task.Run(() =>
                {
                    stocks = GetStocks().Result;
                }).Wait();
            }
            catch (AggregateException ae)
            {
                Console.WriteLine("The following " + ae.InnerExceptions.Count() + " exceptions occurred:");
                Console.WriteLine(GetAggregateExceptionMessages(ae));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            Console.WriteLine("Download Complete.  Press any key to see stocks");
            Console.ReadLine();
            stocks.ForEach(s => Console.WriteLine(s));
            Console.WriteLine("Completed! Press any key to exit");
            Console.ReadLine();
        }

        private static IEnumerable<string> GetAggregateExceptionMessages(AggregateException exception)
        {
            foreach (var ie in exception.InnerExceptions)
            {
                yield return ie.Message;
            }
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
