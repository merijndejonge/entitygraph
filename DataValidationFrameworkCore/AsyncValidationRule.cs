namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <inheritdoc />
    /// <summary>
    /// Class that forms the abstract base for all asynchronous cross-entity validation rules.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public abstract class AsyncValidationRule<TResult> : ValidationRule<TResult>
        where TResult : class
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of the AsyncValidationRule class.
        /// </summary>
        /// <param name="signature"></param>
        protected AsyncValidationRule(params ValidationRuleDependency[] signature) :
            base(signature)
        {
        }

        /// <summary>
        /// Method that checks if bindings are correct for the validation method of this Validation rule
        /// and then invokes teh validation method.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        internal ValidationOperation<TResult> EvaluateAsync(RuleBinding<TResult> binding)
        {
            var bindings = MakeArgumentBindings(binding);
            var result = ValidationMethod.Invoke(binding.ValidationRule, bindings);
            return (ValidationOperation<TResult>) result;
        }
    }
}
