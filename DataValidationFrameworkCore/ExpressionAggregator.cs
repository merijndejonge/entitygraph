using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <summary>
    /// Extension class that provides an aggregate method on linq expressions
    /// </summary>
    public static class ExpressionAggregator
    {
        /// <summary>
        ///     Applies an accumulator function over a Linq expression. The specified seed value
        ///     is used as the initial accumulator value.
        /// </summary>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <param name="expr"></param>
        /// <param name="seed"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TAccumulate Aggregate<TAccumulate>(this Expression expr, TAccumulate seed,
            Func<TAccumulate, Expression, TAccumulate> func)
        {
            switch (expr)
            {
                case ConstantExpression _:
                    return func(seed, expr);
                case ParameterExpression _:
                    return func(seed, expr);
                case UnaryExpression _:
                    var uexpr = (UnaryExpression) expr;
                    if (uexpr.NodeType == ExpressionType.Convert || uexpr.NodeType == ExpressionType.TypeAs)
                    {
                        return Aggregate(uexpr.Operand, seed, func);
                    }
                    else
                    {
                        throw new Exception(expr + " is not a valid lambda expression for an EnumerationBinding.");
                    }
                case MemberExpression _:
                    var memberExpression = expr as MemberExpression;
                    if (memberExpression?.Member is PropertyInfo)
                    {
                        return func(Aggregate(memberExpression.Expression, seed, func), expr);
                    }

                    break;
            }

            throw new Exception("Unsupported expression type: " + expr.GetType().Name);
        }
    }
}