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
    public class Not : Composite
    {
        private Composite _A;

        public Not(Composite A)
        {
            this._A = A;
        }

        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, IUser checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            var aLst = _A.Check(checkItem1, checkItem2);
            foreach (var item in checkItem1.GetAllItems().Value)
            {
                if (!aLst.ContainsKey(item.name))
                {
                    itemsList.Add(item.name,item);
                }
            }
            return itemsList;
        }

        public override bool CheckOneItem(ItemInfo itemInfo, IUser checkItem2)
        {
            return !_A.CheckOneItem(itemInfo, checkItem2);
        }

        public override bool CheckIfDiscount()
        {
            return _A.CheckIfDiscount(); 
        }

        public override Result<double> Get(IBasket basket, IUser user)
        {
            if (CheckIfDiscount())
            {
                double price = basket.GetRegularTotalPrice();
                foreach (var item in basket.GetAllItems().Value)
                {
                    var basicPrice = Convert.ToDouble(item.amount * item.pricePerUnit);
                    if (!_A.CheckOneItem(item, user))
                    {
                        var priceAfterDiscount= _A.GetOneItem(item, user);
                        if (priceAfterDiscount.IsFailure)
                        {
                            return priceAfterDiscount;
                        }
                        basicPrice -= priceAfterDiscount.Value;
                        price -= basicPrice;
                    }
                }

                return Result.Ok<double>(price);
            }
            else
            {
                return Result.Fail<double>("Not a discount");
            }
        }

        public override Result<double> GetOneItem(ItemInfo itemInfo, IUser user)
        {
            var price = itemInfo.amount * itemInfo.pricePerUnit;
            if (_A.CheckOneItem(itemInfo, user))
            {
                return Result.Ok<double>(price);
            }
            else
            {
                return _A.GetOneItem(itemInfo, user);
            }
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