using System;
using System.Text;
using Tonga.Text;

namespace Xemo.Azure;

/// <summary>
///     A name encoded so that it can be used as azure blob name.
/// </summary>
public sealed class DecodedBlobName(string encoded) : TextEnvelope(
    AsText._(() =>
    {
        var bytes = new byte[encoded.Length / 2];
        for (var i = 0; i < encoded.Length; i += 2)
            bytes[i / 2] = Convert.ToByte(encoded.Substring(i, 2), 16); // Convert hex to byte
        return Encoding.UTF8.GetString(bytes);
    })
)
{
}