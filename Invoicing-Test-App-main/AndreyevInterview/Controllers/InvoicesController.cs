#region Namespaces
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using AndreyevInterview.Models.API;
using AndreyevInterview.Shared;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
#endregion

namespace AndreyevInterview.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InvoicesController : ControllerBase
    {
        #region Constractor
        private readonly AIDbContext _context;
        private readonly HelperClass _helperClass;

        public InvoicesController(AIDbContext context)
        {
            _context = context;
            _helperClass = new HelperClass(_context);
        }
        #endregion

        #region All Get API's
        [HttpGet]
        public InvoiceModel GetInvoices()
        {
            List<Invoices> Invoices = new List<Invoices>();
            var invoices = _context.Invoices.ToList();

            foreach (Invoice _invoice in invoices)
            {
                var lineItems = _context.LineItems.AsEnumerable().Where(x => x.InvoiceId == _invoice.Id);
                Invoices invoice = new Invoices
                {
                    Id = _invoice.Id,
                    Description = _invoice.Description,
                    Client = new Client()
                    {
                        Id = _invoice.Id,
                        Name = GetClient(_invoice.ClientId)
                    },
                    CouponCode = _invoice.CouponCode,
                    TotalBillableValue = lineItems.Count() > 0 ? lineItems.Where(x => x.isBillable).Sum(x => x.Cost) : 0,
                    TotalNumberLineItems = lineItems.Count(),
                    TotalValue = lineItems.Count() > 0 ? lineItems.Sum(x => x.Cost) : 0
                };

                Invoices.Add(invoice);
            }

            return new InvoiceModel
            {
                Invoices = Invoices
            };

            //return _context.Invoices.ToList();
        }

        [HttpGet("{id}")]
        public LineItemModel GetInvoiceLineItems(int id)
        {
            var billableInvoices = _context.LineItems.AsEnumerable().Where(x => x.InvoiceId == id && x.isBillable)
                  .GroupBy(r => r.InvoiceId)
                  .Select(a => new
                  {
                      TotalBillableValue = a.Sum(r => r.Cost)
                  }).FirstOrDefault();

            var totalInvoices = _context.LineItems.AsEnumerable().Where(x => x.InvoiceId == id)
                  .GroupBy(r => r.InvoiceId)
                  .Select(a => new
                  {
                      GrandTotal = a.Sum(r => r.Cost)
                  }).FirstOrDefault();

            LineItemModel lineItemModel = new LineItemModel
            {
                LineItem = _context.LineItems.Where(x => x.InvoiceId == id).ToList(),
                GrandTotal = totalInvoices == null ? 0 : totalInvoices.GrandTotal,
                TotalBillableValue = billableInvoices == null ? 0 : billableInvoices.TotalBillableValue
            };

            return lineItemModel;
        }

        #endregion

        #region Create and Update Invoice
        [HttpPut]
        public async Task<Invoice> CreateInvoice(InvoiceInput input)
        {
            var invoice = new Invoice();
            invoice.Description = input.Description;
            int clientId = 0;
            if (!string.IsNullOrEmpty(input.Client))
            {
                var existingClient = _context.Clients.Where(x => x.Name == input.Client).FirstOrDefault();

                if (existingClient == null)
                {
                    var newClient = await _helperClass.CreateClient(new Client()
                    {
                        Name = input.Client
                    });
                    clientId = newClient.Id;
                }
                else
                    clientId = existingClient.Id;
            }
            invoice.ClientId = clientId;
            invoice.CouponCode = input.CouponCode;

            _context.Add(invoice);
            _context.SaveChanges();
            return invoice;
        }

        [HttpPatch("{id}")]
        public async Task<bool> UpdateInvoice([FromRoute] int id, [FromBody] InvoiceInput input)
        {
            bool done = false;

            //check if the client exists
            int clientId = 0;
            if (!string.IsNullOrEmpty(input.Client))
            {
                var existingClient = _context.Clients.Where(x => x.Name == input.Client).FirstOrDefault();

                if (existingClient == null)
                {
                    var newClient = await _helperClass.CreateClient(new Client()
                    {
                        Name = input.Client
                    });
                    clientId = newClient.Id;
                }
                else
                    clientId = existingClient.Id;
            }

            await Task.Run(() =>
            {
                Invoice dbInvoiceItem = _context.Invoices.Find(id);

                if (dbInvoiceItem != null)
                {
                    Invoice lt = dbInvoiceItem;
                    lt.ClientId = !string.IsNullOrEmpty(input.Client) ? clientId : dbInvoiceItem.ClientId;
                    lt.CouponCode = !string.IsNullOrEmpty(input.CouponCode) ? input.CouponCode : dbInvoiceItem.CouponCode;                    
                    lt.Description = !string.IsNullOrEmpty(input.Description) ? input.Description : dbInvoiceItem.Description; 
                    _context.Entry(dbInvoiceItem).CurrentValues.SetValues(lt);

                    if (_context.SaveChanges() == 1)
                    {
                        done = true;
                    }
                }
            });
            return done;
        }
        #endregion

        #region Add and Update Line Items To Invoice
        [HttpPost("{id}")]
        public async Task<LineItem> AddLineItemToInvoice(int id, LineItemInput input)
        {
            var lineItem = new LineItem();
            lineItem.InvoiceId = id;
            lineItem.Description = input.Description;
            lineItem.Quantity = input.Quantity;
            lineItem.Cost = input.Cost;
            lineItem.isBillable = input.isBillable;

            if (input.Id == 0)
            {
                await AddLineItem(lineItem);
            }
            else
            {
                await UpdateLineItem(lineItem);
            }

            //_context.Add(lineItem);
            //_context.SaveChanges();
            return lineItem;
        }

        [HttpPut("Update")]
        public async Task<bool> UpdateBillable(LineItemBillable lineItem)
        {
            return await UpdateLineItem(new LineItem
            {
                InvoiceId = lineItem.InvoiceId,
                isBillable = lineItem.isBillable,
                Id = lineItem.LineItemId
            });
        }

        #endregion

        #region Delete APIs
        //Delete an Invoice
        [HttpDelete("{id}")]
        public async Task<bool> DeleteInvoice([FromRoute] int id)
        {
            return await DeleteInvoiceItem(id);
        }

        //Delete an item from the line of items in the invoice
        [HttpDelete("{id}/items/{itemId}")]
        public async Task<bool> DeleteLineItem([FromRoute] int id, [FromRoute] int itemId)
        {
            return await DeleteLineItem(new LineItem
            {
                InvoiceId = id,
                Id = itemId
            });
        }

        #endregion

        #region Repositories
        public async Task<bool> DeleteInvoiceItem(int invoiceId)
        {
            bool done = false;

            await Task.Run(() =>
            {
                Invoice dbInvoice = _context.Invoices.Find(invoiceId);

                if (dbInvoice != null)
                {

                    _context.Invoices.Remove(dbInvoice);
                    // Save changes to the database
                    if (_context.SaveChanges() == 1)
                    {
                        done = true;
                    }
                }
            });

            return done;
        }

        public async Task<bool> UpdateLineItem(LineItem lineItem)
        {
            bool done = false;

            await Task.Run(() =>
            {
                LineItem dblineItem = _context.LineItems.Find(lineItem.Id);

                if (dblineItem != null)
                {
                    LineItem lt = dblineItem;
                    lt.isBillable = lineItem.isBillable;
                    _context.Entry(dblineItem).CurrentValues.SetValues(lt);

                    if (_context.SaveChanges() == 1)
                    {
                        done = true;
                    }
                }
            });

            return done;
        }

        public async Task<bool> AddLineItem(LineItem lineItem)
        {
            bool done = false;
            await Task.Run(() =>
            {
                _context.LineItems.Add(lineItem);

                if (_context.SaveChanges() == 1)
                {
                    done = true;
                }
            });
            return done;
        }

        public async Task<bool> DeleteLineItem(LineItem lineItem)
        {
            bool done = false;

            await Task.Run(() =>
            {
                LineItem dblineItem = _context.LineItems.Find(lineItem.Id);

                if (dblineItem != null)
                {
                    LineItem lt = dblineItem;

                    _context.LineItems.Remove(lt);

                    // Save changes to the database
                    //_context.SaveChanges();                  

                    if (_context.SaveChanges() == 1)
                    {
                        done = true;
                    }
                }
            });

            return done;
        }

        //public async Task<Client> CreateClient(Client client)
        //{
        //    Client createdClient = null;
        //    await Task.Run(() =>
        //    {
        //        _context.Clients.Add(client);
        //        _context.SaveChanges();

               

        //    }); 
        //    createdClient = _context.Clients.Where(x => x.Name == client.Name).FirstOrDefault();
        //    return createdClient;
        //}

        //public async Task<bool> UpdateInvoiceItem(InvoiceInput input, int invoiceId)
        //{
        //    bool done = false;

        //    //check if the client exists
        //    //var existingClient = _context.Clients.Find(invoice.Client);
        //    int clientId = 0;
        //    if (!string.IsNullOrEmpty(input.Client))
        //    {
        //        var existingClient = _context.Clients.Find(input.Client);

        //        if (existingClient == null)
        //        {
        //            var newClient = await CreateClient(new Client()
        //            {
        //                Name = input.Client
        //            });
        //            clientId = newClient.Id;
        //        }
        //        else
        //            clientId = existingClient.Id;
        //    }

        //    await Task.Run(() =>
        //    {
        //        Invoice dbInvoiceItem = _context.Invoices.Find(invoiceId);

        //        if (dbInvoiceItem != null)
        //        {
        //            Invoice lt = dbInvoiceItem;
        //            lt.CouponCode = !string.IsNullOrEmpty(input.CouponCode) ? input.CouponCode : dbInvoiceItem.CouponCode;
        //            lt.ClientId = dbInvoiceItem.ClientId ?? clientId;
        //            _context.Entry(dbInvoiceItem).CurrentValues.SetValues(lt);

        //            if (_context.SaveChanges() == 1)
        //            {
        //                done = true;
        //            }
        //        }
        //    });

        //    return done;
        //}
        #endregion

        #region Private Methods
        private string GetClient(int? id)
        {
            if (id.HasValue && id > 0)
                return _context.Clients.Find(id)?.Name;
            return null;
        }
        #endregion
    }
}