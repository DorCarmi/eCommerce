using System;
using eCommerce.Common;

namespace eCommerce.Business.PurchaseRules
{
    public class Age : CompositeRule
    {
        private int age;
        private Compare<double> compare;

        public Age(int age, Compare<double> compare)
        {
            this.age = age;
            this.compare = compare;
        }
        
        public bool Check(IBasket checkItem1, IUser checkItem2)
        {
            return compare.GetResult(this.age, (DateTime.Now.Date - checkItem2.MemberInfo.Birthday.Date).Days);
        }
    }
    
}