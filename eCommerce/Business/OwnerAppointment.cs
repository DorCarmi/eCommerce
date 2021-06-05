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
        public string Username { get; set; }
        public string Storename { get; set; }
        public User User { get; set; }
        private ConcurrentDictionary<StorePermission,bool> _permissions;


        //for ef
        public OwnerAppointment()
        {
        }

        public OwnerAppointment(User user, string storename)
        {
            this.User = user;
            this.Username = user.Username;
            this.Storename = storename;
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