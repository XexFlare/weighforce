using System.Collections.Generic;
using WeighForce.Models;

namespace WeighForce.Data.Repositories
{
    public interface ITIsRepository
    {
        IEnumerable<TransportInstruction> GetAllTIs();
        IEnumerable<TransportInstruction> GetLocalTIs(long id);
        IEnumerable<TransportInstruction> GetInboundTIs(long id);
        TransportInstruction PostTI(TransportInstruction ti);
        TransportInstruction GetTI(long id);
        bool Delete(long id, string userId);
        TransportInstruction UpdateTI(long id, TransportInstruction ti);
        TransportInstruction CloseTI(long id);
    }
}