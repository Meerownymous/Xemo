using System;
using System.Linq.Expressions;
using System.Web;
using Tonga.Text;

namespace Xemo.Azure
{
    /// <summary>
    ///     Translates a fact to an Azure tag query.
    /// </summary>
    public sealed class FactAsTagQuery<TInput>(IFact<TInput> fact) : TextEnvelope(
        AsText._(() =>
            TranslateExpressionToAzureQuery(fact.AsExpression())
        )
    )
    {
        private static string TranslateExpressionToAzureQuery(Expression<Func<TInput, bool>> expression)
        {
            return ParseExpression(expression.Body);
        }

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

            if (expression is MemberExpression memberExpression)
            {
                // Handle property access (e.g., c.Name)
                return ParseMemberExpression(memberExpression);
            }

            if (expression is ConstantExpression constantExpression)
            {
                // Handle constant values (e.g., "John", 30)
                return $"'{new EncodedTag(constantExpression.Value.ToString()).AsString()}'"; // Wrap the value in quotes for Azure
            }

            if (expression is UnaryExpression unaryExpression)
            {
                // Handle negation (e.g., !expression)
                if (unaryExpression.NodeType == ExpressionType.Not)
                    return $"NOT({ParseExpression(unaryExpression.Operand)})";
            }

            throw new NotSupportedException($"Unsupported expression type: {expression.NodeType}");
        }

        private static string ParseMemberExpression(MemberExpression memberExpression)
        {
            // If it's accessing a variable (captured from the outer scope), evaluate it
            if (memberExpression.Expression is ConstantExpression)
            {
                // Compile and invoke the expression to get the value of the variable
                var value = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
                return $"'{value}'"; // Return the value wrapped in quotes for Azure
            }

            // Otherwise, it's a property access on the object (like c.Name), so just return the property name
            return memberExpression.Member.Name;
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
}