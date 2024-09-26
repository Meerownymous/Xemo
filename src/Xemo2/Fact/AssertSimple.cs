using System.Linq.Expressions;
using System.Reflection;

namespace Xemo2.Fact;

public sealed class AssertSimple<TContent>(IFact<TContent> origin) : IFact<TContent>
{
    public bool IsTrue(TContent content)
    {
        if (!IsSimpleExpression(origin.AsExpression()))
            throw new ArgumentException(
                $"Facts must be simple expressions. This one is complex: {origin.AsExpression()}"
            );
        return origin.IsTrue(content);
    }

    public Expression<Func<TContent, bool>> AsExpression() => origin.AsExpression();
    
    private static bool IsSimpleExpression(Expression<Func<TContent, bool>> expression)
    {
        return IsSimple(expression.Body);
    }

    private static bool IsSimple(Expression expression)
    {
        if (expression is BinaryExpression binaryExpression)
        {
            // Check if both sides of the binary expression are simple
            return IsSimple(binaryExpression.Left) && IsSimple(binaryExpression.Right) && IsSimpleBinaryOperator(binaryExpression.NodeType);
        }
        else if (expression is MemberExpression memberExpression)
        {
            // Ensure the member expression refers to a property or field, not a method
            return memberExpression.Member.MemberType == MemberTypes.Property || memberExpression.Member.MemberType == MemberTypes.Field;
        }
        else if (expression is ConstantExpression)
        {
            // Constant expressions (e.g., "30" or "John") are simple
            return true;
        }
        else if (expression is UnaryExpression unaryExpression)
        {
            // Handle unary expressions like "Not" (e.g., !someCondition)
            return IsSimple(unaryExpression.Operand);
        }
        else if (expression is ParameterExpression)
        {
            // Parameter (e.g., "c" in "c => c.Age") is simple
            return true;
        }

        // Anything else is considered complex (e.g., method calls like StartsWith)
        return false;
    }

    private static bool IsSimpleBinaryOperator(ExpressionType nodeType)
    {
        // Check if the binary operator is a comparison or logical operator
        return nodeType == ExpressionType.Equal ||
               nodeType == ExpressionType.NotEqual ||
               nodeType == ExpressionType.GreaterThan ||
               nodeType == ExpressionType.LessThan ||
               nodeType == ExpressionType.GreaterThanOrEqual ||
               nodeType == ExpressionType.LessThanOrEqual ||
               nodeType == ExpressionType.AndAlso ||
               nodeType == ExpressionType.OrElse;
    }
}