using eCommerce.Business.CombineRules;
using eCommerce.Common;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class Xor : Composite<double,bool>
    {
        private Composite<double,bool> _A;
        private Composite<double,bool> _B;
        private Composite<double,bool> _xored;
        private Rule ruleForTie;

        public Xor(Composite<double,bool> A, Composite<double,bool> B, Rule rule)
        {
            this._A = A;
            this._B = B;
            this.ruleForTie = rule;
        }

        public void Calculate(IBasket basket)
        {
            _xored = FindTheXored(basket);
            _xored.Calculate(basket);
        }

        public bool Check(IBasket basket)
        {
            return true;
        }

        public Result<double> Get(IBasket basket)
        {
            Calculate(basket);
            return _xored.Get(basket);
        }

        public Result<double> CheckCalculation(IBasket basket)
        {
            var xored = FindTheXored(basket);
            return xored.CheckCalculation(basket);
        }

        private Composite<double, bool> FindTheXored(IBasket basket)
        {
            
            if (_A.Check(basket) && _B.Check(basket))
            {
                return ruleForTie.Decide(_A, _B);
            }
            else if(_A.Check(basket))
            {
                return _A;
            }
            else
            {
                return _A;
            }
        }
    }
}