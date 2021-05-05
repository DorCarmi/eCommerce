namespace eCommerce.Business
{
    public interface Compare<G>
    {
        public bool GetResult(G a, G b);
    }
}