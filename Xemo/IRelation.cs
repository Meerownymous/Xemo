namespace Xemo
{
    public interface IRelation<TTarget>
    {
        TTarget Target();
        void Link(TTarget target);
        void Unlink(TTarget target);
    }
}