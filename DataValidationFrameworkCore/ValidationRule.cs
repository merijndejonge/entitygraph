using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <summary>
    /// Class that forms the abstract base for all cross-entity validation rules.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public abstract class ValidationRule<TResult> where TResult : class
    {
        /// <summary>
        /// Gets the signature of the validation rule
        /// </summary>
        public Signature Signature { get; }

        /// <summary>
        /// Holds the MethodInfo for the validation method of this validation rule.
        /// </summary>
        protected MethodInfo ValidationMethod { get; }

        /// <summary>
        /// Creates a new instance of the ValidationRule class.
        /// </summary>
        /// <param name="signature"></param>
        protected ValidationRule(params ValidationRuleDependency[] signature)
        {
            Signature = new Signature(signature);
            ValidationMethod = GetValidateMethod();
        }

        /// <summary>
        /// Gets the list of dependency rule parameters of this validation rule.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<ParameterObjectBinding> GetValidationRuleDependencyParameters()
        {
            var oBindings = from dependency in Signature
                select new ParameterObjectBinding
                {
                    ParameterName = dependency.ParameterExpression.Name,
                    ParameterObjectType = dependency.ParameterExpression.Type
                };
            return oBindings.Distinct();
        }

        /// <summary>
        /// Method that checks if bindings are correct for the validation method of this Validation rule
        /// and then invokes teh validation method.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        internal TResult Evaluate(RuleBinding<TResult> binding)
        {
            var bindings = MakeArgumentBindings(binding);
            var result = ValidationMethod.Invoke(binding.ValidationRule, bindings);
            return (TResult) result;
        }

        /// <summary>
        /// Returns an array of objects that form the actual parameter bindings
        /// for the validation method call.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        internal object[] MakeArgumentBindings(RuleBinding<TResult> binding)
        {
            var type = GetType();

            if (Signature.Count != binding.DependencyBindings.Length)
            {
                var msg = string.Format(
                    @"Argument count mismatch between Signature and rule method ""{0}"" in class {1}.",
                    ValidationMethod.Name, type.Name);
                throw new Exception(msg);
            }

            var bindings = new object[Signature.Count];
            for (var i = 0; i < Signature.Count; i++)
            {
                var dBinding = binding.DependencyBindings[i];
                bindings[i] =
                    dBinding.ValidationRuleDependency.TargetProperty.GetValue(dBinding.TargetOwnerObject, null);
            }

            return bindings;
        }

        /// <summary>
        /// This method tries to find a method which has the name "Validate" (or is annotated
        /// with the "Validate" attribute) and for which the signature matches the given signature.
        /// </summary>
        /// <returns></returns>
        private MethodInfo GetValidateMethod()
        {
            var type = GetType();

            var validators = (from m in type.GetMethods()
                where
                    m.IsDefined(typeof(ValidateMethodAttribute), true)
                select m).ToArray();
            if (!validators.Any())
            {
                validators = type.GetMethods().Where(m => m.Name.StartsWith("Validate")).ToArray();
            }

            var methods = GetMatchingValidationMethods(validators.ToArray(), Signature);
            if (methods.Length > 1)
            {
                var msg = string.Format("Only one method in class {0} can be decorated with the 'Validator' attribute.",
                    type.Name);
                throw new Exception(msg);
            }

            if (methods.Length != 0) return methods.Single();
            {
                var msg = string.Format("No validation method could be found in {0} that matches the signature.",
                    type.Name);
                throw new Exception(msg);
            }
        }

        /// <summary>
        /// This method filters the array of validation methods of this class for 
        /// methods that match the given signature.
        /// </summary>
        /// <param name="methods"></param>
        /// <param name="signature"></param>
        /// <returns></returns>
        private MethodInfo[] GetMatchingValidationMethods(IEnumerable<MethodInfo> methods, Signature signature)
        {
            var matchingMethods = new List<MethodInfo>();
            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                if (parameters.Length != signature.Count())
                {
                    continue;
                }

                var requiredReturnType = typeof(TResult);
                if (this is AsyncValidationRule<TResult>)
                {
                    requiredReturnType = typeof(ValidationOperation<TResult>);
                }

                if (requiredReturnType.IsAssignableFrom(method.ReturnType) == false)
                {
                    continue;
                }

                var tuples = parameters.Zip(signature, (p, a) => new {p.ParameterType, ArgumentType = a});
                if (tuples.All(t => t.ParameterType.IsAssignableFrom(t.ArgumentType.TargetProperty.PropertyType)))
                {
                    matchingMethods.Add(method);
                }
            }

            return matchingMethods.ToArray();
        }

        /// <summary>
        /// Creates an InputOutput dependency parameter 'A => A.some.path.i'. The target property
        /// 'i' will be invalidated in case of validation errors.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public static ValidationRuleDependency InputOutput<TSource, TTarget>(
            Expression<Func<TSource, TTarget>> dependency)
        {
            return new ValidationRuleDependency
            {
                Expression = dependency
            };
        }

        /// <summary>
        /// Creates an InputOnly dependency parameter 'A => A.some.path.i' to this Signature. The target
        /// property 'i' will not be invalidaed in case of validation errors.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="dependency"></param>
        /// <returns></returns>
        public static ValidationRuleDependency InputOnly<TSource, TTarget>(
            Expression<Func<TSource, TTarget>> dependency)
        {
            return new ValidationRuleDependency
            {
                Expression = dependency,
                InputOnly = true
            };
        }
    }
}
