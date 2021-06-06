using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class ManagerAppointment
    {
        [ForeignKey("User")]
        public String Username;
        public User User { get; private set; }
        
        [ForeignKey("Store")]
        public String StoreName;
        public Store Store { get; private set; }
        public List<StorePermission> _permissions { get; private set; }

        private Mutex _mutex;

        public ManagerAppointment()
        {
            _mutex = new Mutex();
        }
        
        public ManagerAppointment(User user, Store store)
        {
            this.User = user;
            Username = user.Username;
            Store = store;
            StoreName = Store._storeName;
            this._permissions = new List<StorePermission>()
            {
                StorePermission.AddItemToStore,
                StorePermission.EditItemDetails
            };
            _mutex = new Mutex();
        }
        
        /*public Result AddPermissions(StorePermission permission)
        {
            if(_permissions.TryAdd(permission, true))
                return Result.Ok();
            return Result.Fail("Manager already has permission");
        }
        
        public Result RemovePermission(StorePermission permission)
        {
            bool btrue;
            if (_permissions.TryRemove(permission, out btrue))
                return Result.Ok();
            return Result.Fail("Manager does not have the given permission");
        }*/

        public Result HasPermission(StorePermission permission)
        {
            _mutex.WaitOne();
            if (_permissions.Exists(p => p.Equals(permission)))
            {
                _mutex.ReleaseMutex();
                return Result.Ok();
            }
            
            _mutex.ReleaseMutex();
            return Result.Fail("Manager does not have the required permission");
        }

        public Result UpdatePermissions(IList<StorePermission> permissions)
        {
            _mutex.WaitOne();
            foreach (var permission in permissions)
            {
                if (!_permissions.Exists(p => p.Equals(permission)))
                {
                    _permissions.Add(permission);
                }
            }
            _mutex.ReleaseMutex();
            return Result.Ok();
        }

        public List<StorePermission> GetAllPermissions()
        {
            return _permissions;
        }
    }
}