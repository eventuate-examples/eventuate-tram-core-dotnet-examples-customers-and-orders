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
        public long id { get; set; }
        public string name { get; set; }
        [NotMapped]
        public Money creditlimit { get; set; }
        public DateTime creationtime { get; set; }
        public Customer()
        {
        }
        public Customer(String _name, Money _creditLimit)
        {
            name = _name;
            creditlimit = _creditLimit;
            creationtime = System.DateTime.Now;
        }

    }
}
