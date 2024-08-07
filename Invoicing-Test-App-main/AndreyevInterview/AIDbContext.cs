using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AndreyevInterview
{
    public class AIDbContext : DbContext
    {
        public AIDbContext(DbContextOptions options) : base(options)
        {
        }
        //Phina Added:
        public DbSet<Client> Clients { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<LineItem> LineItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Phina Added
            modelBuilder.Entity<Client>().Property(b => b.Name).IsRequired();
            // Invoice
            modelBuilder.Entity<Invoice>().Property(b => b.Description).IsRequired();

            // Line item
            modelBuilder.Entity<LineItem>().Property(b => b.InvoiceId).IsRequired();
            modelBuilder.Entity<LineItem>().Property(b => b.Quantity).IsRequired();
            modelBuilder.Entity<LineItem>().Property(b => b.Cost).IsRequired();
            modelBuilder.Entity<LineItem>().Property(b => b.isBillable).IsRequired();
        }
    }

    //Phina Added
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Invoice
    {
        public int Id { get; set; } 
        public string Description { get; set; }
        public int? ClientId { get; set; }
        public string CouponCode { get; set; }
    }

    public class LineItem
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string Description { get; set; }

        public int Quantity { get; set; }
        public decimal Cost { get; set; }
        public bool isBillable { get; set; } 

        public Invoice Invoice { get; set; }
    }    
}