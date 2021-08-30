using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        // a type of encryption where only one key is used to sign and verify the signature of the token.
        // only one key is used to encrypt and decrypt electronic information.
        // asymmetric encryption has 2 keys public and private keys eg: ssl and https
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;
        public TokenService(IConfiguration config, UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            // This single key is used for encryption and descyption (Symmetric Encryption)
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public async Task<string> CreateToken(AppUser user)
        {
            // adding claims to the jwt token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()), // we use the NameId to store user.Id
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName) // we use the UniqueName to store user.userName
            };

            // roles that the user belongs to
            var roles = await _userManager.GetRolesAsync(user);
            // add the role to the list of claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // adding credentials to token
            // we use the symmetric key and use hmacsha512signature to sign the token
            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // describing the token - what goes inside the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = credentials
            };

            // now create a token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // creating token using the descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // returning the writen token using tokenHandler
            return tokenHandler.WriteToken(token);
        }
    }
}