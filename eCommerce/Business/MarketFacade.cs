using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Auth;
using eCommerce.Business.Service;
using eCommerce.Common;
using eCommerce.Service;

namespace eCommerce.Business
{
    // TODO should be singleton
    // TODO check authException if we should throw them
    public class MarketFacade : IMarketFacade
    {
        private static MarketFacade _instance =
            new MarketFacade(
                UserAuth.GetInstance(),
                new RegisteredUsersRepository(),
                new StoreRepository());

        private StoreRepository _storeRepository;
        private UserManager _userManager;
        
        private MarketFacade(IUserAuth userAuth,
            IRepository<IUser> registeredUsersRepo,
            StoreRepository storeRepo)
        {
            _storeRepository = storeRepo;
            _userManager = new UserManager(userAuth, registeredUsersRepo);
            CreateMainAdmin();
        }

        public static MarketFacade GetInstance()
        {
            return _instance;
        }

        public static MarketFacade CreateInstanceForTests(IUserAuth userAuth,
            IRepository<IUser> registeredUsersRepo,
            StoreRepository storeRepo)
        {
            return new MarketFacade(userAuth, registeredUsersRepo, storeRepo);
        }

        public void CreateMainAdmin()
        {
            MemberInfo adminInfo = new MemberInfo(
                "_Admin",
                "Admin@eCommerce.com",
                "TheAdmin",
                DateTime.Now, 
                null);
            _userManager.AddAdmin(adminInfo, "_Admin");
        }


        #region UserManage
        // <CNAME>Connect</CNAME>
        public string Connect()
        {
            return _userManager.Connect();
        }

        // <CNAME>Disconnect</CNAME>
        public void Disconnect(string token)
        {
            _userManager.Disconnect(token);
        }

        // <CNAME>Register</CNAME>
        public Result Register(string token, MemberInfo memberInfo, string password)
        {
            return _userManager.Register(token, memberInfo, password);
        }

        // <CNAME>Login</CNAME>
        public Result<string> Login(string guestToken, string username, string password, UserToSystemState role)
        {
            return _userManager.Login(guestToken, username, password, role);
        }

        // <CNAME>Logout</CNAME>
        public Result<string> Logout(string token)
        {
            return _userManager.Logout(token);
        }
        
        //<CNAME>PersonalPurchaseHistory</CNAME>
        public Result<IList<PurchaseRecord>> GetPurchaseHistory(string token)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IList<PurchaseRecord>>(userRes.Error);
            }
            IUser user = userRes.Value;

            var result = user.GetUserPurchaseHistory();

            if (result.IsFailure)
            {
                return Result.Fail<IList<PurchaseRecord>>(result.Error);
            }

