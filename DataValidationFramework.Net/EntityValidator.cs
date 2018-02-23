using System.ComponentModel.DataAnnotations;
using OpenSoftware.DataValidationFrameworkCore;

namespace OpenSoftware.DataValidationFramework.Net
{
    /// <inheritdoc />
    /// <summary>
    /// Class that implements signature-based validation for single entities.
    /// This is a WCF DomainServices.Client services-specific instantiation of the 
    /// RiaServicesContrib.DataValidation.EntityValidator class.
    /// </summary>
    public class EntityValidator : EntityValidator<object, ValidationResult>
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the EntityValidator class.
        /// </summary>
        /// <param name="entityToValidate"></param>
        public EntityValidator(object entityToValidate)
            : base(entityToValidate)
        {
            var rulesProvider = new ClassValidationRulesProvider<ValidationResult>(entityToValidate.GetType());
            Validator = new ValidationEngine {RulesProvider = rulesProvider};
        }
    }
}
