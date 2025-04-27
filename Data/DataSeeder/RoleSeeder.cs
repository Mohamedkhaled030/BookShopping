using bookShoop.Constant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace bookShoop.Data.DataSeeder
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> _RoleManager)
        {
            var Rolecount = await _RoleManager.Roles.CountAsync();
            if (Rolecount <= 0)
            {
                var defaultRole = new IdentityRole()
                {
                    Name = Roles.Admin.ToString(),

                }; var defaultRole2 = new IdentityRole()
                {
                    Name = Roles.User.ToString()

                };
                await _RoleManager.CreateAsync(defaultRole);
                await _RoleManager.CreateAsync(defaultRole2);
            }
        }
    }
}
