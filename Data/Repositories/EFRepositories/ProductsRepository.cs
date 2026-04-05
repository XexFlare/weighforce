using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WeighForce.Data.Repositories;
using WeighForce.Models;

namespace WeighForce.Data.EF
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Product AddProduct(Product product)
        {
            if (_context.Product.Any(o => o.Name == product.Name)) throw new Exception("This product already exists");
            product.Unit = _context.Unit.Find(product.Unit.Id);
            product.UpdatedAt = DateTime.UtcNow;
            var e = _context.Product.Add(product).Entity;
            _context.SaveChanges();
            return e;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Product
                .Include(x => x.Unit)
                .Where(q => !q.IsDeleted)
                .OrderBy(p => p.Name)
                .ToList();
        }

        public Product GetProduct(long id)
        {
            return _context.Product.FirstOrDefault(d => d.Id == id);
        }

        public Product UpdateProduct(long id, Product updated)
        {
            if (_context.Product.Any(o => o.Name == updated.Name && o.Id != id)) throw new Exception("This product already exists");
            var cId = _context.Product.AsNoTracking().Where(c => c.Id == id).FirstOrDefault()?.cId;
            var SyncVersion = _context.Product.AsNoTracking().Where(c => c.Id == id).FirstOrDefault()?.SyncVersion;
            updated.Unit = _context.Unit.Find(updated.Unit.Id);
            updated.UpdatedAt = DateTime.UtcNow;
            if (cId != null)
            {
                updated.cId = cId ?? 0;
                updated.SyncVersion = 1 + (SyncVersion ?? 1);;
            }
            var e = _context.Product.Update(updated).Entity;
            _context.SaveChanges();
            return e;
        }
        public bool Delete(long id, string userId)
        {
            var user = _context.ApplicationUser.Find(userId);
            var res = _context.Product.FirstOrDefault(d => d.Id == id)?.Delete(user.Email) ?? false;
            _context.SaveChanges();
            return res;
        }
    }
}