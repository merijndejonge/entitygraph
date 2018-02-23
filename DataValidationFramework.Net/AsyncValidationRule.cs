using System.ComponentModel.DataAnnotations;
using OpenSoftware.DataValidationFrameworkCore;

namespace OpenSoftware.DataValidationFramework.Net
{
    /// <inheritdoc />
    /// <summary>
    /// Class that forms the abstract base for all asynchronous cross-entity validation rules.
    /// This is a WCF DomainServices.Client services specific instantiation of the 
    /// RiaServicesContrib.DataValidation.AsyncValidationRule class.
    /// </summary>
    public class AsyncValidationRule : AsyncValidationRule<ValidationResult>
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the ValidationRule class.
        /// </summary>
        /// <param name="signature"></param>
        public AsyncValidationRule(params ValidationRuleDependency[] signature)
            : base(signature)
        {
        }
    }
}
