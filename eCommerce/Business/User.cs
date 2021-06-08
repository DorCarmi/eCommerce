using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

using eCommerce.Common;
using eCommerce.Publisher;
using eCommerce.DataLayer;
using eCommerce.Statistics;

namespace eCommerce.Business
{
    public class User
    {
        private bool _isRegistered;
        private UserToSystemState _systemState;
        private MemberInfo _memberInfo { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Username { get; set; }
        
        public MemberInfo MemberInfo
        {
            get
            {
                return _memberInfo;
            }
            set
            {
                _memberInfo = value;
            }
        }

        
        private ICart _myCart;
        private Object dataLock;
        //MemberData:
        private ConcurrentDictionary<Store, bool> _storesFounded;
        private ConcurrentDictionary<Store, OwnerAppointment> _storesOwned;
        private ConcurrentDictionary<Store, ManagerAppointment> _storesManaged;
        private ConcurrentDictionary<Store, IList<OwnerAppointment>> _appointedOwners;
        private ConcurrentDictionary<Store, IList<ManagerAppointment>> _appointedManagers;
        private UserTransactionHistory _transHistory ;
        

        //constructors

        public User(string Username)
        {
            this.Username = Username;
            _memberInfo = new MemberInfo(Username, null,null,DateTime.Now, null);
            _systemState = Guest.State;
            _myCart = new Cart(this);
            _isRegistered = false;
            dataLock = new Object();
        }
        public User(MemberInfo MemberInfo)
        {
            _isRegistered = true;
            Username = MemberInfo.Username;
            _memberInfo = MemberInfo;
            dataLock = new Object();
            _systemState = Member.State;
            _myCart = new Cart(this);
            // _userName = memberData.Username;
            _storesFounded = new ConcurrentDictionary<Store, bool>();
            _storesOwned = new ConcurrentDictionary<Store, OwnerAppointment>();
            _storesManaged = new ConcurrentDictionary<Store, ManagerAppointment>();
            _appointedOwners = new ConcurrentDictionary<Store, IList<OwnerAppointment>>();
            _appointedManagers = new ConcurrentDictionary<Store, IList<ManagerAppointment>>();
            _transHistory = new UserTransactionHistory();
        } 
        public User(UserToSystemState state, MemberInfo MemberInfo)
        {
            _isRegistered = true;
            Username = MemberInfo.Username;
            _memberInfo = MemberInfo;
            dataLock = new Object();
            _systemState = state;
            _myCart = new Cart(this);
            _storesFounded = new ConcurrentDictionary<Store, bool>();
            _storesOwned = new ConcurrentDictionary<Store, OwnerAppointment>();
            _storesManaged = new ConcurrentDictionary<Store, ManagerAppointment>();
            _appointedOwners = new ConcurrentDictionary<Store, IList<OwnerAppointment>>();
            _appointedManagers = new ConcurrentDictionary<Store, IList<ManagerAppointment>>();
            _transHistory = new UserTransactionHistory();
        }

        
    #region User Facacde Interface
        public virtual Result Login(UserToSystemState systemState, MemberData memberData)
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


        public virtual Result Logout(string toGuestName)
        {
            return _systemState.Logout(this,toGuestName);
        }

        public virtual Result<bool> IsRegistered()
        {
            return Result.Ok(_isRegistered);
        }

        /// <TEST> UserTest.TestOpenStore </TEST>
        /// <UC> 'Open a Store' </UC>
        /// <REQ> 3.2 </REQ>
        /// <summary>
        ///  receives an Store to open. makes this User a founder and an owner. 
        /// </summary>
        /// <param name="store"></param>
        /// <returns>Result, OK/Fail. </returns>
        public virtual Result OpenStore(Store store)
        {
            return _systemState.OpenStore(this, store);
        }
        
        public virtual Result<List<string>> GetStoreIds()
        {
            return _systemState.GetStoreIds(this);
        }
        
        public virtual Result<IList<string>> GetManagedStoreIds()
        {
            return _systemState.GetManagedStoreIds(this);
        }

        /// <TEST>  </TEST>
        /// <UC> 'Add product to basket' </UC>
        /// <REQ> 2.7 </REQ>
        /// <summary>
        ///  adds the given items to the user's cart. 
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Result, OK/Fail. </returns>
        public virtual Result AddItemToCart(ItemInfo item)
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
        public virtual Result<ICart> GetCartInfo()
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
        public virtual Result EditCart(ItemInfo info)
        {
            return _myCart.EditCartItem(this, info);
        }

        public virtual Result BuyWholeCart(PaymentInfo paymentInfo)
        {
            return this._myCart.BuyWholeCart(this, paymentInfo);
        }
      
        /// <TEST> UserTest.TestAppointUserToOwner </TEST>
        /// <UC> 'Nominate member to be store owner' </UC>
        /// <REQ> 4.5 </REQ>
        /// <summary>
        ///  make the user an owner of the given store. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="store"></param>
        /// <returns>Result, OK/Fail. </returns>
        public virtual Result AppointUserToOwner(Store store, User user)
        {
            return _systemState.AppointUserToOwner(this, store, user);
        }

        /// <TEST> UserTest.TestAppointUserToManager </TEST>
        /// <UC> 'Nominate member to be store manager' </UC>
        /// <REQ> 4.3 </REQ>
        /// <summary>
        ///  make the user a manager of the given store. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="store"></param>
        /// <returns>Result, OK/Fail. </returns>
        public virtual Result AppointUserToManager(Store store, User user)
        {
            return _systemState.AppointUserToManager(this, store, user);
        }
        
        /// <TEST> UserTest.TestRemoveOwnerFromStore </TEST>
        /// <UC> 'Remove member from store ownership' </UC> //@TODO_Sharon : make sure use case documentation matches this
        /// <REQ> 4.4 </REQ>
        /// <summary>
        ///  remove the user from being an owner of the given store. 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="store"></param>
        /// <returns>Result, OK/Fail. </returns>
        public virtual Result RemoveOwnerFromStore(Store store, User user)
        {
            return _systemState.RemoveOwnerFromStore(this, store, user);
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
        public virtual Result UpdatePermissionsToManager(Store store, User user, IList<StorePermission> permissions)
        {
            return _systemState.UpdatePermissionsToManager(this, store, user, permissions);
        }

        /* @Deprecated */
        // public virtual Result RemovePermissionsToManager(Store store, User user, StorePermission permission)
        // {
        //     return _systemState.RemovePermissionsToManager(this, store, user, permission);
        // }
        //
        
        /// <TEST> UserTest.TestUserPurchaseHistory </TEST>
        /// <UC> 'Review purchase history' </UC>
        /// <REQ> 3.7 </REQ>
        /// <summary>
        ///  returns all purchase-records of the user. 
        /// </summary>
        public virtual Result<IList<PurchaseRecord>> GetUserPurchaseHistory()
        {
            return _systemState.GetUserPurchaseHistory(this);
        }
        
        /// <TEST> UserTest.TestAdminGetHistory </TEST>
        /// <UC> 'Admin requests for user history' </UC>
        /// <REQ> 6.4 </REQ>
        /// <summary>
        ///  returns all purchase-records of the requested user. 
        /// </summary>
        public virtual Result<IList<PurchaseRecord>> GetUserPurchaseHistory(User otherUser)
        {
            return _systemState.GetUserPurchaseHistory(this, otherUser);
        }
        
        /// <TEST> UserTest.TestGetStoreHistory </TEST>
        /// <UC> 'Member requests for purchase history for the store' </UC>
        /// <REQ> 4.11 </REQ>
        /// <summary>
        ///  returns all purchase-records of the requested store. 
        /// </summary>
        public virtual Result<IList<PurchaseRecord>> GetStorePurchaseHistory(Store store)
        {
            return _systemState.GetStorePurchaseHistory(this, store);
        }


        //User to User
        public virtual Result<OwnerAppointment> MakeOwner(Store store)
        {
            return _systemState.MakeOwner(this, store);
        }

        public virtual Result<ManagerAppointment> MakeManager(Store store)
        {
            return _systemState.MakeManager(this, store);
        }

        public virtual Result<OwnerAppointment> RemoveOwner(Store store)
        {
            return _systemState.RemoveOwner(this, store);
        }
        
        public virtual Result<ManagerAppointment> RemoveManager(Store store)
        {
            return _systemState.RemoveManager(this, store);
        }

        public virtual Result AnnexStakeholders(Store store, IList<OwnerAppointment> owners, IList<ManagerAppointment> managers)
        {
            return _systemState.AnnexStakeholders(this,store,owners,managers);
        }


        public virtual UserToSystemState GetState()
        {
            return _systemState;
        }

        public virtual Result PublishMessage(string message)
        {
            MainPublisher publisher = MainPublisher.Instance;
            if (publisher == null)
                return Result.Fail("user can not access publisher");
            //@TODO_sharon:: find out whether 'userID' or 'Username' sould be passed
            publisher.AddMessageToUser(MemberInfo.Id, message);
            return Result.Ok();
        }

        //InBusiness

        /// <TEST> UserTest.TestHasPermissions </TEST>
        /// <UC> 'Change management permission for sub-manger' </UC>
        /// <REQ> 5.1 </REQ>
        /// <summary>
        ///  checks if user has the required permission. 
        /// </summary>
        public virtual Result HasPermission(Store store, StorePermission storePermission)
        {
            return _systemState.HasPermission(this, store, storePermission);
        }

        /// <TEST> UserTest.TestUserPurchaseHistory </TEST>
        /// <UC> 'Review purchase history' </UC>
        /// <REQ> 3.7 </REQ>
        /// <summary>
        ///  add records to member's history. 
        /// </summary>
        public virtual Result EnterRecordToHistory(PurchaseRecord record)
        {
            return _systemState.EnterRecordToHistory(this, record);
        }
    #endregion User Facade Interface


    #region Admin Functions
    
    public virtual Result<LoginDateStat> GetLoginStats(DateTime date)
    {
        return _systemState.GetLoginStats(date);
    }
    
    #endregion Admin Functions
    
    
    #region Guest Functions
        public virtual Result Login(Guest guest, UserToSystemState systemState, MemberData memberData)
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
        public virtual Result Logout(Member member, string toGuestName)
        {
            _systemState = Guest.State;
            throw new NotImplementedException();
            // _myData = null;
            // _userName = toGuestName;
            _myCart = new Cart(this);
            _isRegistered = false;
            return Result.Ok();
        }

        public virtual Result OpenStore(Member member, Store store)
        {
            // adds store to both Owned-By and Founded-By
            OwnerAppointment owner = new OwnerAppointment(this, store.StoreName);

            bool res =_storesFounded.TryAdd(store,true) && _storesOwned.TryAdd(store,owner);
            if (res)
            {
                return store.AppointNewOwner(this,owner);
            }
            return Result.Fail("Unable to open store");
        }

        public virtual Result AppointUserToOwner(Member member, Store store, User otherUser)
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
            return store.AppointNewOwner(this,newOwner);
        }

        public virtual Result AppointUserToManager(Member member, Store store, User otherUser)
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
            return store.AppointNewManager(this,newManager);
        }

