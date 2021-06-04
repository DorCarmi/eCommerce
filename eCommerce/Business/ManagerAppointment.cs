using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class ManagerAppointment
    {
        public User User { get; }
        private ConcurrentDictionary<StorePermission,bool> _permissions;

        public ManagerAppointment(User user)
        {
            this.User = user;
            this._permissions = new ConcurrentDictionary<StorePermission, bool>();
            this._permissions.TryAdd(StorePermission.ViewStaffPermission,true);
        }
        
        public Result AddPermissions(StorePermission permission)
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
        }

        public Result HasPermission(StorePermission permission)
        {
            if(_permissions.ContainsKey(permission))
                return Result.Ok();
            return Result.Fail("Manager does not have the required permission");
        }

        public Result UpdatePermissions(IList<StorePermission> permissions)
        {
            bool res = false;
            var newPermissions = new ConcurrentDictionary<StorePermission, bool>();
            foreach (var permission in permissions)
            {
                newPermissions.TryAdd(permission,true);
            }
            this._permissions = newPermissions;
            return Result.Ok();
        }

        public List<StorePermission> GetAllPermissions()
        {
            return _permissions.Keys.ToList();
        }
    }
}