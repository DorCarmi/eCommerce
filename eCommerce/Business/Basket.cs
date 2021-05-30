using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Basket: IBasket
    {
        private IList<ItemInfo> _itemsInBasket;
        private IDictionary<string, ItemInfo> _nameToItem;
        private IStore _store;
        private ICart _cart;
        

        private double currentPrice;
        
        public double GetRegularTotalPrice()
        {
            double totalPrice = 0;
            foreach (var itemInfo in _itemsInBasket)
            {
                totalPrice += itemInfo.amount * itemInfo.pricePerUnit;
            }

            return totalPrice;
        }

        public Result CheckWithStorePolicy()
        {
            return _store.CheckWithStorePolicy(this, _cart.GetUser());
        }

        public Basket(ICart cart, IStore store)
        {
            if (!store.CheckConnectionToCart(cart))
            {
                this._cart = cart;
                this._store = store;
                this._itemsInBasket = new List<ItemInfo>();
                this._nameToItem = new Dictionary<string, ItemInfo>();
                this.currentPrice = 0;
            }
            else
            {
                throw new ArgumentException("This cart is already connected to the store");
            }
        }
        
        public Result CalculateBasketPrices()
        {
            return _store.CalculateBasketPrices(this);
        }

        public Result AddItemToBasket(IUser user,ItemInfo item)
        {
            if (item.amount <= 0)
            {
                return Result.Fail("Bad input- bad item's amount");
            }

            if (!_cart.CheckForCartHolder(user))
            {
                return Result.Fail("User is not cart holder, can't perform changes to cart");
            }

            if (!item.storeName.Equals(this._store.GetStoreName()))
            {
                return Result.Fail("Item doesn't belong to basket's store");
            }

            var itemRes = this._store.TryGetItems(item);
            if (itemRes.IsFailure )
            {
                return itemRes;
            }
            
            else if (this._nameToItem.ContainsKey(item.name))
            {
                this._nameToItem[item.name].amount += item.amount;
                this.currentPrice = this.GetRegularTotalPrice();
                return Result.Ok();
            }
            else
            {
                this._nameToItem.Add(item.name,new ItemInfo(item));
                this._itemsInBasket.Add(item);
                this.currentPrice = this.GetRegularTotalPrice();
                return Result.Ok();
            }
        }

        public Result EditItemInBasket(IUser user,ItemInfo item)
        {
            if (!_cart.CheckForCartHolder(user))
            {
                return Result.Fail("User is not cart holder, can't perform changes to cart");
            }
            if (this._nameToItem.ContainsKey(item.name))
            {
                if (item.amount == -1 || item.amount == 0)
                {
                    this._nameToItem.Remove(item.name);
                    this.currentPrice = this.GetRegularTotalPrice();
                    return Result.Ok();
                }
                else if (item.amount <= 0)
                {
                    return Result.Fail("Bad amount for item info");
                }
                else if(this._store.TryGetItems(item).IsFailure)
                {
                    return Result.Fail("Problem getting items from store");
                }
                else
                {
                    this._nameToItem[item.name].amount = item.amount;
                    this.currentPrice = this.GetRegularTotalPrice();
                    return Result.Ok();
                }
            }
            else
            {
                return Result.Fail("Item doens't exist in basket, you can add it if you want");
            }
        }

        public Result BuyWholeBasket()
        {
            return this._store.FinishPurchaseOfBasket(this);
        }

        public Result<double> GetTotalPrice()
        {
            return Result.Ok(this.currentPrice);
        }

        public Result<IList<ItemInfo>> GetAllItems()
        {
            return Result.Ok(_itemsInBasket);
        }

        public ICart GetCart()
        {
            return this._cart;
        }

        public Result SetTotalPrice()
        {
            var res=this._store.CheckDiscount(this);
            if (res.IsFailure)
            {
                return res;
            }
            else
            {
                this.currentPrice = res.Value;
                return Result.Ok();
            }
            
        }
        
        public void SetTotalPrice(double newTotalPrice)
        {
            currentPrice = newTotalPrice;
            
        }

        public string GetStoreName()
        {
            return this._store.GetStoreName();
        }

        public Result<ItemInfo> GetItem(IUser user,string itemName)
        {
            if (!_cart.CheckForCartHolder(user))
            {
                return Result.Fail<ItemInfo>("User is not cart holder, can't perform changes to cart");
            }
            if (this._nameToItem.ContainsKey(itemName))
            {
                return Result.Ok(_nameToItem[itemName]);
            }
            else
            {
                return Result.Fail<ItemInfo>("Item doens't exist in basket, you can add it if you want");
            }
        }

        public BasketInfo GetBasketInfo()
        {
            BasketInfo basketInfo = new BasketInfo(this);
            return basketInfo;
        }


        public Result AddBasketRecords()
        {
            var resStore=this._store.AddBasketRecordToStore(this);
            if (resStore.IsFailure)
            {
                return Result.Fail(resStore.Error);
            }

            var userRes=this.GetCart().GetUser().EnterRecordToHistory(resStore.Value);
            return userRes;
        }
    }
    
    
}