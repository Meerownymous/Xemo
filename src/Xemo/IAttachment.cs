using System;
using System.IO;
using System.Threading.Tasks;
using Xemo.Patch;
using Xemo.Fabing;

namespace Xemo;

public interface IAttachment
{
    ValueTask<TFormat> Fab<TFormat>(IFabrication<Stream, TFormat> fabrication);
    ValueTask<IAttachment> Patch(IPatch<Stream> patch);
}

public static class AttachmentExtensions
{
    public static ValueTask<IAttachment> Patch(this IAttachment attachment, Func<Stream, Stream> patch)
    {
        return attachment.Patch(new AsPatch<Stream>(patch));
    }

    public static ValueTask<TFormat> Fab<TFormat>(this IAttachment attachment, Func<Stream, TFormat> Fabing)
    {
        return attachment.Fab(
            new AsFabrication<Stream, TFormat>(
                content => Task.Run(() => Fabing(content))
            )
        );
    }
}