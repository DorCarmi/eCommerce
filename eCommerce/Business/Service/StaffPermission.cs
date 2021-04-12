using System.Collections.Generic;

namespace eCommerce.Business.Service
{
    public class StaffPermission
    {
        public string UserId { get; set; }
        public IEnumerable<StorePermission> Permissions { get; set; }
    }
}