using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace eCommerce.Business.Service
{
    public class ItemDto : IItem
    {
        public string ItemName { get; set; }
        public string StoreName { get; set; }
        public int Amount { get; set; }
        
        public string Category { get; set; }
        public ReadOnlyCollection<string> KeyWords { get; set; }
        public float PricePerUnit { get; set; }

        public ItemDto(string productName, string storeName, int amount,
            string category, ReadOnlyCollection<string> keyWords, float pricePerUnit)
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