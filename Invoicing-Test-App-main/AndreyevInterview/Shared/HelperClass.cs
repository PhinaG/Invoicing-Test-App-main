using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace AndreyevInterview.Shared
{
    public class HelperClass(AIDbContext context)
    {
        #region Constractor
        private readonly AIDbContext _context = context;
        #endregion
        public async Task<Client> CreateClient(Client client)
        {
            Client createdClient = null;
            await Task.Run(() =>
            {
                _context.Clients.Add(client);
                _context.SaveChanges();

            });
            createdClient = _context.Clients.Where(x => x.Name == client.Name).FirstOrDefault();
            return createdClient;
        }
    }
}
