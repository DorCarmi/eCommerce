using System;
using System.Collections.Generic;
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
            _aquiredItems = new List<Item>();
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
        
        public List<Item> SearchItemWithCategoryFilter(string stringSearch, string category)
        {
            List<Item> items = new List<Item>();
            foreach (var item in this._itemsInStore)
            {
                if (item.CheckForResemblance(stringSearch)
                    && item.GetCategory().getName().Equals(category)) 
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

        public Result AddNewItem(IUser user, ItemInfo itemInfo)
        {
            var ans = user.HasPermission(this._belongsToStore, StorePermission.AddItemToStore);
            if(!ans.IsFailure)
            {
                if (this._nameToItem.ContainsKey(itemInfo.name) || this._nameToAquiredItem.ContainsKey(itemInfo.name))
                {
                    return Result.Fail("Item already exist in store");
                } else if(itemInfo.pricePerUnit <= 0)
                {
                    return Result.Fail("Price need to be positive");
                } else if (itemInfo.amount < 0)
                {
                    return Result.Fail("Amount need to be not negative");
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


        public Result AddExistingItem(IUser user,string itemName, int amount)
        {
            if (!user.HasPermission(_belongsToStore, StorePermission.AddItemToStore).IsFailure)
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

        public Result<Item> GetItem(ItemInfo item)
        {
            foreach (var curItem in _itemsInStore)
            {
                if (curItem.GetName().Equals(item))
                {
                    return Result.Ok(curItem);
                }
            }

            return Result.Fail<Item>("Couldn't find item in store's inventory");
        }
        
        public Result<Item> GetItem(string itemID)
        {
            if(this._nameToItem.ContainsKey(itemID))
            {
                return Result.Ok<Item>(this._nameToItem[itemID]);
            }
            else
            {
                return Result.Fail<Item>("Item doens't exist in store's inventory");
            }
        }

        public Result RemoveItem(IUser user, ItemInfo newItem)
        {
            if (!user.HasPermission(_belongsToStore, StorePermission.AddItemToStore).IsFailure)
            {
                if (this._nameToItem.ContainsKey(newItem.name))
                {
                    this._itemsInStore.Remove(_nameToItem[newItem.name]);
                    this._nameToItem.Remove(newItem.name);
                    return Result.Ok();
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

        public Result SubtractItems(IUser user, string newItemName, int newItemAmount)
        {
            if (!user.HasPermission(_belongsToStore, StorePermission.AddItemToStore).IsFailure)
            {
                if (this._nameToItem.ContainsKey(newItemName))
                {
                    return this._nameToItem[newItemName].AddItems(user, newItemAmount);
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
    }
}