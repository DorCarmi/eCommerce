﻿using System.Collections.Generic;
using eCommerce.Business.Discounts;
using eCommerce.Business.Purchases;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class TotalPrice : CompositeRule
    {
        private int _totalPrice;
        private Compare _compare;

        public TotalPrice(int totalPrice,Compare compare)
        {
            _totalPrice = totalPrice;
            _compare = compare;
        }
        

        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, IUser checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            int totalPrice = 0;
            foreach (var item in checkItem1.GetAllItems().Value)
            {
                totalPrice += item.amount * item.pricePerUnit;
            }
            var compareAns = _compare.GetResult(_totalPrice, totalPrice);
            if (compareAns > 0)
            {
                foreach (var item in checkItem1.GetAllItems().Value)
                {
                    if (!itemsList.ContainsKey(item.name))
                    {
                        itemsList.Add(item.name,item);
                    }
                }
            }

            return itemsList;
        }

        public override bool CheckOneItem(ItemInfo itemInfo, IUser checkItem2)
        {
            var compareAns = _compare.GetResult(_totalPrice, itemInfo.amount*itemInfo.pricePerUnit);
            return compareAns > 0;
        }

        public override Result<RuleInfoNode> GetRuleInfo()
        {
            return Result.Ok<RuleInfoNode>(new RuleInfoNodeLeaf(new RuleInfo(RuleType.Total_Price, this._totalPrice.ToString(), "", "",
                this._compare.GetComperatorInfo())));
        }
    }
}