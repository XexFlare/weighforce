using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WeighForce.Data.Repositories;
using WeighForce.Models;
namespace WeighForce.Data.EF
{
    public class OfficesRepository : IOfficesRepository
    {
        private readonly ApplicationDbContext _context;

        public OfficesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Office AddOffice(Office office)
        {
            if (_context.Office.Any(o => o.Name == office.Name && o.Country.Id == office.Country.Id)) throw new Exception("This location already exists");
            office.UpdatedAt = DateTime.UtcNow;
            office.Country = _context.Country.Find(office.Country.Id);
            office.Unit = _context.Unit.Find(office.Unit.Id);
            var e = _context.Office.Add(office).Entity;
            _context.SaveChanges();
            return e;
        }

        public IEnumerable<Office> GetAllOffices()
        {
            return _context.Office
                .Where(q => !q.IsDeleted)
                .OrderBy(p => p.Name)
                .Include(o => o.Country)
                .Include(o => o.Unit)
                .ToList();
        }

        public Office GetOffice(long id)
        {
            var office = _context.Office
                .Include(o => o.Country)
                .Include(o => o.Unit)
                .Include(o => o.Branches)
                .FirstOrDefault(d => d.Id == id);
            office.Branches = office.Branches.OrderBy(b => b.Name).ToList();
            return office;
        }

        public IEnumerable<Office> GetUserOffices(string id)
        {
            IEnumerable<Office> offices = _context.ApplicationUser
                .Include(o => o.OfficeUsers)
                .ThenInclude(o => o.Office)
                .FirstOrDefault(d => d.Id == id)
                ?.OfficeUsers
                .Select(of => of.Office);
            return offices;
        }
        public Office UpdateOffice(long id, Office updated)
        {
            if (_context.Office.Any(o => o.Name == updated.Name && o.Country.Id == updated.Country.Id && o.Id != id)) throw new Exception("This location already exists");
            var cId = _context.Office.AsNoTracking().Where(c => c.Id == id).FirstOrDefault()?.cId;
            var SyncVersion = _context.Office.AsNoTracking().Where(c => c.Id == id).FirstOrDefault()?.SyncVersion;
            updated.Country = _context.Country.Find(updated.Country.Id);
            updated.Unit = _context.Unit.Find(updated.Unit.Id);
            updated.UpdatedAt = DateTime.UtcNow;
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            updated.Name = ti.ToTitleCase(updated.Name.ToLower());
            if (cId != null)
            {
                updated.cId = cId ?? 0;
                updated.SyncVersion = 1 + (SyncVersion ?? 1);;
            }
            var e = _context.Office.Update(updated).Entity;
            _context.SaveChanges();
            return e;
        }
        public bool Delete(long id, string userId)
        {
            var user = _context.ApplicationUser.Find(userId);
            var res = _context.Office.FirstOrDefault(d => d.Id == id)?.Delete(user.Email) ?? false;
            _context.SaveChanges();
            return res;
        }
        public Branch AddBranch(Branch branch)
        {
            branch.Office = _context.Office.Find(branch.Office.Id);
            branch.UpdatedAt = DateTime.UtcNow;
            var created = _context.Branch.Add(branch).Entity;
            _context.SaveChanges();
            return created;
        }
        public Branch UpdateBranch(long id, Branch updated)
        {
            var cId = _context.Branch.AsNoTracking().Where(c => c.Id == id).FirstOrDefault()?.cId;
            if (cId != null)
            {
                updated.cId = cId ?? 0;
                updated.UpdatedAt = DateTime.UtcNow;
                updated.SyncVersion++;
                updated.Office = _context.Office.Find(updated.Office.Id);
                var e = _context.Branch.Update(updated).Entity;
                _context.SaveChanges();
                return e;
            }
            return updated;
        }
    }
}