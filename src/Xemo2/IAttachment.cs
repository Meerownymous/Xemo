using Xemo2.Rendering;

namespace Xemo2;

public interface IAttachment
{
    Task<TFormat> Render<TFormat>(IRendering<Stream, TFormat> rendering);
    Task<IAttachment> Patch(Func<Stream, Stream> patch);
    Task<IAttachment> Patch(IPatch<Stream> patch);
}

public static class AttachmentExtensions
{
    public static Task<IAttachment> Patch(this IAttachment attachment, Func<Stream, Stream> patch) =>
        attachment.Patch(patch);
    
    public static Task<TFormat> Render<TFormat>(this IAttachment attachment, Func<Stream, TFormat> rendering) =>
        attachment.Render(new AsRendering<Stream, TFormat>(rendering));
}