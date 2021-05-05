using System;

namespace eCommerce.Business.CombineRules
{
    public class NotEquals<G> : Compare<G>
    {
        public bool GetResult(G a, G b)
        {
            return !a.Equals(b);
        }
        
        public bool GetResult(DateTime a, DateTime b)
        {
            return a.Date.CompareTo(b.Date) != 0;
        }
    }
    
    
}