namespace Xemo2;

public interface IAttachment
{
    TFormat Render<TFormat>(IRendering<Stream, TFormat> rendering);
}