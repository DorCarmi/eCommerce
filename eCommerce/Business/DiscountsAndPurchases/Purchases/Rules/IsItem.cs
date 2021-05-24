using System;
using System.Collections.Generic;

namespace eCommerce.Business.PurchaseRules
{
    public class IsItem: CompositeRule
    {
        private string _itemName;

        public IsItem(string itemName)
        {
            this._itemName = itemName;
        }
        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, IUser checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            foreach (var item in checkItem1.GetAllItems().Value)
            {
                if (item.name.Equals(_itemName))
                {
                    if (!itemsList.ContainsKey(item.name))
                    {
                        itemsList.Add(item.name,item);
                        
                    }
                    return itemsList;
                }
            }

            return itemsList;
        }

        public override bool CheckOneItem(ItemInfo itemInfo, IUser checkItem2)
        {
            return this._itemName.Equals(itemInfo.name);
        }
    }
}