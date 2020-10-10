using CustomerService.Classes;
using IO.Eventuate.Tram.Events.Common;
using ServiceCommon.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerService.Models
{
    [Table("Customer")]
    public class Customer
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        public Money CreditLimit { get; set; }
        [Column("creationtime")]
        public DateTime CreationTime { get; set; }
        public ICollection<CreditReservation> CreditReservations { get; set; }
        public Customer()
        {
        }
        public Customer(string name, Money creditLimit)
        {
            Name = name;
            CreditLimit = creditLimit;
            CreationTime = System.DateTime.Now;
        }
        public ResultsWithEvents Create(String name, Money creditLimit)
        {
            Customer customer = new Customer(name, creditLimit);
            var customerCreatedEvent = new CustomerCreatedEvent(customer.Name, customer.CreditLimit);
            List<IDomainEvent> eventList = new List<IDomainEvent>();
            eventList.Add(customerCreatedEvent);
            return new ResultsWithEvents(customer, eventList);
        }
        Money AvailableCredit()
        {
            return new Money(CreditLimit.Amount - (CreditReservations != null ? CreditReservations.Sum(x => x.OrderTotal.Amount) : 0));
        }
        public CreditReservation ReserveCredit(long orderId, Money orderTotal)
        {
            if (AvailableCredit().IsGreaterThanOrEqual(orderTotal))
            {
                return new CreditReservation(Id, orderId, orderTotal);
            }
            else
            {
                throw new CustomerCreditLimitExceededException();
            }
        }
    }
}
