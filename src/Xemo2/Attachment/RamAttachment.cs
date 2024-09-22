using System.Collections.Concurrent;

namespace Xemo2.Attachment;

public sealed class RamAttachment(string id, ConcurrentDictionary<string, Stream> memory) : IAttachment
{
    public Task<TFormat> Render<TFormat>(IRendering<Stream, TFormat> rendering)
    {
        TFormat result = default;
        memory.AddOrUpdate(
            id, 
            _ =>
            {
                var content = new MemoryStream();
                result = rendering.Render(content);
                return content;
            },
            (_, existing) =>
            {
                result = rendering.Render(existing);
                return existing;
            });
        return Task.FromResult(result);
    }
        

    public Task<IAttachment> Patch(Func<Stream, Stream> patch) =>
        Task.Run<IAttachment>(() =>
            {
                memory.AddOrUpdate(
                    id,
                    _ => patch(new MemoryStream()),
                    (_, existing) => patch(existing)
                );

                return this;
            }
        );

    public Task<IAttachment> Patch(IPatch<Stream> patch) => 
        Patch(patch.Patch);
}