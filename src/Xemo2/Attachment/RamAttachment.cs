using System.Collections.Concurrent;

namespace Xemo2.Attachment;

public sealed class RamAttachment(string id, ConcurrentDictionary<string, Task<Stream>> memory) : IAttachment
{
    public async Task<TFormat> Render<TFormat>(IRendering<Stream, TFormat> rendering)
    {
        TFormat result = default;
        await memory.AddOrUpdate(
            id, 
            async _ =>
            {
                var content = new MemoryStream();
                result = await rendering.Render(content);
                return content;
            },
            async (_, existing) =>
            {
                result = await rendering.Render(await existing);
                return await existing;
            });
        return result;
    }
        

    public Task<IAttachment> Patch(IPatch<Stream> patch) =>
        Task.Run<IAttachment>(() =>
            {
                memory.AddOrUpdate(
                    id,
                    async _ => await patch.Patch(new MemoryStream()),
                    async (_, existing) => await patch.Patch(await existing)
                );
                return this;
            }
        );
}