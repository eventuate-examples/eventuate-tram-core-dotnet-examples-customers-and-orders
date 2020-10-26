using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Models
{
    public class CreditReservation
    {
        [Column("orderId")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long OrderId { get; set; }
        public Money OrderTotal { get; set; }
        [Column("customerId")]
        [ForeignKey("Customer")]
        public long CustomerId { get; set; }
        public CreditReservation()
        {
        }
        public CreditReservation(long customerId, long orderId, Money orderTotal)
        {
            CustomerId = customerId;
            OrderId = orderId;
            OrderTotal = orderTotal;
        }
    }
}
