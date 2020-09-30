using IO.Eventuate.Tram.Events.Common;
using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    [Table("Orders")]
    public class Order
    {
        [Column("id")]
        public long Id { get; set; }
        public OrderDetails OrderDetails { get; set; }
        [Column("state")]
        public OrderState State { get; set; }
        [Column("version")]
        public long Version { get; set; }
        public Order()
        {
        }
        public Order(OrderDetails orderDetails)
        {
            OrderDetails = orderDetails;
            State = OrderState.PENDING;
        }

    }
}
