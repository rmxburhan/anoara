using WebApi.Models;

namespace WebApi.Common;

public interface IJwtTokenGenerator
{
    TokenResult GenerateToken(string identifier, string roles);
}