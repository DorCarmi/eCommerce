using System;
using System.Collections;
using System.Collections.Generic;
using eCommerce.Auth;
using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Service
{
    public class StoreService : IStoreService
    {
        private IMarketFacade _marketFacade;

        internal StoreService(IMarketFacade marketFacade)
        {
            _marketFacade = MarketFacade.GetInstance();
        }
        
        public StoreService()
        {
            _marketFacade = MarketFacade.GetInstance();
        }

        static StoreService CreateUserServiceForTests(IUserAuth userAuth,
            IRepository<IUser> registeredUsersRepo,
            StoreRepository storeRepo)
        {
            IMarketFacade marketFacade = MarketFacade.CreateInstanceForTests(userAuth, registeredUsersRepo, storeRepo);
            return new StoreService(marketFacade);
        }

        public Result<IList<StaffPermission>> GetStoreStaffAndTheirPermissions(string token, string storeId)
        {
            Result<IList<Tuple<string, IList<StorePermission>>>> staffPermissionsRes =
                _marketFacade.GetStoreStaffAndTheirPermissions(token, storeId);
            if (staffPermissionsRes.IsFailure)
            {
                return Result.Fail<IList<StaffPermission>>(staffPermissionsRes.Error);
            }

            IList<StaffPermission> staffPermissions = new List<StaffPermission>();
            foreach (var staffPermission in staffPermissionsRes.Value)
            {
                staffPermissions.Add(new StaffPermission(staffPermission.Item1, staffPermission.Item2));
            }

            return Result.Ok(staffPermissions);
        }

        public Result<IEnumerable<IItem>> SearchForItem(string token, string query)
        {
            return _marketFacade.SearchForItem(token, query);
        }

        public Result<IEnumerable<IItem>> SearchForItemByPriceRange(string token, string query, double @from = 0, double to = Double.MaxValue)
        {
            return _marketFacade.SearchForItemByPriceRange(token, query, from, to);
        }

        public Result<IEnumerable<IItem>> SearchForItemByCategory(string token, string query, string category)
        {
            return _marketFacade.SearchForItemByCategory(token, query, category);
        }

        public Result<IEnumerable<string>> SearchForStore(string token, string query)
        {
            return _marketFacade.SearchForStore(token, query);
        }

        public Result<ServiceStore> GetStore(string token, string storeId)
        {
            Result<IStore> storeRes = _marketFacade.GetStore(token, storeId);
            if (storeRes.IsFailure)
            {
                return Result.Fail<ServiceStore>(storeRes.Error);
            }
            IStore store = storeRes.Value;
            
            IList<IItem> storeItems = new List<IItem>();
            foreach (var item in store.GetAllItems())
            {
                storeItems.Add(item.ShowItem());
            }

            return Result.Ok(new ServiceStore(storeId, storeItems));
        }

        public Result<IEnumerable<IItem>> GetAllStoreItems(string token, string storeId)
        {
            return _marketFacade.GetAllStoreItems(token, storeId);
        }

        public Result<IItem> GetItem(string token, string storeId, string itemId)
        {
            return _marketFacade.GetItem(token, storeId, itemId);
        }

        public Result OpenStore(string token, string storeName, IItem item)
        {
            return _marketFacade.OpenStore(token, storeName, item);
        }

        public Result AddNewItemToStore(string token, IItem item)
        {
            return _marketFacade.AddNewItemToStore(token, item);
        }

        public Result RemoveItemFromStore(string token, string storeId, string itemId)
        {
            return _marketFacade.RemoveItemFromStore(token, storeId, itemId);
        }

        public Result EditItemInStore(string token, IItem item)
        {
            return _marketFacade.EditItemInStore(token, item);
        }

        public Result UpdateStock_AddItems(string token, IItem item)
        {
            //TODO: do we need it there is an edit function
            throw new NotImplementedException();
        }

        public Result UpdateStock_SubtractItems(string token, IItem item)
        {
            //TODO: do we need it there is an edit function
            throw new NotImplementedException();
        }

        public Result<SPurchaseHistory> GetPurchaseHistoryOfStore(string token, string storeId)
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