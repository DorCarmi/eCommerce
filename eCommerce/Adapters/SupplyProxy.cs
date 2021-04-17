using System.Threading.Tasks;

namespace eCommerce.Adapters
{
    public class SupplyProxy : ISupplyAdapter
    {
        private ISupplyAdapter _adapter;

        public SupplyProxy()
        {
            _adapter = null;
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