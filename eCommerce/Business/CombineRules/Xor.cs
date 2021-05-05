using eCommerce.Business.CombineRules;
using eCommerce.Common;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class Xor<G,T> : Composite<G,T>
    {
        private Composite<G,T> _A;
        private Composite<G,T> _B;
        private Composite<G,T> _xored;
        private Rule<G,T> ruleForTie;

        public Xor(Composite<G,T> A, Composite<G,T> B, Rule<G,T> rule)
        {
            this._A = A;
            this._B = B;
            this.ruleForTie = rule;
        }


        public bool Check(G itemToCheck1, T itemToCheck2)
        {
            return (_A.Check(itemToCheck1,itemToCheck2) || _B.Check(itemToCheck1,itemToCheck2));
        }

    }
}