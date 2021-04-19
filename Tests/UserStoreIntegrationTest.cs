using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce;
using eCommerce.Business;
using NUnit.Framework;
using Tests.Mokups;

namespace Tests
{
    public class UserStoreIntegrationTest
    {
        
        #region Integration Tested Here
        
        [Test]
        public void TestGetStoreHistory()
        {
            User user1 = new User(new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis"));
            User user2 = new User(Admin.State,new MemberInfo("GM", "gm@mail.com", "GM", DateTime.Now, "Memphis"));
            User user3 = new User("10-Day-Contract guy");
            // Store store1= new Store("BasketBall stuff.. buy here",user1);
            // user1.OpenStore(store1);
            // var purchaseRecord = new PurchaseRecord(store1,new MokBasket(user3.GetCartInfo().Value,store1),DateTime.Now);
            // var basket1 = new MokBasket(user3.GetCartInfo().Value, store1);
            // store1.EnterBasketToHistory(basket1);
            // var records1 = user1.GetStorePurchaseHistory(store1).Value;
            //
            // var records2 = user2.GetStorePurchaseHistory(store1).Value;
            // Assert.False(user3.GetStorePurchaseHistory(store1).IsSuccess);
            // Assert.Equals(records1,records2);
        }


        [Test]
        public void TestAddItemToCart()
        {
            User user1 = new User(new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis"));
            mokStore store1= new mokStore("BasketBall stuff.. buy here");
            user1.OpenStore(store1);
            Item item1 = new Item("Basketball",new Category("Sports"),store1,1000);
            Assert.True(user1.AddItemToCart(item1.ShowItem()).IsSuccess);

            User user2 = new User("10-Day-Contract guy");
            mokStore store2 = new mokStore("More(!) BasketBall stuff.. buy here");
            Assert.True(user2.AddItemToCart(item1.ShowItem()).IsSuccess);

            Assert.True(user2.GetCartInfo().IsSuccess);
            //@TODO_Sharon:: Assert - the Result matches expected scenario 
        }

        [Test]
        public void TestEditCart()
        {
            User user1 = new User(new MemberInfo("Ja Morant", "ja@mail.com", "Ja", DateTime.Now, "Memphis"));
            mokStore store1= new mokStore("BasketBall stuff.. buy here");
            user1.OpenStore(store1);
            Item item1 = new Item("Basketball",new Category("Sports"),store1,1000);
            Assert.True(user1.AddItemToCart(item1.ShowItem()).IsSuccess);
            var newInfo = item1.ShowItem();
            item1.AddItems(user1,5);
            Assert.True(user1.EditCart(newInfo).IsSuccess);
            
            Assert.True(user1.GetCartInfo().IsSuccess);
            //@TODO_Sharon:: Assert - the Result matches expected scenario (amount changed to 5?) 
            item1.AddItems(user1,0);
            Assert.True(user1.EditCart(newInfo).IsSuccess);
            
            Assert.True(user1.GetCartInfo().IsSuccess);
            //@TODO_Sharon:: Assert - the Result matches expected scenario (item removed from Cart) 

            User user2 = new User("10-Day-Contract guy");
            // maybe scenario should pass
            Assert.False(user2.EditCart(item1.ShowItem()).IsSuccess);
        }


        #endregion

        
    }
}