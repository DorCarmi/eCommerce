using System;

namespace eCommerce.Business.PurchaseRules
{
    public class Date : CompositeRule
    {
        private DateTime _date;
        private Compare<double> _compare;

        public Date(DateTime age, Compare<double> compare)
        {
            this._date = age;
            this._compare = compare;
        }


        public bool Check(DateTime checkItem)
        {
            return this._compare.GetResult(this._date.ToOADate(), this._date.ToOADate());
        }

        public bool Check(IBasket checkItem1, IUser checkItem2)
        {
            return this._compare.GetResult(this._date.Date.ToOADate(), DateTime.Now.Date.ToOADate());
        }
    }
}