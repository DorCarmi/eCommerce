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
                new MemberDataRepository(),
                new StoreRepository());

        private IRepository<MemberData> _memberDataRepository;
        private StoreRepository _storeRepository;
        private ConnectionManager _connectionManager;
        
        private MarketFacade(IUserAuth userAuth,
            IRepository<MemberData> memberDataRepository,
            StoreRepository storeRepo)
        {
            _memberDataRepository = memberDataRepository;
            _storeRepository = storeRepo;
            _connectionManager = new ConnectionManager(userAuth);
        }

        public static MarketFacade GetInstance()
        {
            return _instance;
        }

        public static MarketFacade CreateInstanceForTests(IUserAuth userAuth,
            IRepository<MemberData> memberDataRepository,
            StoreRepository storeRepo)
        {
            return new MarketFacade(userAuth, memberDataRepository, storeRepo);
        }
        
        // <CNAME>Connect</CNAME>
        public string Connect()
        {
            return _connectionManager.CreateNewGuestConnection();
        }

        // <CNAME>Disconnect</CNAME>
        public void Disconnect(string token)
        {
            _connectionManager.Disconnect(token);
        }

        // <CNAME>Register</CNAME>
        public Result Register(string token, MemberInfo memberInfo, string password)
        {
            if (_connectionManager.GetUser(token).IsFailure)
            {
                return Result.Fail("Need to be connected or logged in");
            }
            
            Result validMemberInfoRes = IsValidMemberInfo(memberInfo);
            if (validMemberInfoRes.IsFailure)
            {
                return validMemberInfoRes;
            }

            Result authRegistrationRes = _connectionManager.RegisterAtAuthorization(memberInfo.Username, password);
            if (authRegistrationRes.IsFailure)
            {
                return authRegistrationRes;
            }
            
            MemberData memberData = new MemberData(memberInfo.Clone());
            if (!_memberDataRepository.Add(memberData))
            {
                // TODO maybe remove the user form userAuth and log it
                return Result.Fail("User already exists");
            }

            return Result.Ok();
        }

        // <CNAME>Login</CNAME>
        public Result<string> Login(string guestToken, string username, string password, ServiceUserRole role)
        {
            MemberData memberData = _memberDataRepository.GetOrNull(username);
            if (memberData == null)
            {
                return Result.Fail<string>("Username or password not valid");
            }

            return _connectionManager.Login(guestToken, username,
                password, role, memberData);
        }

        // <CNAME>Logout</CNAME>
        public Result<string> Logout(string token)
        {
            return _connectionManager.Logout(token);
        }

        public Result<IEnumerable<IProduct>> SearchForProduct(string token, string query)
        {
            Result<IUser> userRes = _connectionManager.GetUser(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IProduct>>(userRes.Error);
            }

            return Result.Ok<IEnumerable<IProduct>>(_storeRepository.SearchForProduct(query));
        }

        public Result AddNewItemToStore(string token, IProduct product)
        {
            Result<IUser> userRes = _connectionManager.GetUser(token);
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
            Result<IUser> userRes = _connectionManager.GetUser(token);
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
            Result<IUser> userRes = _connectionManager.GetUser(token);
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
        
        /// <summary>
        /// Check if the member information is valid
        /// </summary>
        /// <returns>Result of the check</returns>
        private Result IsValidMemberInfo(MemberInfo memberInfo)
        {
            Result fullDataRes = memberInfo.IsBasicDataFull();
            if (fullDataRes.IsFailure)
            {
                return fullDataRes;
            }

            if (!RegexUtils.IsValidEmail(memberInfo.Email))
            {
                return Result.Fail("Invalid email address");
            }

            return Result.Ok();
        }
    }
}