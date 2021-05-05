using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business.CombineRules
{
    public class Min : Composite<double,bool>
    {
        private List<Composite<double, bool>> _composits;
        private Composite<double, bool> _minComposite;
        public Min()
        {
            _composits = new List<Composite<double, bool>>();
        }

        public void Add(Composite<double, bool> composite)
        {
            this._composits.Add(composite);
        }
        public void Calculate(IBasket basket)
        {
            if (_composits.Count == 0)
            {
                return;
            }
            var minRes=FindMin(basket);
            if (minRes.IsSuccess)
            {
                _minComposite = minRes.Value;
                _minComposite.Calculate(basket);
            }
            
        }

        public bool Check(IBasket basket)
        {
            return true;
        }

        public Result<double> Get(IBasket basket)
        {
            if (this._minComposite == null)
            {
                if (_composits.Count == 0)
                {
                    return Result.Fail<double>("No discounts to calculate min from");
                }
                else
                {
                    Calculate(basket);
                    return _minComposite.Get(basket);
                }
            }
            else
            {
                return _minComposite.Get(basket);
            }

        }

        public Result<double> CheckCalculation(IBasket basket)
        {
            if (this._composits.Count == 0)
            {
                return Result.Fail<double>("No discounts to calculate min from");
            }

            var minRes = FindMin(basket);
            if (minRes.IsFailure)
            {
                return Result.Fail<double>(minRes.Error);
            }
            else
            {
                return minRes.Value.Get(basket);
            }
            
        }

        private Result<Composite<double, bool>> FindMin(IBasket basket)
        {
            Composite<double, bool> min_composite = _composits[0];
            double min = 0;
            foreach (var composite in _composits)
            {
                var temp = composite.CheckCalculation(basket);
                if (temp.IsFailure)
                {
                    return Result.Fail<Composite<double, bool>>(temp.Error);
                }
                else
                {
                    if (temp.Value < min)
                    {
                        min_composite = composite;
                    }
                }
            }
            return Result.Ok(min_composite);
        }
    }
}
