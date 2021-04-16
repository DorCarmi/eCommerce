using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace eCommerce.Business.Service
{
    public class ProductDto : IProduct
    {
        public string ProductName { get; set; }
        public string StoreName { get; set; }
        public int Amount { get; set; }
        
        public string Category { get; set; }
        public ReadOnlyCollection<string> KeyWords { get; set; }
        public float PricePerUnit { get; set; }

        public ProductDto(string productName, string storeName, int amount,
            string category, ReadOnlyCollection<string> keyWords, float pricePerUnit)
        {
            ProductName = productName;
            StoreName = storeName;
            Amount = amount;
            Category = category;
            KeyWords = keyWords;
            PricePerUnit = pricePerUnit;
        }
    }
}