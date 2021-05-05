using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using eCommerce.Business.CombineRules;
using eCommerce.Common;
using Rule = System.Data.Rule;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class And<G> : Composite<double,bool,G>
    {
        private Composite<double,bool,G> _A;
        private Composite<double,bool,G> _B;
        
        public And(Composite<double,bool,G> A, Composite<double,bool,G> B)
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

        public bool Check(G itemToCheck)
        {
            return _A.Check(itemToCheck) && _B.Check(itemToCheck)
        }
        

        public Result<double> Get(IBasket basket)
        {
            Calculate(basket);
            return basket.GetTotalPrice();
        }

        public Result<double> CheckCalculation(IBasket basket)
        {
            DiscountComposite discA;
            DiscountComposite discB;
            try
            {
                discA = (DiscountComposite) _A;
                discB = (DiscountComposite) _B;
            }
            catch (Exception ex)
            {
                return Result.Fail<double>("Problem converting rule to discount");
            }
            var startPoint = ((DiscountComposite)_A).Get(basket);
            if (startPoint.IsSuccess)
            {
                if (Check(basket))
                {
                    var Adiscounted = _A.CheckCalculation(basket);
                    var Bdiscounted = _B.CheckCalculation(basket);
                    if (Adiscounted.IsSuccess && Bdiscounted.IsSuccess)
                    {
                        var Adiff = startPoint.Value - Adiscounted.Value;
                        var Bdiff = startPoint.Value - Bdiscounted.Value;
                        return Result.Ok(startPoint.Value - Adiff - Bdiff);
                    }
                    else
                    {
                        return Adiscounted.IsFailure ? Adiscounted : Bdiscounted;
                    }
                }
                else
                {
                    return startPoint;
                }
            }
            else
            {
                return startPoint;
            }
        }
    }
}