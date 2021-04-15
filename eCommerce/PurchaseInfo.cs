using System.Collections.Generic;
using eCommerce.Business;

namespace eCommerce
{
    public class PurchaseInfo
    {
        private List<Item> allItems;
        private double totalPrice;
        private User user;

        public PurchaseInfo(List<Item> allItems, double totalPrice, User user)
        {
            this.allItems = allItems;
            this.totalPrice = totalPrice;
            this.user = user;
        }

        public List<Item> GetAllItems()
        {
            return allItems;
        }

        public double GetTotalPrice()
        {
            return totalPrice;
        }

        public User GetUser()
        {
            return user;
        }
    }