﻿using eCommerce.Business.CombineRules;
using eCommerce.Common;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class Or : Composite<double,bool>
    {
        private Composite<double,bool> _A;
        private Composite<double,bool> _B;

        public Or(Composite<double,bool> A, Composite<double,bool> B)
        {
            this._A = A;
            this._B = B;
        }

        public void Calculate(IBasket basket)
        {
            if (Check(basket))
            {
                _A.Calculate(basket);
                _B.Calculate(basket);
            }
        }

        public bool Check(IBasket basket)
        {
            return _A.Check(basket) || _B.Check(basket);
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
                var Bdiscounted = _B.CheckCalculation(basket);
                if (Adiscounted.IsFailure || Bdiscounted.IsFailure)
                {
                    return Adiscounted.IsFailure ? Adiscounted : Bdiscounted;
                }
                var Adiff = startPoint.Value - Adiscounted.Value;
                var Bdiff = startPoint.Value - Bdiscounted.Value;
                return Result.Ok(startPoint.Value - Adiff - Bdiff);
            }
            else
            {
                return startPoint;
            }
        }
    }
}