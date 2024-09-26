namespace Xemo2;

public interface IOptional<TValue>
{
    void IfHas(Action<TValue> act);
    void IfEmpty(Action act);
}