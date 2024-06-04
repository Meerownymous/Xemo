namespace Xemo.Bench
{
    public sealed class Live<TResult> : ILazy<TResult>
    {
        private readonly Func<TResult> func;

        public Live(Func<TResult> func)
        {
            this.func = func;
        }

        public TResult Value()
        {
            return this.func();
        }

        public static implicit operator TResult(Live<TResult> live) => live.Value();
    }

    public static class Live
    {
        public static Live<TResult> _<TResult>(Func<TResult> make) => new Live<TResult>(make);
    }

    public interface ILazy<TResult>
    {
        TResult Value();
    }
}

