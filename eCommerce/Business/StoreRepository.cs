using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
            return _stores.TryAdd(store.GetStoreName(), store);
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

        public IEnumerable<ItemInfo> SearchForItem(string query)
        {
            IList<ItemInfo> queryMatches = new List<ItemInfo>();
            foreach (var store in _stores.Values)
            {
                foreach (var item in store.SearchItem(query))
                {
                    queryMatches.Add(item.ShowItem());
                }
            }

            return queryMatches;
        }
        
        public IEnumerable<ItemInfo> SearchForItemByPrice(string query, double from, double to)
        {
            IList<ItemInfo> queryMatches = new List<ItemInfo>();
            foreach (var store in _stores.Values)
            {
                foreach (var item in store.SearchItemWithPriceFilter(query, (int)from, (int)to))
                {
                    queryMatches.Add(item.ShowItem());
                }
            }

            return queryMatches;
        }

        public IEnumerable<ItemInfo> SearchForItemByCategory(string query, string category)
        {
            IList<ItemInfo> queryMatches = new List<ItemInfo>();
            foreach (var store in _stores.Values)
            {
                foreach (var item in store.SearchItemWithCategoryFilter(query, category))
                {
                    queryMatches.Add(item.ShowItem());
                }
            }

            return queryMatches;
        }

        public IEnumerable<string> SearchForStore(string query)
        { 
            List<string> storeNames = _stores.Keys.ToList();
            storeNames.Sort(EditDistance);
            return storeNames;
        }
        
        
        
        /// <summary>
        /// How match chars we need to add or replace to src to get to traget
        /// </summary>
        /// <param name="src">The source</param>
        /// <param name="target">The target string</param>
        /// <returns>Edit distance</returns>
        public static int EditDistance(string src, string target) {
            int n = src.Length;
            int m = target.Length;
            int[,] d = new int[n + 1, m + 1];
            if (n == 0) {
                return m;
            }
            if (m == 0) {
                return n;
            }
            for (int i = 0; i <= n; d[i, 0] = i++)
                ;
            for (int j = 0; j <= m; d[0, j] = j++)
                ;
            for (int i = 1; i <= n; i++) {
                for (int j = 1; j <= m; j++) {
                    int editDistance = (target[j - 1] == src[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + editDistance);
                }
            }
            return d[n, m];
        }
    }
}