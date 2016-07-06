using StockerCore.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockerCore.SAL
{
    public class StockHTTP
    {
        private static readonly string sCSV_URL = "http://www.nasdaq.com/screening/companies-by-industry.aspx?exchange=NASDAQ&render=download";
        private static readonly string sQuery_SQL_Base = "Select * From yahoo.finance.quotes";
        private static readonly string sQUERY_ENV = "http://datatables.org/alltables.env";
        private static readonly int STOCKS_PER_QUERY = 40;

        private string GetSymbolString(List<Stock> stocks)
        {
            if (stocks == null)
                return null;
            if (stocks.Count <= 0)
                return null;

            string s = "(" 
                     + from stock in stocks
                       select ("\"" + stock.Symbol + "\"") 
                     + ")";
            return s;
        }

        public async Task<List<string>> DownloadCompaniesFromCSVAsync()
        {
            var uri = new UriBuilder(sCSV_URL);
            var csvstring = await DownloadPageAsync(uri.Uri);

            var csvs = csvstring.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> stocks = (from csv in csvs
                                   select csv.Split(',')[0].Replace(" ", string.Empty)).ToList().Skip(1).ToList();
            var queries = new List<string>();
            int totals = (stocks.Count / STOCKS_PER_QUERY) + (stocks.Count % STOCKS_PER_QUERY == 0 ? 1 : 0);
            int end = stocks.Count % STOCKS_PER_QUERY;
            for (int i = 0; i <= totals  - 1; i++)
            {
                string s = string.Empty;
                int entries = (i == (totals - 1) && end > 0 ? end : STOCKS_PER_QUERY);
                for (int j = 0; j < entries; j++)
                {
                    s += stocks[(i * STOCKS_PER_QUERY) + (j)] + ",";
                }
                s = s.Substring(0, s.Length - 1);
                queries.Add(s);
            }
            return queries;
        }

        public async Task<List<Stock>> DownloadStocksFromYQL(string queryString)
        {
            var url = "https://query.yahooapis.com/v1/public/yql";

            string walletString = queryString;

            string query = string.Format("{0} where symbol in ({1})", sQuery_SQL_Base, walletString);

            UriBuilder uri = new UriBuilder(string.Format("{0}?{1}={2}", url, "q", query));
            string queryToAppend = string.Format("{0}={1}", "env", sQUERY_ENV);

            if (uri.Query != null && uri.Query.Length > 1)
                uri.Query = uri.Query.Substring(1) + "&" + queryToAppend;
            else
                uri.Query = queryToAppend;
            var xmlstring = await DownloadPageAsync(uri.Uri);

            return GetStockData(xmlstring);
        }


        public async Task<string> DownloadPageAsync(Uri url)
        {
            // ... Use HttpClient.
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url))
            using (HttpContent content = response.Content)
            {
                // ... Read the string.
                string result = await content.ReadAsStringAsync();

                // ... Display the result.
                return result;
            }
        }

        public List<Stock> GetStockData(string xml)
        {
            var xmlData = XDocument.Parse(xml);
            decimal dividend, asking, booking, change, lastValue, dayHigh, dayLow, delta50, delta200;
            decimal moving200, moving50;
            DateTime lastTrade;
            return (from quote in xmlData.Descendants("quote")
                    select new Stock
                    {
                        Symbol = quote.Attribute("symbol").Value,
                        Name = quote.Element("Name").Value,
                        DayValue = (decimal.TryParse(quote.Element("Ask").Value, out asking) ? asking : 0M),
                        BookValue = (decimal.TryParse(quote.Element("BookValue").Value, out booking) ? booking : 0M),
                        ChangeInMovingAverage50 = (decimal.TryParse(quote.Element("ChangeFromFiftydayMovingAverage").Value, out delta50) ? delta50 : 0M),
                        ChangeInMovingAverage200 = (decimal.TryParse(quote.Element("ChangeFromTwoHundreddayMovingAverage").Value, out delta200) ? delta200 : 0M),
                        DayHigh = (decimal.TryParse(quote.Element("DaysHigh").Value, out dayHigh) ? dayHigh : 0M),
                        DayLow = (decimal.TryParse(quote.Element("DaysLow").Value, out dayLow) ? dayLow : 0M),
                        DividendPerShare = (decimal.TryParse(quote.Element("DividendShare").Value, out dividend) ? dividend : (decimal?)null),
                        MovingAverage200 = (decimal.TryParse(quote.Element("TwoHundreddayMovingAverage").Value, out moving200) ? moving200 : 0M),
                        MovingAverage50 = (decimal.TryParse(quote.Element("FiftydayMovingAverage").Value, out moving50) ? moving50 : 0M),
                        Currency = quote.Element("Currency").Value,
                        Change = (decimal.TryParse(quote.Element("Change").Value, out change) ? change : 0M),
                        LastTradeDate = (DateTime.TryParse(quote.Element("LastTradeDate").Value, out lastTrade) ? lastTrade : (DateTime?)null),
                        LastValue = (decimal.TryParse(quote.Element("LastTradePriceOnly").Value, out lastValue) ? lastValue : 0M)
                    }).ToList();
        }

        public async Task<List<Stock>> GetStocks()
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
                i += 1;
            }
            return stocks;
        }

    }
}
