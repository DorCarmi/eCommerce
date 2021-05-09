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

            foreach (var permission in Enum.GetValues(typeof(StorePermission)))
            {
                _permissions.TryAdd((StorePermission)permission,true);
            }
            // _permissions.TryAdd(StorePermission.GetStoreHistory,true);
            // _permissions.TryAdd(StorePermission.AddItemToStore,true);
            // _permissions.TryAdd(StorePermission.ChangeItemPrice,true);
            // _permissions.TryAdd(StorePermission.EditItemDetails,true);
            // _permissions.TryAdd(StorePermission.EditStorePolicy,true);
            // _permissions.TryAdd(StorePermission.ChangeItemStrategy,true);
            // _permissions.TryAdd(StorePermission.ControlStaffPermission,true);
        }

        public Result HasPermission(StorePermission permission)
        {
            if(_permissions.ContainsKey(permission))
                return Result.Ok();
            return Result.Fail("Owner does not have the required permission");
        }
    }
}