        public virtual Result<OwnerAppointment> MakeOwner(Member member, Store store)
        {
            OwnerAppointment newOwner = new OwnerAppointment(this,store.StoreName);
            if (_storesOwned.TryAdd(store, newOwner))
            {
                return Result.Ok<OwnerAppointment>(newOwner);
            }
            return Result.Fail<OwnerAppointment>("unable to add user \'"+Username+"\' as store owner");
        }

        public virtual Result<ManagerAppointment> MakeManager(Member member, Store store)
        {
            ManagerAppointment newManager = new ManagerAppointment(this, store.StoreName);
            if (!_storesOwned.ContainsKey(store) && _storesManaged.TryAdd(store, newManager))
            {
                return Result.Ok<ManagerAppointment>(newManager);
            }
            return Result.Fail<ManagerAppointment>("unable to add user \'"+Username+"\' as store Manager");
        }
        
        public virtual Result RemoveOwnerFromStore(Member member, Store store,User otherUser)
        {
            if (!_storesOwned.ContainsKey(store)){
                return Result.Fail("user \'"+Username+"\' is not an owner of the given store.");
            }
            if ((!_appointedOwners.ContainsKey(store)) || FindCoOwner(store,otherUser).IsFailure){
                return Result.Fail("user \'"+Username+"\' did not appoint the owner of the given store \'"+otherUser.Username+"\'.");
            }
            var res = otherUser.RemoveOwner(store);
            if (res.IsFailure)
            {
                return res;
            }

            if (_appointedOwners.ContainsKey(store))
            {
                _appointedOwners[store].Remove(res.Value);
            }
            
    
            return Result.Ok();
        }
        
        
        public virtual Result RemoveManagerFromStore(Member member, Store store,User otherUser)
        {
            if (!_storesManaged.ContainsKey(store)){
                return Result.Fail("user \'"+Username+"\' is not a manager of the given store.");
            }
            if ((!_appointedManagers.ContainsKey(store)) || FindCoManager(store,otherUser).IsFailure){
                return Result.Fail("user \'"+Username+"\' did not appoint the manager of the given store \'"+otherUser.Username+"\'.");
            }
            
            //TODO: Check with sharon-> why not exist RemoveManager method
            var res = otherUser.RemoveManager(store);
            if (res.IsFailure)
            {
                return res;
            }

            if (_appointedManagers.ContainsKey(store))
            {
                
                _appointedManagers[store].Remove(res.Value);
            }
            
    
            return Result.Ok();
        }

