using System.Collections.Generic;

namespace eCommerce.Business.Service
{
    public class ProductDto
    {
        public string ProductName { get; set; }
        public string StoreName { get; set; }
        public int Amount { get; set; }
        public IList<string> Categories { get; set; }
        public IList<string> KeyWords { get; set; }
        public float PricePerUnit { get; set; }

        public ProductDto(string productName, string storeName, int amount,
            IList<string> categories, IList<string> keyWords, float pricePerUnit)
        {
            ProductName = productName;
            StoreName = storeName;
            Amount = amount;
            Categories = categories;
            KeyWords = keyWords;
            PricePerUnit = pricePerUnit;
        }
    }
}