using System.Collections.Generic;

namespace eCommerce.Business.Service
{
    public class StoreDto
    {
        public string StoreName { get; }
        public IList<IItem> Items { get; }

        public StoreDto(string storeName, IList<IItem> items)
        {
            StoreName = storeName;
            Items = items;
        }
    }
}