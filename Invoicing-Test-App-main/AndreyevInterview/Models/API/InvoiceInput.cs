using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AndreyevInterview.Models.API
{
    public class InvoiceInput
    {
        public string Description { get; set; }
        //Phina Added:
        public string Client { get; set; }
        public string CouponCode { get; set; }
    }
}
