using System.Collections.Generic;

namespace eCommerce.Business
{
    public class DiscountInfo
    {
        public DiscountType discountType;
        public string WhatIsTheDiscountFor;
        public ItemInfo WhichItems;
        public double TheDiscount;

        public DiscountInfo(DiscountType type, string whatIsTheDiscountFor, ItemInfo whichItems, double theDiscount)
        {
            WhatIsTheDiscountFor = whatIsTheDiscountFor;
            WhichItems = whichItems;
            TheDiscount = theDiscount;
        }

        public bool Equals(DiscountInfo discountInfo)
        {
            return discountInfo.discountType.Equals(this.discountType)
                   && discountInfo.TheDiscount == this.TheDiscount
                   && discountInfo.WhichItems.name.Equals(this.WhichItems.name)
                   && discountInfo.WhichItems.storeName.Equals(this.WhichItems.storeName)
                   && discountInfo.WhatIsTheDiscountFor.Equals(this.WhatIsTheDiscountFor);
        }
    }
}