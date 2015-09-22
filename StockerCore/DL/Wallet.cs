using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Linq;

namespace StockerCore.DL
{
    public class Wallet : INotifyPropertyChanged
    {
        private ObservableCollection<Stock> mStocks;
        private decimal? mRemain, mWorth;

        public decimal? Worth { get { return mWorth; } private set { mWorth = value; OnPropertyChanged(); } }
        public decimal? Remaining { get { return mRemain; } private set { mRemain = value;  OnPropertyChanged(); } }

        public ObservableCollection<Stock> Stocks { get { return mStocks; } }

        public Wallet()
        {
            mRemain = 100;
            decimal? total = mStocks.Sum(s => s.DayValue * s.Quantity);
            mWorth = total ?? 0;
            mStocks = new ObservableCollection<Stock>(new List<Stock>());
        }

        public void BuyStock(Stock s)
        {
            decimal? amount = s.DayValue * s.Quantity;
            if (amount == null)
                throw new ArgumentException("Stock must have a valid quantity and value : " + s, "s");
            if (amount > mRemain)
                throw new ArgumentException("Balance Remaining must be greater than the cost of the Stock " + s.Name, "s");
            
            Remaining -= amount.Value;
            mStocks.Add(s);
            Worth += amount.Value;
        }

        public void SellStock(Stock s, decimal qty)
        {
            var walletStock = mStocks.FirstOrDefault(stock => stock.ID == s.ID);
            if (walletStock == null)
                throw new ArgumentException("Cannot sell Stock " + s.Name + ": It is not in the wallet!", "s");
            if (qty > walletStock.Quantity)
                throw new ArgumentException("Cannot sell " + qty + " of Stock " + s.Name + " as there is only " + walletStock.Quantity + " in the wallet.", "qty");

            var amount = qty * s.DayValue;
            walletStock.Quantity -= qty;
            Worth -= amount;
            Remaining += amount;

            if (walletStock.Quantity == 0)
                mStocks.Remove(s);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string name = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion INotifyPropertyChanged
    }
}
