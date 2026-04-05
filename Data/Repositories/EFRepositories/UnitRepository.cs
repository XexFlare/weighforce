using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WeighForce.Data.Repositories;
using WeighForce.Models;

namespace WeighForce.Data.EF
{
    public class UnitRepository : IUnitRepository
    {
        private readonly ApplicationDbContext _context;

        public UnitRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Unit AddUnit(Unit unit)
        {
            if (_context.Unit.Any(o => o.Name == unit.Name)) throw new Exception("This Unit already exists");
            unit.UpdatedAt = DateTime.UtcNow;
            var e = _context.Unit.Add(unit).Entity;
            _context.SaveChanges();
            return e;
        }

        public IEnumerable<Unit> GetAllUnits()
        {
            return _context.Unit
                .Where(q => !q.IsDeleted)
                .OrderBy(p => p.Name)
                .ToList();
        }

        public Unit UpdateUnit(long id, Unit updated)
        {
            if (_context.Unit.Any(o => o.Name == updated.Name && o.Id != id)) throw new Exception("This Unit already exists");
            var cId = _context.Unit.AsNoTracking().Where(c => c.Id == id).FirstOrDefault()?.cId;
            var SyncVersion = _context.Unit.AsNoTracking().Where(c => c.Id == id).FirstOrDefault()?.SyncVersion;
            if (cId != null)
            {
                updated.cId = cId ?? 0;
                updated.UpdatedAt = DateTime.UtcNow;
                updated.SyncVersion = 1 + (SyncVersion ?? 1);
                var e = _context.Unit.Update(updated).Entity;
                _context.SaveChanges();
                return e;
            }
            return null;
        }
        public bool Delete(long id, string userId)
        {
            var user = _context.ApplicationUser.Find(userId);
            var res = _context.Unit.FirstOrDefault(d => d.Id == id)?.Delete(user.Email) ?? false;
            _context.SaveChanges();
            return res;
        }
    }
}