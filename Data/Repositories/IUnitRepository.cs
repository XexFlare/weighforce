using System.Collections.Generic;
using WeighForce.Models;

namespace WeighForce.Data.Repositories
{
    public interface IUnitRepository
    {
        IEnumerable<Unit> GetAllUnits();
        bool Delete(long id, string userId);
        Unit AddUnit(Unit unit);
        Unit UpdateUnit(long id, Unit unit);
    }
}