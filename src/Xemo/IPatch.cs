namespace Xemo;

public interface IPatch<TContent>
{
    Task<TContent> Patch(TContent content);
}