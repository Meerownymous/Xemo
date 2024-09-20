using System.Linq.Expressions;
using System.Xml.Linq;
using Xunit;

namespace Xemo2.Fact;

public sealed class If<TContent> : IFact<TContent>
{
    private readonly Func<TContent, bool> condition;

    public If(Func<TContent,bool> condition)
    {
        this.condition = condition;
    }
    public bool IsTrue(TContent content)=> this.condition(content);

    public Expression<Func<TContent, bool>> AsExpression()
    {
        var parameter = Expression.Parameter(typeof(TContent), "x");
        var body = Expression.Invoke(Expression.Constant(this.condition), parameter);
        return Expression.Lambda<Func<TContent, bool>>(body, parameter);
    }
}

public static class If
{
    public static If<TContent> True<TContent>(TContent schema, Func<TContent, bool> condition) => new(condition);
    public static If<TContent> True<TContent>(Func<TContent, bool> condition) => new(condition);
}

public sealed class IfTests
{
    [Fact]
    public void DeliversDelegate()
    {
        Assert.True(
            new If<int>(x => x > 1)
                .AsExpression()
                .Compile()
                .Invoke(2)
        );
    }
}