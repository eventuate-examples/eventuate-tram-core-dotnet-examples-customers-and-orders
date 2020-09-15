using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Classes
{
    public class Money
    {
        public static Money ZERO = new Money(0);
        public decimal Amount { get; set; }

        public Money()
        {
        }

        public Money(int i)
        {
            Amount = Convert.ToDecimal(i);
        }
        public Money(String s)
        {
            Amount = Convert.ToDecimal(s);
        }

        public Money(decimal amount)
        {
            Amount = amount;
        }
        public bool IsGreaterThanOrEqual(Money other)
        {
            return Amount >= other.Amount;
        }

        public Money Add(Money other)
        {
            return new Money(Amount + (other.Amount));
        }
        public Money Subtract(Money other)
        {
            return new Money(Amount - (other.Amount));
        }
    }
}