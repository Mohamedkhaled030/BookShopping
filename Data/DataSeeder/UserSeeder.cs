using bookShoop.Constant;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace bookShoop.Data.DataSeeder
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(UserManager<IdentityUser> _userManager)
        {
            var userCount = await _userManager.Users.CountAsync();
            var defaultUser = new IdentityUser
            {
                UserName = "khaled@gmail.com",
                Email = "khaled@gmail.com",
                PhoneNumber = "123456789",
                EmailConfirmed = true
            };
            if (userCount <= 0)
            {
                var result = await _userManager.CreateAsync(defaultUser, "123456_mM");

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
                //await _userManager.AddToRoleAsync(defaultUser, Roles.User.ToString());
            }
        }
    }
}
