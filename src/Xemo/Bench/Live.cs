namespace Xemo.Bench
{
    public sealed class Live<TResult>(Func<TResult> func) : ILazy<TResult>
    {
        public TResult Value() => func();

        public static implicit operator TResult(Live<TResult> live) => live.Value();
    }

    public static class Live
    {
        public static Live<TResult> _<TResult>(Func<TResult> make) => new(make);
    }

    public interface ILazy<TResult>
    {
        TResult Value();
    }
}

