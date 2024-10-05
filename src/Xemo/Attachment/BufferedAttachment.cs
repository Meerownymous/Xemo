using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Xemo.Attachment;

/// <summary>
///     Attachment buffered in memory. Patches memory first, then origin.
/// </summary>
public sealed class BufferedAttachment(IAttachment origin) : IAttachment
{
    private readonly IList<Stream> buffer = new List<Stream>();

    public async ValueTask<TFormat> Grow<TFormat>(IMorph<Stream, TFormat> morph)
    {
        if (buffer.Count == 0)
            buffer.Add(
                await origin.Grow<Stream>(s =>
                {
                    var memory = new MemoryStream();
                    s.CopyTo(memory);
                    memory.Seek(0, SeekOrigin.Begin);
                    return memory;
                })
            );
        buffer[0].Seek(0, SeekOrigin.Begin);
        return await morph.Shaped(new NonClosingStream(buffer[0]));
    }

    public async ValueTask<IAttachment> Infuse(IPatch<Stream> patch)
    {
        if (buffer.Count == 1)
        {
            var patched = await patch.Patch(buffer[0]);
            await origin.Patch(_ => patched);
        }
        else
        {
            await origin.Infuse(patch);
        }

        return this;
    }
}