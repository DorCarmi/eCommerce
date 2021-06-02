using eCommerce.Business;
using eCommerce.Business.CombineRules;

namespace eCommerce.Service.StorePolicies
{
    public class SRuleInfo
    {
        public RuleType RuleType { get; set; }
        public string WhatIsTheRuleFor { get; set; }
        public string Kind { get; set; }
        public string ItemId { get; set; }
        public Comperators Comperator { get; set; }
    }
}