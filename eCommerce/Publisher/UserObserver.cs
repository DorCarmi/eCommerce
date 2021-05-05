using System.Collections.Generic;

namespace eCommerce.Publisher
{
    public interface UserObserver
    {
        public void Notify(string userName, IList<string> message);
    }
}