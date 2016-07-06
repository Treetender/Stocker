
namespace StockerCore.DL
{
    class WalletStock
    {
        public int StockID { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }

        public decimal PurchaseTotal { get { return Quantity * PurchasePrice; } }
    }
}
