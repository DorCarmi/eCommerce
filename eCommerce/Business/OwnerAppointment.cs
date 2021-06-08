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
        public string Ownername { get; set; }
        public string OwnedStorename { get; set; }
        public User User { get; set; }
        private ConcurrentDictionary<StorePermission,bool> _permissions;


        //for ef
        public OwnerAppointment()
        {
            // we are able to simply re-assign all permissions to every owner loaded from DB,
            // because, until further notice, by definition all owners have every permission.
            this._permissions = new ConcurrentDictionary<StorePermission, bool>();

            foreach (var permission in Enum.GetValues(typeof(StorePermission)))
            {
                _permissions.TryAdd((StorePermission)permission,true);
            }
        }

        public OwnerAppointment(User user, string storename)
        {
            this.User = user;
            this.Ownername = user.Username;
            this.OwnedStorename = storename;
            this._permissions = new ConcurrentDictionary<StorePermission, bool>();

            foreach (var permission in Enum.GetValues(typeof(StorePermission)))
            {
                _permissions.TryAdd((StorePermission)permission,true);
            }
        }

        public Result HasPermission(StorePermission permission)
        {
            if(_permissions.ContainsKey(permission))
                return Result.Ok();
            return Result.Fail("Owner does not have the required permission");
        }
    }
}