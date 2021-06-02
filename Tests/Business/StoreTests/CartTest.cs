using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using eCommerce.Business;

using NUnit.Framework;
using Tests.Business.Mokups;

namespace Tests.Business.StoreTests
{
    public class CartTest
    {
        private ICart MyCart;
        private Store MyStore;
        private User Alice;
        private ItemInfo item1;
        private ItemInfo item2;
        private ItemInfo item3;
        public CartTest()
        {
            Alice = new mokUser("Alice");
            item1 = new ItemInfo(50, "IPhone", "Alenby", "Tech", new List<string>(), 5000);
            item2 = new ItemInfo(50, "Dell6598", "Alenby", "Tech", new List<string>(), 10000);
            MyStore = new mokStore("Alenby");
            MyCart = new Cart(Alice);
            item1.AssignStoreToItem(MyStore);
            item2.AssignStoreToItem(MyStore);
            //MyCart.BuyWholeCart()
            //MyCart.CalculatePricesForCart()
            //MyCart.CheckForCartHolder()
        }
        
        [SetUp]
        public void Setup()
        {
            
            //Item item = new Item();
            Debug.WriteLine("Dor");
        }
        [Test]
        public void TestAddItemToCart()
        {
            MyCart.AddItemToCart(Alice, item1);
            var baskets = MyCart.GetBaskets();
            bool foundItem = false;
            for (int i = 0; i <   baskets.Count && !foundItem; i++)
            {
                var allItems = baskets[i].GetAllItems();
                if (!allItems.IsFailure)
                {
                    if (allItems.Value.Contains(item1))
                    {
                        foundItem = true;
                    }
                }

            }
            Assert.AreEqual(true,foundItem);
        }
        
        [Test]
        public void TestEditCartItem()
        {
            var res1=this.MyCart.EditCartItem(Alice, item2);
            Assert.AreEqual(false,res1.IsSuccess);
            item1.amount = 3;
            var res2=this.MyCart.EditCartItem(Alice, item1);
            Assert.AreEqual(true,res2.IsSuccess);
        }

        [Test]
        public void TestCheckCartHolder()
        {
            var ans=this.MyCart.CheckForCartHolder(Alice);
            Assert.AreEqual(true, ans);
            User bob = new mokUser("Bob");
            Assert.AreEqual(false,MyCart.CheckForCartHolder(bob));

        }
        
        
    }
}