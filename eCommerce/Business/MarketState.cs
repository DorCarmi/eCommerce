using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Business
{
    public class MarketState
    {
        private static MarketState _instance = new MarketState();
        public bool ValidState { get; private set; }
        private string _errMessage;

        private MarketState()
        {
            ValidState = true;
            _errMessage = null;
        }

        public static MarketState GetInstance()
        {
            return _instance;
        }

        public void SetErrorState(string message, Func<bool> serviceValidityChecker)
        {
            ValidState = false;
            _errMessage = message;

            Task checker = new Task(() => CheckServiceAndUpdateState(serviceValidityChecker));
            checker.Start();
        }

        public bool TryGetErrMessage(out string message)
        {
            if (ValidState)
            {
                message = null;
                return false;
            }

            message = _errMessage;
            return true;
        }

        private async void CheckServiceAndUpdateState(Func<bool> serviceValidityChecker)
        {
            while (!ValidState)
            {
                bool isServiceAvailable = serviceValidityChecker.Invoke();
                if (isServiceAvailable)
                {
                    ValidState = true;
                    _errMessage = null;
                    continue;
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}