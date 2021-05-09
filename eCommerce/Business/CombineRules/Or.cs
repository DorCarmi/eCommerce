using eCommerce.Business.CombineRules;
using eCommerce.Common;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class Or<G,T> : Composite<G,T>
    {
        private Composite<G,T> _A;
        private Composite<G,T> _B;

        public Or(Composite<G,T> A, Composite<G,T> B)
        {
            this._A = A;
            this._B = B;
        }
        
        public bool Check(G itemToCheck1, T itemToCheck2)
        {
            return _A.Check(itemToCheck1,itemToCheck2) || _B.Check(itemToCheck1,itemToCheck2);
        }
        
    }
}