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

    public async ValueTask<TFormat> Render<TFormat>(IRendering<Stream, TFormat> rendering)
    {
        if (buffer.Count == 0)
            buffer.Add(
                await origin.Render<Stream>(s =>
                {
                    var memory = new MemoryStream();
                    s.CopyTo(memory);
                    memory.Seek(0, SeekOrigin.Begin);
                    return memory;
                })
            );
        buffer[0].Seek(0, SeekOrigin.Begin);
        return await rendering.Render(new NonClosingStream(buffer[0]));
    }

    public async ValueTask<IAttachment> Patch(IPatch<Stream> patch)
    {
        if (buffer.Count == 1)
        {
            var patched = await patch.Patch(buffer[0]);
            await origin.Patch(_ => patched);
        }
        else
        {
            await origin.Patch(patch);
        }

        return this;
    }
}