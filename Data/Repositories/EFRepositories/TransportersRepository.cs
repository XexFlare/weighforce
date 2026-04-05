using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WeighForce.Data.Repositories;
using WeighForce.Models;

namespace WeighForce.Data.EF
{
    public class TransportersRepository : ITransportersRepository
    {
        private readonly ApplicationDbContext _context;

        public TransportersRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Transporter> GetAllTransporters()
        {
            return _context.Transporter
                .Where(q => !q.IsDeleted)
                .OrderBy(p => p.Name)
                .ToList();
        }

        public Transporter GetTransporter(long id)
        {
            return _context.Transporter.FirstOrDefault(d => d.Id == id);
        }

        public Transporter PostTransporter(Transporter transporter)
        {
            transporter.UpdatedAt = DateTime.UtcNow;
            var newTransporter = _context.Transporter.Add(transporter).Entity;
            _context.SaveChanges();
            return newTransporter;
        }

        public Transporter UpdateTransporter(long id, Transporter updated)
        {
            var cId = _context.Transporter.AsNoTracking().Where(c => c.Id == id).FirstOrDefault()?.cId;
            var SyncVersion = _context.Transporter.AsNoTracking().Where(c => c.Id == id).FirstOrDefault()?.SyncVersion;
            if (cId != null)
            {
                updated.cId = cId ?? 0;
                updated.UpdatedAt = DateTime.UtcNow;
                updated.SyncVersion = 1 + (SyncVersion ?? 1);
            }
            var updatedTransporter = _context.Transporter.Update(updated).Entity;
            _context.SaveChanges();
            return updatedTransporter;
        }
        public bool Delete(long id, string userId)
        {
            var user = _context.ApplicationUser.Find(userId);
            var res = _context.Transporter.FirstOrDefault(d => d.Id == id)?.Delete(user.Email) ?? false;
            _context.SaveChanges();
            return res;
        }
    }
}