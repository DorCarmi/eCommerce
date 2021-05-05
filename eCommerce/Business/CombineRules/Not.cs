using eCommerce.Common;

namespace eCommerce.Business.CombineRules
{
    public class Not
    {
        private Composite<double,bool> _A;

        public Not(Composite<double,bool> A)
        {
            this._A = A;
        }

        public void Calculate(IBasket basket)
        {
            if (Check(basket))
            {
                _A.Calculate(basket);
            }
            
        }

        public bool Check(IBasket basket)
        {
            return !_A.Check(basket);
        }

        public Result<double> Get(IBasket basket)
        {
            Calculate(basket);
            return _A.Get(basket);
        }

        public Result<double> CheckCalculation(IBasket basket)
        {
            var startPoint = _A.Get(basket);
            if (startPoint.IsFailure)
            {
                return startPoint;
            }
            if (Check(basket))
            {
                var Adiscounted = _A.CheckCalculation(basket);
                if (Adiscounted.IsFailure)
                {
                    return Adiscounted;
                }
                var Adiff = startPoint.Value - Adiscounted.Value;
                return Result.Ok(startPoint.Value - Adiff);
            }
            else
            {
                return startPoint;
            }
        }
    }
}