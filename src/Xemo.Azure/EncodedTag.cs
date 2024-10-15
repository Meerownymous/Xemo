using System.Text;
using System.Text.RegularExpressions;
using Tonga.Text;

namespace Xemo.Azure;

/// <summary>
/// String encoded so that it can be used in azure blob tags.
/// </summary>
/// <param name="source"></param>
public sealed class EncodedTag(string source) : TextEnvelope(
    new AsText(() =>
        {
            var validChars = new Regex(@"^[a-zA-Z0-9\-_:.\+=]+$", RegexOptions.Compiled);
            StringBuilder encoded = new StringBuilder();
            foreach (char c in source)
            {
                // If character is valid, keep it as-is, otherwise encode
                if (validChars.IsMatch(c.ToString()))
                {
                    encoded.Append(c);
                }
                else
                {
                    // Convert the character to hex format using an underscore as prefix: _XX
                    encoded.Append($"_{(int)c:X2}");
                }
            }
            return encoded.ToString();
        }
    )
);