        public virtual Result AnnexStakeholders(Member member,Store store, IList<OwnerAppointment> owners, IList<ManagerAppointment> managers)
        {
            if (!_storesFounded.ContainsKey(store))
            {
                return Result.Fail("user ["+Username+"] is not a founder of the store ["+store.GetStoreName()+"]");;
            }

            lock (dataLock)
            {
                if (owners != null)
                {
                    foreach (var owner in owners)
                    {
                     
                        _appointedOwners[store].Add(owner);   
                    }
                }
                if (managers != null && _appointedManagers.ContainsKey(store))
                {
                    foreach (var manager in managers)
                    {
                     
                        _appointedManagers[store].Add(manager);   
                    }
                }
            }
            return Result.Ok();
        }

        private Result<ManagerAppointment> FindCoManager(Store store, User otherUser)
        {
            ManagerAppointment manager = null;
            if (_appointedManagers.ContainsKey(store))
            {
                lock (dataLock)
                {
                    manager = _appointedManagers[store].FirstOrDefault((ma) => ma.User == otherUser);
                }
            }

            if (manager == null)
                return Result.Fail<ManagerAppointment>("user\'"+Username+"\' did not appoint the given manager +\'"+otherUser.Username+"\'");
            return Result.Ok<ManagerAppointment>(manager);
        }

