using System;
using System.Linq.Expressions;

namespace Xemo;

public interface IFact<TContent>
{
    bool IsTrue(TContent content);
    Expression<Func<TContent, bool>> AsExpression();
}