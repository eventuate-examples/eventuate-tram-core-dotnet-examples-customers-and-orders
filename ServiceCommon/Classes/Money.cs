using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Classes
{
    public class Money
    {
        public static Money ZERO = new Money(0);
        public decimal amount { get; set; }

        public Money()
        {
        }

        public Money(int i)
        {
            amount = Convert.ToDecimal(i);
        }
        public Money(String s)
        {
            amount = Convert.ToDecimal(s);
        }

        public Money(decimal _amount)
        {
            amount = _amount;
        }

        public decimal getAmount()
        {
            return amount;
        }

        public void setAmount(decimal _amount)
        {
            this.amount = _amount;
        }

        public bool isGreaterThanOrEqual(Money other)
        {
            return amount >= other.amount;
        }

        public Money add(Money other)
        {
            return new Money(amount + (other.amount));
        }
        public Money subtract(Money other)
        {
            return new Money(amount - (other.amount));
        }
    }
}