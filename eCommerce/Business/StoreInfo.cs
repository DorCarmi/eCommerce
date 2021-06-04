

namespace eCommerce.Business
{
    public class StoreInfo
    {
        private string _storeName;

        public StoreInfo(Store store)
        {
            this._storeName = store.GetStoreName();
        }

        public string GetStoreName()
        {
            return this._storeName;
        }

    }
}