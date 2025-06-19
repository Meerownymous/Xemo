using System.Text;
using Tonga.Text;

namespace Xemo.Azure;

/// <summary>
///     A name encoded so that it can be used as azure blob name.
/// </summary>
public sealed class EncodedBlobName(string origin) : TextEnvelope(
    (() =>
    {
        var bytes = Encoding.UTF8.GetBytes(origin);
        var hexString = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes) hexString.Append(b.ToString("x2")); // Convert byte to hex (two digits)
        return hexString.ToString();
    })
);