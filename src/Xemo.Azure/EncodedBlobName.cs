using Tonga.Text;

namespace Xemo.Azure;

/// <summary>
/// A name encoded so that it can be used as azure blob name.
/// </summary>
public sealed class EncodedBlobName(string origin) : TextEnvelope(
    AsText._(() => Encoded(origin))
)
{
    private static string Encoded(string blobName)
    {
        string encodedName = Uri.EscapeDataString(blobName);
        if (encodedName.EndsWith("/") || encodedName.EndsWith("."))
        {
            encodedName = encodedName.TrimEnd('/', '.');
        }
        return encodedName;
    }    
}