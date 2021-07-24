using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> Register(RegisterDto registerDto)
        {
            // check if user already exists in db
            if(await UserExists(registerDto.Username))
            {
                // BadRequest is an http statuscode which is accessible due to ActionResult
                return BadRequest("UserName is taken");
            }


            // The 'using' is used so that it is disposed after use
            // we use Hmacsha512 algoritham to hash the password before saving it to DB.
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)), // converting password to byte[] and passing it to computeHash to generate the hash using hmac algorithm
                PasswordSalt = hmac.Key // this is the key with which password is being hashed in hmac
            };

            _context.Users.Add(user); // start tracking it
            await _context.SaveChangesAsync(); // save changes to DB
            return user;
        }

        // function to check if the same username already exists in db
        // if it exits return true
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}