using System.Security.Cryptography;
using System.Text;

namespace Application.Helpers;

public static class UserHelper
{
    public static string ComputeSha256Hash(string rawData)
    {
        // ComputeHash - returns byte array  
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));

        // Convert byte array to a string   
        var builder = new StringBuilder();
        foreach (var t in bytes)
        {
            builder.Append(t.ToString("x2"));
        }

        return builder.ToString();
    }
}