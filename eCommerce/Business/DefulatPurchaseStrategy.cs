using System;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class DefaultPurchaseStrategy : PurchaseStrategy
    {
        private IStore _store;
        public static Result<PurchaseStrategy> GetPurchaseStrategyByName(PurchaseStrategyName purchaseStrategyName, IStore store)
        {
            if (PurchaseStrategyName.Regular == purchaseStrategyName)
            {
                return Result.Ok<PurchaseStrategy>(new  DefaultPurchaseStrategy(store));
            }
            else
            {
                return Result.Fail<PurchaseStrategy>("No such strategy");
            }
        }

        public DefaultPurchaseStrategy(IStore store)
        {
            this._store = store;
        }

        public Result<ItemInfo> AquireItems(ItemInfo itemInfo)
        {
            return this._store.GetItem(itemInfo).GetValue().GetItems(itemInfo.amount);
        }
        
        public Result AquireItems(IBasket basket)
        {
            return basket.SetTotalPrice();
        }

        public Result BuyItems(IBasket basket)
        {
            return _store.FinishPurchaseOfBasket(basket);
        }
        public Result BuyItems(ItemInfo item)
        {
            return _store.FinishPurchaseOfItems(item);
        }
    }
    
}