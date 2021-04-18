﻿using System;
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
        
        //<CNAME>SearchForProducts</CNAME>
        public Result<IEnumerable<IItem>> SearchForProduct(string token, string query)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IItem>>(userRes.Error);
            }

            return Result.Ok<IEnumerable<IItem>>(_storeRepository.SearchForProduct(query));
        }
        
        public Result<IEnumerable<IItem>> SearchForStore(string token, string query)
        {
            throw new NotImplementedException();
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

            return store.AddItemToStore(DtoUtils.ItemDtoToProductInfo(item), user);
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
            
            return store.EditItemToStore(DtoUtils.ItemDtoToProductInfo(item), user);

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
        
        //<CNAME>UpdateManagerPermissions</CNAME>
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
            throw new System.NotImplementedException();
        }
        
        
        //<CNAME:GetStoreStaff</CNAME>
        public Result<IEnumerable<StaffPermission>> GetStoreStaffAndTheirPermissions(string token, string storeId)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IEnumerable<StaffPermission>>(userAndStoreRes.Error);
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            var staffPermission = new List<StaffPermission>();
            var tuplePermissionRes = store.GetStoreStaffAndTheirPermissions(user);
            if (tuplePermissionRes.IsFailure)
            {
                return Result.Fail<IEnumerable<StaffPermission>>(tuplePermissionRes.Error);
            }
            
            foreach (var (item1, item2) in tuplePermissionRes.Value)
            {
                staffPermission.Add(new StaffPermission(item1, item2));
            }

            return Result.Ok<IEnumerable<StaffPermission>>(staffPermission);
        }

        public Result<IList<IPurchaseHistory>> GetPurchaseHistoryOfStore(string token, string storeId)
        {
            Result<Tuple<IUser, IStore>> userAndStoreRes = GetUserAndStore(token, storeId);
            if (userAndStoreRes.IsFailure)
            {
                return Result.Fail<IList<IPurchaseHistory>>(userAndStoreRes.Error);
            }
            IUser user = userAndStoreRes.Value.Item1;
            IStore store = userAndStoreRes.Value.Item2;

            Result<IList<PurchaseRecord>> purchaseHistoryRes = store.GetPurchaseHistory(user);
            if (purchaseHistoryRes.IsFailure)
            {
                return Result.Fail<IList<IPurchaseHistory>>(purchaseHistoryRes.Error);
            }

            return Result.Ok<IList<IPurchaseHistory>>((IList<IPurchaseHistory>) purchaseHistoryRes.Value);
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
            var newItemInfo = itemRes.Value.ShowItem();
            newItemInfo.amount = amount;
            return user.AddItemToCart(newItemInfo);
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

        public Result PurchaseCart(string token)
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
            Result<PurchaseInfo> purchaseRes = cart.BuyWholeCart(user);
            if (purchaseRes.IsFailure)
            {
                return purchaseRes;
            }

            return Result.Ok();
        }

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
        
        //<CNAME>PersonalPurchaseHistory</CNAME>
        public Result<IEnumerable<IPurchaseHistory>> GetPurchaseHistory(string token)
        {
            Result<IUser> userRes = _userManager.GetUserIfConnectedOrLoggedIn(token);
            if (userRes.IsFailure)
            {
                return Result.Fail<IEnumerable<IPurchaseHistory>>(userRes.Error);
            }
            IUser user = userRes.Value;
            
            // TODO GetStorePurchaseHistory return IBusket instead of purchaseRecord
            //user.GetStorePurchaseHistory();
            throw new System.NotImplementedException();

        }
        
        //<CNAME>AdminGetAllUserHistory</CNAME>
        public Result<IEnumerable<IPurchaseHistory>> AdminGetPurchaseHistoryUser(string token, string storeId)
        {
            throw new System.NotImplementedException();
        }
        //<CNAME>AdminGetStoreHistory</CNAME>
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