using System.Linq.Expressions;
using System.Xml.Linq;
using Xunit;

namespace Xemo2.Fact;

public sealed class If<TContent> : IFact<TContent>
{
    private readonly Expression<Func<TContent, bool>> condition;

    public If(Expression<Func<TContent,bool>> condition)
    {
        this.condition = condition;
    }
    
    public bool IsTrue(TContent content)=> this.condition.Compile().Invoke(content);

    public Expression<Func<TContent, bool>> AsExpression() => condition;
}

public static class If
{
    public static If<TContent> True<TContent>(TContent schema, Expression<Func<TContent, bool>> condition) => new(condition);
    public static If<TContent> True<TContent>(Expression<Func<TContent, bool>> condition) => new(condition);
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