using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCommon.OrderHistoryTextSearchCommon
{
    public class OrderTextView : TextView
    {
        public static string INDEX = "orders";
        public static string TYPE = "order";
        public string CustomerId { get; set; }
        public string OrderTotal { get; set; }
        public string State { get; set; }
    }
}
