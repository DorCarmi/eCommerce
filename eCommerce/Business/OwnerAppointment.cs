using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class OwnerAppointment
    {
        [ForeignKey("User")]
        public String Username;
        public User User { get; private set; }
        
        [ForeignKey("Store")]
        public String StoreName;
        public Store Store { get; private set; }
        public List<StorePermission> _permissions { get; private set; }

        // for ef
        public OwnerAppointment()
        {
        }
        
        public OwnerAppointment(User user, Store store)
        {
            this.User = user;
            Username = user.Username;
            Store = store;
            StoreName = Store._storeName;
            this._permissions = new List<StorePermission>();

            foreach (var permission in Enum.GetValues<StorePermission>())
            {
                _permissions.Add(permission);
            }
        }

        public Result HasPermission(StorePermission permission)
        {
            if(_permissions.Find(p => p == permission) != null)
                return Result.Ok();
            return Result.Fail("Owner does not have the required permission");
        }
    }
}