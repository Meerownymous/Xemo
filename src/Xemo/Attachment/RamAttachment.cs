using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace Xemo.Attachment;

public sealed class RamAttachment(string id, ConcurrentDictionary<string, Task<Stream>> memory) : IAttachment
{
    public async ValueTask<TFormat> Fab<TFormat>(IFabrication<Stream, TFormat> fabrication)
    {
        TFormat result = default;
        await memory.AddOrUpdate(
            id,
            async _ =>
            {
                var content = new MemoryStream();
                result = await fabrication.Fabricate(content);
                return content;
            },
            async (_, existing) =>
            {
                result = await fabrication.Fabricate(await existing);
                return await existing;
            });
        return result;
    }


    public async ValueTask<IAttachment> Patch(IPatch<Stream> patch)
    {
        await
            memory.AddOrUpdate(
                id,
                async _ => await patch.Patch(new MemoryStream()),
                async (_, existing) => await patch.Patch(await existing)
            );
        return this;
    }
}