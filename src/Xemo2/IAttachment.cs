using Xemo2.Patch;
using Xemo2.Rendering;

namespace Xemo2;

public interface IAttachment
{
    ValueTask<TFormat> Render<TFormat>(IRendering<Stream, TFormat> rendering);
    ValueTask<IAttachment> Patch(IPatch<Stream> patch);
}

public static class AttachmentExtensions
{
    public static ValueTask<IAttachment> Patch(this IAttachment attachment, Func<Stream, Stream> patch) =>
        attachment.Patch(new AsPatch<Stream>(patch));
    
    public static ValueTask<TFormat> Render<TFormat>(this IAttachment attachment, Func<Stream, TFormat> rendering) =>
        attachment.Render(
            new AsRendering<Stream, TFormat>(
                content => Task.Run(() => rendering(content))
            )
        );
}