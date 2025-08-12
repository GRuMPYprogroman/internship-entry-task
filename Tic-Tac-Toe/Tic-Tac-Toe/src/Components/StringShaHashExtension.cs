using System.Security.Cryptography;
using System.Text;

namespace Tic_Tac_Toe.Components;

public static class StringShaHashExtension
{
    public static string HashSha256Password(this string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}