using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WeighForce.Data.Repositories;
using WeighForce.Models;

namespace WeighForce.Data.EF
{
    public class CountriesRepository : ICountriesRepository
    {
        private readonly ApplicationDbContext _context;

        public CountriesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Country AddCountry(Country country)
        {
            country.UpdatedAt = System.DateTime.UtcNow;
            var e = _context.Country.Add(country).Entity;
            _context.SaveChanges();
            return e;
        }

        public IEnumerable<Country> GetAllCountries()
        {
            return _context.Country
                .Where(q => !q.IsDeleted)
                .OrderBy(p => p.Name)
                .ToList();
        }

        public Country GetCountry(long id)
        {
            return _context.Country.FirstOrDefault(d => d.Id == id);
        }

        public Country UpdateCountry(Country updated)
        {
            var cId = _context.Country.AsNoTracking().Where(c => c.Id == updated.Id).FirstOrDefault()?.cId;
            var SyncVersion = _context.Country.AsNoTracking().Where(c => c.Id == updated.Id).FirstOrDefault()?.SyncVersion;
            if (cId != null)
            {
                updated.cId = cId ?? 0;
                updated.UpdatedAt = System.DateTime.UtcNow;
                updated.SyncVersion = 1 + (SyncVersion ?? 1);
            }
            var e = _context.Country.Update(updated).Entity;
            _context.SaveChanges();
            return e;
        }
        public bool Delete(long id, string userId)
        {
            var user = _context.ApplicationUser.Find(userId);
            var res = _context.Country.FirstOrDefault(d => d.Id == id)?.Delete(user.Email) ?? false;
            _context.SaveChanges();
            return res;
        }
    }
}