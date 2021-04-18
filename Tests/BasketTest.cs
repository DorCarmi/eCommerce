using System.Collections.Generic;
using System.Diagnostics;
using eCommerce.Business;
using eCommerce.Business.Service;
using NUnit.Framework;
using Tests.Mokups;
using Transaction = System.Transactions.Transaction;

namespace Tests
{
    public class BasketTest
    {
        public IBasket basket;
        public ICart cart;
        public IUser alice;
        
        private Item item1;
        
        private string storeName = "Alenby";
        private string itemName = "IPhone5";
        private int itemPrice = 3500;
        private Category itemCategory = new Category("Electronics");
        private ItemInfo info1;
        
        
        private Item item2;
        private string storeName2 = "Hashalom";
        private string itemName2 = "IPhone5.5";
        private int itemPrice2 = 3000;
        private Category itemCategory2 = new Category("AppleProducts");
        
        private IStore alenbyStore;
        
        [SetUp]
        public void Setup()
        {
            alenbyStore = new mokStore(storeName);
            item1 = new Item(itemName, itemCategory, alenbyStore, itemPrice);
            mokStore mokStore = (mokStore) alenbyStore;
            mokStore.item = item1;
            alice = new mokUserAlice();
            info1 = new ItemInfo(0, itemName, storeName, itemCategory.getName(), new List<string>(), itemPrice);
            alice = new mokUserAlice();
            cart = new Cart(alice);
            basket = new Basket(cart,alenbyStore);
        }

        [Test]
        public void TestBasketBasic()
        {
            mokStore mokStore = (mokStore) alenbyStore;
            mokStore.cart = cart;
            Assert.AreEqual(cart,basket.GetCart());
            Assert.AreEqual(storeName,basket.GetStoreName());
            Assert.AreEqual(false,basket.GetAllItems().IsFailure);
            Assert.AreEqual(0,basket.GetAllItems().GetValue().Count);
            
        }

        [Test]
        public void TestItemsInBasket()
        {
            ItemInfo itemToEdit=null;
            for (int i = 0; i < 10; i++)
            {
                char c = 'A';
                var item = new ItemInfo(i + 5, (c + i).ToString(), storeName, itemCategory.getName(), new List<string>(), 10 + i);
                itemToEdit = item;
                basket.AddItemToBasket(alice,item);
            }

            Assert.AreEqual(10, basket.GetAllItems().GetValue().Count);

            itemToEdit.amount = 500;
            
            Assert.AreEqual(false,basket.EditItemInBasket(alice,itemToEdit).IsFailure);
            Assert.AreEqual(false,basket.GetItem(alice, itemToEdit.name).IsFailure);
            Assert.AreEqual(500,basket.GetItem(alice, itemToEdit.name).GetValue().amount);
            
            
            
            //basket.AddItemToBasket()
            //basket.EditItemInBasket()
        }
        
        [Test]
        public void TestBuyingBasket()
        {
            //basket.BuyWholeBasket()
            //basket.CalculateBasketPrices()
            //basket.GetTotalPrice()
            //basket.SetTotalPrice()
            
            
        }
        
        [TearDown]
        public void TearDown()
        {
            
        }
    }
}