using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace Tests.Business.Mokups
{
    public class mokUserAlice : IUser
    {
        private bool isLoggedIn = false;
        private string userName = "ALICE";

        public mokUserAlice()
        {
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

      
        public Result OpenStore(IStore store)
        {
            throw new System.NotImplementedException();
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

        public Result EnterRecordToHistory(PurchaseRecord record)
        {
            throw new System.NotImplementedException();
        }

        public Result<OwnerAppointment> MakeOwner(IStore store)
        {
            throw new System.NotImplementedException();
        }

        public Result<ManagerAppointment> MakeManager(IStore store)
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