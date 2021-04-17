using System.Collections.Concurrent;
using System.Collections.Generic;
using eCommerce.Business.Service;

namespace eCommerce.Business
{
    public class MemberData
    {
        
        public string Username { get => MemberInfo.Username; }
        public ICart Cart { get; }
        public ConcurrentDictionary<IStore, bool> StoresFounded{ get; }
        public ConcurrentDictionary<IStore, OwnerAppointment> StoresOwned{ get; }
        public ConcurrentDictionary<IStore, ManagerAppointment> StoresManaged{ get; }
        public ConcurrentDictionary<IStore, IList<OwnerAppointment>> AppointedOwners{ get; }
        public ConcurrentDictionary<IStore, IList<ManagerAppointment>> AppointedManagers{ get; }
        public UserTransactionHistory History { get; }
        
        public MemberInfo MemberInfo;

        
        public MemberData(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
        }

        public MemberData(UserTransactionHistory history, MemberInfo memberInfo, ICart cart, ConcurrentDictionary<IStore, bool> storesFounded, ConcurrentDictionary<IStore, OwnerAppointment> storesOwned, ConcurrentDictionary<IStore, ManagerAppointment> storesManaged, ConcurrentDictionary<IStore, IList<ManagerAppointment>> appointedManagers, ConcurrentDictionary<IStore, IList<OwnerAppointment>> appointedOwners)
        {
            History = history;
            MemberInfo = memberInfo;
            Cart = cart;
            StoresFounded = storesFounded;
            StoresOwned = storesOwned;
            StoresManaged = storesManaged;
            AppointedManagers = appointedManagers;
            AppointedOwners = appointedOwners;
        }

        
        private void test()
        {
        }
    }
}