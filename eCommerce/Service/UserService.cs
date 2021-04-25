using System.Collections.Generic;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace eCommerce.Service
{
    public class UserService : IUserService
    {
        private IMarketFacade _marketFacade;
        
        internal UserService(IMarketFacade marketFacade)
        {
            _marketFacade = MarketFacade.GetInstance();
        }
        
        public UserService()
        {
            _marketFacade = MarketFacade.GetInstance();
        }

        static UserService CreateUserServiceForTests(IUserAuth userAuth,
            IRepository<IUser> registeredUsersRepo,
            StoreRepository storeRepo)
        {
            IMarketFacade marketFacade = MarketFacade.CreateInstanceForTests(userAuth, registeredUsersRepo, storeRepo);
            return new UserService(marketFacade);
        }

        public Result<SPurchaseHistory> GetPurchaseHistory(string token)
        {
            Result<IList<PurchaseRecord>> purchaseHistoryRes = _marketFacade.GetPurchaseHistory(token);
            if (purchaseHistoryRes.IsFailure)
            {
                return Result.Fail<SPurchaseHistory>(purchaseHistoryRes.Error);
            }

            return Result.Ok(new SPurchaseHistory(purchaseHistoryRes.Value));
        }

        public Result AppointCoOwner(string token, string storeId, string appointedUserId)
        {
            return _marketFacade.AppointCoOwner(token, storeId, appointedUserId);
        }

        public Result AppointManager(string token, string storeId, string appointedManagerUserId)
        {
            return _marketFacade.AppointManager(token, storeId, appointedManagerUserId);
        }

        public Result UpdateManagerPermission(string token, string storeId, string managersUserId, IList<StorePermission> permissions)
        {
            return _marketFacade.UpdateManagerPermission(token, storeId, managersUserId, permissions);
        }

        public Result<SPurchaseHistory> AdminGetPurchaseHistoryUser(string token, string ofUserId)
        {
            Result<IList<PurchaseRecord>> purchaseRecordRes = _marketFacade.AdminGetPurchaseHistoryUser(token, ofUserId);
            if (purchaseRecordRes.IsFailure)
            {
                return Result.Fail<SPurchaseHistory>(purchaseRecordRes.Error);
            }

            return Result.Ok(new SPurchaseHistory(purchaseRecordRes.Value));
        }

        public Result<SPurchaseHistory> AdminGetPurchaseHistoryStore(string token, string storeId)
        {
            Result<IList<PurchaseRecord>> purchaseRecordRes = _marketFacade.GetPurchaseHistoryOfStore(token, storeId);
            if (purchaseRecordRes.IsFailure)
            {
                return Result.Fail<SPurchaseHistory>(purchaseRecordRes.Error);
            }

            return Result.Ok(new SPurchaseHistory(purchaseRecordRes.Value));
        }
    }
}