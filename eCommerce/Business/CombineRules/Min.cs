using System.Collections.Generic;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business.CombineRules
{
    public class Min : DiscountComposite
    {
        private List<DiscountComposite> _composits;
        private DiscountComposite _minComposite;
        public Min()
        {
            _composits = new List<DiscountComposite>();
        }

        public void Add(DiscountComposite composite)
        {
            this._composits.Add(composite);
        }
        public void Calculate(IBasket basket,IUser user)
        {
            if (_composits.Count == 0)
            {
                return;
            }
            var minRes=FindMin(basket,user);
            if (minRes.IsSuccess)
            {
                _minComposite = minRes.Value;
                _minComposite.Calculate(basket, user);
            }
            
        }

        public CombinationDiscountInfoNode GetDiscountInfo(IStore store)
        {
            IList<CombinationDiscountInfoNode> nodes = new List<CombinationDiscountInfoNode>();

            foreach (var discountComposite in _composits)
            {
                nodes.Add(discountComposite.GetDiscountInfo(store));
            }

            CombinationDiscountInfoNode combinationDiscountInfo = new CombinationDiscountInfoNode(nodes,Combinations.AND);
            return combinationDiscountInfo;
        }

        public bool Check(IBasket basket, IUser user)
        {
            return this._composits.Count > 0;
        }

        public Result<double> Get(IBasket basket,IUser user)
        {
            if (this._minComposite == null)
            {
                if (_composits.Count == 0)
                {
                    return Result.Fail<double>("No discounts to calculate min from");
                }
                else
                {
                    Calculate(basket,user);
                    return _minComposite.Get(basket,user);
                }
            }
            else
            {
                return _minComposite.Get(basket,user);
            }

        }

        public Result<double> CheckCalculation(IBasket basket,IUser user)
        {
            if (this._composits.Count == 0)
            {
                return Result.Fail<double>("No discounts to calculate min from");
            }

            var minRes = FindMin(basket,user);
            if (minRes.IsFailure)
            {
                return Result.Fail<double>(minRes.Error);
            }
            else
            {
                return minRes.Value.Get(basket,user);
            }
            
        }

        private Result<DiscountComposite> FindMin(IBasket basket,IUser user)
        {
            DiscountComposite min_composite = _composits[0];
            double min = 0;
            foreach (var composite in _composits)
            {
                var temp = composite.CheckCalculation(basket,user);
                if (temp.IsFailure)
                {
                    return Result.Fail<DiscountComposite>(temp.Error);
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
