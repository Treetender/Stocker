using SQLite;

namespace StockerCore.DL
{
    class WalletStock
    {
        [PrimaryKey]
        public int StockID { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }

        [Ignore]
        public decimal PurchaseTotal { get { return Quantity * PurchasePrice; } }
    }
}
