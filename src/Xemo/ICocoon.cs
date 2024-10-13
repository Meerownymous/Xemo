namespace Xemo;

public interface ICocoon<TContent>
{
    string ID();
    ValueTask<ICocoon<TContent>> Infuse(TContent content);
    ValueTask<ICocoon<TContent>> Infuse(IPatch<TContent> patch);
    ValueTask<TShape> Grow<TShape>(IMorph<TContent, TShape> morph);
    ValueTask Erase();
}