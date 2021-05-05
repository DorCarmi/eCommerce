namespace eCommerce.Business.CombineRules
{
    public interface Rule
    {
        public Composite<T,U> Decide<T,U>(Composite<T,U> A, Composite<T,U> B);
    }
}