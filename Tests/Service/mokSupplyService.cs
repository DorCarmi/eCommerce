using System.Threading;
using System.Threading.Tasks;
using eCommerce.Adapters;
using eCommerce.Common;

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

        public async Task<Result<int>> SupplyProducts(string storeName, string[] itemsNames, string userAddress)
        {
            await Task.Delay(2000);
            //TODO may generate id
            return Result.Ok(100000);
        }

        public async Task<Result> CheckSupplyInfo(int transactionId)
        {
            await Task.Delay(2000);
            return Result.Ok();
        }
    }
}