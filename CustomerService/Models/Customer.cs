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
        [NotMapped]
        public Money CreditLimit { get; set; }
        [Column("creationtime")]
        public DateTime CreationTime { get; set; }
        public Customer()
        {
        }
        public Customer(string name, Money creditLimit)
        {
            Name = name;
            CreditLimit = creditLimit;
            CreationTime = System.DateTime.Now;
        }

    }
}
