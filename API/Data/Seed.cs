using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public static class Seed
    {
        // to fetch data from json and put it into db
        public static async Task SeedUser(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            // if there are values don't insert
            if (await userManager.Users.AnyAsync()) return;

            // read data from file
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            // deserialize and map it to a list of AppUsers
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);

            if (users == null) return;

            // seed the role values
            var roles = new List<AppRole>
            {
                new AppRole{Name ="Admin"},
                new AppRole{Name ="Member"},
                new AppRole{Name ="Moderator"}
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }

            // loop through the list and track values
            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower(); // converting to lowercase

                await userManager.CreateAsync(user, "Pa$$w0rd");
                await userManager.AddToRoleAsync(user, "Member");
            }

            // create admin user with Admin, Moderator roles
            var admin = new AppUser
            {
                UserName = "admin"
            };

            await userManager.CreateAsync(admin, "Pa$$w0rd");
            await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
        }
    }
}