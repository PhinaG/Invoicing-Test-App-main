using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AndreyevInterview.Models.API
{
    public class LineItemModel
    {
        public List<LineItem> LineItem { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TotalBillableValue { get; set; }
    }
}
