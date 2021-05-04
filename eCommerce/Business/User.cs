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
        public string Username {get => _memberInfo.Username;}
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

        
    #region User Facacde Interface
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

        /// <TEST> UserTest.TestOpenStore </TEST>
        /// <UC> 'Open a Store' </UC>
        /// <REQ> 3.2 </REQ>
        /// <summary>
        ///  receives an IStore to open. makes this User a founder and an owner. 
        /// </summary>
        /// <param name="store"></param>
        /// <returns>Result, OK/Fail. </returns>
        public Result OpenStore(IStore store)
        {
            return _systemState.OpenStore(this, store);
        }

        public Result<List<string>> GetStoreIds()
        {
            return _systemState.GetStoreIds(this);
        }

        /// <TEST>  </TEST>
        /// <UC> 'Add product to basket' </UC>
        /// <REQ> 2.7 </REQ>
        /// <summary>
        ///  adds the given items to the user's cart. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Result, OK/Fail. </returns>
        public Result AddItemToCart(ItemInfo item)
        {
            return _myCart.AddItemToCart(this, item);
        }

        /// <TEST>  </TEST>
        /// <UC> 'View shopping cart' </UC>
        /// <REQ> 2.8 </REQ>
        /// <summary>
        ///  gets the information from the cart. 
        /// </summary>
        /// <returns>Result, ICart. </returns>
        public Result<ICart> GetCartInfo()
        {
            return Result.Ok<ICart>(_myCart);
        }

        /// <TEST>  </TEST>
        /// <UC> 'Edit shopping cart' </UC>
        /// <REQ> 2.8 </REQ>
        /// <summary>
        ///  changes the given items in user's cart. 
        /// </summary>
        /// <param name="info"></param>
        /// <returns>Result, OK/Fail . </returns>
        public Result EditCart(ItemInfo info)
        {
            return _myCart.EditCartItem(this, info);
        }
        
        public Result AppointUserToOwner(IStore store, IUser user)
        {
            return _systemState.AppointUserToOwner(this, store, user);
        }

        /// <TEST> UserTest.TestAppointUserToManager </TEST>
        /// <UC> 'Nominate member to be store manager' </UC>
        /// <REQ> 4.4 </REQ>
        /// <summary>
        ///  make the user a manager of the given store. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="store"></param>
        /// <returns>Result, OK/Fail. </returns>
        public Result AppointUserToManager(IStore store, IUser user)
        {
            return _systemState.AppointUserToManager(this, store, user);
        }
        
        /// <TEST> UserTest.TestUpdatePermissionsToManager </TEST>
        /// <UC> 'Change management permission for sub-manger' </UC>
        /// <REQ> 4.6 </REQ>
        /// <summary>
        ///  change manager's permissions in the given store. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="store"></param>
        /// <param name="permissions"></param>
        /// <returns>Result, OK/Fail. </returns>
        public Result UpdatePermissionsToManager(IStore store, IUser user, IList<StorePermission> permissions)
        {
            return _systemState.UpdatePermissionsToManager(this, store, user, permissions);
        }

        public Result RemovePermissionsToManager(IStore store, IUser user, StorePermission permission)
        {
            throw new NotImplementedException();
        }

        public Result<IList<IUser>> GetAllStoreStakeholders(IStore store)
        {
            //@TODO_SHARON:: find out if this means all of apointed-by's. might need adjustments on Store side.
            throw new NotImplementedException();
        }

        /// <TEST> UserTest.TestUserPurchaseHistory </TEST>
        /// <UC> 'Review purchase history' </UC>
        /// <REQ> 3.7 </REQ>
        /// <summary>
        ///  returns all purchase-records of the user. 
        /// </summary>
        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory()
        {
            return _systemState.GetUserPurchaseHistory(this);
        }
        /// <TEST> UserTest.TestAdminGetHistory </TEST>
        /// <UC> 'Admin requests for user history' </UC>
        /// <REQ> 6.4 </REQ>
        /// <summary>
        ///  returns all purchase-records of the requested user. 
        /// </summary>
        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory(IUser otherUser)
        {
            return _systemState.GetUserPurchaseHistory(this, otherUser);
        }
        /// <TEST> UserTest.TestGetStoreHistory </TEST>
        /// <UC> 'Member requests for purchase history for the store' </UC>
        /// <REQ> 4.11 </REQ>
        /// <summary>
        ///  returns all purchase-records of the requested store. 
        /// </summary>
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

        public UserToSystemState GetState()
        {
            return _systemState;
        }


        //InBusiness
        
        
        /// <TEST> UserTest.TestHasPermissions </TEST>
        /// <UC> 'Change management permission for sub-manger' </UC>
        /// <REQ> 5.1 </REQ>
        /// <summary>
        ///  checks if user has the required permission. 
        /// </summary>
        public Result HasPermission(IStore store, StorePermission storePermission)
        {
            return _systemState.HasPermission(this, store, storePermission);
        }

        /// <TEST> UserTest.TestUserPurchaseHistory </TEST>
        /// <UC> 'Review purchase history' </UC>
        /// <REQ> 3.7 </REQ>
        /// <summary>
        ///  add records to member's history. 
        /// </summary>
        public Result EnterRecordToHistory(PurchaseRecord record)
        {
            return _systemState.EnterRecordToHistory(this, record);
        }
    #endregion User Facade Interface


    #region Admin Functions
    //add necessary functions   

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
            // @TODO_SHARON:: add extra founder's permissions to 'owner'
            
            bool res =_storesFounded.TryAdd(store,true) && _storesOwned.TryAdd(store,owner);
            if (res)
            {
                return store.AppointNewOwner(this,owner);
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
            ManagerAppointment newManager = new ManagerAppointment(this);
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


        public Result UpdatePermissionsToManager(Member member, IStore store, IUser otherUser,
            IList<StorePermission> permissions)
        {
            if (permissions == null || permissions.Count == 0){
                return Result.Fail("Invalid permission list input");
            }
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
        
    
    #region Test Oriented Functions

    public ConcurrentDictionary<IStore, IList<ManagerAppointment>> AppointedManagers => _appointedManagers;
    public ConcurrentDictionary<IStore, IList<OwnerAppointment>> AppointedOwners => _appointedOwners;
    public ConcurrentDictionary<IStore, ManagerAppointment> StoresManaged => _storesManaged;
    public ConcurrentDictionary<IStore, OwnerAppointment> StoresOwned => _storesOwned;

    #endregion
    
    }
    
   }