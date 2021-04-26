using System.Collections.ObjectModel;
using eCommerce.Business.Service;

namespace eCommerce.Service
{
    public class SItem : IItem
    {
        public string ItemName { get; set; }
        public string StoreName { get; set; }
        public int Amount { get; set; }
        
        public string Category { get; set; }
        public ReadOnlyCollection<string> KeyWords { get; set; }
        public double PricePerUnit { get; set; }

        public SItem(string productName, string storeName, int amount,
            string category, ReadOnlyCollection<string> keyWords, double pricePerUnit)
        {
            ItemName = productName;
            StoreName = storeName;
            Amount = amount;
            Category = category;
            KeyWords = keyWords;
            PricePerUnit = pricePerUnit;
        }
    }
}