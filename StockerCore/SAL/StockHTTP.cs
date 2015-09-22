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
        private static readonly string sQuery_SQL = "select * from yahoo.finance.quotes where symbol in (\"YHOO\",\"AAPL\",\"GOOG\",\"MSFT\")";
        private static readonly string sQUERY_ENV = "http://datatables.org/alltables.env";

        public async Task<List<Stock>> DownloadStocksFromYQL()
        {
            var url = "https://query.yahooapis.com/v1/public/yql";

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
