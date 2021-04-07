using System.Threading.Tasks;

namespace eCommerce.Business
{
    public interface IPaymentAdapter
    {
        public Task<bool> Charge();
    }
}