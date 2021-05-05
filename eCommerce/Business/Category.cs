using System;
using System.Collections.Generic;

namespace eCommerce.Business
{
    public class Category
    {
        private static Dictionary<string, double> StoreToDiscount = new Dictionary<string, double>();
        private String name;

        public Category(string name)
        {
            this.name = name;
        }
        public String getName()
        {
            return name;
        }
    }
}