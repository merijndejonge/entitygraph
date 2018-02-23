using System;
using System.Linq.Expressions;
using System.Reflection;

namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <summary>
    /// Class that represents a validation rule dependency of a validation rule.
    ///     
    /// Dependencies are expressed in terms of lambda expressions.
    /// </summary>
    public class ValidationRuleDependency
    {
        private LambdaExpression _expression;

        /// <summary>
        /// Gets the validation rule dependency as a Lambda expression.
        /// </summary>
        public LambdaExpression Expression
        {
            get => _expression;
            set
            {
                _expression = value;
                var tail = GetMemberExpression(value.Body);
                TargetProperty = tail.Member as PropertyInfo;
                TargetPropertyOwnerType = tail.Expression.Type;
                ParameterExpression =
                    tail.Aggregate<ParameterExpression>(null, (x, expr) => x ?? expr as ParameterExpression);
            }
        }

        /// <summary>
        /// Method that returns the MemberExpression of an expression, stripping of
        /// any unary expressions that represent type casts.
        /// </summary>
        /// <param name="expr"></param>
        /// <returns></returns>
        private static MemberExpression GetMemberExpression(Expression expr)
        {
            while (true)
            {
                switch (expr)
                {
                    case MemberExpression _:
                        return expr as MemberExpression;
                    case UnaryExpression _:
                        var uexpr = (UnaryExpression) expr;
                        if (uexpr.NodeType == ExpressionType.Convert || uexpr.NodeType == ExpressionType.TypeAs)
                        {
                            expr = uexpr.Operand;
                            continue;
                        }

                        break;
                }

                throw new Exception(expr + " is invalid. Expression should be a property path.");
            }
        }

        /// <summary>
        /// Gets the parameter expression 'A' of a validation rule dependency 'A => A.B.c'.
        /// </summary>
        public ParameterExpression ParameterExpression { get; private set; }

        /// <summary>
        /// Gets the target property 'c' of a validation rule dependency 'A => A.B.c'.
        /// </summary>
        public PropertyInfo TargetProperty { get; private set; }

        /// <summary>
        /// Gets the type of the property 'B' (i.e., the owning property of 'c') of a 
        /// validation rule dependency 'A => A.B.c'.
        /// </summary>
        public Type TargetPropertyOwnerType { get; private set; }

        /// <summary>
        /// Indicates if the the target property 'c' of a validation rule dependency 'A => A.B.c' 
        /// can be invalidated by a validation rule or not.
        /// </summary>
        public bool InputOnly { get; internal set; }
    }
}