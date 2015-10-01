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
        private static readonly string sQuery_SQL_Base = "Select * From yahoo.finance.quotes where symbol is not null";
        private static readonly string sQuery_SQL = "select * from yahoo.finance.quotes where symbol in (\"YHOO\",\"AAPL\",\"GOOG\",\"MSFT\")";
        private static readonly string sQUERY_ENV = "http://datatables.org/alltables.env";

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
            return (from csv in csvs
                    select csv.Split(',')[0]).ToList();
        }

        public async Task<List<Stock>> DownloadStocksFromYQL(decimal amountToInvest = 0, List<Stock> walletStockSymbols = null)
        {
            var url = "https://query.yahooapis.com/v1/public/yql";

            string walletString = GetSymbolString(walletStockSymbols);

            var query = string.Format
                        (
                            "{0}{4}{1}{2}{3}", 
                            sQuery_SQL_Base,
                            (amountToInvest > 0 ? " Open <= " + amountToInvest : string.Empty), // If we have an amount we can invest, we are only interested in what we have to spend
                            (amountToInvest > 0 && walletString != null ? " Or " : string.Empty), // if we have both an amount to invest, and some wallet stock, we need an "Or"
                            (walletString == null ? string.Empty : "symbol in " + walletString), // If we have stock in our wallet, we want to fetch them
                            (amountToInvest > 0 || walletString != null ? " Where " : string.Empty) // We won't need a where in the statement if we have no money and no wallet stocks
                         );

            if (amountToInvest == 0 && walletString == null)
                query = sQuery_SQL;

            UriBuilder uri = new UriBuilder(string.Format("{0}?{1}={2}", url, "q", sQuery_SQL));
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
            decimal dividend;
            DateTime lastTrade;
            return (from quote in xmlData.Descendants("quote")
                    select new Stock
                    {
                        Symbol = quote.Attribute("symbol").Value,
                        Name = quote.Element("Name").Value,
                        DayValue = (decimal)quote.Element("Ask"),
                        BookValue = (decimal)quote.Element("BookValue"),
                        ChangeInMovingAverage50 = (decimal)quote.Element("ChangeFromFiftydayMovingAverage"),
                        ChangeInMovingAverage200 = (decimal)quote.Element("ChangeFromTwoHundreddayMovingAverage"),
                        DayHigh = (decimal)quote.Element("DaysHigh"),
                        DayLow = (decimal)quote.Element("DaysLow"),
                        DividendPerShare = (decimal.TryParse(quote.Element("DividendShare").Value, out dividend) ? dividend : (decimal?)null),
                        MovingAverage200 = (decimal)quote.Element("TwoHundreddayMovingAverage"),
                        MovingAverage50 = (decimal)quote.Element("FiftydayMovingAverage"),
                        Currency = quote.Element("Currency").Value,
                        Change = (decimal)quote.Element("Change"),
                        LastTradeDate = (DateTime.TryParse(quote.Element("LastTradeDate").Value, out lastTrade) ? lastTrade : (DateTime?)null),
                        LastValue = (decimal)quote.Element("LastTradePriceOnly")
                    }).ToList();
        }

    }
}
