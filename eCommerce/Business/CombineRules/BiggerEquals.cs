using System;

namespace eCommerce.Business.CombineRules
{
    public class BiggerEquals: Compare<double>
    {
        public bool GetResult(double a, double b)
        {
            return a >= b;
        }
        
        public bool GetResult(DateTime a, DateTime b)
        {
            return a.Date.CompareTo(b.Date) == 1 || a.Date.CompareTo(b.Date) == 0;
        }
    }
}