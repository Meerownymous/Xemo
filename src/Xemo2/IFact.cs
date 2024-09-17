namespace Xemo2;

public interface IFact<TContent>
{
    bool IsTrue(TContent content);
}