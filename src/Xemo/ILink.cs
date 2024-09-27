namespace Xemo;

public interface ILink<TContent>
{
    string GrabID(TContent content);
}