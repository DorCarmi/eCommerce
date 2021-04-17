using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace eCommerce.Business.Service
{
    public interface IItem
    {
        public string ItemName { get; }
        public string StoreName { get; }
        public int Amount { get; }
        public string Category { get; }
        public ReadOnlyCollection<string> KeyWords { get; }
        public float PricePerUnit { get; }
    }
}