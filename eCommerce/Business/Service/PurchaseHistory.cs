using System;
using System.Collections.Generic;

namespace eCommerce.Business.Service
{
    public interface IPurchaseHistory
    {
        public string StoreId { get; }

        public BasketInfo BasketInfo { get; }

        public DateTime PurchaseTime { get; }
    }
}