        private Result<OwnerAppointment> FindCoOwner(Store store, User otherUser)
        {
            OwnerAppointment owner = null;
            if (_appointedOwners.ContainsKey(store))
            {
                lock (dataLock)
                {
                    owner = _appointedOwners[store].FirstOrDefault((ma) => ma.User == otherUser);
                }
            }

            if (owner == null)
                return Result.Fail<OwnerAppointment>("user\'"+Username+"\' did not appoint the given owner +\'"+otherUser.Username+"\'");
            return Result.Ok<OwnerAppointment>(owner);
        }

        public virtual Result<OwnerAppointment> RemoveOwner(Member member, Store store)
        {
            if (!_storesOwned.ContainsKey(store))
            {
                return Result.Fail<OwnerAppointment>("user ["+Username+"] is not an owner of the store ["+store.GetStoreName()+"]");
            }
            OwnerAppointment own;
            IList<OwnerAppointment> coowners;
            IList<ManagerAppointment> comanagers;
            
            
            string failMessage = "";
            
            
            if (_appointedOwners.ContainsKey(store) && _appointedOwners[store]!=null)
            {
                var firedList = new List<OwnerAppointment>(_appointedOwners[store]);
                foreach (var owner in firedList)
                {
                    var res = RemoveOwnerFromStore(member, store, owner.User);
                    if(res.IsFailure)
                        failMessage= failMessage+";\n"+res.Error;
                }
            }


            if (_appointedManagers.ContainsKey(store) && _appointedManagers[store]!=null)
            {
                var firedList = new List<ManagerAppointment>(_appointedManagers[store]);
                foreach (var manager in firedList)
                {
                    
                     var res = RemoveManagerFromStore(member, store, manager.User);
                     if(res.IsFailure)
                         failMessage= failMessage+";\n"+res.Error;
                }
            }
            
            _storesOwned.TryRemove(store, out own);
            _appointedOwners.TryRemove(store, out coowners);
            _appointedManagers.TryRemove(store, out comanagers);
            if(failMessage != "")
                return Result.Fail<OwnerAppointment>(failMessage);
            return Result.Ok(own);
        }
        
        
        public virtual Result<ManagerAppointment> RemoveManager(Member member, Store store)
        {
            if (!_storesManaged.ContainsKey(store))
            {
                return Result.Fail<ManagerAppointment>("user ["+Username+"] is not a manager of the store ["+store.GetStoreName()+"]");
            }
            ManagerAppointment mng;
            IList<ManagerAppointment> comanagers;
            
            
            
            string failMessage = "";
            
            
            if (_appointedManagers.ContainsKey(store) && _appointedManagers[store]!=null)
            {
                var firedList = new List<ManagerAppointment>(_appointedManagers[store]);
                foreach (var mngr in firedList)
                {
                    var res = RemoveManagerFromStore(member, store, mngr.User);
                    if(res.IsFailure)
                        failMessage= failMessage+";\n"+res.Error;
                }
            }
            
            
            _storesManaged.TryRemove(store, out mng);
            
            _appointedManagers.TryRemove(store, out comanagers);
            if(failMessage != "")
                return Result.Fail<ManagerAppointment>(failMessage);
            return Result.Ok(mng);
        }


