using System;
using System.Collections.Generic;
using System.ComponentModel;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public interface IUser
    {
        public string Username { get; }
        public MemberInfo MemberInfo { get; }

        //Facade
        public Result OpenStore(IStore store);
        public Result AddItemToCart(ItemInfo item);
        public Result<ICart> GetCartInfo();
        public Result EditCart(ItemInfo info);
        public Result BuyWholeCart(PaymentInfo paymentInfo);
        
        public Result AppointUserToOwner(IStore store, IUser user);
        public Result AppointUserToManager(IStore store, IUser user);
        public Result RemoveOwnerFromStore(IStore store, IUser user);
        public Result UpdatePermissionsToManager(IStore store, IUser user, IList<StorePermission> permissions);
        public Result RemovePermissionsToManager(IStore store, IUser user, StorePermission permission);

        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory();
        public Result<IList<PurchaseRecord>> GetUserPurchaseHistory(IUser otherUser);
        public Result<IList<PurchaseRecord>> GetStorePurchaseHistory(IStore store);
        
        
        //InBusiness
        public Result HasPermission(IStore store, StorePermission storePermission);
        public Result EnterRecordToHistory(PurchaseRecord record);

        public Result<OwnerAppointment> MakeOwner(IStore store);
        public Result<ManagerAppointment> MakeManager(IStore store);
        public Result<Tuple<OwnerAppointment,IList<OwnerAppointment>,IList<ManagerAppointment>>> RemoveOwner(IStore store);
        public Result AnnexStakeholders(IStore store, IList<OwnerAppointment> owners, IList<ManagerAppointment> managers);

        public Result PublishMessage(string message);
       
        public UserToSystemState GetState();
    }

    
}