using System;
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
                    new StoreRepository());

        private IUserAuth _auth;
        private IRepository<MemberData> _memberDataRepository;
        private IRepository<IStore> _storeRepository;
        private IDictionary<string, IUser> _connectedUsers;
        
        private MarketFacade(IUserAuth userAuth,
            IRepository<IStore> storeRepo)
        {
            _auth = userAuth;
            _memberDataRepository = new MemberDataRepository();
            _storeRepository = storeRepo;
            _connectedUsers = new ConcurrentDictionary<string, IUser>();
        }

        public static MarketFacade GetInstance()
        {
            return _instance;
        }

        public static MarketFacade CreateInstanceForTests(IUserAuth userAuth, IRepository<IStore> storeRepo)
        {
            return new MarketFacade(userAuth, storeRepo);
        }
        
        // <CNAME>Connect</CNAME>
        public string Connect()
        {
            string token = _auth.Connect();
            Result<AuthData> userAuthDataRes = _auth.GetDataIfConnected(token);
            if (userAuthDataRes.IsFailure)
            {
                throw new AuthenticationException("Authorization connect returned not valid token");
            }

            string username = userAuthDataRes.Value.Username;
            IUser newUser = CreateGuestUser(username);
            if (!_connectedUsers.TryAdd(username, newUser))
            {
                throw new AuthenticationException("Not new guest has been created");
            }

            newUser.Connect();
            return token;
        }

        // <CNAME>Disconnect</CNAME>
        public void Disconnect(string token)
        {
            Result<AuthData> authData = _auth.GetDataIfConnected(token);
            if (authData.IsFailure)
            {
                return;
            }
            
            _auth.Disconnect(token);
            IUser user = GetOrNullConnectedOrLoggedInUser(authData.Value.Username);
            if (user == null)
            {
                // TODO log it
                return;
            }
            
            user.Disconnect();
            _connectedUsers.Remove(user.Username);
        }

        // <CNAME>Register</CNAME>
        public Result Register(MemberInfo memberInfo, string password)
        {
            Result validMemberInfoRes = IsValidMemberInfo(memberInfo);
            if (validMemberInfoRes.IsFailure)
            {
                return validMemberInfoRes;
            }

            Result authRegistrationRes = _auth.Register(memberInfo.Username, password);
            if (authRegistrationRes.IsFailure)
            {
                return authRegistrationRes;
            }
            
            MemberData memberData = new MemberData(memberInfo.Clone());
            IUser newUser = new User(memberData);
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
            Result<AuthData> guestAuthData = _auth.GetDataIfConnected(guestToken);
            if (guestAuthData.IsFailure)
            {
                return Result.Fail<string>(guestAuthData.Error);;
            }
            
            Result<string> authLoginRes = _auth.Login(username, password, ServiceUserRoleToAuthUserRole(role));
            if (authLoginRes.IsFailure)
            {
                return Result.Fail<string>(authLoginRes.Error);
            }
            
            IUser user = GetOrNullConnectedOrLoggedInUser(guestAuthData.Value.Username);
            MemberData memberData = _memberDataRepository.GetOrNull(username);
            if (user == null || memberData == null 
                || user.Login(ServiceUserRoleToSystemState(role), memberData).IsFailure)
            {
                // TODO log it since in auth the user can log in
                _auth.Logout(authLoginRes.Value);
                return Result.Fail<string>("Invalid username or password");
            }

            ExchangeKeyToConnected(guestAuthData.Value.Username, username);
            _auth.Disconnect(guestToken);
            return Result.Ok(authLoginRes.Value);
        }

        // <CNAME>Logout</CNAME>
        public Result<string> Logout(string token)
        {
            Result<AuthData> userAuthDataRes = _auth.GetDataIfLoggedIn(token);
            if (userAuthDataRes.IsFailure)
            {
                // TODO log it
                return Result.Fail<string>(userAuthDataRes.Error);
            }

            AuthData authData = userAuthDataRes.Value;
            IUser user = GetOrNullConnectedOrLoggedInUser(authData.Username);
            if (user == null)
            {
                // TODO log it since in auth has the user logged in
                return Result.Ok(Connect());
            }

            string newGuestToken = _auth.Connect();
            string newGuestUsername = _auth.GetDataIfConnected(newGuestToken).Value.Username;
            
            _auth.Logout(token);
            user.Logout(newGuestUsername);
            ExchangeKeyToConnected(authData.Username, newGuestUsername);
            return Result.Ok(newGuestToken);
        }

        public Result<IEnumerable<ProductDto>> SearchForProduct(string token, string query)
        {
            throw new System.NotImplementedException();
        }

        public Result AddNewItemToStore(string token, ProductDto product)
        {
            Result<IUser> userRes = GetUser(token);
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

            return store.AddItemToStore(ProductDtoToProductInfo(product), user);
        }

        public Result EditItemAmountInStore(string token, ProductDto product)
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

        private IUser GetOrNullConnectedOrLoggedInUser(string username)
        {
            if (!_connectedUsers.TryGetValue(username, out var user))
            {
                return null;
            }

            return user;
        }
        
        private void ExchangeKeyToConnected(string fromUsername, string toUsername)
        {
            IUser user = GetOrNullConnectedOrLoggedInUser(fromUsername);
            if (user == null)
            {
                return;
            }

            _connectedUsers.Remove(fromUsername);
            _connectedUsers.Add(toUsername, user);
        }
        
        private IUser CreateGuestUser(string guestName)
        {
            // TODO update it with user implementation
            return new User(guestName);
        }

        /// <summary>
        /// Get the user if connected or logged in
        /// </summary>
        /// <param name="token">The Authorization token</param>
        /// <returns>The user recognized by the token</returns>
        private Result<IUser> GetUser(string token)
        {
            Result<AuthData> userAuthDataRes = _auth.GetDataIfLoggedIn(token);
            if (userAuthDataRes.IsFailure)
            {
                userAuthDataRes = _auth.GetDataIfConnected(token);
                if (userAuthDataRes.IsFailure)
                {
                    // TODO log it
                    return Result.Fail<IUser>(userAuthDataRes.Error);
                }
            }

            AuthData authData = userAuthDataRes.Value;
            IUser user = GetOrNullConnectedOrLoggedInUser(authData.Username);
            if (user == null)
            {
                // TODO log it since in auth has the user logged in
                return Result.Fail<IUser>("Error with the connection of the user");
            }

            return Result.Ok(user);
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

        private AuthUserRole ServiceUserRoleToAuthUserRole(ServiceUserRole role)
        {
            switch (role)
            {
                case ServiceUserRole.Member:
                {
                    return AuthUserRole.Member;
                }
                case ServiceUserRole.Admin:
                {
                    return AuthUserRole.Admin;
                }
            }

            // TODO log if it gets here
            return AuthUserRole.Member;
        }
        
        private UserToSystemState ServiceUserRoleToSystemState(ServiceUserRole role)
        {
            // TODO implement
            switch (role)
            {
                case ServiceUserRole.Member:
                {
                    throw new NotImplementedException();
                }
                case ServiceUserRole.Admin:
                {
                    throw new NotImplementedException();
                }
            }

            // TODO log if it gets here
            throw new NotImplementedException();
        }
        
        private ItemInfo ProductDtoToProductInfo(ProductDto productDto)
        {
            return new ItemInfo(
                productDto.Amount,
                productDto.ProductName,
                productDto.StoreName,
                productDto.Categories.FirstOrDefault(),
                productDto.KeyWords);
        }
    }
}