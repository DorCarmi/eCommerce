namespace eCommerce.Business.CombineRules
{
    public interface Rule<G,T>
    {
        public Composite<G,T> Decide<G,T>(Composite<G,T> A, Composite<G,T> B);
    }
}