using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Common;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public TokenResult GenerateToken(string identifier, string roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JwtSettings:SigningKey").Value));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, identifier),
            new Claim(ClaimTypes.Role, roles)
        };

        var ExpiredTime = DateTime.UtcNow.AddDays(1);
        var token = new JwtSecurityToken(
            issuer: configuration.GetSection("JwtSettings:Issuer").Value,
            audience: configuration.GetSection("JwtSettings:Audience").Value,
            expires: ExpiredTime,
            signingCredentials: signingCredentials,
            claims: claims
        );


        var result = new TokenResult(new JwtSecurityTokenHandler().WriteToken(token), ExpiredTime);
        return result;
    }
}