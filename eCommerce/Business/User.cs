using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class User : IUser
    {
        
        private bool _isRegistered;
        private UserToSystemState _systemState;
        private string _userName;
        private ICart _myCart;

        private Object dataLock;
        //MemberData:
        private ConcurrentDictionary<IStore, bool> _storesFounded;
        private ConcurrentDictionary<IStore, OwnerAppointment> _storesOwned;
        private ConcurrentDictionary<IStore, ManagerAppointment> _storesManaged;
        private ConcurrentDictionary<IStore, IList<OwnerAppointment>> _appointedOwners;
        private ConcurrentDictionary<IStore, IList<ManagerAppointment>> _appointedManagers;
        private UserTransactionHistory _transHistory ;



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
            // _myData = memberData;
            _myCart = memberData.Cart;
            _userName = memberData.Username;
            _isRegistered = true;
            _storesFounded = memberData.StoresFounded;
            _storesOwned = memberData.StoresOwned;
            _storesManaged = memberData.StoresManaged;
            _appointedOwners = memberData.AppointedOwners;
            _appointedManagers = memberData.AppointedManagers;
            _transHistory = memberData.History;
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

        public Result AddItemToCart(ItemInfo item)
        {
            return _myCart.AddItemToCart(item);
        }

        public Result<CartInfo> GetCartInfo()
        {
            return _myCart.ShowCart();
        }

        public Result EditCart(ItemInfo info)
        {
            return _myCart.EditCart(info);
        }

        public Result AppointUserToOwner(IStore store, IUser user)
        {
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
        public Result<OwnerAppointment> MakeOwner(IStore store)
        {
            return _systemState.MakeOwner(this, store);
        }

        public Result<ManagerAppointment> MakeManager(IStore store)
        {
            return _systemState.MakeManager(this, store);
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


        
        
        
        



    
    #region Admin Functions
        //@TODO:: add required functions
       

    #endregion Admin Functions
    
    
    #region Guest Functions
        public Result Login(Guest guest, UserToSystemState systemState, MemberData memberData)
        {
            Result res = memberData.Cart.MergeCarts(_myCart);
            if (res.IsFailure)
            {
                return res;
            }

            throw new NotImplementedException();
            _systemState = systemState;
            // _myData = memberData;
            // _myCart = _myData.Cart;
            // _userName = _myData.Username;
            _isRegistered = true;
            return Result.Ok();
        }
    #endregion Guest Functions
    
    
    #region Member Functions
        public Result Logout(Member member, string toGuestName)
        {
            _systemState = Guest.State;
            throw new NotImplementedException();
            // _myData = null;
            _userName = toGuestName;
            _myCart = new Cart(this);
            _isRegistered = false;
            return Result.Ok();
        }

        public Result OpenStore(Member member, IStore store)
        {
            // adds store to both Owned-By and Founded-By
            OwnerAppointment owner = new OwnerAppointment(this);
            // @TODO:: add extra founder's permissions to 'owner'
            
            bool res =_storesFounded.TryAdd(store,true) && _storesOwned.TryAdd(store,owner);
            if (res)
            {
                return Result.Ok();
            }
            return Result.Fail("Unable to open store");
        }

        public Result AppointUserToOwner(Member member, IStore store, IUser otherUser)
        {
            if (!_storesOwned.ContainsKey(store))
            {
                return Result.Fail("user \'"+_userName+"\' is not an owner of the given store.");
            }

            Result<OwnerAppointment> res = otherUser.MakeOwner(store);
            if (res.IsFailure)
            {
                return res;
            }

            OwnerAppointment newOwner = res.Value;
            // acquire user-data lock
            lock (dataLock)
            {
                if (!_appointedOwners.ContainsKey(store))
                {
                    IList<OwnerAppointment> ownerList = new List<OwnerAppointment>();
                    ownerList.Add(newOwner);
                    if (!_appointedOwners.TryAdd(store, ownerList))
                        return Result.Fail("unable to add other-user as an Appointed-Owner");
                }
                else
                {
                    _appointedOwners[store].Add(newOwner);
                }
            }//release lock
            return store.AppointNewOwner(otherUser,newOwner);
        }

        public Result AppointUserToManager(Member member, IStore store, IUser otherUser)
        {
            if (!_storesOwned.ContainsKey(store)){
                return Result.Fail("user \'"+_userName+"\' is not an owner of the given store.");
            }
            Result<ManagerAppointment> res = otherUser.MakeManager(store);
            if (res.IsFailure){
                return res;
            }

            ManagerAppointment newManager = res.Value;
            // acquire user-data lock
            lock (dataLock)
            {
                if (!_appointedManagers.ContainsKey(store))
                {
                    IList<ManagerAppointment> managerList = new List<ManagerAppointment>();
                    managerList.Add(newManager);
                    if (!_appointedManagers.TryAdd(store, managerList))
                        return Result.Fail("unable to add other-user as an Appointed-Manager");
                }
                else
                {
                    _appointedManagers[store].Add(newManager);
                }
            }//release lock
            return store.AppointNewManager(otherUser,newManager);
        }

        public Result<OwnerAppointment> MakeOwner(Member member, IStore store)
        {
            OwnerAppointment newOwner = new OwnerAppointment(this);
            if (_storesOwned.TryAdd(store, newOwner))
            {
                return Result.Ok<OwnerAppointment>(newOwner);
            }
            return Result.Fail<OwnerAppointment>("unable to add user \'"+_userName+"\' as store owner");
        }

        public Result<ManagerAppointment> MakeManager(Member member, IStore store)
        {
            ManagerAppointment newManager = new ManagerAppointment();
            if (!_storesOwned.ContainsKey(store) && _storesManaged.TryAdd(store, newManager))
            {
                return Result.Ok<ManagerAppointment>(newManager);
            }
            return Result.Fail<ManagerAppointment>("unable to add user \'"+_userName+"\' as store Manager");
        }

        public Result AddPermissionsToManager(Member member, IStore store, IUser otherUser, StorePermission permission)
        {
            // if (_appointedOwners.ContainsKey(store))
            // {
            //     OwnerAppointment owner = null;
            //     lock (dataLock)
            //     {
            //         owner = _appointedOwners[store].FirstOrDefault((oa) => oa.User == otherUser);
            //     }
            //     if (owner != null)
            //     {
            //         return owner.AddPermissions(permission);
            //     }
            // }

            if (_appointedManagers.ContainsKey(store))
            {
                ManagerAppointment manager = null;
                lock (dataLock)
                {
                    manager = _appointedManagers[store].FirstOrDefault((ma) => ma.User == otherUser);
                }

                if (manager != null)
                {
                    return manager.AddPermissions(permission);
                }
            }
            return Result.Fail("user\'"+_userName+"\' can not grant permissions to given manager");
        }

        public Result RemovePermissionsToManager(Member member, IStore store, IUser otherUser, StorePermission permission)
        {
            if (_appointedManagers.ContainsKey(store))
            {
                ManagerAppointment manager = null;
                lock (dataLock)
                {
                    manager = _appointedManagers[store].FirstOrDefault((ma) => ma.User == otherUser);
                }

                if (manager != null)
                {
                    return manager.RemovePermission(permission);
                }
            }
            return Result.Fail("user\'"+_userName+"\' can not remove permissions from given manager");
        }

        public Result HasPermission(Member member, IStore store, StorePermission permission)
        {
            if(_storesOwned.ContainsKey(store))
            {
                return _storesOwned[store].HasPermission(permission);
            }

            if (_storesManaged.ContainsKey(store))
            {
                return _storesManaged[store].HasPermission(permission);
            }
            
            return Result.Fail("user\'"+_userName+"\' is not a stakeholder of the given store");
        }

        public Result EnterBasketToHistory(Member member, IBasket basket)
        {
            return _transHistory.EnterBasketToHistory(basket);
        }
        
        #endregion //Member Functions
        
    }
    
   }