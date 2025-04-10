namespace health_app_backend.Helpers;

using System.Security.Cryptography;
using System.Text;

public static class HashingUtils
{
    public static string ComputeSha256Hash(string rawData)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

            StringBuilder builder = new StringBuilder();
            foreach (var t in bytes)
            {
                builder.Append(t.ToString("x2")); // Convert to lowercase hex
            }
            return builder.ToString();
        }
    }
    
    public static byte[] HexStringToByteArray(string hex)
    {
        if (hex.StartsWith("0x"))
            hex = hex.Substring(2);

        return Enumerable.Range(0, hex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();
    }
}