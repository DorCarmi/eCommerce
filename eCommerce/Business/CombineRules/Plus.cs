using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business.CombineRules
{
    public class Plus : Composite<double,bool>
    {
        private List<Composite<double, bool>> _composites;
        public Plus()
        {
            _composites = new List<Composite<double, bool>>();
        }
        public void Calculate(IBasket basket)
        {
            if (_composites.Count > 0)
            {
                foreach (var composite in _composites)
                {
                    if (composite.Check(basket))
                    {
                        composite.Calculate(basket);
                    }
                }
            }
        }

        public bool Check(IBasket basket)
        {
            return true;
        }

        public Result<double> Get(IBasket basket)
        {
            Calculate(basket);
            if (_composites.Count > 0)
            {
                return _composites[0].Get(basket);
            }
            else
            {
                return Result.Fail<double>("No discounts to calculate the plus combine");
            }
        }

        public Result<double> CheckCalculation(IBasket basket)
        {
            if (_composites.Count > 0)
            {
                var startPoint = _composites[0].Get(basket);
                double stratValue = 0;
                if (startPoint.IsFailure)
                {
                    return startPoint;
                }
                if (Check(basket))
                {
                    foreach (var composite in _composites)
                    {
                        if (composite.Check(basket))
                        {
                            var res = composite.CheckCalculation(basket);
                            if (res.IsFailure)
                            {
                                return res;
                            }
                            var diff = startPoint.Value - composite.CheckCalculation(basket).Value;
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