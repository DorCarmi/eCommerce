using System.Collections;
using System.Collections.Generic;
using eCommerce.Auth;
using eCommerce.Business;

using eCommerce.Common;

namespace eCommerce.Service
{
    public class CartService : ICartService
    {
        private IMarketFacade _marketFacade;

        internal CartService(IMarketFacade marketFacade)
        {
            _marketFacade = MarketFacade.GetInstance();
        }
        
        public CartService()
        {
            _marketFacade = MarketFacade.GetInstance();
        }
        
        public static CartService CreateUserServiceForTests(IUserAuth userAuth,
            IRepository<User> registeredUsersRepo,
            StoreRepository storeRepo)
        {
            IMarketFacade marketFacade = MarketFacade.CreateInstanceForTests(userAuth, registeredUsersRepo, storeRepo);
            return new CartService(marketFacade);
        }

        public Result AddItemToCart(string token, string itemId, string storeId, int amount)
        {
            return _marketFacade.AddItemToCart(token, itemId, storeId, amount);
        }

        public Result EditItemAmountOfCart(string token, string itemId, string storeId, int amount)
        {
            return _marketFacade.EditItemAmountOfCart(token, itemId, storeId, amount);
        }

        public Result<SCart> GetCart(string token)
        {
            Result<ICart> cartRes = _marketFacade.GetCart(token);
            if (cartRes.IsFailure)
            {
                return Result.Fail<SCart>(cartRes.Error);
            }
            ICart cart = cartRes.Value;

            return Result.Ok(new SCart(cart.GetBaskets(),cart.GetUser().Username));
        }

        public Result<double> GetPurchaseCartPrice(string token)
        {
            return _marketFacade.GetPurchaseCartPrice(token);
        }

        public Result PurchaseCart(string token, PaymentInfo paymentInfo)
        {
            return _marketFacade.PurchaseCart(token, paymentInfo);
        }
    }
}