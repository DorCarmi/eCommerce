using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using eCommerce.Business.CombineRules;
using Rule = System.Data.Rule;

namespace eCommerce.Business.DiscountPoliciesCombination
{
    public class And : CompositeComponent<bool>
    {
        private List<CompositeComponent<bool>> components;

        public And()
        {
            components = new List<CompositeComponent<bool>>();
        }
        

        public bool GetResult()
        {
            if (components.Count == 0)
            {
                return false;
            }
            else
            {
                bool aggregate = components[0].GetResult();
                foreach (var component in components)
                {
                    aggregate &= component.GetResult();
                    if (!aggregate)
                    {
                        break;
                    }
                }

                return aggregate;
            }
        }
    }
}