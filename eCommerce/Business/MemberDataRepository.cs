using System.Collections.Concurrent;
using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class MemberDataRepository : IRepository<MemberData>
    {

        private ConcurrentDictionary<string, MemberData> _membersData;

        public MemberDataRepository()
        {
            _membersData = new ConcurrentDictionary<string, MemberData>();
        }
        
        public bool Add(MemberData data)
        {
            return _membersData.TryAdd(data.Username, data);
        }

        public MemberData GetOrNull(string id)
        {
            if (!_membersData.TryGetValue(id, out var data))
            {
                return null;
            }

            return data;
        }

        public void Remove(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}