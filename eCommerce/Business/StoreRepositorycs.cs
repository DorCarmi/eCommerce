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
        private IDictionary<string, IStore> _stores;

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
    }
}