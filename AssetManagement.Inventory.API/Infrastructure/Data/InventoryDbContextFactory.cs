using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Inventory.API.Infrastructure.Data
{
    public class InventoryDbContextFactory
       : IDesignTimeDbContextFactory<InventoryDbContext>
    {
        public InventoryDbContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
            optionsBuilder.UseNpgsql(
                config.GetConnectionString("DefaultConnection"));

            return new InventoryDbContext(optionsBuilder.Options);
        }
    }
}
