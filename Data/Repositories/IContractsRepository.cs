using System.Collections.Generic;
using WeighForce.Models;

namespace WeighForce.Data.Repositories
{
    public interface IContractsRepository
    {
        IEnumerable<Contract> GetAllContracts();
        Contract GetContract(long id);
        bool Delete(long id, string userId);
        Contract CreateContract(Contract contract);
        Contract UpdateContract(long id, Contract contract);
    }
}