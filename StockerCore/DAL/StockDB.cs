using SQLite;
using StockerCore.DL;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace StockerCore.DAL
{
    public class StockDB
    {
        private SQLiteConnection mDB;

        public StockDB()
        {
            mDB = DependencyService.Get<ISQLite>().GetConnection();
            mDB.CreateTable<Stock>();
        }

        public IEnumerable<Stock> GetStocks()
        {
            return (from s in mDB.Table<Stock>() select s).ToList();
        }

        public Stock GetStock(int id)
        {
            return mDB.Table<Stock>().FirstOrDefault(s => s.ID == id);
        }

        public void DeleteStock(int id)
        {
            mDB.Delete<Stock>(id);
        }

        public void AddStock(string name, string symbol, decimal qty, decimal value)
        {
            var stock = new Stock()
            {
                Name = name,
                DayValue = value,
                Quantity = qty,
                Symbol = symbol
            };
            mDB.Insert(stock);
        }

        public void SaveStock(Stock s)
        {
            var rowsChanged = mDB.Update(s);
            if (rowsChanged == 0)
                rowsChanged = mDB.Insert(s);
            if (rowsChanged <= 0)
                throw new InvalidOperationException("Failed to Save Stock Data for " + s);
        }

    }
}
