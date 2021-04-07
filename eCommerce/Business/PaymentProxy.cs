using System.Threading.Tasks;

namespace eCommerce.Business
{
    public class PaymentProxy : IPaymentAdapter
    {
        private IPaymentAdapter _adapter;

        public PaymentProxy()
        {
            _adapter = null;
        }
        
        public async Task<bool> Charge()
        {
            if (_adapter == null)
            {
                await Task.Delay(100);
                return true;
            }

            return await _adapter.Charge();
        }
    }
}