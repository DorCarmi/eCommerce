using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private static MarketFacade _instance = new MarketFacade();

        private UserAuth _auth;
        private UserRepository _userRepository;
        
        private MarketFacade()
        {
            _auth = UserAuth.GetInstance();
            _userRepository = new UserRepository();
        }

        public static MarketFacade GetInstance()
        {
            return _instance;
        }
        
        /// <CNAME>Connect</CNAME>
        public string Connect()
        {
            string token = _auth.Connect();
            Result<AuthData> userAuthDataRes = _auth.GetData(token);
            
            if (userAuthDataRes.IsFailure)
            {
                throw new AuthenticationException("Authorization connect returned not valid token");
            }

            if (!_userRepository.Add(CreateGuestUser(userAuthDataRes.Value.Username)))
            {
                throw new AuthenticationException("Not new guest has been created");
            }

            return token;
        }

        public void Disconnect(string token)
        {
            throw new System.NotImplementedException();
        }

        public Result Register(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public Result<string> Login(string guestToken, string username, string password, ServiceUserRole role)
        {
            throw new System.NotImplementedException();
        }

        public string Logout(string token)
        {
            throw new System.NotImplementedException();
        }

        public Result<IEnumerable<ItemDto>> SearchForProduct(string token, string query)
        {
            throw new System.NotImplementedException();
        }

        public Result AddNewItemToStore(string token, ItemDto item)
        {
            throw new System.NotImplementedException();
        }

        public Result EditItemAmountInStore(string token, ItemDto item)
        {
            throw new System.NotImplementedException();
        }

        public Result RemoveProductFromStore(string token, string storeId, string itemId)
        {
            throw new System.NotImplementedException();
        }

        public Result AppointCoOwner(string token, string storeId, string appointedUserId)
        {
            throw new System.NotImplementedException();
        }

        public Result AppointManager(string token, string storeId, string appointedManagerUserId)
        {
            throw new System.NotImplementedException();
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

        private IUser CreateGuestUser(string guestName)
        {
            // TODO update it with user implementation
            return new User(guestName);
        }
    }
}