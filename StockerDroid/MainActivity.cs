using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using StockerCore.DL;
using System.Threading.Tasks;
using StockerCore.SAL;

namespace StockerDroid
{
    [Activity(Label = "StockerDroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private List<Stock> mStocks;
        private ProgressBar mBar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);
            mBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);

            button.Click += delegate
            {
                Task.Run(delegate { mStocks = GetStocks().Result; }).Wait();
            };
        }

        private async Task<List<Stock>> GetStocks()
        {
            var http = new StockHTTP();
            var queries = await http.DownloadCompaniesFromCSVAsync();

            List<Stock> stocks = new List<Stock>();
            decimal i = 1;
            decimal totals = queries.Count;

            foreach (string query in queries)
            {
                List<Stock> results = await http.DownloadStocksFromYQL(query);
                stocks.AddRange(results);
                mBar.Progress = (int)(i / totals);
            }

            return stocks;
        }
    }
}

