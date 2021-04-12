using System;
using System.Collections.Generic;
using System.Linq;
using eCommerce.Business.Basics;

namespace eCommerce.Business
{
    public class ItemsInventory
    {
        private List<Item> _itemsInStore;
        private List<Item> _aquiredItems;
        private Dictionary<string, Item> _nameToItem;
        private Dictionary<string, Item> _nameToAquiredItem;
        private Store _belongsToSotStore;

        public ItemsInventory(Store store)
        {
            this._belongsToSotStore = store;
            this._itemsInStore = new List<Item>();
            this._aquiredItems = new List<Item>();
            this._nameToItem = new Dictionary<string, Item>();
            this._nameToAquiredItem = new Dictionary<string, Item>();
        }

        public Answer<bool> addNewItem(ItemInfo itemInfo)
        {
            if (this._nameToItem.ContainsKey(itemInfo.name))
            {
                return new Answer<bool>("Can't have items with the same name in store");
            }
            else
            {
                Item newItem = new Item(itemInfo, _belongsToSotStore);
                this._itemsInStore.Add(newItem);
                this._nameToItem.Add(itemInfo.name,newItem);
                return new Answer<bool>(true);
            }
            
        }

        

        public Answer<bool> addExistingItem(string itemName, int amount)
        {
            
            if (this._nameToItem.ContainsKey(itemName))
            {
                var item = this._nameToItem[itemName];
                item.addItems(amount);
                return new Answer<bool>(true);
            }
            else
            {
                return new Answer<bool>("No such item in inventory");
            }
        }

        public Answer<int> getItemAmount(string itemName, int amount)
        {
            if (this._nameToItem.ContainsKey(itemName))
            {
                return new Answer<int>(_nameToItem[itemName].getAmount());
            }
            else
            {
                return new Answer<int>("No such item in inventory");
            }
        }

        public Answer<Item> getItemsFromInventory(string itemName, int amount)
        {
            if (this._nameToItem.ContainsKey(itemName))
            {
                var item = _nameToItem[itemName];
                var items = item.getItems(amount);
                if (items.isOk())
                {
                    Item newItem = new Item(item.getItemInfo(), _belongsToSotStore);
                    newItem.addItems(amount);
                    return new Answer<Item>(newItem);
                }
                else
                {
                    return new Answer<Item>(items.getReason());
                }
            }
            else
            {
                return new Answer<Item>("Items doesn't exist in inventory");
            }
        }
        
        public Answer<Item> aquireItems(String itemName, int amount)
        {
            if (this._nameToItem.ContainsKey(itemName))
            {
                var item = _nameToItem[itemName];
                var items = item.getItems(amount);
                if (items.isOk())
                {
                    if (_nameToAquiredItem.ContainsKey(itemName))
                    {
                        _nameToAquiredItem[itemName].addItems(amount);
                        return new Answer<Item>(_nameToAquiredItem[itemName]);
                    }
                    else
                    {
                        var newItem = new Item(item.getItemInfo(), this._belongsToSotStore);
                        newItem.addItems(amount);
                        newItem.aquireItem();
                        _nameToAquiredItem.Add(itemName, newItem);
                        return new Answer<Item>(newItem);

                    }
                }
                else
                {
                    return new Answer<Item>(items.getReason());
                }
                
            }
            else
            {
                return new Answer<Item>("Item doesn't exists in store");
            }
        }

        public Answer<Item> getItemsToBasket(string itemName, int amount)
        {
            if (this._nameToItem.ContainsKey(itemName))
            {
                return this._nameToItem[itemName].getCurrentPurchaseStrategy().getItemsToBasket(amount);
            }
            else
            {
                return new Answer<Item>("Item doesn't exist in inventory");
            }
        }

        public Answer<List<ItemInfo>> findItems(string searchString)
        {
            List<ItemInfo> items = new List<ItemInfo>();
            foreach (var item in this._itemsInStore)
            {
                if (item.checkResemblance(searchString))
                {
                    items.Add(item.getItemInfo());
                }
            }

            if (items.Count > 0)
            {
                return new Answer<List<ItemInfo>>(items);
            }
            else
            {
                return new Answer<List<ItemInfo>>("No items match to search string");
            }
        }
    }
}