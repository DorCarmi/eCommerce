using System.Collections.Generic;
using eCommerce.Business;

namespace eCommerce
{
    public class PurchaseInfo
    {
        private List<ItemInfo> allItems;
        private double totalPrice;
        private IUser user;

        public PurchaseInfo(List<ItemInfo> allItems, double totalPrice, IUser user)
        {
            this.allItems = allItems;
            this.totalPrice = totalPrice;
            this.user = user;
        }

        public List<ItemInfo> GetAllItems()
        {
            return allItems;
        }

        public double GetTotalPrice()
        {
            return totalPrice;
        }

        public IUser GetUser()
        {
            return user;
        }
    }
}