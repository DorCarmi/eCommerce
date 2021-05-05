using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Auth;
using eCommerce.Business.Service;
using eCommerce.Common;
using eCommerce.Service;
using NLog;

namespace eCommerce.Business
{
    // TODO check authException if we should throw them
    public class MarketFacade : IMarketFacade
    {
        private static MarketFacade _instance =
            new MarketFacade(
                UserAuth.GetInstance(),
                new RegisteredUsersRepository(),
                new StoreRepository());

        private static Logger _logger = LogManager.GetCurrentClassLogger();

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
            _userManager.CreateMainAdmin();
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

        public bool IsUserConnected(string token)
        {
            return _userManager.IsUserConnected(token);
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
                _logger.Error($"Error for user {user.Username} in getting the Purchase history");
                return Result.Fail<IList<PurchaseRecord>>(result.Error);
            }

            _logger.Info($"User {user.Username} request purchase history");
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
            
            _logger.Info($"AppointCoOwner({user.Username}, {store.GetStoreName()}, {appointedUserId})");
            
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
            
            _logger.Info($"AppointManager({user.Username}, {store.GetStoreName()}, {appointedManagerUserId})");
            
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
            
            _logger.Info($"UpdateManagerPermission({user.Username}, {store.GetStoreName()}, {managersUserId}, {permissions})");

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
            
            _logger.Info($"RemoveManagerPermission({user.Username}, {store.GetStoreName()}, {managersUserId}, {permissions})");

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
        public Result<IList<Tuple<string, IList<StorePermission>>>> GetStoreStaffAndTheirPermissions(string token,
            string storeId)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IList<Tuple<string, IList<StorePermission>>>>(userAndStoreRes.Error);
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            _logger.Info($"GetStoreStaffAndTheirPermissions({user.Username}, {store.GetStoreName()})");

            return store.GetStoreStaffAndTheirPermissions(user);
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
             
             _logger.Info($"AdminGetPurchaseHistoryUser({user.Username}, {ofUserId})");

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

             _logger.Info($"AdminGetPurchaseHistoryStore({user.Username}, {store.GetStoreName()})");

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

            _logger.Info($"SearchForItem({userRes.Value.Username}, {query})");

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
            
            _logger.Info($"SearchForItemByPriceRange({userRes.Value.Username}, {query}, {from}, {to})");

            return Result.Ok<IEnumerable<IItem>>(_storeRepository.SearchForItemByPrice(query, from, to));
        }

        public Result<IEnumerable<IItem>> SearchForItemByCategory(string token, string query, string category)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IItem>>(userRes.Error);
            }

            _logger.Info($"SearchForItemByCategory({userRes.Value.Username}, {query}, {category})");

            return Result.Ok<IEnumerable<IItem>>(_storeRepository.SearchForItemByCategory(query, category));
        }

        public Result<IEnumerable<string>> SearchForStore(string token, string query)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<string>>(userRes.Error);
            }
            
            _logger.Info($"SearchForStore({userRes.Value.Username}, {query})");

            return Result.Ok(_storeRepository.SearchForStore(query));
        }
        
        public Result<IStore> GetStore(string token, string storeId)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IStore>(userAndStoreRes.Error);
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            _logger.Info($"GetStore({user.Username}, {storeId})");

            return Result.Ok(store);
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
            
            _logger.Info($"GetAllStoreItems({user.Username}, {storeId})");

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
            
            _logger.Info($"GetItem({user.Username}, {storeId}, {itemId})");

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

            _logger.Info($"AddItemToCart({user.Username}, {productId}, {storeId}, {amount})");

            Result<Item> itemRes = store.GetItem(productId);
            if (itemRes.IsFailure)
            {
                return itemRes;
            }
            var newItemInfo = itemRes.Value.ShowItem();
            newItemInfo.amount = amount;
            //newItemInfo.AssignStoreToItem(store);
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
            
            _logger.Info($"EditItemAmountOfCart({user.Username}, {itemId}, {storeId}, {amount})");

            // TODO implement store and user
            return null;
        }
        //<CNAME>GetCart</CNAME>
        public Result<ICart> GetCart(string token)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<ICart>(userRes.Error);
            }
            IUser user = userRes.Value;

            _logger.Info($"GetCart({user.Username})");

            return user.GetCartInfo();
        }
        
        public Result<double> GetPurchaseCartPrice(string token)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<double>(userRes.Error);
            }
            IUser user = userRes.Value;

            _logger.Info($"GetPurchaseCartPrice({user.Username})");

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

            _logger.Info($"GetPurchaseCartPrice({user.Username} {paymentInfo})");
            
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
        public Result OpenStore(string token, string storeName)
        {
            // TODO check with user and store
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail(userRes.Error);
            }
            IUser user = userRes.Value;
            
            _logger.Info($"OpenStore({user.Username})");
            
            IStore newStore = new Store(storeName, user);
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

            _logger.Info($"AddNewItemToStore({user.Username} ,{item})");

            return store.AddItemToStore(DtoUtils.ItemDtoToProductInfo(item), user);
        }
        
        //<CNAME>ItemsInStore</CNAME>
        public Result RemoveItemFromStore(string token, string storeId, string itemId)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            _logger.Info($"RemoveItemFromStore({user.Username} ,{storeId}, {itemId})");

            return store.RemoveItemToStore(itemId, user);
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
            
            _logger.Info($"EditItemInStore({user.Username} , {item})");

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

            _logger.Info($"GetPurchaseHistoryOfStore({user.Username} , {storeId})");

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
            
            _logger.Info($"GetUserAndStore({user.Username} , {storeId})");

            IStore store = _storeRepository.GetOrNull(storeId);
            if (store == null)
            {
                _logger.Error($"User {user.Username} requested invalid sotre {storeId}");
                return Result.Fail<Tuple<IUser, IStore>>("Store doesn't exist");
            }

            return Result.Ok(new Tuple<IUser, IStore>(user, store));
        }
    }
}