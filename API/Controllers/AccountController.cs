using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<AppUser>> Register(string username, string password)
        {
            // The 'using' is used so that it is disposed after use
            // we use Hmacsha512 algoritham to hash the password before saving it to DB.
            using var hmac = new HMACSHA512();

            var user = new AppUser
            {
                UserName = username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)), // converting password to byte[] and passing it to computeHash to generate the hash using hmac algorithm
                PasswordSalt = hmac.Key // this is the key with which password is being hashed in hmac
            };

            _context.Users.Add(user); // start tracking it
            await _context.SaveChangesAsync(); // save changes to DB
            return user;
        }
    }
}