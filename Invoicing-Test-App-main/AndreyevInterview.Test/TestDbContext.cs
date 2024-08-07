using AndreyevInterview.Models.API;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AndreyevInterview.Test
{
    public class TestDbContext: DbContext
    {

        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
    }
}
