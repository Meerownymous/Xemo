using Xemo;
using Xemo.Morph;
using Xemo.Patch;

// ReSharper disable once CheckNamespace
public static class AttachmentSmarts
{
    public static ValueTask<IAttachment> Patch(this IAttachment attachment, Func<Stream, Stream> patch)
    {
        return attachment.Infuse(new AsPatch<Stream>(patch));
    }

    public static ValueTask<TFormat> Grow<TFormat>(this IAttachment attachment, Func<Stream, TFormat> morph)
    {
        return attachment.Grow(
            new AsMorph<Stream, TFormat>(
                content => Task.Run(() => morph(content))
            )
        );
    }
}