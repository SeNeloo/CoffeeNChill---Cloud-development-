using System;
using System.Collections.Generic;

namespace CoffeeNChill.Models
{
    public class Order
    {
        public string OrderId { get; set; }

        public string CustomerName { get; set; }

        public List<string> SelectedItemSKUs { get; set; }

        public double TotalPrice { get; set; }

        public DateTime OrderTimestamp { get; set; }

        public string OrderDate { get; set; }

        public string Status { get; set; }
    }
}
