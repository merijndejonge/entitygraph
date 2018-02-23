namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <summary>
    /// Class that represents a collection of dependency bindings for a validation rule.
    /// It holds references to all bindings for its validation rule dependencies,
    /// and a reference to the validation rule itself.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    internal class RuleBinding<TResult> where TResult : class
    {
        /// <summary>
        /// Gets or sets the collection of validation rule dependency bindings for this rule binding.
        /// </summary>
        public ValidationRuleDependencyBinding[] DependencyBindings { get; internal set; }

        /// <summary>
        /// Gets or sets the validation rule for this validation rule binding.
        /// </summary>
        public ValidationRule<TResult> ValidationRule { get; internal set; }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is RuleBinding<TResult> == false)
            {
                return false;
            }

            var binding = (RuleBinding<TResult>) obj;
            if (binding.DependencyBindings.Length != DependencyBindings.Length)
            {
                return false;
            }

            for (var i = 0; i < binding.DependencyBindings.Length; i++)
            {
                if (binding.DependencyBindings[i].Equals(DependencyBindings[i]) == false)
                {
                    return false;
                }
            }

            return binding.ValidationRule.Equals(ValidationRule);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return ValidationRule.GetHashCode();
        }
    }
}
