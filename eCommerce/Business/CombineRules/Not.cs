using eCommerce.Common;

namespace eCommerce.Business.CombineRules
{
    public class Not<G,T> : Composite<G,T>
    {
        private Composite<G,T> _A;

        public Not(Composite<G,T> A)
        {
            this._A = A;
        }

        public bool Check(G itemToCheck1, T itemToCheck2)
        {
            return !_A.Check(itemToCheck1,itemToCheck2);
        }
        
    }
}