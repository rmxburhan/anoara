using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.OutputCaching;

namespace WebApi.Common;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string Password)
    {
        using (var hmac = SHA256.Create())
        {
            var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(Password));
            var builder = new StringBuilder();

            foreach (var item in bytes)
            {
                builder.Append(item.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}