using System.Collections.Generic;

namespace eCommerce.Business
{
    public class TotalAmounts : CompositeRule
    {
        private int _totalAmount;
        private Compare _compare;

        public TotalAmounts(int totalAmount,Compare compare)
        {
            _totalAmount = totalAmount;
            _compare = compare;
        }
        

        public override Dictionary<string, ItemInfo> Check(IBasket checkItem1, IUser checkItem2)
        {
            Dictionary<string, ItemInfo> itemsList = new Dictionary<string, ItemInfo>();
            int totalAmountChecked = 0;
            foreach (var item in checkItem1.GetAllItems().Value)
            {
                totalAmountChecked += item.amount;
            }
            var compareAns = _compare.GetResult(_totalAmount, totalAmountChecked);
            if (compareAns > 0)
            {
                foreach (var item in checkItem1.GetAllItems().Value)
                {
                    if (!itemsList.ContainsKey(item.name))
                    {
                        itemsList.Add(item.name,item);
                    }
                }
            }

            return itemsList;
        }

        public override bool CheckOneItem(ItemInfo itemInfo, IUser checkItem2)
        {
            return itemInfo.amount == _totalAmount;
        }
    }
}