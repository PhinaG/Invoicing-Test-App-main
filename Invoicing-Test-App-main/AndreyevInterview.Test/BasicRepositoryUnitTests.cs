using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Xml;
using Xunit;

namespace AndreyevInterview.Test
{
    public class BasicRepositoryUnitTests
    {
        private DbContextOptions<TestDbContext> CreateNewContextOptions() 
        {
            return new DbContextOptionsBuilder<TestDbContext>()
          .UseSqlite("DataSource=:memory:") // Use in-memory SQLite database
          .Options;
        }

        [Fact]
        public void ClientsTest()
        {
            var options = CreateNewContextOptions();

            // Create a new instance of the context with the in-memory database
            using (var context = new TestDbContext(options))
            {
                // Initialize the database and seed data
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                context.Clients.Add(new Client { Id = 1, Name = "Test Client1" });
                context.Clients.Add(new Client { Id = 2, Name = "Test Client2" });
                context.SaveChanges();

                //Performing the test
                var result = context.Clients.ToList();

                Assert.Equal(2, result.Count);
                Assert.Contains(result, e => e.Name == "Test Client1");
                Assert.Contains(result, e => e.Name == "Test Client2");
            }

        }

        [Fact]
        public void InvoicesTest()
        {
            var options = CreateNewContextOptions();

            // Create a new instance of the context with the in-memory database
            using (var context = new TestDbContext(options))
            {
                // Initialize the database and seed data
                context.Database.OpenConnection();
                context.Database.EnsureCreated();

                context.Invoices.Add(new Invoice { Id = 1, Description = "Test Invoice 1" });
                context.Invoices.Add(new Invoice { Id = 2, Description = "Test Invoice 2" });
                context.SaveChanges();

                var result = context.Invoices.ToList();
                var invoice = context.Invoices.Find(2);

                Assert.Equal(2, result.Count);
                Assert.Contains(result, e => e.Description == "Test Invoice 1");
                Assert.Equal("Test Invoice 2", invoice.Description);
            }

            
        }
    }
}
