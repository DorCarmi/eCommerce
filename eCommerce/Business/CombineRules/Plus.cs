using System.Collections.Generic;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;

namespace eCommerce.Business.CombineRules
{
    public class Plus : DiscountComposite
    {
        private List<DiscountComposite> _composites;
        public Plus()
        {
            _composites = new List<DiscountComposite>();
        }
        public void Calculate(IBasket basket,IUser user)
        {
            if (_composites.Count > 0)
            {
                foreach (var composite in _composites)
                {
                    if (composite.Check(basket,user))
                    {
                        composite.Calculate(basket,user);
                    }
                }
            }
        }

        public CombinationDiscountInfoNode GetDiscountInfo(IStore store)
        {
            IList<CombinationDiscountInfoNode> nodes = new List<CombinationDiscountInfoNode>();

            foreach (var discountComposite in _composites)
            {
                nodes.Add(discountComposite.GetDiscountInfo(store));
            }

            CombinationDiscountInfoNode combinationDiscountInfo = new CombinationDiscountInfoNode(nodes,Combinations.AND);
            return combinationDiscountInfo;
        }

        public bool Check(IBasket basket, IUser user)
        {
            return true;
        }

        public Result<double> Get(IBasket basket,IUser user)
        {
            Calculate(basket,user);
            if (_composites.Count > 0)
            {
                return _composites[0].Get(basket,user);
            }
            else
            {
                return Result.Fail<double>("No discounts to calculate the plus combine");
            }
        }

        public Result<double> CheckCalculation(IBasket basket,IUser user)
        {
            if (_composites.Count > 0)
            {
                var startPoint = _composites[0].Get(basket,user);
                double stratValue = 0;
                if (startPoint.IsFailure)
                {
                    return startPoint;
                }
                if (Check(basket,user))
                {
                    foreach (var composite in _composites)
                    {
                        if (composite.Check(basket,user))
                        {
                            var res = composite.CheckCalculation(basket,user);
                            if (res.IsFailure)
                            {
                                return res;
                            }
                            var diff = startPoint.Value - composite.CheckCalculation(basket,user).Value;
                            stratValue = stratValue - diff;
                        }
                    }
                    return startPoint;
                }
                else
                {
                    return startPoint;
                }
            }
            else
            {
                return Result.Fail<double>("No discounts to calculate the plus combine");
            }
        }
    }
}