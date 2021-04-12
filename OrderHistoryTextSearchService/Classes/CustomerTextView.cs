using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderHistoryTextSearchService.Classes
{
    public class CustomerTextView : TextView
    {
        public static string INDEX = "customers";
        public static string TYPE = "customer";
        public string Name { get; set; }
        public string CreditLimit { get; set; }
    }
}
