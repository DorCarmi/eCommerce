namespace eCommerce.Business.CombineRules
{
    public interface Rule : CompositeComponent<bool>
    {
        public bool GetResult();
    }
}