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
        private MemberInfo _memberInfo;
        private ICart _myCart;
        public string Username {get => _memberInfo.Username;}
        
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
            _memberInfo = new MemberInfo(userName, null,null,DateTime.Now, null);
            _systemState = Guest.State;
            _myCart = new Cart(this);
            _isRegistered = false;
            dataLock = new Object();
        }


        public User(MemberInfo info)
        {
            _isRegistered = true;
            _memberInfo = info;
            dataLock = new Object();
            _systemState = Member.State;
            _myCart = new Cart(this);
            // _userName = memberData.Username;
            _storesFounded = new ConcurrentDictionary<IStore, bool>();
            _storesOwned = new ConcurrentDictionary<IStore, OwnerAppointment>();
            _storesManaged = new ConcurrentDictionary<IStore, ManagerAppointment>();
            _appointedOwners = new ConcurrentDictionary<IStore, IList<OwnerAppointment>>();
            _appointedManagers = new ConcurrentDictionary<IStore, IList<ManagerAppointment>>();
            _transHistory = new UserTransactionHistory();
        } 
        public User(UserToSystemState state, MemberInfo info)
        {
            _isRegistered = true;
            _memberInfo = info;
            dataLock = new Object();
            _systemState = state;
            _myCart = new Cart(this);
            _storesFounded = new ConcurrentDictionary<IStore, bool>();
            _storesOwned = new ConcurrentDictionary<IStore, OwnerAppointment>();
            _storesManaged = new ConcurrentDictionary<IStore, ManagerAppointment>();
            _appointedOwners = new ConcurrentDictionary<IStore, IList<OwnerAppointment>>();
            _appointedManagers = new ConcurrentDictionary<IStore, IList<ManagerAppointment>>();
            _transHistory = new UserTransactionHistory();
        }


        //Facade
        public Result Login(UserToSystemState systemState, MemberData memberData)
        {
            if (systemState == null)
            {
                return Result.Fail("Invalid State for user " + Username);
            }

            if (memberData == null)
            {
                return Result.Fail("Invalid memberData for user " + Username);
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
            return _myCart.AddItemToCart(this, item);
        }

        public Result<ICart> GetCartInfo()
        {
            return Result.Ok<ICart>(_myCart);
        }

        public Result EditCart(ItemInfo info)
        {
            return _myCart.EditCartItem(this, info);
        }

        public Result AppointUserToOwner(IStore store, IUser user)
        {
            return _systemState.AppointUserToOwner(this, store, user);
        }

        public Result AppointUserToManager(IStore store, IUser user)
        {
            return _systemState.AppointUserToManager(this, store, user);
        }

        // remove 2 functions:
        public Result AddPermissionsToManager(IStore store, IUser user, StorePermission permission)
        {
            return _systemState.AddPermissionsToManager(this, store, user, permission);
        }

        public Result RemovePermissionsToManager(IStore store, IUser user, StorePermission permission)
        {
            return _systemState.RemovePermissionsToManager(this, store, user, permission);
        }

        public Result UpdatePermissionsToManager(IStore store, IUser user, IList<StorePermission> permissions)
        {
            return _systemState.UpdatePermissionsToManager(this, store, user, permissions);
        }


        public Result<IList<IUser>> GetAllStoreStakeholders(IStore store)
        {
            //@TODO: find out if this means all of apointed-by's
            throw new NotImplementedException();
        }

        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory()
        {
            return _systemState.GetUserPurchaseHistory(this);
        }
        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory(IUser otherUser)
        {
            return _systemState.GetUserPurchaseHistory(this, otherUser);
        }

        public Result<IList<PurchaseRecord>> GetStorePurchaseHistory(IStore store)
        {
            return _systemState.GetStorePurchaseHistory(this, store);
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

        public Result EnterRecordToHistory(PurchaseRecord record)
        {
            return _systemState.EnterRecordToHistory(this, record);
        }



        #region Admin Functions
        //@TODO:: add required functions
       

    #endregion Admin Functions
    
    
    #region Guest Functions
        public Result Login(Guest guest, UserToSystemState systemState, MemberData memberData)
        {
            throw new NotImplementedException();

            // Result res = memberData.Cart.MergeCarts(_myCart);
            // if (res.IsFailure)
            // {
            //     return res;
            // }

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
            // _userName = toGuestName;
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
                return Result.Fail("user \'"+Username+"\' is not an owner of the given store.");
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
                return Result.Fail("user \'"+Username+"\' is not an owner of the given store.");
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
            return Result.Fail<OwnerAppointment>("unable to add user \'"+Username+"\' as store owner");
        }

        public Result<ManagerAppointment> MakeManager(Member member, IStore store)
        {
            ManagerAppointment newManager = new ManagerAppointment();
            if (!_storesOwned.ContainsKey(store) && _storesManaged.TryAdd(store, newManager))
            {
                return Result.Ok<ManagerAppointment>(newManager);
            }
            return Result.Fail<ManagerAppointment>("unable to add user \'"+Username+"\' as store Manager");
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
            return Result.Fail("user\'"+Username+"\' can not grant permissions to given manager");
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
            return Result.Fail("user\'"+Username+"\' can not remove permissions from given manager");
        }

        
        public Result UpdatePermissionsToManager(Member member, IStore store, IUser otherUser, IList<StorePermission> permissions)
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
                    return manager.UpdatePermissions(permissions);
                }
            }
            return Result.Fail("user\'"+Username+"\' can not remove permissions from given manager");
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
            
            return Result.Fail("user\'"+Username+"\' is not a stakeholder of the given store");
        }

        public Result EnterRecordToHistory(Member member, PurchaseRecord record)
        {
            return _transHistory.EnterRecordToHistory(record);
        }
        

        public Result<IList<PurchaseRecord>> GetUserHistory()
        {
            return _transHistory.GetUserHistory();
        }

        public Result<IList<PurchaseRecord>> GetStoreHistory(IStore store)
        {
            return store.GetPurchaseHistory(this);
        }
        
        #endregion //Member Functions
    }
    
   }