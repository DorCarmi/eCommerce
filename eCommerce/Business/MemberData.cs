using System.Collections.Concurrent;
using eCommerce.Business.Service;

namespace eCommerce.Business
{
    public class MemberData
    {
        public string Username { get => MemberInfo.Username; }
        public ICart Cart { get; }
        public ConcurrentDictionary<IStore, bool> StoresFounded;
        public MemberInfo MemberInfo;

        public MemberData(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
        }

        
        private void test()
        {
        }
    }
}