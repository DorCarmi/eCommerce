﻿using System;
using System.Reflection.Metadata;
using eCommerce.Business.CombineRules;
using eCommerce.Business.DiscountPoliciesCombination;
using eCommerce.Business.Discounts;
using eCommerce.Business.PurchaseRules;
using eCommerce.Common;

namespace eCommerce.Business.Purchases
{
    public class RuleHandler
    {
         public static Result<Composite> HandleRule(RuleInfoNode ruleInfoNode)
        {
            return ruleInfoNode.handleRuleInfo();
        }

        public static Result<Composite> Handle(RuleInfoNodeComposite ruleInfoCompositeNode)
        {
            Composite retComp;
            var resA = HandleRule(ruleInfoCompositeNode.ruleA);
            if (resA.IsFailure)
            {
                return resA;
            }

            var resB = HandleRule(ruleInfoCompositeNode.ruleB);
            if (resB.IsFailure)
            {
                return resB;
            }

            switch (ruleInfoCompositeNode.combination)
            {
                case Combinations.AND:
                    retComp = new And(resA.Value, resB.Value);
                    break;
                case Combinations.OR:
                    retComp = new Or(resA.Value, resB.Value);
                    break;
                case Combinations.MAX:
                    retComp = new Max(resA.Value, resB.Value);
                    break;
                case Combinations.MIN:
                    retComp = new Min(resA.Value, resB.Value);
                    break;
                case Combinations.NOT:
                    retComp = new Not(resA.Value);
                    break;
                case Combinations.XOR:
                     retComp = new Xor(resA.Value, resB.Value,HandleComperator(ruleInfoCompositeNode.Comperator),
                         ruleInfoCompositeNode.FieldToCompare,ruleInfoCompositeNode.ItemInfoToCompare);
                     break;
                case Combinations.PLUS:
                    retComp = new Plus(resA.Value, resB.Value);
                    break;
                default:
                    retComp = new DefaultDiscount();
                    break;
            }

            return Result<Composite>.Ok(retComp);
        }
        
        public static Result<Composite> Handle(RuleInfoNodeLeaf ruleInfoLeaf)
        {
            Composite retComp;
            var therule = ruleInfoLeaf.rule;
            bool parseAns;
            switch (therule.RuleType)
            {
                case RuleType.Date:
                    
                    DateTime theDate;
                    parseAns = DateTime.TryParse(therule.WhatIsTheRuleFor, out theDate);
                    if (parseAns)
                    {
                        Dates dates = new Dates(theDate, HandleComperator(therule.Comperators));
                        retComp = dates;
                    }
                    else
                    {
                        return Result.Fail<Composite>("Bad date input to discount");
                    }
                    
                    break;
                case RuleType.Amount:
                    
                    int itemAmount;
                    parseAns = int.TryParse(therule.WhatIsTheRuleFor, out itemAmount);
                    if (parseAns)
                    {
                        Amounts amounts = new Amounts(therule.WhichItems,itemAmount, HandleComperator(therule.Comperators));
                        retComp = amounts;
                    }
                    else
                    {
                        return Result.Fail<Composite>("Bad item amount input to discount");
                    }
                    break;
                case RuleType.Total_Amount:
                    
                    int totalAmount;
                    parseAns = int.TryParse(therule.WhatIsTheRuleFor, out totalAmount);
                    if (parseAns)
                    {
                        TotalAmounts totalAmounts = new TotalAmounts(totalAmount, HandleComperator(therule.Comperators));
                        retComp = totalAmounts;
                    }
                    else
                    {
                        return Result.Fail<Composite>("Bad item amount input to discount");
                    }
                    break;
                case RuleType.IsItem:
                    IsItem isItem = new IsItem(therule.WhichItems.name);
                    retComp = isItem;
                    break;
                case RuleType.Age:
                    int theAge;
                    parseAns = Int32.TryParse(therule.WhatIsTheRuleFor, out theAge);
                    if (parseAns)
                    {
                        Ages ages = new Ages(theAge, HandleComperator(therule.Comperators));
                        retComp = ages;
                    }
                    else
                    {
                        return Result.Fail<Composite>("Bad date input to discount");
                    }
                    break;
                case RuleType.Category:
                    if (therule.WhatIsTheRuleFor!=null && therule.WhatIsTheRuleFor.Length>0)
                    {
                        Categories categories = new Categories(therule.WhatIsTheRuleFor);
                        retComp = categories;
                    }
                    else
                    {
                        return Result.Fail<Composite>("Bad date input to discount");
                    }
                    break;
                case RuleType.Default:
                    retComp = new DefaultDiscount();
                    break;

                default:
                    retComp = new DefaultDiscount(); 
                    break;
            }
            return Result.Ok(retComp);
        }

        private static Compare HandleComperator(Comperators theruleComperators)
        {
            Compare retCompare;
            switch (theruleComperators)
            {
                case Comperators.SMALLER:
                    retCompare = new Smaller();
                    break;
                case Comperators.SMALLER_EQUALS:
                    retCompare = new SmallerEquals();
                    break;
                case Comperators.EQUALS:
                    retCompare = new Equals();
                    break;
                case Comperators.BIGGER_EQUALS:
                    retCompare = new BiggerEquals();
                    break;
                case Comperators.BIGGER:
                    retCompare = new Bigger();
                    break;
                default:
                    retCompare = new SmallerEquals();
                    break; 
                    
            }
            return retCompare;
        }
    }
    
}