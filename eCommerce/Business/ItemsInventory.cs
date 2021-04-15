using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.Basics;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class ItemsInventory
    {
        private List<Item> _itemsInStore;
        private List<Item> _aquiredItems;
        private Dictionary<string, Item> _nameToItem;
        private Dictionary<string, Item> _nameToAquiredItem;
        private IStore _belongsToStore;

        public ItemsInventory(IStore store)
        {
            this._belongsToStore = store;
            this._itemsInStore = new List<Item>();
            this._aquiredItems = new List<Item>();
            this._nameToItem = new Dictionary<string, Item>();
            this._nameToAquiredItem = new Dictionary<string, Item>();
        }
        
        //Searches

        public List<Item> SearchItem(string stringSearch)
        {
            List<Item> items = new List<Item>();
            foreach (var item in this._itemsInStore)
            {
                if (item.CheckForResemblance(stringSearch))
                {
                    items.Add(item);
                }
            }

            return items;
        }
        
        public List<Item> SearchItemWithPriceFilter(string stringSearch, int startPrice, int endPrice)
        {
            List<Item> items = new List<Item>();
            foreach (var item in this._itemsInStore)
            {
                if (item.CheckForResemblance(stringSearch)
                    &&item.CheckPricesInBetween(startPrice,endPrice).GetValue())
                {
                    items.Add(item);
                }
            }

            return items;
        }


        public List<Item> GetAllItemsInStore()
        {
            return this._itemsInStore;
        }

        public Result AddNewItem(User user, ItemInfo itemInfo)
        {
            if(user.hasPermission(this._belongsToStore,StorePermission.AddItemToStore))
            {
                if (this._nameToItem.ContainsKey(itemInfo.name) || this._nameToAquiredItem.ContainsKey(itemInfo.name))
                {
                    return Result.Fail("Item already exist in store");
                }
                else
                {
                    var newItem = new Item(itemInfo);
                    this._itemsInStore.Add(newItem);
                    this._nameToItem.Add(itemInfo.name, newItem);
                    return Result.Ok();
                }
            }
            else
            {
                return Result.Fail("User doesn't have permission to add item to store");
            }
        }


        public Result AddExistingItem(User user,string itemName, int amount)
        {
            if (user.hasPermission(_belongsToStore, StorePermission.AddItemToStore))
            {
                if (this._nameToItem.ContainsKey(itemName))
                {
                    return this._nameToItem[itemName].AddItems(user, amount);
                }
                else
                {
                    return Result.Fail("Item doesn't exist in store");
                }
            }
            else
            {
                return Result.Fail("User doesn't have permission to add item to store");
            }
        }


        public Result<ItemInfo> GetItems(string itemName, int amount)
        {
            if (this._nameToItem.ContainsKey(itemName))
            {
                if (_nameToItem[itemName].GetAmount() - amount <= 1)
                {
                    return Result.Fail<ItemInfo>("Amount requested will leave store with under limit amount of items");
                }
                else
                {
                    return _nameToItem[itemName].GetItems(amount);    
                }
            
                
            }
            else
            {
                return Result.Fail<ItemInfo>("Item doesn't exist in store");
            }
        }

        public Result FinalizeGetItems(string itemName, int amount)
        {
            if (this._nameToItem.ContainsKey(itemName))
            {
                if (_nameToItem[itemName].GetAmount() - amount <= 1)
                {
                    return Result.Fail<ItemInfo>("Amount requested will leave store with under limit amount of items");
                }
                else
                {
                    return _nameToItem[itemName].FinalizeGetItems(amount);    
                }
            }
            else
            {
                return Result.Fail<ItemInfo>("Item doesn't exist in store");
            }
        }
        
    }
}