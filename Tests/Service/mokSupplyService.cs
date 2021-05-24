using System.Threading.Tasks;
using eCommerce.Adapters;

namespace Tests.Service
{
    public class mokSupplyService : ISupplyAdapter
    {
        private bool checkAns;
        private bool chargeAns;
        public mokSupplyService(bool checkAns, bool chargeAns)
        {
            this.checkAns = checkAns;
            this.chargeAns = chargeAns;
        }
        public async Task<bool> SupplyProducts(string storeName, string[] itemsNames, string userAddress)
        {
            await Task.Delay(5000);
            return chargeAns;
        }

        public async Task<bool> CheckSupplyInfo(string storeName, string[] itemsNames, string userAddress)
        {
            await Task.Delay(5000);
            return checkAns;
        }
    }
}