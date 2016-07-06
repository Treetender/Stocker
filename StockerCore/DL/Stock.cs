using System;

namespace StockerCore.DL
{
    public class Stock
    {
        #region Properties
        public int ID { get; set; }

        public string Symbol { get; set; }
        public string Name { get; set; } 
        public string Currency { get; set; }

        public decimal? DayValue { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? DividendPerShare { get; set; }

        public decimal MovingAverage50 { get; set; }
        public decimal ChangeInMovingAverage50 { get; set; }
        public decimal PercentMovingAverage50 { get; set; }

        public decimal MovingAverage200 { get; set; }
        public decimal ChangeInMovingAverage200 { get; set; }
        public decimal PercentMovingAverage200 { get; set; }

        public decimal BookValue { get; set; }
        public decimal DayLow { get; set; }
        public decimal DayHigh { get; set; }
        public decimal YearLow { get; set; }
        public decimal YearHigh { get; set; }
        public decimal PERatio { get; set; }
        public decimal Open { get; set; }
        public decimal PreviousClose { get; set; } 
        public decimal Change { get; set; }  
        public decimal LastValue { get; set; }
        
        public DateTime? LastTradeDate { get; set; }

        //These are computed, and thus, we need not the DB here
        public decimal Total { get { return Quantity ?? 0 * DayValue ?? 0; } }
        public decimal DividendValue { get { return Quantity ?? 0 * DividendPerShare ?? 0; } }
        public bool IsUp { get { return Change > 0; } }
        #endregion Properties

        public override bool Equals(object obj)
        {
            var other = obj as Stock;
            if (other == null)
                return false;
            return other.ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}) - @ {2} ({3} from {4} on {5:dd-MMM-yyyy})", Name, Symbol, 
                                 DayValue, (IsUp ? "Up" : "Down"), LastValue, LastTradeDate);
        }

    }
}
