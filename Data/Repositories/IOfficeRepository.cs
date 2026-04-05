using System.Collections.Generic;
using WeighForce.Models;

namespace WeighForce.Data.Repositories
{
    public interface IOfficesRepository
    {
        IEnumerable<Office> GetAllOffices();
        IEnumerable<Office> GetUserOffices(string userId);
        Office AddOffice(Office office);
        Branch AddBranch(Branch branch);
        Branch UpdateBranch(long id, Branch branch);
        Office UpdateOffice(long id, Office office);
        Office GetOffice(long id);
        bool Delete(long id, string userId);
    }
}