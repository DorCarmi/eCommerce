using System.Collections.Generic;
using eCommerce.DataLayer;

namespace eCommerce.Business
{
    public class Bid
    {
        private List<Pair<string, bool>> ownersApproved;
        private double priceBid;
        private ItemInfo item;

        public Bid(ItemInfo itemInfo, double priceBid)
        {
            this.item = itemInfo;
            this.priceBid = priceBid;
        }
    }
}