using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.Service;
using eCommerce.Common;
using Microsoft.AspNetCore.Authorization;

namespace eCommerce.Business
{
    public class Cart : ICart
    {
        private readonly IUser _cartHolder;
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
                var storeRes = item.GetStore();
                if (storeRes.IsFailure)
                {
                    return Result.Fail(storeRes.Error);
                }
                else
                {
                    if (!this._baskets.ContainsKey(storeRes.Value))
                    {
                        if (storeRes.Value.TryAddNewCartToStore(this))
                        {
                            var newBasket = new Basket(this, storeRes.Value);
                            var basketRes=storeRes.Value.ConnectNewBasketToStore(newBasket);
                            
                            if (basketRes.IsFailure)
                            {
                                return Result.Fail(basketRes.Error);
                            }
                            else
                            {
                                this._baskets.Add(storeRes.Value,newBasket);
                            }
                        }
                        else
                        {
                            return Result.Fail("Problem- multiple connection of the same cart to the same store");
                        }

                    }

                    return this._baskets[storeRes.Value].AddItemToBasket(user, item);
                }

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
                var storeRes = item.GetStore();
                if (storeRes.IsFailure)
                {
                    return Result.Fail(storeRes.Error);
                }
                else
                {
                    if (this._baskets.ContainsKey(storeRes.Value))
                    {
                        return _baskets[storeRes.Value].EditItemInBasket(user, item);
                    }
                    else
                    {
                        return Result.Fail("No basket exists for the store the item is from");
                    }
                }
            }
            else
            {
                return Result.Fail("Only cart holder can edit cart");
            }
        }


        public Result<double> CalculatePricesForCart()
        {
            this._totalPrice = 0;
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

        public CartInfo GetCartInfo()
        {
            IList<BasketInfo> baskets = new List<BasketInfo>();
            foreach (var di_basket in _baskets)
            {
                baskets.Add(di_basket.Value.GetBasketInfo());
            }

            CalculatePricesForCart();
            CartInfo info = new CartInfo(baskets, this._totalPrice);
            return info;
        }

        public IUser GetUser()
        {
            return this._cartHolder;
        }
    }
}