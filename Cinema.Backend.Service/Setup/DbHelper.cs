using Cinema.Backend.Service.Models;
using Cinema.Backend.Service.Models.Core;
using Microsoft.AspNetCore.Identity;
using Template.Service.Configuration;

namespace Cinema.Backend.Service.Setup
{
    public class DbHelper
    {
        public static async Task SeedAdminUser(UserManager<User> userManager, IConfiguration configuration)
        {
            IList<User> adminUsers = await userManager.GetUsersInRoleAsync(Roles.ADMINISTRATOR);

            if (!adminUsers.Any())
            {

                var admin = new User
                {
                    Email = configuration.GetAdminEmail(),
                    UserName = configuration.GetAdminUserName()
                };

                IdentityResult result = await userManager.CreateAsync(admin, configuration.GetAdminPassword());

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, Roles.ADMINISTRATOR);
                }
            }
        }

        public static async Task SeedUserRoles(RoleManager<Role> roleManager)
        {
            bool adminRoleExists = await roleManager.RoleExistsAsync(Roles.ADMINISTRATOR);
            if (!adminRoleExists)
            {
                var adminRole = new Role
                {
                    Name = Roles.ADMINISTRATOR,
                    NormalizedName = Roles.ADMINISTRATOR.ToUpper()
                };

                await roleManager.CreateAsync(adminRole);
            }

            bool registeredUserRoleExists = await roleManager.RoleExistsAsync(Roles.REGISTERED_USER);
            if (!registeredUserRoleExists)
            {
                var registeredUserRole = new Role
                {
                    Name = Roles.REGISTERED_USER,
                    NormalizedName = Roles.REGISTERED_USER.ToUpper()
                };

                await roleManager.CreateAsync(registeredUserRole);
            }
        }
    }
}
