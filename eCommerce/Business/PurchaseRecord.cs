using System;

using eCommerce.Service;

namespace eCommerce.Business
{
    public class PurchaseRecord
    {
        private StoreInfo _storeInfo;
        private BasketInfo _basketInfo;
        private DateTime _dateTime;
        // TODO add username
        private string _username;
        public PurchaseRecord(Store store, IBasket basket, DateTime now)
        {
            this._storeInfo = new StoreInfo(store);
            this._basketInfo = new BasketInfo(basket);
            this._username = basket.GetCart().GetUser().Username;
            this._dateTime = now;
        }

        public StoreInfo GetStoreInfo()
        {
            return _storeInfo;
        }

        public BasketInfo GetBasketInfo()
        {
            return _basketInfo;
        }

        public DateTime GetDate()
        {
            return _dateTime;
        }

        public string Username { get => _username; }
        public string StoreId { get => _storeInfo.GetStoreName(); }
        public BasketInfo BasketInfo { get => _basketInfo; }
        public DateTime PurchaseTime { get => _dateTime; }
    }
}