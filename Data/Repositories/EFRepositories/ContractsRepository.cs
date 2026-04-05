using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WeighForce.Data.Repositories;
using WeighForce.Models;

namespace WeighForce.Data.EF
{
    public class ContractsRepository : IContractsRepository
    {
        private readonly ApplicationDbContext _context;

        public ContractsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Contract CreateContract(Contract contract)
        {
            contract.UpdatedAt = System.DateTime.UtcNow;
            var newContract = _context.Contract.Add(contract).Entity;
            _context.SaveChanges();
            return newContract;
        }

        public IEnumerable<Contract> GetAllContracts()
        {
            return _context.Contract
                .Where(q => !q.IsDeleted)
                .OrderBy(p => p.ContractNumber)
                .ToList();
        }

        public Contract GetContract(long id)
        {
            return _context.Contract.FirstOrDefault(d => d.Id == id);
        }

        public Contract UpdateContract(long id, Contract updated)
        {
            var cId = _context.Contract.AsNoTracking().Where(c => c.Id == id).FirstOrDefault()?.cId;
            var SyncVersion = _context.Contract.AsNoTracking().Where(c => c.Id == id).FirstOrDefault()?.SyncVersion;
            if (cId != null)
            {
                updated.UpdatedAt = System.DateTime.UtcNow;
                updated.cId = cId ?? 0;
                updated.SyncVersion = 1 + (SyncVersion ?? 1);
                var updatedContract = _context.Contract.Update(updated).Entity;
                _context.SaveChanges();
                return updatedContract;
            }
            else return null;
        }
        public bool Delete(long id, string userId)
        {
            var user = _context.ApplicationUser.Find(userId);
            var res = _context.Contract.FirstOrDefault(d => d.Id == id)?.Delete(user.Email) ?? false;
            _context.SaveChanges();
            return res;
        }
    }
}