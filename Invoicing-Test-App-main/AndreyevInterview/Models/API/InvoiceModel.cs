using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AndreyevInterview.Models.API
{
    public class InvoiceModel
    {
        public List<Invoices> Invoices { get; set; }
        //public int Id { get; set; }
        //public string Description { get; set; }
        //public decimal TotalBillableValue { get; set; }
    }

    public class Invoices
    {
        public int Id { get; set; }
        public Client Client{ get; set; } //Phina Added
        public string ClientName { get; set; } //Phina Added
        public string CouponCode { get; set; }
        public string Description { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalBillableValue { get; set; }
        public decimal TotalNumberLineItems { get; set; }
    }
}
