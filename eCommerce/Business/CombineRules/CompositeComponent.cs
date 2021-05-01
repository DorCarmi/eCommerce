namespace eCommerce.Business.CombineRules
{
    public interface CompositeComponent<T>
    {
        public T GetResult();
    }
}