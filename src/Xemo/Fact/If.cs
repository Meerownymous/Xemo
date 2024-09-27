using System;
using System.Linq.Expressions;

namespace Xemo.Fact;

public sealed class If<TContent> : IFact<TContent>
{
    private readonly Expression<Func<TContent, bool>> condition;

    public If(Expression<Func<TContent, bool>> condition)
    {
        this.condition = condition;
    }

    public bool IsTrue(TContent content)
    {
        return condition.Compile().Invoke(content);
    }

    public Expression<Func<TContent, bool>> AsExpression()
    {
        return condition;
    }
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