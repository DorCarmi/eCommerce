using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class StoreRepository : IRepository<IStore>
    {
        private ConcurrentDictionary<string, IStore> _stores;

        public StoreRepository()
        {
            _stores = new ConcurrentDictionary<string, IStore>();
        }

        public bool Add([NotNull] IStore store)
        {
            return _stores.TryAdd(store.StoreName, store);
        }

        public IStore GetOrNull([NotNull] string storeName)
        {
            IStore store = null;
            _stores.TryGetValue(storeName, out store);
            return store;
        }

        public void Remove(string id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ItemInfo> SearchForProduct(string query)
        {
            IList<ItemInfo> queryMatches = new List<ItemInfo>();
            foreach (var store in _stores.Values)
            {
                foreach (var item in store.SearchForItems(query))
                {
                    queryMatches.Add(item);
                }
            }

            return queryMatches;
        }
    }
}