using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;
using Rule = System.Data.Rule;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class Or : Composite
    {
        private Composite _A;
        private Composite _B;

        public Or(Composite A, Composite B)
        {
            this._A = A;
            this._B = B;
        }

        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, IUser checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            var aLst = _A.Check(checkItem1, checkItem2);
            if (aLst.Count > 0)
            {
                itemsList=CombineDictionaries(itemsList, aLst);
            }
            var bLst = _B.Check(checkItem1, checkItem2);
            if (bLst.Count > 0)
            {
                itemsList=CombineDictionaries(itemsList, bLst);
            }

            return itemsList;
        }

        public override bool CheckOneItem(ItemInfo itemInfo, IUser checkItem2)
        {
            return _A.CheckOneItem(itemInfo, checkItem2) || _B.CheckOneItem(itemInfo, checkItem2);
        }

        public override bool CheckIfDiscount()
        {
            return _A.CheckIfDiscount() && _B.CheckIfDiscount();
        }

        public override Result<double> Get(IBasket basket, IUser user)
        {
            if (CheckIfDiscount())
            {
                double price = basket.GetRegularTotalPrice();
                var priceA = _A.Get(basket, user);
                if (priceA.IsSuccess)
                {
                    var diffA = priceA.Value/price;
                    price = price * diffA;
                }
                var priceB = _B.Get(basket, user);
                if (priceB.IsSuccess)
                {
                        
                    var diffB = priceB.Value/price;
                    price=price*diffB;
                }

                return Result.Ok(price);
            }
            else
            {
                return Result.Fail<double>("Not a discount");
            }
        }

        public override Result<double> GetOneItem(ItemInfo itemInfo, IUser user)
        {
            var getA = _A.GetOneItem(itemInfo, user);
            var getB = _B.GetOneItem(itemInfo, user);
            if (getA.IsFailure)
            {
                return getA;
            }

            if (getB.IsFailure)
            {
                return getB;
            }

            var price = itemInfo.amount * itemInfo.pricePerUnit;
            var diffA = getA.Value/price ;
            var diffB = getB.Value/price;
            return Result.Ok(price*diffA*diffB);

        }

        public Result<DiscountInfoNode> GetDiscountInfo()
        {
            if (CheckIfDiscount())
            {
                
            }
            else
            {
                
            }
            return null;
        }

        public Result<RuleInfoNode> GetRuleInfo()
        {
            if (CheckIfDiscount())
            {
                
            }
            return null;
        }
    }
}