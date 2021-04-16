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
            return _userManager.CreateNewGuestConnection();
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

        public Result<IEnumerable<IProduct>> SearchForProduct(string token, string query)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IProduct>>(userRes.Error);
            }

            return Result.Ok<IEnumerable<IProduct>>(_storeRepository.SearchForProduct(query));
        }

        public Result AddNewItemToStore(string token, IProduct product)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return userRes;
            }
            IUser user = userRes.Value;
            
            IStore store = _storeRepository.GetOrNull(product.StoreName);
            if (store == null)
            {
                return Result.Fail("Store doesn't exist");
            }

            return store.AddItemToStore(DtoUtils.ProductDtoToProductInfo(product), user);
        }

        public Result EditItemAmountInStore(string token, IProduct product)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return userRes;
            }
            IUser user = userRes.Value;
            
            IStore store = _storeRepository.GetOrNull(product.StoreName);
            if (store == null)
            {
                return Result.Fail("Store doesn't exist");
            }
            
            return store.EditProduct(DtoUtils.ProductDtoToProductInfo(product), user);

        }

        public Result RemoveProductFromStore(string token, string storeId, string productId)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return userRes;
            }
            IUser user = userRes.Value;
            
            IStore store = _storeRepository.GetOrNull(storeId);
            if (store == null)
            {
                return Result.Fail("Store doesn't exist");
            }
            
            return store.RemoveProduct(productId, user);
        }

        public Result AppointCoOwner(string token, string storeId, string appointedUserId)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return userRes;
            }
            IUser user = userRes.Value;
            
            IStore store = _storeRepository.GetOrNull(storeId);
            if (store == null)
            {
                return Result.Fail("Store doesn't exist");
            }
            
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
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return userRes;
            }
            IUser user = userRes.Value;
            
            IStore store = _storeRepository.GetOrNull(storeId);
            if (store == null)
            {
                return Result.Fail("Store doesn't exist");
            }
            
            Result<IUser> appointedUserRes = _userManager.GetUser(appointedManagerUserId);
            if (appointedUserRes.IsFailure)
            {
                return appointedUserRes;
            }
            IUser appointedUser = appointedUserRes.Value;

            return user.AppointUserToOwner(store, appointedUser);
        }

        public Result UpdateManagerPermission(string token, string storeId, string appointedManagerUserId, IList<Service.StorePermission> permissions)
        {
            throw new System.NotImplementedException();
        }

        public Result<IEnumerable<StaffPermission>> GetStoreStaffAndTheirPermissions(string token, string storeId)
        {
            throw new System.NotImplementedException();
        }

        public Result<IEnumerable<PurchaseHistory>> GetPurchaseHistoryOfStore(string token, string storeId)
        {
            throw new System.NotImplementedException();
        }

        public Result AddItemToCart(string token, string itemId, string storeId, int amount)
        {
            throw new System.NotImplementedException();
        }

        public Result EditItemAmountOfCart(string token, string itemId, string storeId, int amount)
        {
            throw new System.NotImplementedException();
        }

        public Result<CartDto> GetCart(string token)
        {
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

        public Result OpenStore(string token, string storeName)
        {
            throw new System.NotImplementedException();
        }

        public Result<IEnumerable<PurchaseHistory>> GetPurchaseHistory(string token)
        {
            throw new System.NotImplementedException();
        }

        public Result<IEnumerable<PurchaseHistory>> AdminGetPurchaseHistoryUser(string token, string storeId, string ofUserId)
        {
            throw new System.NotImplementedException();
        }

        public Result<IEnumerable<PurchaseHistory>> AdminGetPurchaseHistoryStore(string token, string storeId)
        {
            throw new System.NotImplementedException();
        }
    }
}