namespace eCommerce.Service.StorePolicies
{
    public class SDiscountNode
    {
        public SDiscountNodeType Type { get; set; }

        public SDiscountNode(SDiscountNodeType type)
        {
            Type = type;
        }
    }
}