using WeighForce.Models;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System;

namespace WeighForce.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
        public DbSet<Meta> Meta { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<TIChange> TIChange { get; set; }
        public DbSet<Dispatch> Dispatch { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<OfficeUser> OfficeUser { get; set; }
        public DbSet<Office> Office { get; set; }
        public DbSet<Branch> Branch { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Contract> Contract { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<Transporter> Transporter { get; set; }
        public DbSet<TransportInstruction> TransportInstruction { get; set; }
        public DbSet<UserMail> UserMail { get; set; }
        public DbSet<MailingList> MailingList { get; set; }
        public DbSet<OsrData> OsrData { get; set; }
        public DbSet<Weight> Weight { get; set; }
        public DbSet<EventLog> EventLog { get; set; }
        public DbSet<Unit> Unit { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<Dispatch>()
            //     .Property(b => b.TareWeight)
            //     .HasDefaultValue(0);
            // modelBuilder.Entity<Dispatch>()  
            //     .Property(b => b.GrossWeight)
            //     .HasDefaultValue(0);
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(85));
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(85));

            modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(85));

            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));

            modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));

            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(85));

            modelBuilder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<IdentityUserClaim<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(85));
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.Id).HasMaxLength(85));
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(85));
            modelBuilder.Entity<OfficeUser>()
             .HasKey(t => new { t.UserId, t.OfficeId });

            modelBuilder.Entity<OfficeUser>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.OfficeUsers)
                .HasForeignKey(pt => pt.UserId);

            modelBuilder.Entity<Weight>()
                .Property(b => b.Printed)
                .HasDefaultValue(true);

            modelBuilder.Entity<OfficeUser>()
                .HasOne(pt => pt.Office)
                .WithMany(t => t.OfficeUsers)
                .HasForeignKey(pt => pt.OfficeId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
