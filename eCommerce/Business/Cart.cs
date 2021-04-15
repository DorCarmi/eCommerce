using System;
using System.Collections.Generic;
using System.Transactions;
using eCommerce.Business.Basics;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Cart : ICart
    {
        private User _cartHolder;
        private Transaction _performTransaction;
        
        private Dictionary<IStore, IBasket> _baskets;

        public Cart(User user)
        {
            this._cartHolder = user;
            _baskets = new Dictionary<IStore, IBasket>();
        }

        public Result AddItemToCart(User user,ItemInfo item)
        {
            if (user == this._cartHolder)
            {
                if (!this._baskets.ContainsKey(item.GetStore()))
                {
                    if (item.GetStore().TryAddNewCartToStore(this))
                    {
                        var newBasket = new Basket(item.GetStore(), this);
                        item.GetStore().ConnectNewBasketToStore(newBasket);
                    }
                    else
                    {
                        return Result.Fail("Problem- multiple connection of the same cart to the same store");
                    }
                   
                }
                return this._baskets[item.GetStore()].AddItemToBasket(item);

            }
            else
            {
                return Result.Fail("Only cart holder can edit cart");
            }
        }

        public Result EditCartItem(User user, ItemInfo item)
        {
            if (user == this._cartHolder)
            {
                if (this._baskets.ContainsKey(item.GetStore()))
                {
                    return _baskets[item.GetStore()].EditItemInBasket(item);
                }
                else
                {
                    return Result.Fail("No basket exists for the store the item is from");
                }
            }
            else
            {
                return Result.Fail("Only cart holder can edit cart");
            }
        }


        public Result CalculatePricesForCart()
        {
            foreach (var storeBasket in _baskets)
            {
                var currAns = storeBasket.Key.CalculateBasketPrices(storeBasket.Value);
                if (currAns.IsFailure)
                {
                    return currAns;
                }
            }

            return Result.Ok();
        }
        

        public Result<PurchaseInfo> BuyWholeCart(User user)
        {
            List<Item> items = new List<Item>();
            double totalPrice = 0;
            if (user == this._cartHolder)
            {
                foreach (var basket in _baskets)
                {
                    if (!basket.Value.BuyWholeBasket().IsFailure)
                    {
                        var resPrice = basket.Value.GetTotalPrice();
                        if (!resPrice.IsFailure)
                        {
                            totalPrice += resPrice.GetValue();
                            var resItems = basket.Value.GetAllItems();
                            if (!resItems.IsFailure)
                            {
                                items.AddRange(resItems.GetValue());
                            }
                            else
                            {
                                return Result.Fail<PurchaseInfo>("Error getting all items from basket to buy");
                            }
                            
                        }
                        else
                        {
                            return Result.Fail<PurchaseInfo>("Error getting total price from basket to buy");
                        }
                    }
                    return Result.Fail<PurchaseInfo>("Error trying to buy all items from basket");
                }

                return Result.Ok(new PurchaseInfo(items, totalPrice, user));
            }
            else
            {
                return Result.Fail<PurchaseInfo>("Only cart holder can edit cart");
            }
        }
    }
}