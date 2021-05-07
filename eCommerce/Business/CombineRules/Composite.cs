using eCommerce.Common;

namespace eCommerce.Business.CombineRules
{
    public interface Composite<G,T>
    {
 

        public bool Check(G checkItem1, T checkItem2);

        
    }
}