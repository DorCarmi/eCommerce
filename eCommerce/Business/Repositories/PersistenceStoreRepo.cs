using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using eCommerce.Adapters;
using eCommerce.Common;

namespace eCommerce.Business.Repositories
{
    public class PersistenceStoreRepo : AbstractStoreRepo
    {
        // TODO remove dictionary and use dal in all the functions
        private ConcurrentDictionary<string, Store> _stores;

        public PersistenceStoreRepo()
        {
            _stores = new ConcurrentDictionary<string, Store>();
            PaymentProxy._adapter = new WSEPPaymentAdapter();
            SupplyProxy._adapter = new WSEPSupplyAdapter();
        }

        public override IEnumerable<ItemInfo> SearchForItem(string query)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ItemInfo> SearchForItemByPrice(string query, double @from, double to)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ItemInfo> SearchForItemByCategory(string query, string category)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> SearchForStore(string query)
        {
            throw new NotImplementedException();
        }

        public override bool Add([NotNull] Store store)
        {
            return _stores.TryAdd(store.GetStoreName().ToUpper(), store);
        }

        public override Store GetOrNull([NotNull] string storeName)
        {
            Store store = null;
            _stores.TryGetValue(storeName.ToUpper(), out store);
            return store;
        }

        public override void Remove(string id)
        {
            throw new System.NotImplementedException();
        }

        public override void Update(Store data)
        {
            throw new System.NotImplementedException();
        }
    }
}