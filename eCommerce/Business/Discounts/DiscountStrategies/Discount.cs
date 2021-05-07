using System;
using System.Collections.Generic;
using eCommerce.Common;

namespace eCommerce.Business
{
    public class Discount
    {
        private int min_amount;
        private double discount;

        public Discount(int amount, double discount)
        {
            this.min_amount = amount;
            this.discount = discount;
        }

        public double CheckDiscount(int amount,double pricePerUnit)
        {
            if (amount < this.min_amount)
            {
                return amount * pricePerUnit;
            }
            else
            {
                return amount * pricePerUnit * discount;
            }
        }
        
        public double CheckDiscountForList(List<Tuple<int,double>> amountsAndPricesPerUnits)
        {
            int totalAmountOfList = 0;
            double currPrice = 0;
            foreach (var amountsAndPricesPerUnit in amountsAndPricesPerUnits)
            {
                totalAmountOfList += amountsAndPricesPerUnit.Item1;
                currPrice += amountsAndPricesPerUnit.Item1 * amountsAndPricesPerUnit.Item2;
            }

            if (totalAmountOfList <= min_amount)
            {
                return currPrice;
            }
            else
            {
                return currPrice * discount;
            }
        }

        public double CalculateAmountForTotalPrice(int amount, double totalPrice)
        {
            if (amount >= this.min_amount)
            {
                return totalPrice * this.discount;
            }
            else
            {
                return totalPrice;
            }
        }

        public int GetAmount()
        {
            return this.min_amount;
        }

        public double GetDiscount()
        {
            return this.discount;
        }
    }
}