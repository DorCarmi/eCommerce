using System.Threading.Tasks;
using eCommerce.Adapters;

namespace Tests.Service
{
    public class mokSupplyService : ISupplyAdapter
    {
        public async Task<bool> SupplyProducts(string storeName, string[] itemsNames, string userAddress)
        {
            await Task.Delay(5000);
            return true;
        }
    }
}