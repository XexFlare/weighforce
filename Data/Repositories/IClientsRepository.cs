using System.Collections.Generic;
using WeighForce.Models;

namespace WeighForce.Data.Repositories
{
    public interface IClientsRepository
    {
        IEnumerable<Client> GetAllClients();
        bool Delete(long id, string userId);
        Client GetClient(long id);
    }
}