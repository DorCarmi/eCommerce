using System;
using System.Collections.Generic;
using eCommerce.Business;
using eCommerce.Business.Discounts;

using eCommerce.Common;

namespace Tests.Business.Mokups
{
    public class mokStore: Store
    {
        public string storeName { get; set; }
        public Item item;
        public ICart cart;

        public mokStore(string storeName): base(storeName, null)
        {
            this.storeName = storeName;
        }

        public override Result<Item> GetItem(ItemInfo item)
        {
            return Result.Ok<Item>(this.item);
        }

        public override Result TryGetItems(ItemInfo item)
        {
            return Result.Ok();
        }

        public override Result AppointNewOwner(User user, OwnerAppointment ownerAppointment)
        {
            Console.WriteLine("MokStore: "+user.Username+" Appointed new Owner: "+ownerAppointment.User.Username);
            return Result.Ok();
        }

        public override Result AppointNewManager(User user, ManagerAppointment managerAppointment)
        {
            Console.WriteLine("MokStore: "+user.Username+" Appointed new Manager: "+managerAppointment.User.Username);
            return Result.Ok();
        }

        public override Result RemoveOwnerFromStore(User theOneWhoFires, User theFierd, OwnerAppointment ownerAppointment)
        {
            Console.WriteLine("MokStore: "+theOneWhoFires.Username+" removed the Owner: "+theFierd.Username);
            return Result.Ok();
        }
        

        public override Result<IList<PurchaseRecord>> GetPurchaseHistory(User user)
        {
            Console.WriteLine("MokStore: Getting Purchase History (its empty..)");
            return Result.Ok<IList<PurchaseRecord>>(new List<PurchaseRecord>());
        }

        public override Result EnterBasketToHistory(IBasket basket)
        {
            Console.WriteLine("MokStore: Entered Basket to History.");
            return  Result.Ok();
        }

        public override string GetStoreName()
        {
            return this.storeName;
        }

        public override bool TryAddNewCartToStore(ICart cart)
        {
            Console.WriteLine("MokStore: Added New Cart To Store");
            return true;
        }

        public override Result ConnectNewBasketToStore(IBasket newBasket)
        {
            Console.WriteLine("MockStore: Connected new basket to store");
            return Result.Ok();
        }

        public override bool CheckConnectionToCart(ICart cart)
        {
            return cart == this.cart;
        }
    }
}