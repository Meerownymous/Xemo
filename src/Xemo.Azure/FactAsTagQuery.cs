using System.Linq.Expressions;
using Tonga.Text;

namespace Xemo.Azure;

/// <summary>
/// Translates a fact to an azure query.
/// </summary>
public sealed class FactAsTagQuery<TInput>(IFact<TInput> fact) : TextEnvelope(
    AsText._(() =>
        TranslateExpressionToAzureQuery(fact.AsExpression())
    )
)
{
    private static string TranslateExpressionToAzureQuery(Expression<Func<TInput, bool>> expression) =>
        ParseExpression(expression.Body);

    private static string ParseExpression(Expression expression)
    {
        if (expression is BinaryExpression binaryExpression)
        {
            // Handle binary expressions (e.g., ==, !=, >, <, >=, <=, &&, ||)
            var left = ParseExpression(binaryExpression.Left);
            var right = ParseExpression(binaryExpression.Right);
            var op = GetOperator(binaryExpression.NodeType);
            return $"{left} {op} {right}";
        }
        else if (expression is MemberExpression memberExpression)
        {
            // Handle property access (e.g., c.Name)
            return memberExpression.Member.Name;
        }
        else if (expression is ConstantExpression constantExpression)
        {
            // Handle constant values (e.g., "John", 30)
            return $"'{constantExpression.Value}'"; // Wrap the value in quotes for Azure
        }
        else if (expression is UnaryExpression unaryExpression)
        {
            // Handle negation (e.g., !expression)
            if (unaryExpression.NodeType == ExpressionType.Not)
            {
                return $"NOT({ParseExpression(unaryExpression.Operand)})";
            }
        }

        throw new NotSupportedException($"Unsupported expression type: {expression.NodeType}");
    }

    private static string GetOperator(ExpressionType expressionType)
    {
        // Translate C# operators to Azure SQL-like operators
        return expressionType switch
        {
            ExpressionType.Equal => "=",
            ExpressionType.NotEqual => "!=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.AndAlso => "AND",
            ExpressionType.OrElse => "OR",
            _ => throw new InvalidOperationException($"Unsupported operator in fact check: {expressionType}")
        };
    }
}