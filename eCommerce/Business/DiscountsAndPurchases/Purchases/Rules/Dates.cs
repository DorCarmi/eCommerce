using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Dates : CompositeRule
    {
        private DateTime _date;
        private Compare _compare;
        public Dates(DateTime date, Compare compare)
        {
            _date = date;
            _compare = compare;
        }
        
        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, IUser checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            var compareAns = _compare.GetResult(_date, DateTime.Now);
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
            var compareAns = _compare.GetResult(_date, DateTime.Now);
            if (compareAns > 0)
            {
                return true;
            }

            return false;
        }
    }
}