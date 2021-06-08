using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Cart : ICart
    {
        private readonly User _cartHolder;
        private Transaction _performTransaction;
        
        private Dictionary<Store, IBasket> _baskets;
        private double _totalPrice;

        public Cart(User user)
        {
            this._cartHolder = user;
            _baskets = new Dictionary<Store, IBasket>();
            _totalPrice = 0;
        }

        public bool CheckForCartHolder(User user)
        {
            return this._cartHolder == user;
        }

        public Result AddItemToCart(User user,ItemInfo item)
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

        public Result EditCartItem(User user, ItemInfo item)
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
        

        public Result BuyWholeCart(User user, PaymentInfo paymentInfo)
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

        public User GetUser()
        {
            return this._cartHolder;
        }

        public IList<ItemInfo> GetAllItems()
        {
            List<ItemInfo> allItems = new List<ItemInfo>();
            foreach (var basket in _baskets)
            {
                allItems.AddRange(basket.Value.GetAllItems().Value);
            }

            return allItems;
        }

        public void Free()
        {
            foreach (var basket in _baskets)
            {
                basket.Value.Free();
            }

            this._baskets.Clear();
        }
    }
}