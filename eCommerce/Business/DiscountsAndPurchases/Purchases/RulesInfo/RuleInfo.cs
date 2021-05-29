using eCommerce.Business.CombineRules;
using eCommerce.Business.DiscountsAndPurchases.Purchases.RulesInfo;

namespace eCommerce.Business.Discounts
{
    public class RuleInfo
    {
        public RuleType RuleType;
        public string WhatIsTheRuleFor;
        public string WhatKind;
        public ItemInfo WhichItems;
        public Comperators Comperators;
        

        public RuleInfo(RuleType ruleType, string whatIsTheRuleFor, string whatKind, ItemInfo whichItems, Comperators comperators)
        {
            RuleType = ruleType;
            WhatIsTheRuleFor = whatIsTheRuleFor;
            WhatKind = whatKind;
            WhichItems = whichItems;
            Comperators = comperators;
        }
        public RuleInfo(RuleType ruleType, string whatIsTheRuleFor, string whatKind, ItemInfo whichItems)
        {
            RuleType = ruleType;
            WhatIsTheRuleFor = whatIsTheRuleFor;
            WhatKind = whatKind;
            WhichItems = whichItems;
        }
        
    }
}