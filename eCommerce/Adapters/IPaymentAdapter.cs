using System.Threading.Tasks;

namespace eCommerce.Adapters
{
    public interface IPaymentAdapter
    {
        public Task<bool> Charge();
    }
}