        public virtual Result AddPermissionsToManager(Member member, Store store, User otherUser, StorePermission permission)
        {
            ManagerAppointment manager = null;
            var res = FindCoManager(store, otherUser);
            if (res.IsSuccess)
            {
                manager = res.Value;
                return manager.AddPermissions(permission);
            }
            return Result.Fail("user\'"+Username+"\' can not grant permissions to given manager");
        }

        public virtual Result RemovePermissionsToManager(Member member, Store store, User otherUser, StorePermission permission)
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


        public virtual Result UpdatePermissionsToManager(Member member, Store store, User otherUser,
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
        
        public virtual Result HasPermission(Member member, Store store, StorePermission permission)
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

        public virtual Result EnterRecordToHistory(Member member, PurchaseRecord record)
        {
            return _transHistory.EnterRecordToHistory(record);
        }
        

        public virtual Result<IList<PurchaseRecord>> GetUserHistory()
        {
            return _transHistory.GetUserHistory();
        }

        public virtual Result<IList<PurchaseRecord>> GetStoreHistory(Store store)
        {
            return store.GetPurchaseHistory(this);
        }
        
        public virtual Result<IList<User>> GetAllStoreStakeholders(Member member, Store store)
        {
            throw new NotImplementedException();
        }
        
        
    #endregion //Member Functions
        
    
    #region Test Oriented Functions
    
    public ConcurrentDictionary<Store, IList<ManagerAppointment>> AppointedManagers => _appointedManagers;
    public ConcurrentDictionary<Store, IList<OwnerAppointment>> AppointedOwners => _appointedOwners;
    public ConcurrentDictionary<Store, ManagerAppointment> StoresManaged => _storesManaged;
    public ConcurrentDictionary<Store, OwnerAppointment> StoresOwned => _storesOwned;
    public ConcurrentDictionary<Store, bool> StoresFounded => _storesFounded;

    #endregion

    
    #region DAL Oriented Functions

    public virtual List<Store> storesFoundedBackup { get; set; }
    public virtual List<Pair<Store, OwnerAppointment>> storesOwnedBackup 
    { get; set; }
    public virtual List<Pair<Store, ManagerAppointment>> storesManagedBackup 
    { get; set; }
    public virtual List<ListPair<Store, OwnerAppointment>> appointedOwnersBackup { get; set; }
    public virtual List<ListPair<Store, ManagerAppointment>> appointedManagersBackup { get; set; }


    public User()
    {
        _isRegistered = false;
        dataLock = new Object();
        _systemState = Member.State;
        _storesFounded = new ConcurrentDictionary<Store, bool>();
        _storesOwned = new ConcurrentDictionary<Store, OwnerAppointment>();
        _storesManaged = new ConcurrentDictionary<Store, ManagerAppointment>();
        _appointedOwners = new ConcurrentDictionary<Store, IList<OwnerAppointment>>();
        _appointedManagers = new ConcurrentDictionary<Store, IList<ManagerAppointment>>();
        _transHistory = new UserTransactionHistory();
    }

    public void SyncToBusiness()
    {
        Console.WriteLine("setting pairs!");

        if(storesFoundedBackup != null)
        {
            foreach (Store store in storesFoundedBackup)
            {
                StoresFounded.TryAdd(store, true);
            }
        }
        
        if(storesOwnedBackup != null)
        {
            foreach (Pair<Store, OwnerAppointment> p in storesOwnedBackup)
            {
                StoresOwned.TryAdd(p.Key, p.Value);
            }
        }

        if (storesManagedBackup != null)
        {
            foreach (Pair<Store, ManagerAppointment> p in storesManagedBackup)
            {
                p.Value.syncToDict();
                StoresManaged.TryAdd(p.Key, p.Value);
            }
        }

        if (appointedOwnersBackup != null)
        {
            foreach (ListPair<Store, OwnerAppointment> p in appointedOwnersBackup)
            {
                AppointedOwners.TryAdd(p.Key, p.ValList);
            }
        }

        if (appointedManagersBackup != null)
        {
            foreach (ListPair<Store, ManagerAppointment> p in appointedManagersBackup)
            {
                foreach (var manager in p.ValList)
                {
                    manager.syncToDict();
                }
                AppointedManagers.TryAdd(p.Key, p.ValList);
            }
        }
        
    }
    
    
    public void SyncFromBusiness()
    {
        Console.WriteLine("getting pairs!");
        lock (dataLock)
        {
            storesFoundedBackup = new List<Store>(_storesFounded.Keys);
            
            storesOwnedBackup = new List<Pair<Store, OwnerAppointment>>();
            foreach (Store key in StoresOwned.Keys)
            {
                storesOwnedBackup.Add(new Pair<Store, OwnerAppointment>(){Key = key,KeyId = key.StoreName, Value = StoresOwned[key], HolderId = this.Username});
            }

            storesManagedBackup = new List<Pair<Store, ManagerAppointment>>();
            foreach (Store key in StoresManaged.Keys)
            {
                var val = StoresManaged[key];
                val.syncFromDict();
                storesManagedBackup.Add(new Pair<Store, ManagerAppointment>(){Key = key,KeyId = key.StoreName, Value = val, HolderId = this.Username});
            }

            appointedOwnersBackup = new List<ListPair<Store,OwnerAppointment>>();
            foreach (Store key in AppointedOwners.Keys)
            {
                appointedOwnersBackup.Add(new ListPair<Store, OwnerAppointment>(){Key = key,KeyId = key.StoreName, ValList = new List<OwnerAppointment>(AppointedOwners[key]), HolderId = this.Username});
            }

            appointedManagersBackup = new List<ListPair<Store,ManagerAppointment>>();
            foreach (Store key in AppointedManagers.Keys)
            {
                var managers = new List<ManagerAppointment>(AppointedManagers[key]);

                foreach (var manager in managers)
                {
                    manager.syncFromDict();
                }
                appointedManagersBackup.Add(new ListPair<Store, ManagerAppointment>(){Key = key,KeyId = key.StoreName, ValList = managers, HolderId = this.Username});
            }

            
        }
    }

    #endregion
    
    }
    
}