            return result;
        }
        
         //<CNAME>AppointCoOwner</CNAME>
        public Result AppointCoOwner(string token, string storeId, string appointedUserId)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;
            
            Result<IUser> appointedUserRes = _userManager.GetUser(appointedUserId);
            if (appointedUserRes.IsFailure)
            {
                return appointedUserRes;
            }
            IUser appointedUser = appointedUserRes.Value;

            return user.AppointUserToOwner(store, appointedUser);
        }
                
        //<CNAME>AppointManager</CNAME>
        public Result AppointManager(string token, string storeId, string appointedManagerUserId)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;
            
            Result<IUser> appointedUserRes = _userManager.GetUser(appointedManagerUserId);
            if (appointedUserRes.IsFailure)
            {
                return appointedUserRes;
            }
            IUser appointedUser = appointedUserRes.Value;

            return user.AppointUserToOwner(store, appointedUser);
        }
        
        public Result UpdateManagerPermission(string token, string storeId, string managersUserId, IList<StorePermission> permissions)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;
            
            Result<IUser> mangerUserRes = _userManager.GetUser(managersUserId);
            if (mangerUserRes.IsFailure)
            {
                return mangerUserRes;
            }
            IUser managerUser = mangerUserRes.Value;
            return user.UpdatePermissionsToManager(store, managerUser, permissions);
        }
        
        //<CNAME>RemoveManagerPermissions</CNAME>
        public Result RemoveManagerPermission(string token, string storeId, string managersUserId,
            IList<StorePermission> permissions)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;
            
            Result<IUser> mangerUserRes = _userManager.GetUser(managersUserId);
            if (mangerUserRes.IsFailure)
            {
                return mangerUserRes;
            }
            IUser managerUser = mangerUserRes.Value;
            foreach (var permission in permissions)
            {
                var res = user.RemovePermissionsToManager(store, managerUser, permission);
                if(res.IsFailure)
                {
                    return res;
                }
            }

            return Result.Ok();
        }
        //<CNAME:GetStoreStaff</CNAME>
        public Result<IList<StaffPermission>> GetStoreStaffAndTheirPermissions(string token, string storeId)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IList<StaffPermission>>(userAndStoreRes.Error);
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            var staffPermission = new List<StaffPermission>();
            var tuplePermissionRes = store.GetStoreStaffAndTheirPermissions(user);
            if (tuplePermissionRes.IsFailure)
            {
                return Result.Fail<IList<StaffPermission>>(tuplePermissionRes.Error);
            }
            
            foreach (var (item1, item2) in tuplePermissionRes.Value)
            {
                staffPermission.Add(new StaffPermission(item1, item2));
            }

            return Result.Ok<IList<StaffPermission>>(staffPermission);
        }
        //<CNAME>AdminGetAllUserHistory</CNAME>
        public Result<IList<PurchaseRecord>> AdminGetPurchaseHistoryUser(string token, string ofUserId)
        {
            var userAndStoreRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
             if (userAndStoreRes.IsFailure)
             {
                 return Result.Fail<IList<PurchaseRecord>>(userAndStoreRes.Error);
             }


             IUser user = userAndStoreRes.Value;

             var ofUser=_userManager.GetUser(ofUserId);
             if (ofUser.IsFailure)
             {
                 return Result.Fail<IList<PurchaseRecord>>(ofUser.Error);
             }

             return user.GetUserPurchaseHistory(ofUser.Value);
        }
         
         //<CNAME>AdminGetStoreHistory</CNAME>
         public Result<IList<PurchaseRecord>> AdminGetPurchaseHistoryStore(string token, string storeId)
         {
             Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
             if (userAndStoreRes.IsFailure)
             {
                 return Result.Fail<IList<PurchaseRecord>>(userAndStoreRes.Error);
             }
             IUser user = userAndStoreRes.Value.Item1;
             IStore store = userAndStoreRes.Value.Item2;

             return user.GetStorePurchaseHistory(store);
         }
        #endregion

        #region ItemsAndStores
        //<CNAME>SearchForProducts</CNAME>
        public Result<IEnumerable<IItem>> SearchForItem(string token, string query)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IItem>>(userRes.Error);
            }

            return Result.Ok<IEnumerable<IItem>>(_storeRepository.SearchForItem(query));
        }

        public Result<IEnumerable<IItem>> SearchForItemByPriceRange(string token, string query, double @from = 0, double to = Double.MaxValue)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IItem>>(userRes.Error);
            }

            if (to < from)
            {
                to = from;
            }
            
            return Result.Ok<IEnumerable<IItem>>(_storeRepository.SearchForItemByPrice(query, from, to));
        }

        public Result<IEnumerable<IItem>> SearchForItemByCategory(string token, string query, string category)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IItem>>(userRes.Error);
            }

            return Result.Ok<IEnumerable<IItem>>(_storeRepository.SearchForItemByCategory(query, category));
        }

        public Result<IEnumerable<string>> SearchForStore(string token, string query)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<string>>(userRes.Error);
            }

            return Result.Ok(_storeRepository.SearchForStore(query));
        }
        
        public Result<StoreDto> GetStore(string token, string storeId)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<StoreDto>(userAndStoreRes.Error);
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            IList<IItem> storeItems = new List<IItem>();
            foreach (var item in store.GetAllItems())
            {
                storeItems.Add(item.ShowItem());
            }

            return Result.Ok(new StoreDto(storeId, storeItems));
        } 
        public Result<IEnumerable<IItem>> GetAllStoreItems(string token, string storeId)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IItem>>(userAndStoreRes.Error);
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            IList<IItem> storeItems = new List<IItem>();
            foreach (var item in store.GetAllItems())
            {
                storeItems.Add(item.ShowItem());
            }

            return Result.Ok<IEnumerable<IItem>>(storeItems);
            
        }
        
        public Result<IItem> GetItem(string token, string storeId, string itemId)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IItem>(userAndStoreRes.Error);
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            Result<Item> itemRes = store.GetItem(itemId);
            if (itemRes.IsFailure)
            {
                return Result.Fail<IItem>(itemRes.Error);
            }

            return Result.Ok<IItem>(itemRes.Value.ShowItem());
        }
        
        
        
        #endregion

        #region UserBuyingFromStores
        
        //<CNAME>AddItemToCart</CNAME>
        public Result AddItemToCart(string token, string productId, string storeId, int amount)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;


            Result<Item> itemRes = store.GetItem(productId);
            if (itemRes.IsFailure)
            {
                return itemRes;
            }
            var newItemInfo = itemRes.Value.ShowItem();
            newItemInfo.amount = amount;
            return user.AddItemToCart(newItemInfo);
        }

        //<CNAME>EditCart</CNAME>  
        public Result EditItemAmountOfCart(string token, string itemId, string storeId, int amount)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;
            
            // TODO implement store and user
            return null;
        }
        //<CNAME>GetCart</CNAME>
        public Result<CartDto> GetCart(string token)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<CartDto>(userRes.Error);
            }
            IUser user = userRes.Value;

            Result<ICart> cartRes = user.GetCartInfo();
            if (cartRes.IsFailure)
            {
                return Result.Fail<CartDto>(cartRes.Error);
            }

            ICart cart = cartRes.Value;
            var baskets = new List<BasketDto>();
            foreach (var basket in cart.GetBaskets())
            {
                baskets.Add(DtoUtils.IBasketToBasketDto(basket));
            }

            return Result.Ok<CartDto>(new CartDto(baskets));
        }
        
        public Result<double> GetPurchaseCartPrice(string token)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<double>(userRes.Error);
            }
            IUser user = userRes.Value;

            Result<ICart> cartRes = user.GetCartInfo();
            if (cartRes.IsFailure)
            {
                return Result.Fail<double>(cartRes.Error);
            }

            ICart cart = cartRes.Value;
            return cart.CalculatePricesForCart();
        }

        


        //<CNAME>BuyWholeCart</CNAME>
        public Result PurchaseCart(string token, PaymentInfo paymentInfo)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<double>(userRes.Error);
            }
            IUser user = userRes.Value;

            Result<ICart> cartRes = user.GetCartInfo();
            if (cartRes.IsFailure)
            {
                return Result.Fail<double>(cartRes.Error);
            }

            ICart cart = cartRes.Value;
            Result purchaseRes = cart.BuyWholeCart(user,paymentInfo);
            if (purchaseRes.IsFailure)
            {
                return purchaseRes;
            }

            return Result.Ok();
        }
        
        #endregion

        #region StoreManage
        //<CNAME>OpenStore</CNAME>
        public Result OpenStore(string token, string storeName, IItem item)
        {
            // TODO check with user and store
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail(userRes.Error);
            }
            IUser user = userRes.Value;

            IStore newStore = new Store(storeName, user, DtoUtils.ItemDtoToProductInfo(item));
            if (!_storeRepository.Add(newStore))
            {
                return Result.Fail("Store name taken");
            }

            if (user.OpenStore(newStore).IsFailure)
            {
                return Result.Fail("Error");
            }

            return Result.Ok();
        }
        
        //<CNAME>ItemsToStore</CNAME>
        public Result AddNewItemToStore(string token, IItem item)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, item.StoreName);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            return store.AddItemToStore(DtoUtils.ItemDtoToProductInfo(item), user);
        }
        
        //<CNAME>ItemsInStore</CNAME>
        public Result RemoveProductFromStore(string token, string storeId, string itemID)
        {
            
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            return store.RemoveItemToStore(itemID, user);
        }

        public Result EditItemInStore(string token, IItem item)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, item.StoreName);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;
            //public ItemInfo(int amount, string name, string storeName, string category,int pricePerUnit, List<string> keyWords,Item theItem)
            store.EditItemToStore(
                new ItemInfo(item.Amount, item.ItemName, item.StoreName, item.Category, item.KeyWords.ToList(),
                    (int) item.PricePerUnit), user);
            
            return store.EditItemToStore(DtoUtils.ItemDtoToProductInfo(item), user);
        }

        public Result UpdateStock_AddItems(string token, IItem item)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, item.StoreName);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            return store.UpdateStock_AddItems(DtoUtils.ItemDtoToProductInfo(item), user);
        }
        
        public Result UpdateStock_SubtractItems(string token, IItem item)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, item.StoreName);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            return store.UpdateStock_SubtractItems(DtoUtils.ItemDtoToProductInfo(item), user);
        }

        public Result AddBuyingStrategyToStorePolicy(string token, string storeId,  PurchaseStrategyName purchaseStrategy)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            return store.AddPurchaseStrategyToStore(user, purchaseStrategy);
        }
        
        public Result<IList<PurchaseStrategyName>> GetStorePolicyPurchaseStrategies(string token, string storeId,  PurchaseStrategyName purchaseStrategy)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IList<PurchaseStrategyName>>(userAndStoreRes.Error);
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            return store.GetStorePurchaseStrategy(user);
        }
        
        public Result UpdateStorePurchaseStrategies(string token, string storeId,  PurchaseStrategyName purchaseStrategy)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            return store.UpdatePurchaseStrategies(user, purchaseStrategy);
        }

        public Result AddPurchaseStrategyToStoreItem(string token, string storeID, string itemID,
            PurchaseStrategyName strategyName)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeID);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            return store.AddPurchaseStrategyToStoreItem(user, storeID,itemID,strategyName);
        }
        
        public Result RemovePurchaseStrategyToStoreItem(string token, string storeID, string itemID,
            PurchaseStrategyName strategyName)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeID);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            return store.RemovePurchaseStrategyToStoreItem(user, storeID,itemID,strategyName);
        }
        
        public Result<IList<PurchaseStrategyName>> GetPurchaseStrategyToStoreItem(string token, string storeID, string itemID,
            PurchaseStrategyName strategyName)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeID);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IList<PurchaseStrategyName>>(userAndStoreRes.Error);
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            return store.GetPurchaseStrategyToStoreItem(user, storeID,itemID,strategyName);
        }

        public Result AddDiscountToProduct()
        {
            throw new NotImplementedException();
        }

        public Result RemoveDiscountsFromProduct()
        {
            throw new NotImplementedException();
        }
        
        public Result GetProductDiscounts()
        {
            throw new NotImplementedException();
        }

        public Result AddAllowedDiscountsToStore()
        {
            throw new NotImplementedException();
        }
        
        public Result UpdateAllowedDiscountsToStore()
        {
            throw new NotImplementedException();
        }

        public Result GetPolicy()
        {
            throw new NotImplementedException();
        }
        
        //<CNAME>GetStoreHistory</CNAME>
        public Result<IList<PurchaseRecord>> GetPurchaseHistoryOfStore(string token, string storeId)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IList<PurchaseRecord>>(userAndStoreRes.Error);
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            Result<IList<PurchaseRecord>> purchaseHistoryRes = store.GetPurchaseHistory(user);
            if (purchaseHistoryRes.IsFailure)
            {
                return Result.Fail<IList<PurchaseRecord>>(purchaseHistoryRes.Error);
            }

            return Result.Ok<IList<PurchaseRecord>>((IList<PurchaseRecord>) purchaseHistoryRes.Value);
        }
        #endregion

        private Result<Tuple<IUser, IStore>> GetUserAndStore(string token, string storeId)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<Tuple<IUser, IStore>>(userRes.Error);
            }
            IUser user = userRes.Value;
            
            IStore store = _storeRepository.GetOrNull(storeId);
            if (store == null)
            {
                return Result.Fail<Tuple<IUser, IStore>>("Store doesn't exist");
            }

            return Result.Ok(new Tuple<IUser, IStore>(user, store));
        }
    }
}