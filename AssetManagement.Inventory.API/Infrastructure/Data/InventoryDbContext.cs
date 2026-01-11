using AssetManagement.Inventory.API.Domain.Entities;
using AssetManagement.Inventory.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace AssetManagement.Inventory.API.Infrastructure.Data
{
    public class InventoryDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {
        }

        public DbSet<Area> Areas => Set<Area>();
        public DbSet<Item> Items => Set<Item>();
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<EnvironmentEntity> Environments { get; set; }
        public DbSet<EnvironmentImage> EnvironmentImages { get; set; }
        public DbSet<ProofDocumento> Documents { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Area>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Name)
                      .IsRequired()
                      .HasMaxLength(100);
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.Property(i => i.Name)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.HasOne(i => i.Area)
                      .WithMany(a => a.Items)
                      .HasForeignKey(i => i.AreaId);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.HasOne(r => r.User)
                      .WithMany()
                      .HasForeignKey(r => r.UserId);
            });

        }

    }
}
