using AndreyevInterview.Models.API;
using AndreyevInterview.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AndreyevInterview.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientsController : ControllerBase
    {
        #region Constractor
        private readonly AIDbContext _context;
        private readonly HelperClass _helperClass;

        public ClientsController(AIDbContext context)
        {
            _context = context;
            _helperClass = new HelperClass(_context);
        }
        #endregion

        #region API's
        [HttpGet]
        public ClientModel GetClients()
        {
            List<Client> Clients = new List<Client>();
            var clients = _context.Clients.ToList();

            foreach (Client _client in clients)
            {
                var clientItems = _context.Clients.AsEnumerable().Where(x => x.Id == _client.Id);
                Client client = new Client
                {
                    Id = _client.Id,
                    Name = _client.Name
                };

                Clients.Add(client);
            }

            return new ClientModel
            {
                ClientList = Clients
            };
        }

        [HttpGet("{id}")]
        public async Task<Client> ReadClient([FromRoute] int id)
        {
            Client dbClientItem = null;

            await Task.Run(() =>
            {
                dbClientItem = _context.Clients.Find(id);

               
            });
            return dbClientItem;
        }

        [HttpPost]
        public async Task<string> CreateClient([FromBody] ClientInput input)
        {
            //check if the client exists
            string response = "";
            if (!string.IsNullOrEmpty(input.Name))
            {
                var existingClient = _context.Clients.Where(x => x.Name == input.Name).FirstOrDefault();

                if (existingClient == null)
                {
                    var newClient = await _helperClass.CreateClient(new Client()
                    {
                        Name = input.Name
                    });
                    response = $"New client {input.Name} is created";
                }
                else
                    response = $"New client {input.Name} could not be created as client already exists";
            }
            return response;
        }

        [HttpPatch("{id}")]
        public async Task<bool> UpdateClient([FromRoute] int id, [FromBody] ClientInput input)
        {
            bool done = false;

            await Task.Run(() =>
            {
                Client dbClientItem = _context.Clients.Find(id);

                if (dbClientItem != null)
                {
                    Client lt = dbClientItem;
                    lt.Id = id;
                    lt.Name = !string.IsNullOrEmpty(input.Name) ? input.Name : dbClientItem.Name;
                    _context.Entry(dbClientItem).CurrentValues.SetValues(lt);

                    if (_context.SaveChanges() == 1)
                    {
                        done = true;
                    }
                }
            });
            return done;
        }

        [HttpDelete("{id}")]
        public async Task<bool> DeleteClient([FromRoute] int id)
        {
            return await DeleteClientItem(id);
        }



        #endregion

        #region Repositories
        public async Task<bool> DeleteClientItem(int clientId)
        {
            bool done = false;

            await Task.Run(() =>
            {
                Client dbClient = _context.Clients.Find(clientId);

                if (dbClient != null)
                {

                    _context.Clients.Remove(dbClient);
                    // Save changes to the database
                    if (_context.SaveChanges() == 1)
                    {
                        done = true;
                    }
                }
            });

            return done;
        }
        #endregion
    }
}
