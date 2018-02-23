using System.Collections.Generic;

namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <summary>
    /// Represents a collection of validation rules.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IValidationRulesProvider<TResult> where TResult : class
    {
        /// <summary>
        /// Gets the collection of Validation rules that are provided by this validation rules provider.
        /// </summary>
        IEnumerable<ValidationRule<TResult>> ValidationRules { get; }
    }
}