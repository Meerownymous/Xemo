namespace Xemo2;

public interface IPatch<TContent>
{
    Task<TContent> Patch(TContent content);
}