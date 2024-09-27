using System;
using System.IO;
using System.Threading.Tasks;
using Xemo.Patch;
using Xemo.Rendering;

namespace Xemo;

public interface IAttachment
{
    ValueTask<TFormat> Render<TFormat>(IRendering<Stream, TFormat> rendering);
    ValueTask<IAttachment> Patch(IPatch<Stream> patch);
}

public static class AttachmentExtensions
{
    public static ValueTask<IAttachment> Patch(this IAttachment attachment, Func<Stream, Stream> patch)
    {
        return attachment.Patch(new AsPatch<Stream>(patch));
    }

    public static ValueTask<TFormat> Render<TFormat>(this IAttachment attachment, Func<Stream, TFormat> rendering)
    {
        return attachment.Render(
            new AsRendering<Stream, TFormat>(
                content => Task.Run(() => rendering(content))
            )
        );
    }
}