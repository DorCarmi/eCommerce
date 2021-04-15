using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using eCommerce.Business;
using eCommerce.Common;

namespace Tests.Business
{
    /// <summary>
    /// <para>Implementation of synchronized use.</para>
    /// </summary>
    public class MemberDataRepositoryMock : IRepository<MemberData>
    {
        private IDictionary<string, MemberData> _membersData;
        private long _id;
        
        public MemberDataRepositoryMock()
        {
            _membersData = new Dictionary<string, MemberData>();
            _id = 0;
        }

        public bool Add([NotNull] MemberData memberData)
        {
            bool added = _membersData.TryAdd(_id.ToString(), memberData);
            _id++;
            return added;
        }

        public MemberData GetOrNull([NotNull] string id)
        {
            if(!_membersData.TryGetValue(id, out var data))
            {
                return null;
            }
            return data;
        }

        public void Remove(string id)
        {
            _membersData.Remove(id);
        }
    }
}