using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Cart : ICart
    {
        private IUser _cartHolder;
        private Transaction _performTransaction;
        
        private Dictionary<IStore, IBasket> _baskets;
        private double _totalPrice;

        public Cart(IUser user)
        {
            this._cartHolder = user;
            _baskets = new Dictionary<IStore, IBasket>();
            _totalPrice = 0;
        }

        public bool CheckForCartHolder(IUser user)
        {
            return this._cartHolder == user;
        }

        public Result AddItemToCart(IUser user,ItemInfo item)
        {
            if (user == this._cartHolder)
            {
                if (!this._baskets.ContainsKey(item.GetStore()))
                {
                    if (item.GetStore().TryAddNewCartToStore(this))
                    {
                        var newBasket = new Basket(this,item.GetStore());
                        return item.GetStore().ConnectNewBasketToStore(newBasket);
                    }
                    else
                    {
                        return Result.Fail("Problem- multiple connection of the same cart to the same store");
                    }
                   
                }
                return this._baskets[item.GetStore()].AddItemToBasket(user,item);

            }
            else
            {
                return Result.Fail("Only cart holder can edit cart");
            }
        }

        public Result EditCartItem(IUser user, ItemInfo item)
        {
            if (user == this._cartHolder)
            {
                if (this._baskets.ContainsKey(item.GetStore()))
                {
                    return _baskets[item.GetStore()].EditItemInBasket(user,item);
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


        public Result<double> CalculatePricesForCart()
        {
            foreach (var storeBasket in _baskets)
            {
                var currAns = storeBasket.Key.CalculateBasketPrices(storeBasket.Value);
                if (currAns.IsFailure)
                {
                    return Result.Fail<double>(currAns.Error);
                }

                double currBasketPrice = storeBasket.Value.GetTotalPrice().GetValue();
                this._totalPrice += currBasketPrice;
            }
            return Result.Ok(_totalPrice);
        }

        public double GetCurrentCartPrice()
        {
            return this._totalPrice;
        }
        

        public Result BuyWholeCart(IUser user, PaymentInfo paymentInfo)
        {
            this._performTransaction = new Transaction(this);
            var result=_performTransaction.BuyWholeCart(paymentInfo);
            return result;
        }
        public IList<IBasket> GetBaskets()
        {
            return this._baskets.Values.ToList();
        }
    }
}