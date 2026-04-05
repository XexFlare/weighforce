using System.Collections.Generic;
using System.Linq;
using WeighForce.Data.Repositories;
using WeighForce.Models;

namespace WeighForce.Data.EF
{
    public class MetaRepository : IMetaRepository
    {
        private readonly ApplicationDbContext _context;

        public MetaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

    public bool DeleteMeta(string Name)
    {
        var res = _context.Meta.FirstOrDefault(d => d.name == Name);
        if(res != null){
            _context.Remove(res);
            _context.SaveChanges();
            return true;
        }
        return false;
    }

    public Meta GetMeta(string Name)
    {
      return _context.Meta.FirstOrDefault(d => d.name == Name);
    }

    public Meta SetMeta(Meta meta)
    {
        var createdMeta = _context.Meta.Add(meta).Entity;
        _context.SaveChanges();
        return createdMeta;
    }
  }
}