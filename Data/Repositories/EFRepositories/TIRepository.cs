using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WeighForce.Data.Repositories;
using WeighForce.Models;

namespace WeighForce.Data.EF
{
    public class TIRepository : ITIsRepository
    {
        private readonly ApplicationDbContext _context;

        public TIRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<TransportInstruction> GetAllTIs()
        {
            return _context.TransportInstruction
              .Include(t => t.Product)
              .Include(t => t.Contract)
              .Include(t => t.FromLocation)
              .Include(t => t.ToLocation)
              .Include(t => t.FromLocation.Country)
              .Include(t => t.ToLocation.Country)
              .Where(q => !q.IsDeleted)
              .OrderBy(p => p.Contract.ContractNumber)
              .ToList();
        }

        public IEnumerable<TransportInstruction> GetLocalTIs(long OfficeId)
        {
            var hasDefault = _context.Meta.FirstOrDefault(x => x.name == "PrefContract:"+OfficeId);
            if(hasDefault?.value != null) {
                var contract = _context.Contract.FirstOrDefault(x => x.Id == long.Parse(hasDefault.value));
                var office = _context.Office.Include(x => x.Unit).FirstOrDefault(x => x.Id == OfficeId);
                var products = _context.Product
                    .Include(x => x.Unit)
                    .Where(x => (x.Unit != null && office.Unit != null && x.Unit.Id == office.Unit.Id) || x.Unit == null)
                    .Where(q => !q.IsDeleted)
                    .ToList();
                return products.Select(x => new TransportInstruction {
                    Id = x.Id,
                    FromLocation = office,
                    ToLocation = new Office { Name = ""},
                    Contract = contract,
                    Product = x,
                })
                .OrderBy(p => p.Product.Name);
            }
            return _context.TransportInstruction
                .Where(t => !t.Closed
                && t.FromLocation.Id == OfficeId
                && t.Contract.ContractNumber != "Pending"
                 && t.Product.Name != "Pending")
              .Include(t => t.Product)
              .Include(t => t.Contract)
              .Include(t => t.FromLocation)
              .Include(t => t.ToLocation)
              .Where(q => !q.IsDeleted)
              .OrderBy(p => p.Product.Name)
              .ToList();
        }
        public IEnumerable<TransportInstruction> GetInboundTIs(long OfficeId)
        {
            var hasDefault = _context.Meta.FirstOrDefault(x => x.name == "PrefContract:"+OfficeId);
            if(hasDefault?.value != null) {
                var contract = _context.Contract.FirstOrDefault(x => x.Id == long.Parse(hasDefault.value));
                var office = _context.Office.Include(x => x.Unit).FirstOrDefault(x => x.Id == OfficeId);
                var products = _context.Product
                    .Include(x => x.Unit)
                    .Where(x => (x.Unit != null && office.Unit != null && x.Unit.Id == office.Unit.Id) || x.Unit == null)
                    .Where(q => !q.IsDeleted)
                    .ToList();
                return products.Select(x => new TransportInstruction {
                    Id = x.Id,
                    ToLocation = office,
                    FromLocation = new Office { Name = ""},
                    Contract = contract,
                    Product = x,
                })
                .OrderBy(p => p.Product.Name);
            }
            return _context.TransportInstruction
                .Where(t => !t.Closed
                && t.ToLocation.Id == OfficeId
                && t.Contract.ContractNumber != "Pending"
                 && t.Product.Name != "Pending")
              .Include(t => t.Product)
              .Include(t => t.Contract)
              .Include(t => t.FromLocation)
              .Include(t => t.ToLocation)
              .Where(q => !q.IsDeleted)
              .OrderBy(p => p.Product.Name)
              .ToList();
        }

        public TransportInstruction GetTI(long id)
        {
            return _context.TransportInstruction
              .Include(t => t.Product)
              .Include(t => t.Contract)
              .Include(t => t.FromLocation)
              .Include(t => t.ToLocation)
            .FirstOrDefault(d => d.Id == id);
        }

        public TransportInstruction PostTI(TransportInstruction ti)
        {

            var newTI = _context.TransportInstruction.Add(new TransportInstruction
            {
                Contract = _context.Contract.Find(ti.Contract.Id),
                Product = _context.Product.Find(ti.Product.Id),
                FromLocation = _context.Office.Find(ti.FromLocation.Id),
                ToLocation = _context.Office.Find(ti.ToLocation.Id),
                UpdatedAt = DateTime.UtcNow
            }).Entity;
            _context.SaveChanges();
            return newTI;
        }

        public TransportInstruction UpdateTI(long id, TransportInstruction updated)
        {
            System.Console.WriteLine(JsonSerializer.Serialize(updated));
            var ti = _context.TransportInstruction.Find(updated.Id);
            ti.Contract = _context.Contract.Find(updated.Contract.Id);
            ti.Product = _context.Product.Find(updated.Product.Id);
            ti.FromLocation = _context.Office.Find(updated.FromLocation.Id);
            ti.ToLocation = _context.Office.Find(updated.ToLocation.Id);
            ti.KineticRef = updated.KineticRef;
            ti.UpdatedAt = DateTime.UtcNow;
            ti.SyncVersion++;
            var updatedTI = _context.TransportInstruction.Update(ti).Entity;
            _context.SaveChanges();
            return updatedTI;
        }
        public TransportInstruction CloseTI(long id)
        {
            var ti = _context.TransportInstruction.Find(id);
            if (ti != null)
            {
                ti.UpdatedAt = DateTime.UtcNow;
                ti.Closed = !ti.Closed;
                ti.SyncVersion++;
                _context.TransportInstruction.Update(ti);
            }
            _context.SaveChanges();
            return ti;
        }
        public bool Delete(long id, string userId)
        {
            var user = _context.ApplicationUser.Find(userId);
            var res = _context.TransportInstruction.FirstOrDefault(d => d.Id == id)?.Delete(user.Email) ?? false;
            _context.SaveChanges();
            return res;
        }
    }
}