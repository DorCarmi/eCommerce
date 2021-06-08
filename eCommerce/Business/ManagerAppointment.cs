using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class ManagerAppointment
    {
        public string Managername { get; set; }
        public string ManagedStorename { get; set; }
        public User User { get; set; }
        private ConcurrentDictionary<StorePermission, bool> _permissions;
        private static readonly  IList<StorePermission> BaseManagerPermissions = ImmutableList.Create(new StorePermission[] 
            {StorePermission.ViewStaffPermission});

        public ManagerAppointment(User user, string storename)
        {
            this.User = user;
            this.Managername = user.Username;
            this.ManagedStorename = storename;
            this._permissions = new ConcurrentDictionary<StorePermission, bool>();
            foreach (StorePermission permission in BaseManagerPermissions)
            {
                _permissions.TryAdd(permission, true);
            }
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
            //@TODO::sharon check if should add BaseManagerPermissions as well?  
            foreach (var permission in BaseManagerPermissions)
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


        #region EF
        public virtual string permissionsString { get; set;}
        public ManagerAppointment()
        {
            this._permissions = new ConcurrentDictionary<StorePermission, bool>();
            this.permissionsString = "";
        }

         public void syncFromDict()
        {
            permissionsString = string.Join(";", _permissions.Keys.Select(p => (int)p));
        }
         
         
        public void syncToDict()
        {
            string[] list = permissionsString.Split(";");
            foreach (string p in list)
            {
                _permissions.TryAdd((StorePermission)int.Parse(p), true);
            }
        }

       

        #endregion EF
        
    }
}