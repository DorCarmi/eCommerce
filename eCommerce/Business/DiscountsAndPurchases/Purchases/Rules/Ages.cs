using System;
using System.Collections.Generic;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business.PurchaseRules
{
    public class Ages : CompositeRule
    {
        private int age;
        private Compare compare;

        public Ages(int age, Compare compare)
        {
            this.age = age;
            this.compare = compare;
        }
        
        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, IUser checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            var compareAns = compare.GetResult(age, (checkItem2.MemberInfo.Birthday.Date - DateTime.Now.Date).Days);
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
            var compareAns = compare.GetResult(age, (checkItem2.MemberInfo.Birthday.Date - DateTime.Now.Date).Days);
            return compareAns > 0;
        }

        
    }
    
}