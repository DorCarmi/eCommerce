﻿using System;
using System.Collections.Generic;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public sealed class Guest : UserToSystemState
    {  
        private static readonly Guest state = new Guest();  
        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit  
        static Guest(){}  
        private Guest(){}  
        public static Guest State  
        {  
            get  
            {  
                return state;  
            }  
        }

        public Result Login(User user,UserToSystemState systemState, MemberData memberData)
        {
            return user.Login(this, systemState, memberData);
        }

        public Result Logout(User user, string toGuestName)
        {
            return Result.Fail("Illegal action for guest (Logout).");
        }

        public Result OpenStore(User user, IStore store)
        {
            return Result.Fail("Illegal action for guest (Open-Store).");
        }


        public Result AppointUserToOwner(User user, IStore store, IUser otherUser)
        {
            return Result.Fail("Illegal action for guest (Appoint-User).");
        }

        public Result AppointUserToManager(User user, IStore store, IUser otherUser)
        {
            return Result.Fail("Illegal action for guest (Appoint-User).");
        }

        public Result<OwnerAppointment> MakeOwner(User user, IStore store)
        {
            return Result.Fail<OwnerAppointment>("Illegal action for guest (Make-Owner).");
        }

        public Result<ManagerAppointment> MakeManager(User user, IStore store)
        {
            return Result.Fail<ManagerAppointment>("Illegal action for guest (Make-Manager).");
        }

        public Result AddPermissionsToManager(User user, IStore store, IUser otherUser, StorePermission permission)
        {
            return Result.Fail("Illegal action for guest (Give-Store-Permissions-To-User).");
        }

        public Result RemovePermissionsToManager(User user, IStore store, IUser otherUser, StorePermission permission)
        {
            return Result.Fail("Illegal action for guest (Take-Store-Permissions-From-User).");
        }

        public Result UpdatePermissionsToManager(User user, IStore store, IUser user1, IList<StorePermission> permissions)
        {
            return Result.Fail("Illegal action for guest (Update-Store-Permissions-From-User).");
        }


        public Result HasPermission(User user, IStore store, StorePermission storePermission)
        {
            return Result.Fail("Guest has no permissions in a store.");
        }

        public Result EnterBasketToHistory(User user, IBasket basket)
        {
            return Result.Fail("Guest has no transaction-history.");
        }

    }  
}