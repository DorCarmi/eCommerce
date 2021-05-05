using System.Collections.Generic;
using eCommerce.Business.CombineRules;

namespace eCommerce.Business.Discounts
{
    public class CombinationDiscountInfoLeaf : CombinationDiscountInfoNode
    {
        public IList<DiscountInfo> DiscountInfos;

        public CombinationDiscountInfoLeaf(IList<DiscountInfo> infos) : base(null, Combinations.LEAF)
        {
            this.DiscountInfos = infos;
        }
    }
}