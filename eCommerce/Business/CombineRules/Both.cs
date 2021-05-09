using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using eCommerce.Business.CombineRules;
using eCommerce.Business.Discounts;
using eCommerce.Business.Service;
using eCommerce.Common;
using Rule = System.Data.Rule;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class Both : DiscountComposite
    {
        private DiscountComposite _A;
        private DiscountComposite _B;
        
        public Both(DiscountComposite A, DiscountComposite B)
        {
            this._A = A;
            this._B = B;
        }
        

        public void Calculate(IBasket basket, IUser user)
        {
            if (Check(basket,user))
            {
                _A.Calculate(basket,user);
                _B.Calculate(basket,user);
            }
        }

        public CombinationDiscountInfoNode GetDiscountInfo(IStore store)
        {
            IList<CombinationDiscountInfoNode> nodes = new List<CombinationDiscountInfoNode>();
            nodes.Add(_A.GetDiscountInfo(store));
            nodes.Add(_B.GetDiscountInfo(store));
            
            CombinationDiscountInfoNode combinationDiscountInfo = new CombinationDiscountInfoNode(nodes,Combinations.AND);
            return combinationDiscountInfo;
        }

        public bool Check(IBasket itemToCheck, IUser user)
        {
            return _A.Check(itemToCheck, user) && _B.Check(itemToCheck,user);
        }
        

        public Result<double> Get(IBasket basket, IUser user)
        {
            Calculate(basket,user);
            return basket.GetTotalPrice();
        }

        public Result<double> CheckCalculation(IBasket basket, IUser user)
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
            var startPoint = ((DiscountComposite)_A).Get(basket,user);
            if (startPoint.IsSuccess)
            {
                if (Check(basket,user))
                {
                    var Adiscounted = _A.CheckCalculation(basket,user);
                    var Bdiscounted = _B.CheckCalculation(basket,user);
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