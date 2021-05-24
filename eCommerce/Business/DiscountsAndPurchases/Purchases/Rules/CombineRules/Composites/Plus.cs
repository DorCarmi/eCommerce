using System;
using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class Plus : Composite
    {
        private Composite _A;
        private Composite _B;
        public Plus(Composite A, Composite B)
        {
            this._A = A;
            this._B = B;
        }
        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, IUser checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            foreach (var item in checkItem1.GetAllItems().Value)
            {
                itemsList.Add(item.name,item);
            }

            return itemsList;
        }

        public override bool CheckOneItem(ItemInfo itemInfo, IUser checkItem2)
        {
            return true;
        }

        public override bool CheckIfDiscount()
        {
            return _A.CheckIfDiscount() && _B.CheckIfDiscount();
        }
        
        
        public override Result<double> Get(IBasket basket, IUser user)
        {
            if (CheckIfDiscount())
            {
                var price = basket.GetRegularTotalPrice();
                var discountA=_A.Get(basket, user);
                if (discountA.IsFailure)
                {
                    return discountA;
                }

                var discountB = _B.Get(basket, user);
                if (discountB.IsFailure)
                {
                    return discountB;
                }

                var theDiscoutA = discountA.Value / price;
                var theDiscoutB = discountB.Value / price;
                var plusDiscount = theDiscoutA + theDiscoutB;
                return Result.Ok(price * (1 - plusDiscount));
            }
            else
            {
                return Result.Fail<double>("Not a discount");
            }
        }

        public override Result<double> GetOneItem(ItemInfo itemInfo, IUser user)
        {
            var aGet = _A.GetOneItem(itemInfo, user);
            var bGet = _B.GetOneItem(itemInfo, user);
            if (aGet.IsSuccess && bGet.IsSuccess)
            {
                if (aGet.Value >= bGet.Value)
                {
                    return aGet;
                }
                else
                {
                    return bGet;
                }
            }
            else if(aGet.IsSuccess)
            {
                return aGet;
            }
            else if(bGet.IsSuccess)
            {
                return bGet;
            }
            else
            {
                return aGet;
            }
        }
    }
}