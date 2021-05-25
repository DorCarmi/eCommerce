using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Amounts: CompositeRule
    {
        private ItemInfo _item;
        private int _amount;
        private Compare _compare;
        
        public Amounts(ItemInfo item,int amount,Compare compare)
        {
            _item = item;
            _amount = amount;
            _compare = compare;
        }

        private Dictionary<string, ItemInfo> CheckForSpecificItem(IBasket basket)
        {
            bool ans = false;
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            foreach (var item in basket.GetAllItems().Value)
            {
                if (item.name.Equals(_item.name) &&
                    item.storeName.Equals(_item.storeName)
                    && _compare.GetResult(_amount,item.amount)>0)
                    {
                        if (!itemsList.ContainsKey(item.name))
                        {
                            itemsList.Add(item.name,item);
                        }
                    }
            }

            return itemsList;
        }
        
        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, IUser checkItem2)
        {
            var lst=CheckForSpecificItem(checkItem1);
            return lst;
        }

        public override bool CheckOneItem(ItemInfo item, IUser checkItem2)
        {
            if (item.name.Equals(_item.name) &&
                item.storeName.Equals(_item.storeName)
                && _compare.GetResult(_amount,item.amount)>0)
            {
                return true;
            }

            return false;
        }
    }
}