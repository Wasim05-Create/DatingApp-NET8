using System;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
    {
        var tokenKey = config["TokenKey"]?? throw new Exception("Cannot access tokenKey from appsettings");
        if(tokenKey.Length < 64) throw new Exception("Your tokenKey needs to be longer");

        // Creating key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

        // token is going to contain claims about the users
        var claims = new List<Claim>
        {
            // we are specifying the new claims that we want to add inside our token.
            new(ClaimTypes.NameIdentifier, user.UserName)
        };

        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        // Describing the other elements of our token
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Writing token to the response and returning token
        return tokenHandler.WriteToken(token);
    }
}
