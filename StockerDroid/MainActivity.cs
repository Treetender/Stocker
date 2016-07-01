using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using StockerCore.DL;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockerCore.SAL;

namespace StockerDroid
{
    [Activity(Label = "StockerDroid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private List<Stock> mStocks;
        private Button mButton;
        private bool mIsFetching = false;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            mStocks = new List<Stock>();
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            if(mButton == null)
                mButton = FindViewById<Button>(Resource.Id.MyButton);

            mButton.Click += delegate 
            {         
                if (!mIsFetching) 
                {
                    mIsFetching = true;
                    mButton.Text = "Downloading Stocks...";
                    var fetcher = new StockFetcher();
                    fetcher.OnDownloadComplete += (sender, e) => { mButton.Text = "Download Complete"; Console.WriteLine("Download Complete"); };
                    fetcher.OnProgressChanged += (sender, progress) => { mButton.Text = string.Format("{0:p} Complete", progress); Console.WriteLine(string.Format("{0:p}")); };
                    Task.Run(() => { mStocks = fetcher.GetStocks().Result; });
                }
            };
       }

       private class StockFetcher 
       {
            public event EventHandler<decimal> OnProgressChanged;
            public event EventHandler OnDownloadComplete; 

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
                    OnProgressChanged?.Invoke(this, (i / totals));
                    i += 1;
                }
                OnProgressChanged?.Invoke(this, 1);
                OnDownloadComplete?.Invoke(this, EventArgs.Empty);
                return stocks;
            }
        }
    }
}

