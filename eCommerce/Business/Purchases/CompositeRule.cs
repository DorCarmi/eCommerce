﻿using eCommerce.Business.CombineRules;

namespace eCommerce.Business
{
    public interface CompositeRule : Composite<IBasket,IUser>
    {
        
    }
}