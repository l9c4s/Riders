using System.Security.Cryptography;
using System.Text;

namespace Domain.Password.Utils
{
    public class Utils
    {
        public static string GenerateHash(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool VerifyHash(string password, string hash)
        {
            return GenerateHash(password) == hash;
        }
    }
}
