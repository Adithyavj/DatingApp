using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // check if user already exists in db
            if (await UserExists(registerDto.Username))
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
            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // get user from db
            var user = await _context.Users
                .SingleOrDefaultAsync(user => user.UserName == loginDto.Username);

            if (user == null)
            {
                return Unauthorized("Invalid username");
            }

            // if user is there in db, we need to compare the password stored in DB to what user has provided
            // so we need to get the hashed version of the user provided password
            // we pass in the passwordSalt stored in db as the key to decrypt it
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid password");
                }
            }

            return new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.Url
            };
        }

        // function to check if the same username already exists in db
        // if it exits return true
        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}