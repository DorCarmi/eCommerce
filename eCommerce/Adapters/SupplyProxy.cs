namespace eCommerce.Adapters
{
    public class SupplyProxy : ISupplyAdapter
    {
        private ISupplyAdapter _adapter;

        public SupplyProxy()
        {
            _adapter = null;
        }
    }
}