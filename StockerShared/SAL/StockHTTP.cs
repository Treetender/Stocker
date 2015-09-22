using StockerCore.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockerShared.SAL
{
    public class StockHTTP
    {
        private static readonly string BASE_URL = "https://query.yahooapis.com/v1/public/yql"; 
 	    private static readonly string PARAM_QUERY = "q"; 
        private static readonly string VALUE_ENV = "http://datatables.org/alltables.env"; 
        private static readonly string PARAM_ENV = "env";
        private static readonly string STOCK_TAG = "quote"; 
        private static readonly string NAME_TAG = "Name";

        public static string CreateRequest()
        {
            string url = string.Format("{0}/{1}", BASE_URL, PARAM_QUERY);
            //          .appendQueryParameter(PARAM_QUERY, "select * from yahoo.finance.quotes where symbol in (\"YHOO\",\"AAPL\",\"GOOG\",\"MSFT\")")
            //          .appendQueryParameter(PARAM_ENV, VALUE_ENV)        
            //          .build().toString();
            return url;
        }

        //private string ToQueryString(NameValueCollection nvc)
        //{
        //    var array = (from key in nvc.AllKeys
        //                 from value in nvc.GetValues(key)
        //                 select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value)))
        //        .ToArray();
        //    return "?" + string.Join("&", array);
        //}



        public async Task<List<Stock>> GetFeedItems(DateTime date)
        {
            Stock s = new Stock();
            var feed = "http://planet.xamarin.com/feed/";
            var response = await httpClient.GetStringAsync(feed);
            var items = await ParseFeedAsync(response);
            return items.Where(item => item.Published.Date == date).ToList();
        }
    }
}
