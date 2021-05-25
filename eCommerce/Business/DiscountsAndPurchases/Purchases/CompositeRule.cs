﻿using System.Collections.Generic;
using eCommerce.Business.CombineRules;
using eCommerce.Common;

namespace eCommerce.Business
{
    public abstract class CompositeRule : Composite
    {

        public override bool CheckIfDiscount()
        {
            return false;
        }

        public override Result<double> Get(IBasket basket, IUser user)
        {
            return Result.Fail<double>("Not a discount, only a rule");
        }

        public override Result<double> GetOneItem(ItemInfo itemInfo, IUser user)
        {
            return Result.Fail<double>("Not a discount");
        }
    }
}