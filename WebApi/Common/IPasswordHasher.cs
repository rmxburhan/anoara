using System.IO;
namespace WebApi.Common;

public interface IPasswordHasher
{
    string HashPassword(string Password);
}