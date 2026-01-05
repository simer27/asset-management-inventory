using AssetManagement.Inventory.API.Domain.Constants;
using AssetManagement.Inventory.API.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace AssetManagement.Inventory.API.Infrastructure.Seed
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var roles = new[] { Roles.Master, Roles.Admin, Roles.User };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new ApplicationRole { Name = role });
                }
            }

            var masterEmail = "admin@gestaopatrimonial.com";

            var masterUser = await userManager.FindByEmailAsync(masterEmail);

            if (masterUser == null)
            {
                masterUser = new ApplicationUser
                {
                    UserName = masterEmail,
                    Email = masterEmail,
                    FullName = "Administrador do Sistema",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(masterUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(masterUser, Roles.Master);
                }
            }
        }
    }
}
