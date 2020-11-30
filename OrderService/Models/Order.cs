using IO.Eventuate.Tram.Events.Common;
using OrderService.Classes;
using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Models
{
    [Table("Order")]
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
        public Order NoteCreditReserved()
        {
            this.State = OrderState.APPROVED;
            return this;
        }

        public Order NoteCreditReservationFailed()
        {
            this.State = OrderState.REJECTED;
            return this;
        }
        public Order Cancel()
        {
            switch (State)
            {
                case OrderState.PENDING:
                    throw new PendingOrderCantBeCancelledException();
                case OrderState.APPROVED:
                    this.State = OrderState.CANCELLED;
                    return this;
                default:
                    throw new InvalidOperationException("Can't cancel in this state: " + State);
            }
        }

    }
}
