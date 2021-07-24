using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
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
        public TokenService(IConfiguration config)
        {
            // This single key is used for encryption and descyption (Symmetric Encryption)
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            // adding claims to the jwt token
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName) // we use the NameId to store user.UserName
            };

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