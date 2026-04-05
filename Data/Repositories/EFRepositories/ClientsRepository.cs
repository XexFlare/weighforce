using System.Collections.Generic;
using System.Linq;
using WeighForce.Data.Repositories;
using WeighForce.Models;

namespace WeighForce.Data.EF
{
    public class ClientsRepository : IClientsRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Client> GetAllClients()
        {
            return _context.Client
                .Where(q => !q.IsDeleted)
                .ToList();
        }

        public Client GetClient(long id)
        {
            return _context.Client.FirstOrDefault(d => d.Id == id);
        }
        public bool Delete(long id, string userId)
        {
            var user = _context.ApplicationUser.Find(userId);
            var res = _context.Client.FirstOrDefault(d => d.Id == id)?.Delete(user.Email) ?? false;
            _context.SaveChanges();
            return res;
        }
    }
}