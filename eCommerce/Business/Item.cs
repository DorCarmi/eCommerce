using System;
using System.Collections.Generic;
using eCommerce.Business.Basics;

namespace eCommerce.Business
{
    public class Item
    {

        private String _name;
        private Store _belongsToStore;
        private Category _category;
        private List<String> _keyWords;
        

        public Item(String name, Category category, Store store)
        {
            this._name = name;
            this._category = category;
            this._belongsToStore = store;
            this._keyWords=new List<string>();
        }

        public String getName()
        {
            return this._name;
        }


        public bool checkResemblance(string searchString)
        {
            if (this._name.Contains(searchString))
            {
                return true;
            }
            else if (_category.getName().Contains(searchString))
            {
                return true;
            }
            else if(this._keyWords.Contains(searchString))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        
        
        
    }
}