using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Tonga.Text;

namespace Xemo.Azure;

/// <summary>
/// A name escaped so that it can be used as identifier for azure containers.
/// </summary>
public sealed class EncodedContainerName(string origin) : TextEnvelope(
    AsText._(() => $"{Escaped(origin)}")
)
{
    private static readonly char[] base36Chars = 
        "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();
    private static string Escaped(string subject)
    {
        using SHA256 sha256 = SHA256.Create();
        var legalized = Regex.Replace(subject, @"[^a-zA-Z0-9-]", "-");
        var maxLength = 54;
        legalized = legalized.Length <= maxLength ? legalized : legalized.Substring(0, maxLength);
        return
            Regex.Replace(
                $"{legalized}-{
                    AsBase36(sha256.ComputeHash(Encoding.UTF8.GetBytes(subject)))
                }", 
                "-{2,}", 
                "-"
            );
    }

    private static string AsBase36(byte[] bytes)
    {
        BigInteger bigInt = new BigInteger(bytes.Concat(new byte[] { 0 }).ToArray());
        StringBuilder result = new StringBuilder();
        while (bigInt > 0)
        {
            bigInt = BigInteger.DivRem(bigInt, 36, out BigInteger remainder);
            result.Insert(0, base36Chars[(int)remainder]);
        }
        var hash = result.ToString();
        return hash.Substring(0, Math.Min(hash.Length, 8));
    }
}