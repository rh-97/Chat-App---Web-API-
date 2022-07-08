using System.Security.Cryptography;
using System.Text;

namespace Test.Helpers;

public static class UserHelpers
{


    // A helper function to generate password hash
    public static string GenerateHash(string password)
    {
        var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("Learnathon"));
        var temp = Encoding.UTF8.GetBytes(password);
        var hash = hmac.ComputeHash(temp);
        return BitConverter.ToString(hash, 0, hash.Length).Replace("-", String.Empty);
    }

}
