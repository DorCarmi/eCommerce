using System;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace Tests.Business.Mokups
{
    public class mokUser : IUser
    {
        private bool isLoggedIn = false;
        
        private string userName;

        public mokUser(string username)
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

        public Result OpenStore(IStore store)
        {
            throw new System.NotImplementedException();
        }

        public Result<List<string>> GetStoreIds()
        {
            throw new System.NotImplementedException();
        }

        public Result<IList<string>> GetManagedStoreIds()
        {
            throw new NotImplementedException();
        }

        public Result AddItemToCart(ItemInfo item)
        {
            throw new System.NotImplementedException();
        }

        Result<ICart> IUser.GetCartInfo()
        {
            throw new System.NotImplementedException();
        }

        public Result EditCart(ItemInfo info)
        {
            throw new System.NotImplementedException();
        }

        public Result AppointUserToOwner(IStore store, IUser user)
        {
            throw new System.NotImplementedException();
        }

        public Result AppointUserToManager(IStore store, IUser user)
        {
            throw new System.NotImplementedException();
        }

        public Result RemoveOwnerFromStore(IStore store, IUser user)
        {
            throw new NotImplementedException();
        }

        public Result EnterRecordToHistory(PurchaseRecord record)
        {
            return Result.Ok();
        }

        public Result<OwnerAppointment> MakeOwner(IStore store)
        {
            throw new System.NotImplementedException();
        }

        public Result<ManagerAppointment> MakeManager(IStore store)
        {
            throw new System.NotImplementedException();
        }

        public Result<OwnerAppointment> RemoveOwner(IStore store)
        {
            throw new NotImplementedException();
        }

        public Result<ManagerAppointment> RemoveManager(IStore store)
        {
            throw new NotImplementedException();
        }

        public void FreeCart()
        {
            throw new NotImplementedException();
        }

        public Result AnnexStakeholders(IStore store, IList<OwnerAppointment> owners, IList<ManagerAppointment> managers)
        {
            throw new NotImplementedException();
        }

        public Result PublishMessage(string message)
        {
            throw new NotImplementedException();
        }

        public UserToSystemState GetState()
        {
            throw new System.NotImplementedException();
        }

        public Result BuyWholeCart(PaymentInfo paymentInfo)
        {
            throw new System.NotImplementedException();
        }

        public Result UpdatePermissionsToManager(IStore store, IUser user, IList<StorePermission> permission)
        {
            throw new System.NotImplementedException();
        }

        public Result RemovePermissionsToManager(IStore store, IUser user, StorePermission permission)
        {
            throw new System.NotImplementedException();
        }

        public Result<IList<IUser>> GetAllStoreStakeholders(IStore store)
        {
            throw new System.NotImplementedException();
        }

        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory()
        {
            throw new System.NotImplementedException();
        }

        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory(IUser otherUser)
        {
            throw new System.NotImplementedException();
        }

        public Result<IList<PurchaseRecord>> GetStorePurchaseHistory(IStore store)
        {
            throw new System.NotImplementedException();
        }

        public Result HasPermission(IStore store, StorePermission storePermission)
        {
            return Result.Ok();
        }

        public string Username { get; }
    }
}