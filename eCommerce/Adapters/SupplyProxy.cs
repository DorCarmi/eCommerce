using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace eCommerce.Adapters
{
    public class SupplyProxy : ISupplyAdapter
    {
        private static ISupplyAdapter _adapter;
        
        public static int REAL_HITS = 0;
        public static int PROXY_HITS = 0;
        
        public static void AssignSupplyService(ISupplyAdapter supplyAdapter)
        {
            _adapter = supplyAdapter;
        }

        public SupplyProxy()
        {
            
        }

        public async Task<bool> SupplyProducts(string storeName, string[] itemsNames, string userAddress)
        {
            if (_adapter == null)
            {
                await Task.Delay(100);
                PROXY_HITS++;
                return true;
            }
            var ans=await _adapter.SupplyProducts(storeName, itemsNames, userAddress);
            if (ans)
            {
                REAL_HITS++;
            }

            return ans;

        }

        public async Task<bool> CheckSupplyInfo(string storeName, string[] itemsNames, string userAddress)
        {
            if (_adapter == null)
            {
                await Task.Delay(100);
                return true;
            }

            return await _adapter.CheckSupplyInfo(storeName, itemsNames, userAddress);
        }
    }
}