using System;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace Tests.Business.Mokups
{
    public class mokUser : User
    {
        private bool isLoggedIn = false;
        
        private string userName;

        public mokUser(string username) : base(username)
        {
            this.Username = username;
        }

        public Result Login(UserToSystemState systemState)
        {
            if (!isLoggedIn)
            {
                this.isLoggedIn = true;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("User already logged in");
            }
        }

        public Result Login()
        {
            throw new System.NotImplementedException();
        }

        public Result Logout()
        {
            if (isLoggedIn)
            {
                this.isLoggedIn = false;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("User already logged in");
            }
        }


        public MemberInfo MemberInfo { get; set; }

        public override Result OpenStore(IStore store)
        {
            throw new System.NotImplementedException();
        }

        public override Result<List<string>> GetStoreIds()
        {
            throw new System.NotImplementedException();
        }

        public override Result<IList<string>> GetManagedStoreIds()
        {
            throw new NotImplementedException();
        }

        public override Result AddItemToCart(ItemInfo item)
        {
            throw new System.NotImplementedException();
        }

        Result<ICart> GetCartInfo()
        {
            throw new System.NotImplementedException();
        }

        public override Result EditCart(ItemInfo info)
        {
            throw new System.NotImplementedException();
        }

        public override Result AppointUserToOwner(IStore store, User user)
        {
            throw new System.NotImplementedException();
        }

        public override Result AppointUserToManager(IStore store, User user)
        {
            throw new System.NotImplementedException();
        }

        public override Result RemoveOwnerFromStore(IStore store, User user)
        {
            throw new NotImplementedException();
        }

        public override Result EnterRecordToHistory(PurchaseRecord record)
        {
            return Result.Ok();
        }

        public override Result<OwnerAppointment> MakeOwner(IStore store)
        {
            throw new System.NotImplementedException();
        }

        public override Result<ManagerAppointment> MakeManager(IStore store)
        {
            throw new System.NotImplementedException();
        }

        public override Result<OwnerAppointment> RemoveOwner(IStore store)
        {
            throw new NotImplementedException();
        }

        public override Result<ManagerAppointment> RemoveManager(IStore store)
        {
            throw new NotImplementedException();
        }

        public override Result AnnexStakeholders(IStore store, IList<OwnerAppointment> owners, IList<ManagerAppointment> managers)
        {
            throw new NotImplementedException();
        }

        public override Result PublishMessage(string message)
        {
            throw new NotImplementedException();
        }

        public override UserToSystemState GetState()
        {
            throw new System.NotImplementedException();
        }

        public override Result BuyWholeCart(PaymentInfo paymentInfo)
        {
            throw new System.NotImplementedException();
        }

        public override Result UpdatePermissionsToManager(IStore store, User user, IList<StorePermission> permission)
        {
            throw new System.NotImplementedException();
        }

        public override Result RemovePermissionsToManager(IStore store, User user, StorePermission permission)
        {
            throw new System.NotImplementedException();
        }

        public override Result<IList<User>> GetAllStoreStakeholders(Member member, IStore store)
        {
            throw new System.NotImplementedException();
        }

        public override Result<IList<PurchaseRecord>> GetUserPurchaseHistory()
        {
            throw new System.NotImplementedException();
        }

        public override Result<IList<PurchaseRecord>> GetUserPurchaseHistory(User otherUser)
        {
            throw new System.NotImplementedException();
        }

        public override Result<IList<PurchaseRecord>> GetStorePurchaseHistory(IStore store)
        {
            throw new System.NotImplementedException();
        }

        public override Result HasPermission(IStore store, StorePermission storePermission)
        {
            return Result.Ok();
        }

        public string Username { get; }
    }
}