using System.Threading.Tasks;

namespace eCommerce.Adapters
{
    public interface ISupplyAdapter
    {

        public Task<bool> SupplyProducts(string storeName, string[] itemsNames, string userAddress);
        public Task<bool> CheckSupplyInfo(string storeName, string[] itemsNames, string userAddress);

    }
}