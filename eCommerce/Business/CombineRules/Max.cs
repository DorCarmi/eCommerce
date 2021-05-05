﻿using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Common;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class Max : Composite<double,bool>
    {
        private List<Composite<double, bool>> _composits;
        private Composite<double, bool> _maxComposite;
        public Max()
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
            var maxRes=FindMax(basket);
            {
                if (maxRes.IsSuccess)
                {
                    this._maxComposite = maxRes.Value;
                    _maxComposite.Calculate(basket);
                }
            }
        }
        

        public bool Check(IBasket basket)
        {
            throw new System.NotImplementedException();
        }


        public Result<double> Get(IBasket basket)
        {
            Calculate(basket);
            if (this._maxComposite == null)
            {
                if (_composits.Count == 0)
                {
                    return Result.Fail<double>("No discounts to calculate max from");
                }
                else
                {
                    Calculate(basket);
                    return _maxComposite.Get(basket);
                }
            }
            else
            {
                return _maxComposite.Get(basket);
            }

        }

        public Result<double> CheckCalculation(IBasket basket)
        {
            if (this._composits.Count == 0)
            {
                return Result.Fail<double>("No discounts to calculate max from");
            }
            
            var maxRes=FindMax(basket);
            if (maxRes.IsSuccess)
            {
                return maxRes.Value.Get(basket);
            }
            else
            {
                return Result.Fail<double>(maxRes.Error);
            }
            
        }
        

        private Result<Composite<double, bool>> FindMax(IBasket basket)
        {
            Composite<double, bool> max_composite = _composits[0];
            double max = 0;
            foreach (var composite in _composits)
            {
                var temp=composite.CheckCalculation(basket);
                if (temp.IsFailure)
                {
                    return Result.Fail<Composite<double, bool>>(temp.Error);
                }
                if (temp.Value > max)
                {
                    max_composite = composite;
                }
            }

            return Result.Ok(max_composite);
        }
    }
}