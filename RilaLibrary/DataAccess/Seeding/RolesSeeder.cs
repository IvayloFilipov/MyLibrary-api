using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Common.Models.Seeding;

using static Common.GlobalConstants;

namespace DataAccess.Seeding
{
    internal class RolesSeeder : ISeeder
    {
        public async Task SeedAsync(LibraryDbContext dbContext, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await SeedRoleAsync(roleManager, ADMIN_ROLE_NAME);
            await SeedRoleAsync(roleManager, LIBRARIAN_ROLE_NAME);
            await SeedRoleAsync(roleManager, READER_ROLE_NAME);
        }

        private static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);

            if (role == null)
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));

                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
