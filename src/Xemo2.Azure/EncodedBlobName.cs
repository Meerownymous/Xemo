using System.Text;
using Tonga.Text;

namespace Xemo2.Azure;

/// <summary>
/// A name encoded so that it can be used as azure blob name.
/// </summary>
public sealed class EncodedBlobName(string origin) : TextEnvelope(
    AsText._(() =>
    {
        byte[] bytes = Encoding.UTF8.GetBytes(origin);
        StringBuilder hexString = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            hexString.Append(b.ToString("x2"));  // Convert byte to hex (two digits)
        }
        return hexString.ToString();
    })
);