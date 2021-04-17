using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface UserToSystemState
    {
        public Result Login(User user,UserToSystemState systemState, MemberData memberData);
        Result Logout(User user,string toGuestName);
        Result OpenStore(User user,IStore store);
        Result AppointUserToOwner(User user,IStore store, IUser otherUser);
        Result AppointUserToManager(User user,IStore store, IUser otherUser);
        Result<OwnerAppointment> MakeOwner(User user,IStore store);
        Result<ManagerAppointment> MakeManager(User user,IStore store);
        Result AddPermissionsToManager(User user,IStore store, IUser otherUser, StorePermission permission);
        Result RemovePermissionsToManager(User user,IStore store, IUser otherUser, StorePermission permission);
        Result HasPermission(User user,IStore store, StorePermission storePermission);
        Result EnterBasketToHistory(User user,IBasket basket);
    }
}