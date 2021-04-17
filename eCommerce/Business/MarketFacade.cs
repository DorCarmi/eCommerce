using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using eCommerce.Auth;
using eCommerce.Business.Service;
using eCommerce.Common;

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
        public Result<string> Login(string guestToken, string username, string password, ServiceUserRole role)
        {
            return _userManager.Login(guestToken, username, password, role);
        }

        // <CNAME>Logout</CNAME>
        public Result<string> Logout(string token)
        {
            return _userManager.Logout(token);
        }

        public Result<IEnumerable<IItem>> SearchForProduct(string token, string query)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IItem>>(userRes.Error);
            }

            return Result.Ok<IEnumerable<IItem>>(_storeRepository.SearchForProduct(query));
        }

        public Result AddNewItemToStore(string token, IItem item)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, item.StoreName);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            return store.AddItemToStore(DtoUtils.ProductDtoToProductInfo(item), user);
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
            
            return store.EditItemToStore(DtoUtils.ProductDtoToProductInfo(item), user);

        }

        public Result RemoveProductFromStore(string token, string storeId, string productId)
        {
            // TODO verify 
            /*Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return userAndStoreRes;
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;
            
            return store.RemoveItemToStore(productId, user);*/
            throw new NotImplementedException();
        }

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

        public Result<IEnumerable<StaffPermission>> GetStoreStaffAndTheirPermissions(string token, string storeId)
        {
            throw new System.NotImplementedException();
        }

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
            
            return Result.Ok(purchaseHistoryRes.Value);
        }

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

            return user.AddItemToCart(itemRes.Value, amount);
        }

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

        public Result<CartDto> GetCart(string token)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<CartDto>(userRes.Error);
            }
            IUser user = userRes.Value;
            
            // TODO return cart info
            throw new System.NotImplementedException();

        }

        public Result<int> GetPurchaseCartPrice(string token)
        {
            throw new System.NotImplementedException();
        }

        public Result PurchaseCart(string token)
        {
            throw new System.NotImplementedException();
        }

        public Result OpenStore(string token, string storeName, IItem item)
        {
            throw new NotImplementedException();
        }

        public Result<IEnumerable<IPurchaseHistory>> GetPurchaseHistory(string token)
        {
            throw new System.NotImplementedException();
        }

        public Result<IEnumerable<IPurchaseHistory>> AdminGetPurchaseHistoryUser(string token, string storeId, string ofUserId)
        {
            throw new System.NotImplementedException();
        }

        public Result<IEnumerable<IPurchaseHistory>> AdminGetPurchaseHistoryStore(string token, string storeId)
        {
            throw new System.NotImplementedException();
        }

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