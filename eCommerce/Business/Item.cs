using System;
using System.Collections.Generic;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Item
    {
        private String _name;
        private int _amount;
        private IStore _belongsToStore;
        private Category _category;
        private List<String> _keyWords;
        private PurchaseStrategy _purchaseStrategy;
        private int _pricePerUnit;

        public int GetTotalPrice()
        {
            return _pricePerUnit * _amount;
        }

        public int GetPricePerUnit()
        {
            return this._pricePerUnit;
        }
        
        public Item(String name, Category category, IStore store, int pricePer)
        {
            this._name = name;
            this._category = category;
            this._belongsToStore = store;
            _amount = 1;
            this._keyWords=new List<string>();
            this._pricePerUnit = pricePer;
            _purchaseStrategy = new DefaultPurchaseStrategy(_belongsToStore);
        }

        public Item(ItemInfo info, IStore store)
        {
            this._name = info.name;
            this._amount = info.amount;
            this._category = new Category(info.category);
            this._belongsToStore = store;
            CopyKeyWords(info.keyWords);
            this._purchaseStrategy = new DefaultPurchaseStrategy(_belongsToStore);
            this._pricePerUnit = info.pricePerUnit;
            this._belongsToStore = store;
        }

        private void CopyKeyWords(IList<string> words)
        {
            this._keyWords = new List<string>();
            foreach (var ketWord in words)
            {
                _keyWords.Add(ketWord);
            }
        }

        public Result SetPrice(IUser user,int pricePerUnit)
        {
            if (!user.HasPermission(_belongsToStore, StorePermission.ChangeItemPrice).IsFailure)
            {
                if (pricePerUnit > 0)
                {
                    this._pricePerUnit = pricePerUnit;
                    return Result.Ok();
                }
                else
                {
                    return Result.Fail("Bad new price input");
                }
            }
            else
            {
                return Result.Ok("User doesn't have the permission for this store");
            }
        }

        public IStore GetStore()
        {
            return this._belongsToStore;
        }

        public String GetName()
        {
            return this._name;
        }

        public int GetAmount()
        {
            return _amount;
        }

        public Category GetCategory()
        {
            return _category;
        }

        public Result EditCategory(User user,Category category)
        {
            if (user.HasPermission(this._belongsToStore, StorePermission.EditItemDetails).IsSuccess)
            {
                this._category = category;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("User doesn't have permission to edit item");
            }
        }
        
        public Result AddKeyWord(IUser user,String keyWord)
        {
            
            if (!user.HasPermission(this._belongsToStore, StorePermission.EditItemDetails).IsFailure)
            {
                if (keyWord != null && keyWord.Length > 0)
                {
                    this._keyWords.Add(keyWord);
                    return Result.Ok();
                }
                else
                {
                    return Result.Fail("Bad input of key word");
                }
            }
            else
            {
                return Result.Fail("User doesn't have permission to edit item");
            }
        }

        public Result<bool> CheckPricesInBetween(int startPrice, int endPrice)
        {
            if (startPrice > 0 && startPrice <= endPrice)
            {
                if (this._pricePerUnit >= startPrice && this._pricePerUnit <= endPrice)
                {
                    return Result<bool>.Ok(true);
                }
                else
                {
                    return Result<bool>.Ok(false);
                }
            }
            else
            {
                return Result.Fail<bool>("Bad input- start or end price");
            }
        }


        public bool CheckForResemblance(string searchString)
        {
            if (this._name.Contains(searchString))
            {
                return true;
            }
            else if (_category.getName().Contains(searchString))
            {
                return true;
            }
            else if(this._keyWords.Contains(searchString))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ItemInfo ShowItem()
        {
            return GetItemInfo(this._amount);
        }

        public Result<bool> CheckItemAvailability(int amount)
        {
            if (amount <= 0)
            {
                return Result.Fail<bool>("Bad amount input");
            }
            else if (this._amount - amount <= 1)
            {
                return Result.Ok<bool>(false);
            }
            else
            {
                return Result.Ok<bool>(true);
            }
        }

        private ItemInfo GetItemInfo(int amount)
        {
            var info=new ItemInfo(amount, this._name, this._belongsToStore.GetStoreName(), this._category.getName(),
                this._pricePerUnit,this._keyWords,this);
            info.AssignStoreToItem(this._belongsToStore);
            return info;
        }

        public Result<ItemInfo> AquireItems(ItemInfo itemInfo)
        {
            return this._purchaseStrategy.AquireItems(itemInfo);
        }

        public Result<ItemInfo> GetItems(int amount)
        {
            //Use to get items to put in basket
            if (this._amount - amount <= 1)
            {
                return Result.Fail<ItemInfo>("There are no enough items to answer the requested amount");
            }
            else
            {
                
                return Result.Ok<ItemInfo>(GetItemInfo(amount));
            }
        }

        public Result FinalizeGetItems(int amount)
        {
            if (this._amount - amount < 1)
            {
                return Result.Fail("There are no enough items to answer the requested amount");
            }
            else
            {
                this._amount -= amount;
                return Result.Ok();
            }
        }

        public Result AddItems(IUser user,int amount)
        {
            if (amount > 0)
            {
                this._amount += amount;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("Bad input");
            }
        }
        
        
        public Result SubtractItems(IUser user,int amount)
        {
            if (this._amount-amount >= 1)
            {
                this._amount -= amount;
                return Result.Ok();
            }
            else
            {
                return Result.Fail("Bad input- amount can't stay lower than 1");
            }
        }

        public Result AssignPurchaseStrategy(User user,PurchaseStrategyName purchaseStrategy)
        {
            if (user.HasPermission(_belongsToStore, StorePermission.ChangeItemStrategy).IsSuccess)
            {
                if (_belongsToStore.CheckWithPolicy(purchaseStrategy))
                {
                    var resStrategy=DefaultPurchaseStrategy.GetPurchaseStrategyByName(purchaseStrategy,_belongsToStore);
                    if (resStrategy.IsFailure)
                    {
                        return Result.Fail(resStrategy.GetErrorReason());
                    }
                    else
                    {
                        
                        this._purchaseStrategy = resStrategy.GetValue();
                        return Result.Ok();
                    }
                    
                }
                else
                {
                    return Result.Fail("Strategy not permitted by store's policy");
                }
            }
            else
            {
                return Result.Fail(
                    "User doesn't have the permission to change the strategy for this item in this store");
            }
        }

        public Result EditItem(ItemInfo newItem)
        {
            if (!newItem.name.Equals(this._name))
            {
                return Result.Fail("Item info is not for the same item");
            }

            if (!newItem.storeName.Equals(this._belongsToStore.GetStoreName()))
            {
                return Result.Fail("Item info is not about the same store");
            }

            if (newItem.amount < 0)
            {
                return Result.Fail("Item in store can't be negative");
            }

            if (newItem.pricePerUnit <= 0)
            {
                return Result.Fail("Item price can't be lower or equal to zero");
            }
            this._amount=newItem.amount;
            this._category = new Category(newItem.category);
            CopyKeyWords(newItem.keyWords);
            this._pricePerUnit = newItem.pricePerUnit;
            return Result.Ok();
        }
    }
}