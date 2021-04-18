using System.Collections.Generic;

namespace eCommerce.Business.Service
{
    public class CartDto
    {
        public IList<BasketDto> Baskets { get; }

        public CartDto(IList<BasketDto> baskets)
        {
            Baskets = baskets;
        }
    }
}