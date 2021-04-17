﻿using System.Collections.Generic;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Member : UserToSystemState
    {
        private static readonly Member state = new Member();  
        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit  
        static Member(){}
        protected Member(){}  
        public static Member State  
        {  
            get  
            {  
                return state;  
            }  
        }
        
        public Result Login(User user,UserToSystemState systemState, MemberData memberData)
        {
            return Result.Fail("Illegal action for member (login).");
        }

        public Result Logout(User user, string toGuestName)
        {
            return user.Logout(this, toGuestName);
        }

        public Result OpenStore(User user,IStore store)
        {
            return user.OpenStore(this, store);
        }


        public Result AppointUserToOwner(User user,IStore store, IUser otherUser)
        {
            return user.AppointUserToOwner(this, store, otherUser);
        }

        public Result AppointUserToManager(User user, IStore store, IUser otherUser)
        {
            return user.AppointUserToManager(this, store, otherUser);
        }

        public Result<OwnerAppointment> MakeOwner(User user, IStore store)
        {
            return user.MakeOwner(this, store);
        }

        public Result<ManagerAppointment> MakeManager(User user, IStore store)
        {
            return user.MakeManager(this, store);
        }

        public Result AddPermissionsToManager(User user, IStore store, IUser otherUser, StorePermission permission)
        {
            return user.AddPermissionsToManager(this,store,otherUser,permission);
        }

        public Result RemovePermissionsToManager(User user, IStore store, IUser otherUser, StorePermission permission)
        {
            return user.RemovePermissionsToManager(this,store, otherUser,permission);
        }

        public Result UpdatePermissionsToManager(User user, IStore store, IUser otherUser, IList<StorePermission> permissions)
        {
            return user.UpdatePermissionsToManager(this,store, otherUser,permissions);
        }

        public virtual Result HasPermission(User user, IStore store, StorePermission storePermission)
        {
            return user.HasPermission(this, store,storePermission);
        }

        public Result EnterBasketToHistory(User user, IBasket basket)
        {
            return user.EnterBasketToHistory(this, basket);
        }
    }
}