namespace Xemo2;

public interface ILink<TContent>
{
    string GrabID(TContent content);
    ICocoon<TContent> Fetch(string id);
}