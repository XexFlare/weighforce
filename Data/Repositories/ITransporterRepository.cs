using System.Collections.Generic;
using WeighForce.Models;

namespace WeighForce.Data.Repositories
{
    public interface ITransportersRepository
    {
        IEnumerable<Transporter> GetAllTransporters();
        Transporter PostTransporter(Transporter transporter);
        Transporter GetTransporter(long id);
        bool Delete(long id, string userId);
        Transporter UpdateTransporter(long id, Transporter transporter);
    }
}