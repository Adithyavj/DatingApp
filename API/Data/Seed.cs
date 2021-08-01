using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public static class Seed
    {
        // to fetch data from json and put it into db
        public static async Task SeedUser(DataContext context)
        {
            // if there are values don't insert
            if (await context.Users.AnyAsync())
            {
                return;
            }

            // read data from file
            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
            // deserialize and map it to a list of AppUsers
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
            // loop through the list and track values
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();

                user.UserName = user.UserName.ToLower(); // converting to lowercase
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("password")); // generating passwordhash using hardcoded password: "password"
                user.PasswordSalt = hmac.Key; // salt/key of the password

                context.Users.Add(user);                
            }
            // add values to Database
            await context.SaveChangesAsync();
        }
    }
}