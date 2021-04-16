using System;
using System.Collections.Generic;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class User : IUser
    {
        // 
        // private Transactions - history
        private bool _isRegistered;
        private UserToSystemState _systemState;
        private string _userName;
        private ICart _myCart;
        private MemberData _myData;



        //constructors
        public User(string userName)
        {
            _userName = userName;
            _systemState = Guest.State;
            _myCart = new Cart(this);
            _isRegistered = false;
        }

        public User(UserToSystemState systemState, MemberData memberData)
        {
            _systemState = systemState;
            _myData = memberData;
            _myCart = _myData.Cart;
            _userName = _myData.Username;
            _isRegistered = true;
        }
    

    //Facade
        public Result Login(UserToSystemState systemState, MemberData memberData)
        {
            if (systemState == null)
            {
                return Result.Fail("Invalid State for user " + _userName);
            }

            if (memberData == null)
            {
                return Result.Fail("Invalid memberData for user " + _userName);
            }

            return _systemState.Login(this,systemState, memberData);
        }


        public Result Logout(string toGuestName)
        {
            return _systemState.Logout(this,toGuestName);
        }

        public Result<bool> IsRegistered()
        {
            return Result.Ok(_isRegistered);
        }

        public Result OpenStore(IStore store)
        {
            return _systemState.OpenStore(this, store);
        }

        public Result AddItemToCart(Item item)
        {
            return _myCart.AddItemToCart(item);
        }

        public Result<CartInfo> GetCartInfo()
        {
            //@TODO:: cart.getInfo();
            throw new NotImplementedException();
        }

        public Result EditCart(ItemInfo info)
        {
            return _myCart.EditCart(info);
        }

        public Result AppointUserToOwner(IStore store, IUser user)
        {
            // Result res = user.MakeOwner(store, this);
            // if (res.IsFailure)
            //     return res; //@TODO::move block to Member.AppointUserToOwner
            return _systemState.AppointUserToOwner(this, store, user);
        }

        public Result AppointUserToManager(IStore store, IUser user)
        {
            return _systemState.AppointUserToManager(this, store, user);
        }

        public Result AddPermissionsToManager(IStore store, IUser user, StorePermission permission)
        {
            return _systemState.AddPermissionsToManager(this, store, user, permission);
        }

        public Result RemovePermissionsToManager(IStore store, IUser user, StorePermission permission)
        {
            return _systemState.RemovePermissionsToManager(this, store, user, permission);
        }

        public Result<IList<IUser>> GetAllStoreStakeholders(IStore store)
        {
            //@TODO: find out if this means all of apointed-by's
            throw new NotImplementedException();
        }

        public Result<IBasket> GetUserPurchaseHistory(IStore store)
        {
            //@TODO:: find out why store?;
            throw new NotImplementedException();
        }

        public Result<IBasket> GetStorePurchaseHistory(IStore store)
        {
            //@TODO:: find out why this is here..?;
            throw new NotImplementedException();
        }


        //User to User
        public Result MakeOwner(IStore store)
        {
            return _systemState.MakeOwner(this, store);
        }

        public Result MakeManager(IStore store)
        {
            return _systemState.MakeManager(this, store);
        }

        public Result AddPermissions(IStore store, StorePermission permission)
        {
            return _systemState.AddPermissions(this, store, permission);
        }

        public Result RemovePermissions(IStore store, StorePermission permission)
        {
            return _systemState.RemovePermissions(this, store, permission);
        }



        //InBusiness
        public Result HasPermission(IStore store, StorePermission storePermission)
        {
            return _systemState.HasPermission(this, store, storePermission);
        }

        public Result EnterBasketToHistory(IBasket basket)
        {
            return _systemState.EnterBasketToHistory(this, basket);
        }




        public void testPrint()
        {
            UserToSystemState guest = Guest.State;
        }

        public void testPrint(Guest s)
        {
            Console.WriteLine("printing Guest");
        }

        public void testPrint(Member s)
        {
            Console.WriteLine("printing Member");
        }
        
        
        



        //Admin Functions


        //Guest Functions

        public Result Login(Guest guest, UserToSystemState systemState, MemberData memberData)
        {
            Result res = memberData.Cart.MergeCarts(_myCart);
            if (res.IsFailure)
            {
                return res;
            }
            _systemState = systemState;
            _myData = memberData;
            _myCart = _myData.Cart;
            _userName = _myData.Username;
            _isRegistered = true;
            return Result.Ok();
        }
        
        
        //Member Functions
        public Result Logout(Member member, string toGuestName)
        {
            _systemState = Guest.State;
            _myData = null;
            _userName = toGuestName;
            _myCart = new Cart(this);
            _isRegistered = false;
            return Result.Ok();
        }

        public Result OpenStore(Member member, IStore store)
        {
            bool res =_myData.StoresFounded.TryAdd(store,true);
            
            if (res)
            {
                return Result.Ok();
            }
            return Result.Fail("Unable to open store");
        }

        public Result AppointUserToOwner(Member store, IStore user, IUser otherUser)
        {
            throw new NotImplementedException();
        }

        public Result AppointUserToManager(Member store, IStore user, IUser otherUser)
        {
            throw new NotImplementedException();
        }

        public Result MakeOwner(Member store, IStore store1)
        {
            throw new NotImplementedException();
        }

        public Result MakeManager(Member store, IStore store1)
        {
            throw new NotImplementedException();
        }

        public Result AddPermissionsToManager(Member store, IStore user, IUser otherUser, StorePermission permission)
        {
            throw new NotImplementedException();
        }

        public Result RemovePermissionsToManager(Member store, IStore user, IUser otherUser, StorePermission permission)
        {
            throw new NotImplementedException();
        }

        public Result AddPermissions(Member store, IStore permission, StorePermission storePermission)
        {
            throw new NotImplementedException();
        }

        public Result RemovePermissions(Member store, IStore permission, StorePermission storePermission)
        {
            throw new NotImplementedException();
        }

        public Result HasPermission(Member store, IStore storePermission, StorePermission storePermission1)
        {
            throw new NotImplementedException();
        }

        public Result EnterBasketToHistory(Member basket, IBasket basket1)
        {
            throw new NotImplementedException();
        }
    }

}