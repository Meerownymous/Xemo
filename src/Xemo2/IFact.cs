namespace Xemo2;
using System.Linq.Expressions;

public interface IFact<TContent>
{
    bool IsTrue(TContent content);
    Expression<Func<TContent, bool>> AsExpression();
}