using System;
using eCommerce.Business.CombineRules;
using eCommerce.Business.DiscountPoliciesCombination;
using eCommerce.Business.Purchases;
using eCommerce.Common;

namespace eCommerce.Business.Discounts
{
    public class DiscountHandler
    {
        public static Result<Composite> HandleDiscount(DiscountInfoNode discountInfoNode)
        {
            return discountInfoNode.handleDiscountInfo();
        }

        public static Result<Composite> Handle(DiscountInfoCompositeNode discountInfoCompositeNode)
        {
            Composite retComp;
            var resA = HandleDiscount(discountInfoCompositeNode.combinationDiscountInfoNodeA);
            if (resA.IsFailure)
            {
                return resA;
            }

            var resB = HandleDiscount(discountInfoCompositeNode.combinationDiscountInfoNodeB);
            if (resB.IsFailure)
            {
                return resB;
            }

            switch (discountInfoCompositeNode._combination)
            {
                case Combinations.AND:
                    retComp = new And(resA.Value, resB.Value);
                    break;
                default:
                    retComp = new DefaultDiscount();
                    break;
            }

            return Result<Composite>.Ok(retComp);
        }

        public static Result<Composite> Handle(DiscountInfoLeaf discountInfoLeaf)
        {
            Composite retComp;
            var rule = RuleHandler.HandleRule(discountInfoLeaf.theRule);
            DiscountComposite discountComposite = new DiscountComposite(rule.Value, discountInfoLeaf.theDiscount);
            retComp = discountComposite;
            return Result.Ok(retComp);
        }
    }
}