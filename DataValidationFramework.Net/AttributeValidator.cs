using System.ComponentModel.DataAnnotations;
using OpenSoftware.DataValidationFrameworkCore;

namespace OpenSoftware.DataValidationFramework.Net
{
    /// <inheritdoc />
    /// <summary>
    /// Abstract base class for attribute validators.
    /// This is a WCF DomainServices.Client services-specific instantation of the 
    /// RiaServicesContrib.DataValidation.AttributeValidator class.
    /// </summary>
    public abstract class AttributeValidator : AttributeValidator<ValidationResult>
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the AttributeValidator class.
        /// </summary>
        /// <param name="signature"></param>
        protected AttributeValidator(params ValidationRuleDependency[] signature) : base(signature)
        {
        }
    }
}
