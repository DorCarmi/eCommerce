using System;

namespace eCommerce.Business
{
    public class Category
    {
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