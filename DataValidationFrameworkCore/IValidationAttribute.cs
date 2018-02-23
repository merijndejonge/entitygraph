using System.Reflection;

namespace OpenSoftware.DataValidationFrameworkCore
{
    /// <summary>
    /// Interface for attribute-based validation.
    /// 
    /// Since subclasses of .Net attribute classes cannot be generic, we use a parameterized
    /// interface instead.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IValidationAttribute<TResult> where TResult : class
    {
        /// <summary>
        /// Method that binds the validation rule to the given property.
        /// </summary>
        /// <param name="propInfo"></param>
        /// <returns></returns>
        ValidationRule<TResult> BindTo(PropertyInfo propInfo);
    }
}
