using WeighForce.Models;
using Microsoft.EntityFrameworkCore;

namespace WeighForce.Data
{
    public class OnTractContext : DbContext
    {
        public OnTractContext(
            DbContextOptions<OnTractContext> options) : base(options)
        {
        }

        public DbSet<OT_Train> Trains { get; set; }
        public DbSet<OT_Site> Sites { get; set; }
        public DbSet<OT_WagonData> WagonData { get; set; }
        public DbSet<Tag> Tag_Data { get; set; }
    }
}
