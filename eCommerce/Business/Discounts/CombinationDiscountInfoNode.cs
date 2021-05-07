using System.Collections.Generic;
using eCommerce.Business.CombineRules;

namespace eCommerce.Business.Discounts
{
    public class CombinationDiscountInfoNode
    {
        public IList<CombinationDiscountInfoNode> combinationDiscountInfoNodes;
        public Combinations _combination;

        public CombinationDiscountInfoNode(IList<CombinationDiscountInfoNode> combinationDiscountInfoNodes, Combinations combination)
        {
            this.combinationDiscountInfoNodes = combinationDiscountInfoNodes;
            this._combination = combination;
        }
        
        
    }
}