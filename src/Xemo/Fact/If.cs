using System.Linq.Expressions;

namespace Xemo.Fact;

/// <summary>
/// Simple fact.
/// </summary>
public sealed class If<TContent>(Expression<Func<TContent, bool>> condition) : IFact<TContent>
{
    public bool IsTrue(TContent content) =>
        condition.Compile().Invoke(content);

    public Expression<Func<TContent, bool>> AsExpression() => condition;
}

public static class If
{
    public static If<TContent> True<TContent>(TContent schema, Expression<Func<TContent, bool>> condition)
    {
        return new If<TContent>(condition);
    }

    public static If<TContent> True<TContent>(Expression<Func<TContent, bool>> condition)
    {
        return new If<TContent>(condition);
    }
}