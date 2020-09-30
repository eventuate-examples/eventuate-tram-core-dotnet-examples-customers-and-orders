using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.Classes
{
    public class OrderDetails
    {
        [Column("customerId")]
        public long CustomerId { get; set; }
        public Money OrderTotal { get; set; }
        public OrderDetails()
        {
        }
        public OrderDetails(long customerId, Money orderTotal)
        {
            CustomerId = customerId;
            OrderTotal = orderTotal;
        }
    }
}