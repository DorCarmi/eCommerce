using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace eCommerce.Adapters
{
    public class SupplyProxy : ISupplyAdapter
    {
        private static ISupplyAdapter _adapter;
        
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
                return true;
            }

            return await _adapter.SupplyProducts(storeName, itemsNames, userAddress);

        }
    }
}