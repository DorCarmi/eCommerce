namespace eCommerce.Business
{
    public interface RuleComposite<T>
    {
        public T GetCombinedResult(T first, T second);

        public bool CheckCondition(T first, T second);
    }
}