using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class OwnerAppointment
    {
        public IUser User { get; set; }
        private ConcurrentDictionary<StorePermission,bool> _permissions;

        public OwnerAppointment(IUser user)
        {
            this.User = user;
            this._permissions = new ConcurrentDictionary<StorePermission, bool>();
            InitPermissions();
        }

        public void InitPermissions()
        {
            foreach (var storePermission in Enum.GetValues<StorePermission>())
            {
                _permissions.TryAdd(storePermission, true);
            }
        }

        public Result AddPermissions(StorePermission permission)
        {
            if(_permissions.TryAdd(permission, true))
                return Result.Ok();
            return Result.Fail("Owner already has permission");
        }
        
        public Result RemovePermission(StorePermission permission)
        {
            bool btrue;
            if (_permissions.TryRemove(permission, out btrue))
                return Result.Ok();
            return Result.Fail("Owner does not have the given permission");
        }

        public Result HasPermission(StorePermission permission)
        {
            if(_permissions.ContainsKey(permission))
                return Result.Ok();
            return Result.Fail("Owner does not have the required permission");
        }
    }
}