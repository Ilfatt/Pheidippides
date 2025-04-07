using System.Security.Cryptography;
using System.Text;

namespace Pheidippides.DomainServices.Extensions;

public static class HashExtensions
{
    public static string HashSha256(this string inputString)
    {
        var inputBytes = Encoding.UTF32.GetBytes(inputString);
        var hashedBytes = SHA256.HashData(inputBytes);

        return Convert.ToBase64String(hashedBytes);
    }
}