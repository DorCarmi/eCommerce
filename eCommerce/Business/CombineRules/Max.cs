using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class Max : DiscountComposite
    {
        private List<DiscountComposite> _composits;
        private DiscountComposite _maxComposite;
        public Max()
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
            var maxRes=FindMax(basket,user);
            {
                if (maxRes.IsSuccess)
                {
                    this._maxComposite = maxRes.Value;
                    _maxComposite.Calculate(basket,user);
                }
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
            Calculate(basket,user);
            if (this._maxComposite == null)
            {
                if (_composits.Count == 0)
                {
                    return Result.Fail<double>("No discounts to calculate max from");
                }
                else
                {
                    Calculate(basket,user);
                    return _maxComposite.Get(basket,user);
                }
            }
            else
            {
                return _maxComposite.Get(basket,user);
            }

        }

        public Result<double> CheckCalculation(IBasket basket,IUser user)
        {
            if (this._composits.Count == 0)
            {
                return Result.Fail<double>("No discounts to calculate max from");
            }
            
            var maxRes=FindMax(basket,user);
            if (maxRes.IsSuccess)
            {
                return maxRes.Value.Get(basket,user);
            }
            else
            {
                return Result.Fail<double>(maxRes.Error);
            }
            
        }
        

        private Result<DiscountComposite> FindMax(IBasket basket,IUser user)
        {
            DiscountComposite max_composite = _composits[0];
            double max = 0;
            foreach (var composite in _composits)
            {
                var temp=composite.CheckCalculation(basket,user);
                if (temp.IsFailure)
                {
                    return Result.Fail<DiscountComposite>(temp.Error);
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