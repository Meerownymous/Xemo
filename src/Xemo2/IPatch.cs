namespace Xemo2;

public interface IPatch<TContent>
{
    TContent Patch(TContent content);
}