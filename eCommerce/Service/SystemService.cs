using eCommerce.Business;

namespace eCommerce.Service
{
    public class SystemService: ISystemService
    {

        private MarketState _marketState;
        
        public SystemService()
        {
            _marketState = MarketState.GetInstance();
        }
        
        public bool IsSystemValid()
        {
            return _marketState.ValidState;
        }

        public bool GetErrMessageIfValidSystem(out string message)
        {
            return _marketState.TryGetErrMessage(out message);
        }
    }
}