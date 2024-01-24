namespace Xemo
{
    public interface IRelation<TTarget>
    {
        string TargetSubject();
        TTarget Target();
        void Link(TTarget target);
        void Unlink(TTarget target);
    }
}