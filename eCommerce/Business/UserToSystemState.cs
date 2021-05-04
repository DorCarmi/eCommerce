using System.Collections.Generic;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface UserToSystemState
    {
        public Result Login(User user,UserToSystemState systemState, MemberData memberData);
        Result Logout(User user,string toGuestName);
        Result OpenStore(User user,IStore store);
        Result<List<string>> GetStoreIds(User user);
        Result AppointUserToOwner(User user,IStore store, IUser otherUser);
        Result AppointUserToManager(User user,IStore store, IUser otherUser);
        Result<OwnerAppointment> MakeOwner(User user,IStore store);
        Result<ManagerAppointment> MakeManager(User user,IStore store);
        Result AddPermissionsToManager(User user,IStore store, IUser otherUser, StorePermission permission);
        Result RemovePermissionsToManager(User user,IStore store, IUser otherUser, StorePermission permission);
        Result UpdatePermissionsToManager(User user, IStore store, IUser otherUser, IList<StorePermission> permissions);
        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory(User user);
        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory(User user, IUser otherUser);
        public Result<IList<PurchaseRecord>> GetStorePurchaseHistory(User user, IStore store);
        Result HasPermission(User user,IStore store, StorePermission storePermission);
        Result EnterRecordToHistory(User user, PurchaseRecord record);
    }
}